using Microsoft.AspNetCore.Mvc;

namespace SessionControlService.Helpers;

public class ControllerBaseExtended : ControllerBase
{
    protected Guid UserId
    {
        get => getUserId();
    }

    private Guid getUserId()
    {
        var userId = (Guid?)this.HttpContext.Items["UserId"];
        if (userId == null)
            throw new UnauthorizedException("User ID not found.");
        return userId.Value;
    }
}
