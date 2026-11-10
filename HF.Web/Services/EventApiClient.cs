
using System.Net.Http.Json;
using System.Text.Json;
using HF.Web.Models;

namespace HF.Web.Services;

public class EventApiClient
{
    private readonly HttpClient _http;

    public EventApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EventDto>> GetEventsAsync(
        DateTime? from = null,
        DateTime? to = null)
    {
        var parameters = new List<string>();

        if (from.HasValue)
            parameters.Add($"from={from:yyyy-MM-dd}");

        if (to.HasValue)
            parameters.Add($"to={to:yyyy-MM-dd}");

        var url = "events";

        if (parameters.Count > 0)
            url += "?" + string.Join("&", parameters);

        return await _http.GetFromJsonAsync<List<EventDto>>(url)
            ?? new List<EventDto>();
    }

    public async Task<EventDto?> GetEventAsync(int id)
    {
        using var response = await _http.GetAsync($"events/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<EventDto>();
    }

    public async Task<(bool Success, string Message)> RegisterAsync(
        int eventId,
        RegistrationRequest request)
    {
        using var response = await _http.PostAsJsonAsync(
            $"events/{eventId}/registrations", request);

        if (response.IsSuccessStatusCode)
            return (true, "Uspešno si se prijavila na dogodek.");

        try
        {
            using var json = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var root = json.RootElement;

            if (root.TryGetProperty("detail", out var detail) &&
                detail.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(detail.GetString()))
            {
                return (false, detail.GetString()!);
            }

            if (root.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String)
            {
                return (false, title.GetString() ?? "Napaka pri prijavi.");
            }
        }
        catch (JsonException)
        {
            // Nepričakovan format odgovora.
        }

        return (false, $"Prijava ni uspela (HTTP {(int)response.StatusCode}).");
    }
}
