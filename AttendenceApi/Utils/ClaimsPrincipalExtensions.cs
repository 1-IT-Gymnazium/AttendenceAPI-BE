using System.Security.Claims;

namespace StrictCode.Agrades.Identity.Api.Utils;
public static class Claims
{
    public static string GetName(this ClaimsPrincipal user)
    {
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new InvalidOperationException("user not logged in");
        }
        var name = user.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        return name;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new InvalidOperationException("user not logged in");
        }
        var idString = user.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        return int.Parse(idString);
    }
}