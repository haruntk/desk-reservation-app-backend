using DeskReservationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeskReservationApp.Infrastructure.Persistance.Configurations
{
    public class FloorConfiguration : IEntityTypeConfiguration<Floor>
    {
        public void Configure(EntityTypeBuilder<Floor> builder)
        {
            builder.HasKey(f => f.FloorId);

            builder.Property(f => f.FloorNumber)
                .IsRequired();

            builder.HasMany(f => f.Desks)
                .WithOne(d => d.Floor)
                .HasForeignKey(d => d.FloorId);
        }
    }
}
