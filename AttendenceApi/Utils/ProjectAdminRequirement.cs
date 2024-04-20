using AttendenceApi.Data;
using Microsoft.AspNetCore.Authorization;

namespace AttendenceApi.Utils;
public class ProjectAdminRequirement : IAuthorizationRequirement
{
}

public class CompanyAdminRequirementHandler : AuthorizationHandler<ProjectAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAdminRequirement requirement)
    {
        if (context.User.HasClaim(x => x.Type == Claims.SUPERUSER))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (context.Resource is HttpContext)
        {
            if (context.User.HasClaim(x => x.Type == Claims.USER))
            {
                // HttpContext jako resource chodi v pripade [Authorize(Policy = ...)], a nevime
                // o jakou company se jedna. Takze tady musi jakykoliv CA projit nezavisle na currentCompany.
                // Resource-based kontrola musi byt uvnitr akce.
                context.Succeed(requirement);
            }
        }
       
        return Task.CompletedTask;
    }
}
