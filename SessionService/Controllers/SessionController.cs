using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionControlService.Controllers.ControllerModels;
using SessionControlService.Helpers;
using SessionControlService.Helpers.Pagination;
using SessionControlService.Models;
using SessionControlService.Services;

namespace SessionControlService.Controllers;

[Route("api/sessions")]
[ApiController]
[Authorize]
public class SessionController : ControllerBaseExtended
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<ActionResult<SessionResponse>> CreateSession(
        [FromBody] SessionRequest request
    )
    {
        var createdSession = await _sessionService.CreateSession(request, this.UserId);
        return CreatedAtAction(
            nameof(GetSession),
            new { id = createdSession.SessionId },
            createdSession
        );
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Session>> GetSession(Guid id)
    {
        var session = await _sessionService.GetSession(id);
        if (session == null)
        {
            throw new NotFoundException();
        }
        return Ok(session);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<Session>>> GetSessions(
        [FromQuery] SessionSearchRequest request,
        int page = 1,
        int pageSize = 10
    )
    {
        var response = await _sessionService.GetSessions(request, page, pageSize);

        return Ok(response);
    }
}
