using DeskReservationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace DeskReservationApp.Infrastructure.Persistance.Configurations
{
    /// <summary>
    /// Entity configuration for User domain entity (Windows Authentication users)
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Id)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            // Roles are configured through UserRole many-to-many relationship
            // No need for JSON conversion - EF Core handles this through navigation properties

            // Add unique constraint on UserName
            builder.HasIndex(u => u.UserName)
                .IsUnique()
                .HasDatabaseName("IX_WindowsUsers_UserName");

            // Add index on Email
            builder.HasIndex(u => u.Email)
                .HasDatabaseName("IX_WindowsUsers_Email");
        }
    }
}
