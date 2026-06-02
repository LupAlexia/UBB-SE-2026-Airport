using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using AirportLib.Domain.User;

namespace AirportApp.Web.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireAirportRoleAttribute : ActionFilterAttribute
{
    private readonly AirportModuleRole[] allowedRoles;

    public RequireAirportRoleAttribute(params AirportModuleRole[] allowedRoles)
    {
        this.allowedRoles = allowedRoles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.RequestServices.GetRequiredService<WebUserSession>();
        if (!allowedRoles.Contains(session.AirportRole))
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Content = "You are not authorized to access this airport management page.",
            };
        }
    }
}

