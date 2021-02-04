using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAdminService
    {
        Task<List<UserWithRoleDTO>> GetUsersWithRoles();
        Task<IList<string>> EditRoles(string username, string roles);
    }
}