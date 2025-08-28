namespace DeskReservationApp.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work interface for Windows Authentication operations
    /// </summary>
    public interface IAuthUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
