using CloudNet.Web.Pages.Shared;
using CloudNet.Web.Services.ApiClients;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CloudNet.Web.Pages.Account;

public sealed class LogoutModel : ApiPageModel
{
    private const string AccessTokenCookie = "access_token";
    private const string RefreshTokenCookie = "refresh_token";
    private readonly AuthApiClient _authApiClient;

    public LogoutModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie] ?? string.Empty;
        var response = await _authApiClient.LogoutAsync(refreshToken, ct);

        Response.Cookies.Delete(AccessTokenCookie);
        Response.Cookies.Delete(RefreshTokenCookie);

        if (!response.IsSuccess && response.StatusCode != HttpStatusCode.Unauthorized)
        {
            TempErrorMessage = response.Problem?.Detail ?? "We could not complete logout. Please sign in again.";
        }

        return RedirectToPage("/Account/Login");
    }

    public IActionResult OnGet()
    {
        return RedirectToPage("/Drive/Index");
    }
}
