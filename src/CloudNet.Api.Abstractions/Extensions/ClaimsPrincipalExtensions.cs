using System.Security.Claims;

namespace CloudNet.Api.Abstractions.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        return user.TryGetUserId(out var userId) ? userId : Guid.Empty;
    }

    public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
    {
        userId = default;

        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var parsed) || parsed == Guid.Empty)
            return false;

        userId = parsed;
        return true;
    }
}
