using Microsoft.EntityFrameworkCore;
using SessionControlService.Data;
using SessionControlService.Models;

namespace SessionControlService.Services;

public class UserService
{
    private readonly SessionDbContext _context;

    public UserService(SessionDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> AddUser(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        var newUser = _context.Users.Add(user).Entity;
        await _context.SaveChangesAsync();

        return newUser;
    }
}
