using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FintechTestTask.WebAPI.Filters;

public class OnlyForAnnonymousFilterAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context?.HttpContext?.User.Identity?.IsAuthenticated == true)
            context.Result = new ForbidResult();
    }
}