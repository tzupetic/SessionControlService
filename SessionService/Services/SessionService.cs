using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SessionControlService.Controllers.ControllerModels;
using SessionControlService.Data;
using SessionControlService.Helpers.Pagination;
using SessionControlService.Models;
using StackExchange.Redis;

namespace SessionControlService.Services;

public class SessionService : ISessionService
{
    private readonly SessionDbContext _context;
    private readonly IDatabase _redisDatabase;
    private IRecurringJobManager _recurringJobManager;

    public SessionService(
        SessionDbContext context,
        IConnectionMultiplexer redisConnectionMultiplexer,
        IRecurringJobManager recurringJobManager
    )
    {
        _context = context;
        _redisDatabase = redisConnectionMultiplexer.GetDatabase();
        _recurringJobManager = recurringJobManager;
    }

    public async Task<SessionResponse> CreateSession(SessionRequest sessionRequest, Guid userId)
    {
        var session = new Session
        {
            State = SessionState.Play,
            StartTime = DateTime.UtcNow,
            UserId = userId,
            ContentType = sessionRequest.ContentType,
            LastActivityTime = DateTime.UtcNow
        };

        _context.Add(session);
        await _context.SaveChangesAsync();

        await CacheSession(session);

        _recurringJobManager.AddOrUpdate(
            $"job_{session.Id}",
            () => CleanupIdleSession(session.Id),
            Cron.Minutely
        );

        var response = new SessionResponse { SessionId = session.Id };

        return response;
    }

    public async Task<Session> GetSession(Guid id, bool onlyActive = false)
    {
        var session = await TryGetSessionFromCache(id);

        if (session == null)
        {
            session =
                await _context.Sessions.FirstOrDefaultAsync(
                    s =>
                        s.Id == id
                        && (
                            !onlyActive
                            || (s.State != SessionState.Stop && s.State != SessionState.Closed)
                        )
                ) ?? throw new BadHttpRequestException("Session does not exist");

            if (session != null)
            {
                await CacheSession(session);
            }
        }

        return session;
    }

    public async Task<PagedResult<Session>> GetSessions(
        SessionSearchRequest searchRequest,
        int page = 1,
        int pageSize = 10
    )
    {
        var dateFrom = searchRequest.DateFrom.HasValue
            ? DateTime.SpecifyKind(searchRequest.DateFrom.Value, DateTimeKind.Utc)
            : (DateTime?)null;
        var dateTo = searchRequest.DateTo.HasValue
            ? DateTime.SpecifyKind(searchRequest.DateTo.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        var sessions = await _context.Sessions
            .Include(s => s.User)
            .Where(
                s =>
                    s.ContentType.ToLower().Contains(searchRequest.Content.ToLower())
                    && (!dateFrom.HasValue || s.LastActivityTime >= dateFrom)
                    && (!dateTo.HasValue || s.LastActivityTime <= dateTo)
            )
            .OrderBy(s => s.LastActivityTime)
            .GetPagedAsync(page, pageSize);
        return sessions;
    }

    public async Task HandlePing(Ping ping)
    {
        var session = await GetSession(ping.SessionId, true);

        if (session.State == SessionState.Play)
        {
            throw new Exception("Invalid SessionState");
        }

        session.LastActivityTime = DateTime.UtcNow;
        session.State = ping.SessionState;

        if (ping.SessionState == SessionState.Stop || ping.SessionState == SessionState.Closed)
            await EndSession(session);
        else
            await CacheSession(session);
    }

    private async Task CacheSession(Session session)
    {
        await _redisDatabase.StringSetAsync(
            $"Session:{session.Id}",
            JsonConvert.SerializeObject(session)
        );
    }

    private async Task<Session> TryGetSessionFromCache(Guid id)
    {
        var sessionJson = await _redisDatabase.StringGetAsync($"Session:{id}");

        if (!sessionJson.IsNullOrEmpty)
        {
            return JsonConvert.DeserializeObject<Session>(sessionJson);
        }

        return null;
    }

    public async Task EndSession(Session session)
    {
        session.EndTime = DateTime.UtcNow;
        session.LastActivityTime = DateTime.UtcNow;
        _context.Update(session);
        await _context.SaveChangesAsync();
        await _redisDatabase.KeyDeleteAsync($"Session:{session.Id}");
        _recurringJobManager.RemoveIfExists($"job_{session.Id}");
    }

    public async Task CleanupIdleSession(Guid id)
    {
        var session = await GetSession(id, true);

        if (session != null && session.LastActivityTime < DateTime.UtcNow.AddSeconds(-60))
        {
            session.State = SessionState.Closed;
            await EndSession(session);
        }
    }
}
