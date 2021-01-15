using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Controllers;
using DTOs;
using Entities;
using Extensions;
using Helpers;
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
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await _userService.GetUserByUsername(User.GetUsername());

            userParams.CurrentUsername = user.UserName;

            if (string.IsNullOrEmpty(userParams.Gender)) 
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            var users = await _userService.GetUsers(userParams);

            Response.AddPaginationHeader(
                users.CurrentPage,
                users.PageSize,
                users.TotalCount,
                users.TotalPages
            );

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

            var photo = await _userService.AddPhotoAsync(result, user);

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
            var result = await _userService.SetMainPhotoAsync(User.GetUsername(), photoId);

            return result ? NoContent() : BadRequest("Fail to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userService.GetUserByUsername(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            if (await _userService.DeletePhotoAsync(user, photo)) return Ok();

            return BadRequest("Fail to delete the photo");
        } 
    }
}