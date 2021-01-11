using System.Threading.Tasks;
using API.Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using Interfaces;
using Services.Interface;

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
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            AppUser user = await _accountService.VerifyUser(loginDTO);
            
            if (user == null) return Unauthorized("Invalid user name or password");

            return new UserDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
            };
        }
    }
}