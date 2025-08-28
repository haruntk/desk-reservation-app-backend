using DeskReservationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeskReservationApp.Infrastructure.Persistance.Configurations
{
    /// <summary>
    /// Entity configuration for Role domain entity (Windows Authentication roles)
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Id)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(256);

            // Add unique constraint on Role Name
            builder.HasIndex(r => r.Name)
                .IsUnique()
                .HasDatabaseName("IX_Roles_Name");
        }
    }
}
