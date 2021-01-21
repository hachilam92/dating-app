using System.Collections.Generic;
using System.Threading.Tasks;
using CustomExceptions;
using Data.Repository.Interface;
using DTOs;
using Entities;
using Interfaces;
using Services.Interface;

namespace Services
{
    public class LikesService : ILikesService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likeRepository;
        public LikesService(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likeRepository = likesRepository;
        }

        public async Task<bool> AddLike(int currentUserId, string likedUsername)
        {
            var LikedUser = await _userRepository.GetUserByUsernameAsync(likedUsername);
            var SourceUser = await _likeRepository.GetUserWithLikes(currentUserId);

            if (SourceUser.UserName == likedUsername) throw new BadRequestException("You cannot like yourself");

            var userLike = await _likeRepository.GetUserLike(currentUserId, LikedUser.Id);

            if (userLike != null) throw new BadRequestException("You already like this user"); 

            userLike = new UserLike
            {
                SourceUserId = currentUserId,
                LikedUserId = LikedUser.Id,
            };

            SourceUser.LikedUsers.Add(userLike);

            return await _userRepository.SaveAllAsync();
        }

        public async Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int currentUserId)
        {
            return await _likeRepository.GetUserLikes(predicate, currentUserId);
        }
    }
}