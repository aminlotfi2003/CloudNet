using CloudNet.Web.Services.Models.FileModels;

namespace CloudNet.Web.Services.ApiClients;

public sealed class ShareApiClient : ApiClientBase
{
    public ShareApiClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<ApiResponse<ShareLinkTokenDto>> CreateShareLinkAsync(Guid fileId, CreateShareLinkRequest request, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/v1/files/{fileId}/share-links")
        {
            Content = JsonContent.Create(request)
        };
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<ShareLinkTokenDto>(httpRequest, ct);
    }
}
