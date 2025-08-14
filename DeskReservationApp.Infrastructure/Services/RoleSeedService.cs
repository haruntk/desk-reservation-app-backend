using Microsoft.AspNetCore.Identity;

namespace DeskReservationApp.Infrastructure.Services
{
    /// <summary>
    /// Service to seed initial roles in the system
    /// </summary>
    public class RoleSeedService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeedService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            // Admin rolü
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // User rolü
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Takım lideri rolü 
            if (!await _roleManager.RoleExistsAsync("TeamLead"))
            {
                await _roleManager.CreateAsync(new IdentityRole("TeamLead"));
            }
        }
    }
}
