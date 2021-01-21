using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Repository.Interface;
using DTOs;
using Entities;
using Extensions;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikesService _likesService;
        public LikesController(ILikesService likesService)
        {
            _likesService = likesService;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            if (await _likesService.AddLike(User.GetUserId(), username)) return Ok();

            return BadRequest("Failed to like user");
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes(string predicate)
        {
            var users =  await _likesService.GetUserLikes(predicate, User.GetUserId());

            return Ok(users);
        }
    }

}