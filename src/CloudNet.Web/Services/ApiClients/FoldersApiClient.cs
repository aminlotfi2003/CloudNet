using CloudNet.Web.Services.Models.FolderModels;

namespace CloudNet.Web.Services.ApiClients;

public sealed class FoldersApiClient : ApiClientBase
{
    public FoldersApiClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<ApiResponse<FolderDto>> CreateAsync(CreateFolderRequest request, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/folders")
        {
            Content = JsonContent.Create(request)
        };
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<FolderDto>(httpRequest, ct);
    }

    public async Task<ApiResponse<IReadOnlyList<FolderDto>>> ListChildrenAsync(Guid? parentId, CancellationToken ct)
    {
        var url = parentId.HasValue ? $"api/v1/folders/children?parentId={parentId}" : "api/v1/folders/children";
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<IReadOnlyList<FolderDto>>(httpRequest, ct);
    }

    public async Task<ApiResponse<FolderDto>> UpdateAsync(Guid folderId, UpdateFolderRequest request, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"api/v1/folders/{folderId}")
        {
            Content = JsonContent.Create(request)
        };
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<FolderDto>(httpRequest, ct);
    }

    public async Task<ApiResponse> SoftDeleteAsync(Guid folderId, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/folders/{folderId}");
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync(httpRequest, ct);
    }

    public async Task<ApiResponse> RestoreAsync(Guid folderId, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/v1/folders/{folderId}/restore");
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync(httpRequest, ct);
    }

    public async Task<ApiResponse<IReadOnlyList<FolderDto>>> ListDeletedAsync(CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/v1/folders/deleted");
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<IReadOnlyList<FolderDto>>(httpRequest, ct);
    }
}
