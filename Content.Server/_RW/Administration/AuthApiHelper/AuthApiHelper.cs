// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Schrödinger <132720404+Schrodinger71@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Content.Server._RW.Administration;

public sealed partial class AuthApiHelper
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<string> GetCreationDate(string uuid)
    {
        var url = $"https://auth.spacestation14.com/api/query/userid?userid={uuid}";

        try
        {
            var response = await HttpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Logger.Warning($"API request failed for UUID {uuid}: {response.StatusCode}");
                return "Дата не найдена";
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(jsonResponse);

            if (jsonDoc.RootElement.TryGetProperty("createdTime", out var createdTimeElement) &&
                createdTimeElement.ValueKind != JsonValueKind.Null &&
                createdTimeElement.ValueKind != JsonValueKind.Undefined)
            {
                var createdTime = createdTimeElement.GetString();
                if (!string.IsNullOrEmpty(createdTime))
                    return DateTimeOffset.Parse(createdTime).ToString("dd.MM.yyyy");
            }

            Logger.Warning($"CreatedTime property missing or invalid for UUID: {uuid}");
            return "Дата не найдена";
        }
        catch (HttpRequestException httpEx)
        {
            Logger.Warning($"HTTP error for UUID {uuid}: {httpEx.Message}");
            return "Ошибка соединения";
        }
        catch (JsonException jsonEx)
        {
            Logger.Warning($"JSON parsing error for UUID {uuid}: {jsonEx.Message}");
            return "Ошибка данных";
        }
        catch (FormatException)
        {
            Logger.Warning($"Invalid date format for UUID: {uuid}");
            return "Неверный формат даты";
        }
        catch (Exception ex)
        {
            Logger.Warning($"Unexpected error for UUID {uuid}: {ex.Message}");
            return "Ошибка системы";
        }
    }
}
