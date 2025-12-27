using CloudNet.Web.Services.ApiClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace CloudNet.Web.Pages.Shared;

public abstract class ApiPageModel : PageModel
{
    [TempData]
    public string? TempErrorMessage { get; set; }

    public string? ErrorMessage { get; protected set; }

    protected IActionResult RedirectToLogin()
        => RedirectToPage("/Account/Login");

    protected void ApplyProblemDetails(ApiProblemDetails problem, params string[] prefixes)
    {
        if (problem.Errors.Count > 0)
        {
            foreach (var (key, errors) in problem.Errors)
            {
                foreach (var error in errors)
                {
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        ModelState.AddModelError(string.Empty, error);
                        continue;
                    }

                    ModelState.AddModelError(key, error);

                    if (prefixes.Length > 0)
                    {
                        foreach (var prefix in prefixes)
                        {
                            ModelState.AddModelError($"{prefix}.{key}", error);
                        }
                    }
                }
            }
        }
        else
        {
            ErrorMessage = problem.Detail ?? problem.Title ?? "Request failed.";
        }
    }

    protected bool TryHandleUnauthorized(HttpStatusCode statusCode, out IActionResult? result)
    {
        if (statusCode == HttpStatusCode.Unauthorized)
        {
            result = RedirectToLogin();
            return true;
        }

        result = null;
        return false;
    }

    protected void LoadTempDataErrors()
    {
        if (!string.IsNullOrWhiteSpace(TempErrorMessage))
        {
            ErrorMessage = TempErrorMessage;
        }
    }

}
