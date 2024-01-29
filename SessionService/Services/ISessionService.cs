using SessionControlService.Controllers.ControllerModels;
using SessionControlService.Helpers.Pagination;
using SessionControlService.Models;

namespace SessionControlService.Services;

public interface ISessionService
{
    Task<SessionResponse> CreateSession(SessionRequest sessionRequest, Guid userId);
    Task<Session> GetSession(Guid id, bool onlyActive = false);
    Task<PagedResult<Session>> GetSessions(
        SessionSearchRequest searchRequest,
        int page = 1,
        int pageSize = 10
    );
    Task HandlePing(Ping ping);
    Task EndSession(Session session);
}
