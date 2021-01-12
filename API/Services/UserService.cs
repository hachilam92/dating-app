using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
	}
}