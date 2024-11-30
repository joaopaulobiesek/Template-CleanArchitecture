using System.Text.Json.Serialization;

namespace Template.Application.Common.Models;

/// <summary>
/// Representa um evento no Google Calendar.
/// </summary>
public class GoogleCalendarEvent
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("start")]
    public GoogleCalendarEventDate Start { get; set; }

    [JsonPropertyName("end")]
    public GoogleCalendarEventDate End { get; set; }
}

/// <summary>
/// Representa a data de início ou término de um evento no Google Calendar.
/// </summary>
public class GoogleCalendarEventDate
{
    [JsonPropertyName("dateTime")]
    public string DateTime { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }
}

/// <summary>
/// Representa a resposta da API do Google Calendar para eventos.
/// </summary>
public class GoogleCalendarEventsResponse
{
    [JsonPropertyName("items")]
    public List<GoogleCalendarEvent> Items { get; set; }
}