using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Controllers;
using DTOs;
using Entities;
using Extensions;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        public UsersController(
            IUserService userService,
            IPhotoService photoService,
            IMapper mapper
        )
        {
            _userService = userService;
            _photoService = photoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await _userService.GetUsers();

            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
           return await _userService.GetUser(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            bool isUpdated = await _userService.UpdateUser(memberUpdateDTO, User.GetUsername());

            if (isUpdated) { return NoContent(); }

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await _userService.GetUserByUsername(User.GetUsername());

            var result = await _photoService.UploadPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = await _photoService.AddPhotoAsync(result, user);

            if (photo != null) 
            {
                return CreatedAtRoute(
                    "GetUser",
                    new { username = user.UserName },
                    _mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var result = await _photoService.SetMainPhotoAsync(User.GetUsername(), photoId);

            return result ? NoContent() : BadRequest("Fail to set main photo");
        }
    }
}