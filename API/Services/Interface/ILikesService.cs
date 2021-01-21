using System.Threading.Tasks;
using DTOs;
using Helpers;

namespace Services.Interface
{
    public interface ILikesService
    {
        Task<bool> AddLike(int currentUserId, string likedUsername);
        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
}