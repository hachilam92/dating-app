using CustomExceptions;
using DTOs;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserWithRoleDTO>> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new UserWithRoleDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return users;
        }

        public async Task<IList<string>> EditRoles(string username, string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) throw new NotFoundException("Could not find user");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) throw new BadRequestException("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) throw new BadRequestException("Failed to remove from roles");

            return await _userManager.GetRolesAsync(user);
        }
    }
}