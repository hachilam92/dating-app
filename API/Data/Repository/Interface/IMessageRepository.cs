using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Entities;
using Helpers;

namespace Data.Repository.Interface
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDTO>> GetMessageForUser();
        Task<IEnumerable<MessageDTO>> GetMessageThread(int currentUserId, int recipientId);
        Task<bool> SaveAllAsync();
    }
}