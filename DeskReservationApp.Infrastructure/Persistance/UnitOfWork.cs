using DeskReservationApp.Domain.Interfaces;
using DeskReservationApp.Infrastructure.Persistance.Repositories;

namespace DeskReservationApp.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DeskReservationDbContext _context;
        public IDeskRepository Desks { get; }
        public IFloorRepository Floors { get; }
        public IReservationRepository Reservations { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(
            DeskReservationDbContext context,
            IDeskRepository deskRepository,
            IFloorRepository floorRepository,
            IReservationRepository reservationRepository,
            IUserRepository userRepository)
        {
            _context = context;
            Desks = deskRepository;
            Floors = floorRepository;
            Reservations = reservationRepository;
            Users = userRepository;
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
