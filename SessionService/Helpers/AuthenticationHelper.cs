using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SessionControlService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SessionControlService.Helpers;

public class AuthenticationHelper
{
    public static string GenerateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(StaticConfiguration.AppSettingsSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(
                Convert.ToInt32(StaticConfiguration.AppSettingsExpirationDays)
            ),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string HashPassword(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.HashPassword(user, password);
    }

    public static bool VerifyPassword(User user, string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        var verified = false;
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
        switch (result)
        {
            case PasswordVerificationResult.Success:
            case PasswordVerificationResult.SuccessRehashNeeded:
                verified = true;
                break;
            case PasswordVerificationResult.Failed:
                verified = false;
                break;
            default:
                verified = false;
                break;
        }

        return verified;
    }
}
