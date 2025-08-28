using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Infrastructure.Persistance
{
    /// <summary>
    /// Unit of Work implementation for Windows Authentication database
    /// </summary>
    public class AuthUnitOfWork : IAuthUnitOfWork
    {
        private readonly DeskReservationAuthDbContext _context;

        public AuthUnitOfWork(DeskReservationAuthDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
