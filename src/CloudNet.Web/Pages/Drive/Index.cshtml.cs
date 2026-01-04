using CloudNet.Web.Pages.Shared;
using CloudNet.Web.Services.ApiClients;
using CloudNet.Web.Services.Models.FileModels;
using CloudNet.Web.Services.Models.FolderModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CloudNet.Web.Pages.Drive;

public sealed class IndexModel : ApiPageModel
{
    private readonly FoldersApiClient _foldersApiClient;
    private readonly FilesApiClient _filesApiClient;
    private readonly ShareApiClient _shareApiClient;
    private readonly string _apiBaseUrl;

    public IndexModel(
        FoldersApiClient foldersApiClient,
        FilesApiClient filesApiClient,
        ShareApiClient shareApiClient,
        IOptions<ApiOptions> options)
    {
        _foldersApiClient = foldersApiClient;
        _filesApiClient = filesApiClient;
        _shareApiClient = shareApiClient;
        _apiBaseUrl = options.Value.BaseUrl.TrimEnd('/');
    }

    [BindProperty(SupportsGet = true)]
    public Guid? FolderId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Path { get; set; }

    [TempData]
    public string? ShareToken { get; set; }

    [TempData]
    public string? ShareFileName { get; set; }

    public IReadOnlyList<FolderDto> Folders { get; private set; } = Array.Empty<FolderDto>();
    public IReadOnlyList<FileEntryDto> Files { get; private set; } = Array.Empty<FileEntryDto>();
    public IReadOnlyList<BreadcrumbItem> Breadcrumbs { get; private set; } = Array.Empty<BreadcrumbItem>();
    public string ApiBaseUrl => _apiBaseUrl;

    [BindProperty]
    public CreateFolderInput CreateFolder { get; set; } = new();

    [BindProperty]
    public UploadFileInput UploadFile { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        LoadTempDataErrors();
        Breadcrumbs = BuildBreadcrumbs(Path);

        var folderResponse = await _foldersApiClient.ListChildrenAsync(FolderId, ct);
        if (TryHandleUnauthorized(folderResponse.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!folderResponse.IsSuccess)
        {
            if (folderResponse.Problem is not null)
            {
                ApplyProblemDetails(folderResponse.Problem);
            }
            return Page();
        }

        Folders = folderResponse.Result ?? Array.Empty<FolderDto>();

        if (FolderId.HasValue)
        {
            var filesResponse = await _filesApiClient.ListByFolderAsync(FolderId.Value, ct);
            if (TryHandleUnauthorized(filesResponse.StatusCode, out unauthorizedResult))
            {
                return unauthorizedResult!;
            }

            if (!filesResponse.IsSuccess)
            {
                if (filesResponse.Problem is not null)
                {
                    ApplyProblemDetails(filesResponse.Problem);
                }
                return Page();
            }

            Files = filesResponse.Result ?? Array.Empty<FileEntryDto>();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCreateFolderAsync(CancellationToken ct)
    {
        ModelState.Remove($"{nameof(UploadFile)}.{nameof(UploadFileInput.File)}");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(ct);
        }

        var response = await _foldersApiClient.CreateAsync(new CreateFolderRequest
        {
            ParentId = FolderId,
            Name = CreateFolder.Name
        }, ct);

        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            if (response.Problem is not null)
            {
                ApplyProblemDetails(response.Problem, nameof(CreateFolder));
            }
            return await OnGetAsync(ct);
        }

        return RedirectToPage(new { folderId = FolderId, path = Path });
    }

    public async Task<IActionResult> OnPostRenameFolderAsync(Guid folderId, string name, CancellationToken ct)
    {
        var response = await _foldersApiClient.UpdateAsync(folderId, new UpdateFolderRequest { Name = name }, ct);

        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            if (response.Problem is not null)
            {
                TempErrorMessage = response.Problem.Detail ?? response.Problem.Title;
            }
            return RedirectToPage(new { folderId = FolderId, path = Path });
        }

        return RedirectToPage(new { folderId = FolderId, path = Path });
    }

    public async Task<IActionResult> OnPostDeleteFolderAsync(Guid folderId, CancellationToken ct)
    {
        var response = await _foldersApiClient.SoftDeleteAsync(folderId, ct);

        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            TempErrorMessage = response.Problem?.Detail ?? "Unable to delete folder.";
        }

        return RedirectToPage(new { folderId = FolderId, path = Path });
    }

    public async Task<IActionResult> OnPostUploadFileAsync(CancellationToken ct)
    {
        ModelState.Remove($"{nameof(CreateFolder)}.{nameof(CreateFolderInput.Name)}");

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(ct);
        }

        if (!FolderId.HasValue)
        {
            TempErrorMessage = "Select a folder before uploading files.";
            return RedirectToPage(new { folderId = FolderId, path = Path });
        }

        if (UploadFile.File is null || UploadFile.File.Length == 0)
        {
            ModelState.AddModelError(nameof(UploadFile.File), "Please select a file to upload.");
            return await OnGetAsync(ct);
        }

        await using var stream = UploadFile.File.OpenReadStream();
        var response = await _filesApiClient.UploadAsync(
            FolderId.Value,
            stream,
            UploadFile.File.FileName,
            UploadFile.File.ContentType ?? "application/octet-stream",
            UploadFile.Description,
            ct);

        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            if (response.Problem is not null)
            {
                ApplyProblemDetails(response.Problem, nameof(UploadFile));
            }
            return await OnGetAsync(ct);
        }

        return RedirectToPage(new { folderId = FolderId, path = Path });
    }

    public async Task<IActionResult> OnPostUpdateFileAsync(Guid fileId, string fileName, string? description, CancellationToken ct)
    {
        var response = await _filesApiClient.UpdateAsync(fileId, new UpdateFileEntryRequest
        {
            FileName = fileName,
            Description = description
        }, ct);

        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            TempErrorMessage = response.Problem?.Detail ?? "Unable to update file.";
        }

        return RedirectToPage(new { folderId = FolderId, path = Path });
    }

    public async Task<IActionResult> OnPostDeleteFileAsync(Guid fileId, CancellationToken ct)
    {
        var response = await _filesApiClient.SoftDeleteAsync(fileId, ct);

        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            TempErrorMessage = response.Problem?.Detail ?? "Unable to delete file.";
        }

        return RedirectToPage(new { folderId = FolderId, path = Path });
    }

    public async Task<IActionResult> OnPostShareAsync(Guid fileId, string fileName, CancellationToken ct)
    {
        var response = await _shareApiClient.CreateShareLinkAsync(fileId, new CreateShareLinkRequest(), ct);

        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            TempErrorMessage = response.Problem?.Detail ?? "Unable to create share link.";
            return RedirectToPage(new { folderId = FolderId, path = Path });
        }

        ShareToken = response.Result?.Token;
        ShareFileName = fileName;

        return RedirectToPage(new { folderId = FolderId, path = Path });
    }

    public async Task<IActionResult> OnGetDownloadAsync(Guid fileId, CancellationToken ct)
    {
        var rangeHeader = Request.Headers.Range.ToString();
        using var response = await _filesApiClient.DownloadAsync(fileId, rangeHeader, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return RedirectToLogin();
        }

        if (!response.IsSuccessStatusCode)
        {
            TempErrorMessage = "Unable to download file.";
            return RedirectToPage(new { folderId = FolderId, path = Path });
        }

        Response.StatusCode = (int)response.StatusCode;
        if (response.Content.Headers.ContentType is not null)
        {
            Response.ContentType = response.Content.Headers.ContentType.ToString();
        }

        if (response.Content.Headers.ContentLength.HasValue)
        {
            Response.ContentLength = response.Content.Headers.ContentLength.Value;
        }

        foreach (var header in response.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in response.Content.Headers)
        {
            if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)
                || header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            Response.Headers[header.Key] = header.Value.ToArray();
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(ct);
        await responseStream.CopyToAsync(Response.Body, ct);
        return new EmptyResult();
    }

    public string BuildChildPath(Guid folderId, string name)
    {
        var encodedName = Uri.EscapeDataString(name);
        if (string.IsNullOrWhiteSpace(Path))
        {
            return $"{folderId}:{encodedName}";
        }

        return $"{Path}|{folderId}:{encodedName}";
    }

    public sealed class CreateFolderInput
    {
        [Required]
        [Display(Name = "Folder name")]
        public string Name { get; set; } = string.Empty;
    }

    public sealed class UploadFileInput
    {
        [Required]
        public IFormFile? File { get; set; }

        public string? Description { get; set; }
    }

    public sealed record BreadcrumbItem(Guid? FolderId, string Name, string? Path);

    private static IReadOnlyList<BreadcrumbItem> BuildBreadcrumbs(string? path)
    {
        var items = new List<BreadcrumbItem>
        {
            new(null, "Drive", null)
        };

        if (string.IsNullOrWhiteSpace(path))
        {
            return items;
        }

        var segments = path.Split('|', StringSplitOptions.RemoveEmptyEntries);
        var builtSegments = new List<string>();
        foreach (var segment in segments)
        {
            var parts = segment.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || !Guid.TryParse(parts[0], out var id))
            {
                continue;
            }

            builtSegments.Add(segment);
            var name = Uri.UnescapeDataString(parts[1]);
            items.Add(new BreadcrumbItem(id, name, string.Join('|', builtSegments)));
        }

        return items;
    }
}
