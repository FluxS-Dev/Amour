// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2024 username <113782077+whateverusername0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 whateverusername0 <whateveremail>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Goobstation.Maths.FixedPoint;
using Content.Server.GameTicking.Events;
using Content.Shared._White.StoreDiscount;
using Content.Shared.GameTicking;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Server.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._RW.StoreDiscount;

/// <summary>
/// Generates and owns the shared uplink discounts and shortages for the current round.
/// </summary>
public sealed class StoreDiscountSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private readonly Dictionary<ProtoId<ListingPrototype>, UplinkMarketListing> _market = new();
    private bool _marketGenerated;
    private bool _roundStarted;
    private int _roundStartPlayerCount;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartingEvent>(OnRoundStarting);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestartCleanup);
    }

    private void OnRoundStarting(RoundStartingEvent args)
    {
        _market.Clear();
        _marketGenerated = false;
        _roundStarted = true;
        _roundStartPlayerCount = _playerManager.PlayerCount;

        var query = EntityQueryEnumerator<StoreComponent>();
        while (query.MoveNext(out _, out var store))
        {
            if (!store.Sales.Enabled)
                continue;

            GenerateMarket(store, _roundStartPlayerCount);
            RaiseLocalEvent(new UplinkMarketChangedEvent());
            break;
        }
    }

    private void OnRoundRestartCleanup(RoundRestartCleanupEvent args)
    {
        _market.Clear();
        _marketGenerated = false;
        _roundStarted = false;
        _roundStartPlayerCount = 0;
        RaiseLocalEvent(new UplinkMarketChangedEvent());
    }

    private void GenerateMarket(StoreComponent store, int playerCount)
    {
        _market.Clear();
        _marketGenerated = true;

        var sales = store.Sales;
        if (!sales.Enabled)
            return;

        var storeCategories = store.Categories.ToHashSet();
        storeCategories.Remove(sales.SalesCategory);
        storeCategories.Remove(sales.ShortageCategory);

        var candidates = _prototype.EnumeratePrototypes<ListingPrototype>()
            .Where(listing =>
                !listing.SaleBlacklist &&
                listing.Cost.Any(cost => cost.Value > 0) &&
                storeCategories.Overlaps(listing.Categories))
            .ToList();

        _random.Shuffle(candidates);
        GenerateDiscounts(candidates, sales, playerCount);

        if (!sales.ShortagesEnabled)
            return;

        candidates.RemoveAll(listing => _market.ContainsKey(listing.ID));
        _random.Shuffle(candidates);
        GenerateShortages(candidates, sales);
    }

    private void GenerateDiscounts(
        List<ListingPrototype> candidates,
        SalesSpecifier sales,
        int playerCount)
    {
        var targetCount = GetRandomCount(sales.MinItems, sales.MaxItems, candidates.Count);
        var maxStock = GetDiscountStockMaximum(playerCount, sales.PlayersPerDiscountStock);

        foreach (var listing in candidates)
        {
            if (_market.Count >= targetCount)
                break;

            var multiplier = GetMultiplier(sales.MinMultiplier, sales.MaxMultiplier);
            var modifiedCost = GetDiscountedCost(listing.Cost, multiplier);
            if (CostsEqual(listing.Cost, modifiedCost))
                continue;

            var discount = GetActualPriceChangePercent(listing.Cost, modifiedCost);
            var stock = _random.Next(1, maxStock + 1);
            _market.Add(listing.ID,
                new UplinkMarketListing(UplinkMarketListingType.Discount, modifiedCost, discount, stock));
        }
    }

    private void GenerateShortages(List<ListingPrototype> candidates, SalesSpecifier sales)
    {
        var targetCount = GetRandomCount(sales.ShortageMinItems, sales.ShortageMaxItems, candidates.Count);
        var minimumStock = Math.Max(1, sales.ShortageMinStock);
        var maximumStock = Math.Max(minimumStock, sales.ShortageMaxStock);

        for (var i = 0; i < targetCount; i++)
        {
            var listing = candidates[i];
            var multiplier = GetMultiplier(sales.ShortageMinMultiplier, sales.ShortageMaxMultiplier);
            var modifiedCost = GetMarkedUpCost(listing.Cost, multiplier);
            var markup = GetActualPriceChangePercent(listing.Cost, modifiedCost);
            var stock = _random.Next(minimumStock, maximumStock + 1);
            _market.Add(listing.ID,
                new UplinkMarketListing(UplinkMarketListingType.Shortage, modifiedCost, markup, stock));
        }
    }

    /// <summary>
    /// Applies the current shared market state to a store's local listing copies.
    /// </summary>
    public void ApplyMarket(IEnumerable<ListingData> listings, StoreComponent store)
    {
        if (!store.Sales.Enabled)
            return;

        if (_roundStarted && !_marketGenerated)
            GenerateMarket(store, _roundStartPlayerCount);

        foreach (var listing in listings)
        {
            if (!_prototype.TryIndex<ListingPrototype>(listing.ID, out var prototype))
                continue;

            ResetListing(listing, prototype);
            if (!_marketGenerated || !_market.TryGetValue(prototype.ID, out var marketListing))
                continue;

            switch (marketListing.Type)
            {
                case UplinkMarketListingType.Discount when marketListing.RemainingStock > 0:
                    ApplyDiscount(listing, store.Sales, marketListing);
                    break;
                case UplinkMarketListingType.Shortage when marketListing.RemainingStock > 0:
                    ApplyShortage(listing, store.Sales, marketListing);
                    break;
                case UplinkMarketListingType.Shortage:
                    listing.Categories.Clear();
                    break;
            }
        }
    }

    /// <summary>
    /// Consumes one unit of shared market stock for an active special listing.
    /// </summary>
    public bool TryConsumeStock(ListingData listing)
    {
        if (!_market.TryGetValue(listing.ID, out var marketListing) ||
            marketListing.RemainingStock <= 0)
        {
            return false;
        }

        marketListing.RemainingStock--;
        if (marketListing.RemainingStock == 0)
            RaiseLocalEvent(new UplinkMarketChangedEvent());

        return true;
    }

    private static void ResetListing(ListingData listing, ListingPrototype prototype)
    {
        listing.Cost = prototype.Cost.ToDictionary();
        listing.Categories = prototype.Categories.ToList();
        listing.SaleLimit = prototype.SaleLimit;
        listing.DiscountValue = 0;
        listing.MarkupValue = 0;
        listing.OldCost = new();
        listing.SaleCost = null;
    }

    private static void ApplyDiscount(
        ListingData listing,
        SalesSpecifier sales,
        UplinkMarketListing marketListing)
    {
        listing.OldCost = listing.Cost;
        listing.Cost = marketListing.Cost.ToDictionary();
        listing.SaleCost = marketListing.Cost.ToDictionary();
        listing.DiscountValue = marketListing.PriceChangePercent;
        listing.Categories = new() { sales.SalesCategory };
    }

    private static void ApplyShortage(
        ListingData listing,
        SalesSpecifier sales,
        UplinkMarketListing marketListing)
    {
        listing.OldCost = listing.Cost;
        listing.Cost = marketListing.Cost.ToDictionary();
        listing.MarkupValue = marketListing.PriceChangePercent;
        listing.Categories = new() { sales.ShortageCategory };
    }

    private Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> GetDiscountedCost(
        Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> cost,
        float multiplier)
    {
        return cost.ToDictionary(
            entry => entry.Key,
            entry => entry.Value <= 0
                ? FixedPoint2.Zero
                : FixedPoint2.New(Math.Max(1, (int) MathF.Round(entry.Value.Float() * multiplier))));
    }

    private static Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> GetMarkedUpCost(
        Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> cost,
        float multiplier)
    {
        return cost.ToDictionary(
            entry => entry.Key,
            entry => entry.Value <= 0
                ? FixedPoint2.Zero
                : FixedPoint2.New(Math.Max(1, (int) MathF.Ceiling(entry.Value.Float() * multiplier))));
    }

    private int GetRandomCount(int minimum, int maximum, int available)
    {
        if (available <= 0)
            return 0;

        minimum = Math.Clamp(minimum, 0, available);
        maximum = Math.Clamp(maximum, minimum, available);
        return _random.Next(minimum, maximum + 1);
    }

    private float GetMultiplier(float minimum, float maximum)
    {
        if (maximum < minimum)
            (minimum, maximum) = (maximum, minimum);

        return _random.NextFloat() * (maximum - minimum) + minimum;
    }

    internal static int GetDiscountStockMaximum(int playerCount, int playersPerStock)
    {
        if (playersPerStock <= 0)
            return 1;

        return Math.Max(1, (Math.Max(0, playerCount) + playersPerStock - 1) / playersPerStock);
    }

    internal static int GetActualPriceChangePercent(
        Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> originalCost,
        Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> modifiedCost)
    {
        foreach (var (currency, original) in originalCost)
        {
            if (original <= 0 || !modifiedCost.TryGetValue(currency, out var modified))
                continue;

            return Math.Abs((modified / original * 100).Int() - 100);
        }

        return 0;
    }

    private static bool CostsEqual(
        Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> first,
        Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> second)
    {
        return first.Count == second.Count &&
               first.All(entry => second.TryGetValue(entry.Key, out var value) && value == entry.Value);
    }

    private sealed class UplinkMarketListing
    {
        public readonly UplinkMarketListingType Type;
        public readonly Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Cost;
        public readonly int PriceChangePercent;
        public int RemainingStock;

        public UplinkMarketListing(
            UplinkMarketListingType type,
            Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> cost,
            int priceChangePercent,
            int stock)
        {
            Type = type;
            Cost = cost;
            PriceChangePercent = priceChangePercent;
            RemainingStock = stock;
        }
    }

    private enum UplinkMarketListingType : byte
    {
        Discount,
        Shortage,
    }
}

/// <summary>
/// Raised whenever the shared uplink market has been generated, cleared, or a listing has sold out.
/// </summary>
public sealed class UplinkMarketChangedEvent : EntityEventArgs;
