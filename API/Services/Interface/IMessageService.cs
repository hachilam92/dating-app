using DTOs;
using Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMessageService
    {
        Task<MessageDTO> CreateMessage(string currentUsername, CreateMessageDTO createMessageDTO);
        Task<PagedList<MessageDTO>> GetMessagesForUser(string currentUsername, MessageParams messageParams);
        Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string username);
        Task<bool> DeleteMessage(string currentUsername, int id);
    }
}