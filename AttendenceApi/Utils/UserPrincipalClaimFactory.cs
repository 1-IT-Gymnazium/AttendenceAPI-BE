using System.Security.Claims;
using AttendenceApi.Data;
using AttendenceApi.Data.Indentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


namespace AttendenceApi.Utils;
public class UserPrincipalClaimFactory : UserClaimsPrincipalFactory<User>
{
    private readonly AppDbContext dbContext;

    public UserPrincipalClaimFactory(
        AppDbContext dbContext,
        UserManager<User> userManager,
        IOptions<IdentityOptions> options)
        : base(userManager, options)
    {
        this.dbContext = dbContext;
    }
    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        var basePrincipal = await base.CreateAsync(user);

        //await dbContext.Entry(user).Collection(x => x.Companies).LoadAsync();

        if (!basePrincipal.HasClaim(x => x.Type == Claims.SUPERUSER))
        {
            basePrincipal.Identities.First().AddClaims(GetClaims(user));
        }

        return basePrincipal;
    }

    private IEnumerable<Claim> GetClaims(User user)
    {
        // zatim do cookie ulozime ze je to CA jenom pro aktualni company, setri to velikost identity cookie.
        // Podle use case se mozna casem bude hodit druha verze ktera tam nacpe vsechny company.
        yield return new Claim(Claims.USER, string.Empty);

        //foreach (var project in user.Projects)
        //{
        //    yield return new Claim(Claims.PROJECT_ADMIN, project.ProjectId.ToString().ToLowerInvariant());
        //}
    }
}
