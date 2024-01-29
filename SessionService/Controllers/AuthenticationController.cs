using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionControlService.Controllers.ControllerModels;
using SessionControlService.Helpers;
using SessionControlService.Services;

namespace SessionControlService.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBaseExtended
{
    private readonly AuthenticationService _authenticationService;

    public AuthenticationController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("registration")]
    public async Task<ActionResult<UserAuthenticationResponse>> Register(
        [FromBody] RegistrationRequest model
    )
    {
        return Created("", await _authenticationService.Register(model));
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserAuthenticationResponse>> Authenticate(
        [FromBody] UserLoginRequest model
    )
    {
        return Ok(await _authenticationService.Authenticate(model.Email, model.Password));
    }
}
