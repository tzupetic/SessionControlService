using SessionControlService.Models;

namespace SessionControlService.Controllers.ControllerModels;

public class Ping
{
    public Guid SessionId { get; set; }
    public SessionState SessionState { get; set; }
}
