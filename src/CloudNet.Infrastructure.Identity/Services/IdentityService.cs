using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CloudNet.Infrastructure.Identity.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(
        string userName,
        string email,
        string password,
        CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            CreatedAt = DateTimeOffset.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
            return (true, Array.Empty<string>());

        var errors = result.Errors.Select(e => e.Description).ToArray();
        return (false, errors);
    }

    public async Task<ApplicationUser?> FindByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken ct)
    {
        ApplicationUser? user;

        if (userNameOrEmail.Contains('@'))
            user = await _userManager.FindByEmailAsync(userNameOrEmail);
        else
            user = await _userManager.FindByNameAsync(userNameOrEmail);

        return user;
    }

    public async Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken ct)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password, CancellationToken ct)
    {
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        return result.Succeeded;
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(ApplicationUser user, CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return (IReadOnlyList<string>)roles;
    }
}

