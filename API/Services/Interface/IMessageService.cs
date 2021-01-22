using System.Threading.Tasks;
using DTOs;

namespace Services.Interface
{
    public interface IMessageService
    {
        Task<MessageDTO> CreateMessage(string currentUsername, CreateMessageDTO createMessageDTO);
    }
}