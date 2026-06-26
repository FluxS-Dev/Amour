// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using Content.Goobstation.Maths.FixedPoint;
using Content.Server._RW.StoreDiscount;
using Content.Shared.Store;
using NUnit.Framework;
using Robust.Shared.Prototypes;

namespace Content.Tests.Server._RW.StoreDiscount;

[TestFixture]
[TestOf(typeof(StoreDiscountSystem))]
public sealed class StoreDiscountSystemTest
{
    [TestCase(0, 10, 1)]
    [TestCase(1, 10, 1)]
    [TestCase(10, 10, 1)]
    [TestCase(11, 10, 2)]
    [TestCase(50, 10, 5)]
    [TestCase(50, 0, 1)]
    public void DiscountStockMaximumScalesWithRoundStartPopulation(
        int playerCount,
        int playersPerStock,
        int expected)
    {
        Assert.That(
            StoreDiscountSystem.GetDiscountStockMaximum(playerCount, playersPerStock),
            Is.EqualTo(expected));
    }

    [TestCase(1, 2, 100)]
    [TestCase(3, 4, 33)]
    [TestCase(5, 3, 40)]
    public void PriceChangePercentUsesTheRoundedFinalPrice(int originalPrice, int modifiedPrice, int expected)
    {
        ProtoId<CurrencyPrototype> currency = "Telecrystal";
        var original = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>
        {
            [currency] = originalPrice,
        };
        var modified = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>
        {
            [currency] = modifiedPrice,
        };

        Assert.That(
            StoreDiscountSystem.GetActualPriceChangePercent(original, modified),
            Is.EqualTo(expected));
    }
}
