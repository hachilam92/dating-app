using System.Threading.Tasks;
using AutoMapper;
using CustomExceptions;
using Data.Repository.Interface;
using DTOs;
using Entities;
using Interfaces;
using Services.Interface;

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
        ) {
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
    }
}