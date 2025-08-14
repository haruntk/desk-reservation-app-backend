namespace DeskReservationApp.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDeskRepository Desks { get; }
        IFloorRepository Floors { get; }
        IReservationRepository Reservations { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
