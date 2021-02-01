using AutoMapper;
using CustomExceptions;
using Data.Repository.Interface;
using DTOs;
using Entities;
using Helpers;
using Interfaces;
using Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class MessageService : IMessageService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessageService(
            IUserRepository userRepository,
            IMessageRepository messageRepository,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<MessageDTO> CreateMessage(string currentUsername, CreateMessageDTO createMessageDTO)
        {
            if (currentUsername == createMessageDTO.RecipientUsername)
            {
                throw new BadRequestException("You cannot send message to yourself");
            }

            var sender = await _userRepository.GetUserByUsernameAsync(currentUsername);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null) throw new NotFoundException();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return _mapper.Map<MessageDTO>(message);

            return null;
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(
            string currentUsername,
            MessageParams messageParams
        )
        {
            messageParams.Username = currentUsername;

            return await _messageRepository.GetMessageForUser(messageParams);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string username)
        {
            return await _messageRepository.GetMessageThread(currentUsername, username);
        }

        public async Task<bool> DeleteMessage(string currentUsername, int id)
        {
            var message = await _messageRepository.GetMessage(id);

            if (message.Sender.UserName != currentUsername
                && message.Recipient.UserName != currentUsername)
            {
                throw new UnauthorizedException();
            }

            if (message.Sender.UserName == currentUsername)
            {
                message.SenderDelted = true;
            }

            if (message.Recipient.UserName == currentUsername)
            {
                message.RecipientDelted = true;
            }

            if (message.SenderDelted && message.RecipientDelted)
            {
                _messageRepository.DeleteMessage(message);
            }

            return await _messageRepository.SaveAllAsync();
        }
    }
}