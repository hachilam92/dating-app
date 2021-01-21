using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;

namespace Services.Interface
{
    public interface ILikesService
    {
        Task<bool> AddLike(int currentUserId, string likedUsername);
        Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int currentUserId);
    }
}