using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using AutoMapper;
using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore;
using Services.Interface;

namespace Services
{
  public class AccountService : IAccountService
  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public AccountService(DataContext context, IMapper mapper) 
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<AppUser> AddUser(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.UserName)) return null;

        var user = _mapper.Map<AppUser>(registerDTO);

        user.UserName = registerDTO.UserName;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<AppUser> VerifyUser(LoginDTO loginDTO)
    {
        var user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync<AppUser>(x => x.UserName == loginDTO.UserName);

        if(user == null) return null;

        return user;
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username);
    }
  }
}