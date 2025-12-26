using System.Security.Claims;

namespace Lumix.Blazor.Extensions
{
    public static class ClaimExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue("userId");
            return Guid.TryParse(userIdClaim, out var id)
                ? id
                : null;
        }
    }
}
