using System.Security.Claims;

namespace SessionControlService.Helpers;

public class PermissionLevelMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionLevelMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User != null && context.User.Identity.IsAuthenticated)
        {
            context.Items["UserId"] = Guid.Parse(
                context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value
            );
        }

        await _next(context);
    }
}

public static class PermissionLevelMiddlewareExtensions
{
    public static IApplicationBuilder UsePermissionLevel(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PermissionLevelMiddleware>();
    }
}
