using System.Threading.Tasks;
using AutoMapper;
using Controllers;
using DTOs;
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
            var users = await _userService.GetUsers(User.GetUsername(), userParams);

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
            var photo = await _userService.AddPhotoAsync(User.GetUsername(), file);

            if (photo != null) 
            {
                return CreatedAtRoute(
                    "GetUser",
                    new { username = User.GetUsername() },
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
            if (await _userService.DeletePhotoAsync(User.GetUsername(), photoId)) return Ok();

            return BadRequest("Fail to delete the photo");
        } 
    }
}