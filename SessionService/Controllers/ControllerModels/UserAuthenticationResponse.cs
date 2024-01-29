using SessionControlService.Models;

namespace SessionControlService.Controllers.ControllerModels;

public class UserAuthenticationResponse
{
    public string Token { get; set; }
    public User User { get; set; }

    public UserAuthenticationResponse() { }

    public UserAuthenticationResponse(string token, User user)
    {
        Token = token;
        User = user;
    }
}
