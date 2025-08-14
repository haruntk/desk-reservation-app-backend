using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeskReservationApp.Infrastructure.Persistance
{
    /// <summary>
    /// Authentication DbContext for Identity framework
    /// Migrated from old project Data/DeskReservationAuthDbContext.cs
    /// </summary>
    public class DeskReservationAuthDbContext : IdentityDbContext
    {
        public DeskReservationAuthDbContext(DbContextOptions<DeskReservationAuthDbContext> options) : base(options)
        {
        }
    }
}
