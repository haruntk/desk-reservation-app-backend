using DeskReservationApp.Domain.Entities;
using DeskReservationApp.Infrastructure.Persistance.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DeskReservationApp.Infrastructure.Persistance
{
    /// <summary>
    /// Windows Authentication DbContext - separate from business data
    /// </summary>
    public class DeskReservationAuthDbContext : DbContext
    {
        public DeskReservationAuthDbContext(DbContextOptions<DeskReservationAuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

            // Seed default roles with fixed IDs for consistency
            var userRoleId = "550e8400-e29b-41d4-a716-446655440001";
            var teamLeadRoleId = "550e8400-e29b-41d4-a716-446655440002";
            var adminRoleId = "550e8400-e29b-41d4-a716-446655440003";

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = userRoleId, Name = "User" },
                new Role { Id = teamLeadRoleId, Name = "TeamLead" },
                new Role { Id = adminRoleId, Name = "Admin" }
            );
        }
    }
}
