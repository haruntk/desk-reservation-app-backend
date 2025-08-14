using DeskReservationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeskReservationApp.Infrastructure.Persistance.Configurations
{
    public class DeskConfiguration : IEntityTypeConfiguration<Desk>
    {
        public void Configure(EntityTypeBuilder<Desk> builder)
        {
            builder.HasKey(d => d.DeskId);

            builder.Property(d => d.DeskName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(d => d.Floor)
                .WithMany(f => f.Desks)
                .HasForeignKey(d => d.FloorId);
        }
    }
}
