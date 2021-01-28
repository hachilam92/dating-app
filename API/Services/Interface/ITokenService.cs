using System.Threading.Tasks;
using Entities;

namespace Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}