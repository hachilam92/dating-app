using AutoMapper;
using CustomExceptions;
using DTOs;
using Entities;
using Helpers;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Services.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IPhotoService photoService
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        public async Task<MemberDTO> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        public async Task<AppUser> GetUserByUsername(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<PagedList<MemberDTO>> GetUsers(string username, UserParams userParams)
        {
            var user = await GetUserByUsername(username);

            userParams.CurrentUsername = user.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            return await _userRepository.GetMembersAsync(userParams);
        }

        public async Task<bool> UpdateUser(MemberUpdateDTO memberUpdateDTO, string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDTO, user);

            _userRepository.Update(user);

            return await _userRepository.SaveAllAsync();
        }

        public async Task<Photo> AddPhotoAsync(string username, IFormFile file)
        {
            var user = await GetUserByUsername(username);

            var result = await _photoService.UploadPhotoAsync(file);

            if (result.Error != null) throw new BadRequestException(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            photo.IsMain = !user.Photos.Any();

            user.Photos.Add(photo);

            return await _userRepository.SaveAllAsync() ? photo : null;
        }

        public async Task<bool> SetMainPhotoAsync(string username, int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return true;

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return true;

            return false;
        }

        public async Task<bool> DeletePhotoAsync(string username, int photoId)
        {
            var user = await GetUserByUsername(username);

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) throw new NotFoundException();

            if (photo.IsMain) throw new BadRequestException("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) throw new BadRequestException(result.Error.Message);
            }

            user.Photos.Remove(photo);

            return await _userRepository.SaveAllAsync();
        }
    }
}