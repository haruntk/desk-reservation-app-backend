namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// Service interface for reservation status management operations
    /// This allows Application layer to request status updates without knowing implementation details
    /// </summary>
    public interface IReservationStatusService
    {
        /// <summary>
        /// Updates expired reservations and activates scheduled reservations
        /// </summary>
        /// <returns>Number of reservations updated</returns>
        Task<int> UpdateReservationStatusesAsync();

        /// <summary>
        /// Gets the configuration for reservation status management
        /// </summary>
        /// <returns>Configuration options</returns>
        Task<Domain.Configuration.ReservationStatusOptions> GetConfigurationAsync();
    }
}
