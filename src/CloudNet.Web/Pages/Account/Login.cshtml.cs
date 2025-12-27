using CloudNet.Web.Pages.Shared;
using CloudNet.Web.Services.ApiClients;
using CloudNet.Web.Services.Models.AuthModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CloudNet.Web.Pages.Account;

public sealed class LoginModel : ApiPageModel
{
    private readonly AuthApiClient _authApiClient;

    public LoginModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public IActionResult OnGet()
    {
        LoadTempDataErrors();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var response = await _authApiClient.LoginAsync(new LoginRequest
        {
            Identifier = Input.Identifier,
            Password = Input.Password
        }, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Invalid credentials. Please try again.";
            return Page();
        }

        if (!response.IsSuccess)
        {
            if (response.Problem is not null)
            {
                ApplyProblemDetails(response.Problem, nameof(Input));
            }
            else
            {
                ErrorMessage = "Login failed. Please try again.";
            }

            return Page();
        }

        if (response.Result is not null)
        {
            SetAuthCookies(response.Result.Tokens);
        }

        return RedirectToPage("/Drive/Index");
    }

    public sealed class LoginInput
    {
        [Required]
        [Display(Name = "Email or username")]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    private void SetAuthCookies(AuthTokensResponse tokens)
    {
        var accessOptions = BuildAuthCookieOptions(tokens.AccessTokenExpiresAt);
        var refreshOptions = BuildAuthCookieOptions(tokens.RefreshTokenExpiresAt);

        Response.Cookies.Append("access_token", tokens.AccessToken, accessOptions);
        Response.Cookies.Append("refresh_token", tokens.RefreshToken, refreshOptions);
    }

    private CookieOptions BuildAuthCookieOptions(DateTimeOffset expiresAt)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt
        };
    }
}
