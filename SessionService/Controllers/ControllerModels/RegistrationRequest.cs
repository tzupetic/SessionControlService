using System.ComponentModel.DataAnnotations;

namespace SessionControlService.Controllers.ControllerModels;

public class RegistrationRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }
}
