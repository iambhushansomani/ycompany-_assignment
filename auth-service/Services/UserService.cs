using YCompanyClaimsApi.Data;
using YCompanyClaimsApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace YCompanyClaimsApi.Services
{
    public class UserService : IUserService
    {
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        if (password == passwordHash)
        {
            return true;
        }
        return false;
    }

    public User GetUserByUsername(string username)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Username == username);
    }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
    {
        return await _context.Users
            .Where(u => u.Role == role)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                // Email = u.Email,
                Role = u.Role,
                WorkshopId = u.WorkshopId  // Include WorkshopId in the DTO

            })
            .ToListAsync();
    }
    }

    
}