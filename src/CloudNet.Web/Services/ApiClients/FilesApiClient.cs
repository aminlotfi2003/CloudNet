using CloudNet.Web.Services.Models.FileModels;
using System.Net.Http.Headers;

namespace CloudNet.Web.Services.ApiClients;

public sealed class FilesApiClient : ApiClientBase
{
    public FilesApiClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<ApiResponse<IReadOnlyList<FileEntryDto>>> ListByFolderAsync(Guid folderId, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/files/by-folder/{folderId}");
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<IReadOnlyList<FileEntryDto>>(httpRequest, ct);
    }

    public async Task<ApiResponse<FileEntryDto>> UploadAsync(Guid folderId, Stream fileStream, string fileName, string contentType, string? description, CancellationToken ct)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(folderId.ToString()), "folderId");
        if (!string.IsNullOrWhiteSpace(description))
        {
            content.Add(new StringContent(description), "description");
        }

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/files/upload")
        {
            Content = content
        };
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<FileEntryDto>(httpRequest, ct);
    }

    public async Task<ApiResponse<FileEntryDto>> UpdateAsync(Guid fileId, UpdateFileEntryRequest request, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Patch, $"api/v1/files/{fileId}")
        {
            Content = JsonContent.Create(request)
        };
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<FileEntryDto>(httpRequest, ct);
    }

    public async Task<ApiResponse> SoftDeleteAsync(Guid fileId, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/files/{fileId}");
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync(httpRequest, ct);
    }

    public async Task<ApiResponse> RestoreAsync(Guid fileId, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/v1/files/{fileId}/restore");
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync(httpRequest, ct);
    }

    public async Task<ApiResponse> PurgeAsync(Guid fileId, CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/files/{fileId}/purge");
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync(httpRequest, ct);
    }

    public async Task<ApiResponse<IReadOnlyList<FileEntryDto>>> ListDeletedAsync(CancellationToken ct)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/v1/files/deleted");
        ApplyJsonContentHeaders(httpRequest);
        return await SendAsync<IReadOnlyList<FileEntryDto>>(httpRequest, ct);
    }

    public Task<HttpResponseMessage> DownloadAsync(Guid fileId, string? rangeHeader, CancellationToken ct)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/files/{fileId}/download");
        if (!string.IsNullOrWhiteSpace(rangeHeader))
        {
            request.Headers.TryAddWithoutValidation("Range", rangeHeader);
        }
        return HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
    }
}
