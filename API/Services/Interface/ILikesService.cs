using DTOs;
using Helpers;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ILikesService
    {
        Task<bool> AddLike(int currentUserId, string likedUsername);
        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
}