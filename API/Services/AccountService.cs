using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore;
using Services.Interface;

namespace Services
{
  public class AccountService : IAccountService
  {
    private readonly DataContext _context;
    public AccountService(DataContext context) 
    {
        _context = context;
    }
    public async Task<AppUser> AddUser(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.UserName)) return null;

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDTO.UserName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<AppUser> VerifyUser(LoginDTO loginDTO)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync<AppUser>(x => x.UserName == loginDTO.UserName);

        if(user == null) return null;

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

        for (int i = 0; i < computedHash.Length; i++) 
        {
            if (computedHash[i] != user.PasswordHash[i]) return null;
        }

        return user;
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username);
    }
  }
}