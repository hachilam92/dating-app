using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Data.Repository.Interface;
using DTOs;
using Entities;
using Extensions;
using Helpers;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageService _messageService;
        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var message = await _messageService.CreateMessage(User.GetUsername(), createMessageDTO);

            if (message != null) return Ok(message);

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser(
            [FromQuery]MessageParams messageParams
        ) {
            var messages = await _messageService.GetMessagesForUser(
                User.GetUsername(),
                messageParams);
            
            Response.AddPaginationHeader(
                messages.CurrentPage,
                messages.PageSize,
                messages.TotalCount,
                messages.TotalPages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
            return Ok(await _messageService.GetMessageThread(User.GetUsername(), username));
        }
    }
}