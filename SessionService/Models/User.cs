using SessionControlService.Helpers;
using System.ComponentModel.DataAnnotations;

namespace SessionControlService.Models
{
    public class User
    {
        public Guid Id { get; set; } = IdProvider.NewId();

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
