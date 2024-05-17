using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Tor.Application.Abstractions;
using Tor.Domain.BusinessAggregate.Enums;

namespace Tor.Infrastructure.Filters;

public class PermissionAttribute : ActionFilterAttribute
{
    private readonly PositionType _position;

    public PermissionAttribute(PositionType position)
    {
        _position = position;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<ITorDbContext>();

        bool isUserTokenExists = context.HttpContext.Request.Headers.TryGetValue(Constants.UserTokenHeaderName, out StringValues value);
        if (!isUserTokenExists)
        {
            context.HttpContext.Response.StatusCode = 403;
            await context.HttpContext.Response.WriteAsync("missing user token");
            return;
        }

        string userToken = value.ToString();

        PositionType? staffMemberPosition = await dbContext.StaffMembers
            .Where(x => x.User.UserToken == userToken)
            .Select(x => (PositionType?)x.Position)
            .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

        if (staffMemberPosition is null)
        {
            context.HttpContext.Response.StatusCode = 403;
            await context.HttpContext.Response.WriteAsync("user token not exist");
            return;
        }

        if (_position != staffMemberPosition)
        {
            context.HttpContext.Response.StatusCode = 403;
            await context.HttpContext.Response.WriteAsync("not allowed to perform this action");
            return;
        }

        await next();
    }
}
