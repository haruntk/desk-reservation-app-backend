using DeskReservationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeskReservationApp.Infrastructure.Persistance.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(r => r.ReservationId);

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(r => r.Desk)
                .WithMany(d => d.Reservations)
                .HasForeignKey(r => r.DeskId);
        }
    }
}
