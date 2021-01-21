using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Entities;

namespace Data.Repository.Interface
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId);

    }
}