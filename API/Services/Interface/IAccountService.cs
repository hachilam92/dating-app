using DTOs;
using Entities;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAccountService
    {
        Task<AppUser> AddUser(RegisterDTO registerDTO);

        Task<AppUser> VerifyUser(LoginDTO loginDTO);
    }
}