using CloudNet.Web.Pages.Shared;
using CloudNet.Web.Services.ApiClients;
using CloudNet.Web.Services.Models.FileModels;
using CloudNet.Web.Services.Models.FolderModels;
using Microsoft.AspNetCore.Mvc;

namespace CloudNet.Web.Pages.Drive;

public sealed class DeletedModel : ApiPageModel
{
    private readonly FoldersApiClient _foldersApiClient;
    private readonly FilesApiClient _filesApiClient;

    public DeletedModel(FoldersApiClient foldersApiClient, FilesApiClient filesApiClient)
    {
        _foldersApiClient = foldersApiClient;
        _filesApiClient = filesApiClient;
    }

    public IReadOnlyList<FolderDto> DeletedFolders { get; private set; } = Array.Empty<FolderDto>();
    public IReadOnlyList<FileEntryDto> DeletedFiles { get; private set; } = Array.Empty<FileEntryDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        LoadTempDataErrors();

        var foldersResponse = await _foldersApiClient.ListDeletedAsync(ct);
        if (TryHandleUnauthorized(foldersResponse.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!foldersResponse.IsSuccess)
        {
            if (foldersResponse.Problem is not null)
            {
                ApplyProblemDetails(foldersResponse.Problem);
            }
            return Page();
        }

        DeletedFolders = foldersResponse.Result ?? Array.Empty<FolderDto>();

        var filesResponse = await _filesApiClient.ListDeletedAsync(ct);
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

        DeletedFiles = filesResponse.Result ?? Array.Empty<FileEntryDto>();
        return Page();
    }

    public async Task<IActionResult> OnPostRestoreFolderAsync(Guid folderId, CancellationToken ct)
    {
        var response = await _foldersApiClient.RestoreAsync(folderId, ct);
        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            TempErrorMessage = response.Problem?.Detail ?? "Unable to restore folder.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRestoreFileAsync(Guid fileId, CancellationToken ct)
    {
        var response = await _filesApiClient.RestoreAsync(fileId, ct);
        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            TempErrorMessage = response.Problem?.Detail ?? "Unable to restore file.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPurgeFileAsync(Guid fileId, CancellationToken ct)
    {
        var response = await _filesApiClient.PurgeAsync(fileId, ct);
        if (TryHandleUnauthorized(response.StatusCode, out var unauthorizedResult))
        {
            return unauthorizedResult!;
        }

        if (!response.IsSuccess)
        {
            TempErrorMessage = response.Problem?.Detail ?? "Unable to purge file.";
        }

        return RedirectToPage();
    }
}
