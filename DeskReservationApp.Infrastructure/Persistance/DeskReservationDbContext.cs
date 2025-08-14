using Microsoft.EntityFrameworkCore;
using DeskReservationApp.Domain.Entities;
using System.Reflection;

namespace DeskReservationApp.Infrastructure.Persistance
{
    /// <summary>
    /// Business DbContext for desk reservation domain entities
    /// Migrated from old project Data/DeskReservationDbContext.cs
    /// </summary>
    public class DeskReservationDbContext : DbContext
    {
        public DeskReservationDbContext(DbContextOptions<DeskReservationDbContext> options) : base(options)
        {
        }

        public DbSet<Floor> Floors { get; set; }
        public DbSet<Desk> Desks { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
