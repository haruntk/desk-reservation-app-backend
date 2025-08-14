using DeskReservationApp.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeskReservationApp.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

        public async Task AssignUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _roleManager.RoleExistsAsync(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task CreateRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task<(bool Succeeded, string UserId)> CreateUserAsync(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            return (result.Succeeded, user.Id);
        }

        public async Task<IList<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<string?> GetUserIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.Id;
        }

        public async Task<string?> GetUserNameByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName;
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return await _userManager.GetRolesAsync(user);
        }

        public async Task RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _roleManager.RoleExistsAsync(roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }
    }
}
