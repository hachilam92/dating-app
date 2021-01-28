using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using Interfaces;
using Services.Interface;
using System.Linq;
using Entities;

namespace Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IAccountService _accountService;
        public AccountController(
            ITokenService tokenService,
            IAccountService accountService
        ) {
            _tokenService = tokenService;
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            var user = await _accountService.AddUser(registerDTO);
             
            if (user == null) return BadRequest("Username is taken");

            return new UserDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            AppUser user = await _accountService.VerifyUser(loginDTO);

            return new UserDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
    }
}