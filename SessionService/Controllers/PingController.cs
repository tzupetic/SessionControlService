using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionControlService.Controllers.ControllerModels;
using SessionControlService.Helpers;
using SessionControlService.Models;
using SessionControlService.Services;

namespace SessionControlService.Controllers;

[Route("api/pings")]
[ApiController]
[Authorize]
public class PingController : ControllerBaseExtended
{
    private readonly ISessionService _sessionService;

    public PingController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPut]
    public async Task<ActionResult> Ping(Ping ping)
    {
        if (
            !Enum.IsDefined(typeof(SessionState), ping.SessionState)
            || ping.SessionState == SessionState.Play
        )
        {
            throw new BadRequestException("Invalid SessionState");
        }

        await _sessionService.HandlePing(ping);
        return Ok();
    }
}
