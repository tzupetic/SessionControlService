using SessionControlService.Controllers.ControllerModels;
using SessionControlService.Helpers;
using SessionControlService.Models;

namespace SessionControlService.Services;

public class AuthenticationService
{
    private readonly UserService _userService;

    public AuthenticationService(UserService userService)
    {
        _userService = userService;
    }

    public UserAuthenticationResponse GenerateAuthenticationResponse(User user)
    {
        return new UserAuthenticationResponse(AuthenticationHelper.GenerateToken(user), user);
    }

    public async Task<UserAuthenticationResponse> Register(RegistrationRequest model)
    {
        var existingUser = await _userService.GetUserByEmail(model.Email);

        if (existingUser != null)
            throw new BadRequestException("Email is already registered");

        var user = new User
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };

        if (model.Password != null)
            user.Password = AuthenticationHelper.HashPassword(user, model.Password);

        await _userService.AddUser(user);

        return GenerateAuthenticationResponse(user);
    }

    public async Task<UserAuthenticationResponse> Authenticate(string email, string password)
    {
        var user =
            await _userService.GetUserByEmail(email)
            ?? throw new UnauthorizedException("Invalid email or password");

        if (!AuthenticationHelper.VerifyPassword(user, password))
            throw new UnauthorizedException("Invalid email or password");

        return GenerateAuthenticationResponse(user);
    }
}
