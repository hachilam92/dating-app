using API.Data;
using AutoMapper;
using CustomExceptions;
using DTOs;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Interface;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
  public class AccountService : IAccountService
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IMapper _mapper;
    public AccountService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IMapper mapper
    ) {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }
    public async Task<AppUser> AddUser(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.UserName)) return null;

            var user = _mapper.Map<AppUser>(registerDTO);

        user.UserName = registerDTO.UserName;

        var result = await  _userManager.CreateAsync(user, registerDTO.Password);
        
        if (!result.Succeeded) throw new BadRequestException(result.Errors.ToString());

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if (!roleResult.Succeeded) throw new BadRequestException(result.Errors.ToString());

            return user;
        }

    public async Task<AppUser> VerifyUser(LoginDTO loginDTO)
    {
        var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync<AppUser>(x => x.UserName == loginDTO.UserName);

        if(user == null) throw new UnauthorizedException("Invalid user name or password");

        var result = await _signInManager
            .CheckPasswordSignInAsync(user, loginDTO.Password, false);

        if(!result.Succeeded) throw new UnauthorizedException("Invalid user name or password");

            return user;
        }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username);
    }
}