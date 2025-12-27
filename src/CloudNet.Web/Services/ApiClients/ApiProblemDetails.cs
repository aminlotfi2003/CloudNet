using System.Text.Json.Serialization;

namespace CloudNet.Web.Services.ApiClients;

public sealed class ApiProblemDetails
{
    public string? Type { get; init; }
    public string? Title { get; init; }
    public int? Status { get; init; }
    public string? Detail { get; init; }
    public string? Instance { get; init; }

    [JsonPropertyName("errors")]
    public IDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>();
}
