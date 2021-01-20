using System.Threading.Tasks;
using DTOs;
using Entities;

namespace Services.Interface
{
    public interface IAccountService
    {
        Task<AppUser> AddUser(RegisterDTO registerDTO);

        Task<AppUser> VerifyUser(LoginDTO loginDTO);
    }
}