using System.Security.Claims;

namespace FintechTestTask.Application.Abstractions;

public static class ClaimsExtensions
{
    public static Guid? GetUserId(this IEnumerable<Claim> claims)
    {
        var userIdString = claims.First(c => c.Type == "userId").Value;

        if (Guid.TryParse(userIdString, out var userId))
            return userId;

        return null;
    }
}