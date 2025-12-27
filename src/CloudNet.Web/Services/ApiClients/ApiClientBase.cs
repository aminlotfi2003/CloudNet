using System.Net.Http.Headers;
using System.Text.Json;

namespace CloudNet.Web.Services.ApiClients;

public abstract class ApiClientBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected ApiClientBase(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    protected HttpClient HttpClient { get; }

    protected async Task<ApiResponse<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken ct)
    {
        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                return new ApiResponse<T>(result: default, response.StatusCode);
            }

            var payload = await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
            return new ApiResponse<T>(payload, response.StatusCode);
        }

        var problem = await ReadProblemDetailsAsync(response, ct)
            ?? new ApiProblemDetails
            {
                Title = "Request failed.",
                Status = (int)response.StatusCode,
                Detail = response.ReasonPhrase
            };

        return new ApiResponse<T>(problem, response.StatusCode);
    }

    protected async Task<ApiResponse> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (response.IsSuccessStatusCode)
        {
            return new ApiResponse(response.StatusCode);
        }

        var problem = await ReadProblemDetailsAsync(response, ct)
            ?? new ApiProblemDetails
            {
                Title = "Request failed.",
                Status = (int)response.StatusCode,
                Detail = response.ReasonPhrase
            };

        return new ApiResponse(problem, response.StatusCode);
    }

    protected static bool IsProblemJson(HttpResponseMessage response)
    {
        return response.Content.Headers.ContentType?.MediaType?.Contains("problem+json", StringComparison.OrdinalIgnoreCase) == true;
    }

    protected static async Task<ApiProblemDetails?> ReadProblemDetailsAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (!IsProblemJson(response))
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ApiProblemDetails>(JsonOptions, ct);
    }

    protected static void ApplyJsonContentHeaders(HttpRequestMessage request)
    {
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
