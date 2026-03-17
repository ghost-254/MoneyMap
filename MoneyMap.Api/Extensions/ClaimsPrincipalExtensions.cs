using System.Security.Claims;
using MoneyMap.Api.Exceptions;

namespace MoneyMap.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claimValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(claimValue, out var userId))
        {
            throw new UnauthorizedException("The current access token is invalid.");
        }

        return userId;
    }
}
