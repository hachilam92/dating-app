using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet.Actions;
using DTOs;
using Entities;
using Interfaces;
using Services.Interface;

namespace Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		public UserService(IUserRepository userRepository, IMapper mapper)
		{
			_userRepository = userRepository;
			_mapper = mapper;
		}

		public async Task<MemberDTO> GetUser(string username)
		{
			return await _userRepository.GetMemberAsync(username);
		}
	
		public async Task<AppUser> GetUserByUsername(string username)
		{
			return await _userRepository.GetUserByUsernameAsync(username);
		}

		public async Task<IEnumerable<MemberDTO>> GetUsers()
		{
			return await _userRepository.GetMembersAsync();
		}

		public async Task<bool> UpdateUser(MemberUpdateDTO memberUpdateDTO, string username)
		{
			var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDTO, user);

            _userRepository.Update(user);

            return await _userRepository.SaveAllAsync();
		}

		public async Task<Photo> AddPhotoAsync(ImageUploadResult result, AppUser user)
		{
			var photo = new Photo
			{
				Url = result.SecureUrl.AbsoluteUri,
				PublicId = result.PublicId
			};

			if(user.Photos.Count == 0)
			{
				photo.IsMain = true;
			}

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

		public async Task<bool> DeletePhotoAsync(AppUser user, Photo photo)
		{
			user.Photos.Remove(photo);

			return await _userRepository.SaveAllAsync();
		}
	}
}