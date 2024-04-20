using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;

namespace AttendenceApi.Data.Seeds
{
    public class ClaimSeed
    {


        public static async Task UserClaim(AppDbContext dbcontext)
        {
            var user = dbcontext.Users.FirstOrDefault(s => s.UserName == "1024");
            if (dbcontext.UserClaims.Where(s=> s.UserId == user.Id ) != null) {
                return;
            }
            dbcontext.UserClaims.Add(new IdentityUserClaim<Guid> { UserId = user.Id, ClaimValue = Claims.SUPERUSER, ClaimType = Claims.SUPERUSER });
            _ = dbcontext.SaveChangesAsync();
        }
    }
}
