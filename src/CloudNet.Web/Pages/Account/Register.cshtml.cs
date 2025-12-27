using CloudNet.Web.Pages.Shared;
using CloudNet.Web.Services.ApiClients;
using CloudNet.Web.Services.Models.AuthModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CloudNet.Web.Pages.Account;

public sealed class RegisterModel : ApiPageModel
{
    private readonly AuthApiClient _authApiClient;

    public RegisterModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

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

        var response = await _authApiClient.RegisterAsync(new RegisterRequest
        {
            Email = Input.Email,
            UserName = Input.UserName,
            Password = Input.Password,
            ConfirmPassword = Input.ConfirmPassword
        }, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Registration failed. Please try again.";
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
                ErrorMessage = "Registration failed. Please try again.";
            }

            return Page();
        }

        return RedirectToPage("/Drive/Index");
    }

    public sealed class RegisterInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
