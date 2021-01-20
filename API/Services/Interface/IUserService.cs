using System.Collections.Generic;
using System.Threading.Tasks;
using DTOs;
using Entities;

namespace Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<MemberDTO>> GetUsers();
        Task<MemberDTO> GetUser(string username);
        Task<AppUser> GetUserByUsername(string username);
        Task<bool> UpdateUser(MemberUpdateDTO memberUpdateDTO, string username);
    }
}