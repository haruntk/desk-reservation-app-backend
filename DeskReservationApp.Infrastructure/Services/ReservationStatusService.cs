using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Configuration;
using DeskReservationApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeskReservationApp.Infrastructure.Services
{
    /// <summary>
    /// Reservation status management
    /// </summary>
    public class ReservationStatusService : IReservationStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReservationStatusService> _logger;
        private readonly ReservationStatusOptions _options;

        public ReservationStatusService(
            IUnitOfWork unitOfWork,
            ILogger<ReservationStatusService> logger,
            IOptions<ReservationStatusOptions> options)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<ReservationStatusOptions> GetConfigurationAsync()
        {
            return await Task.FromResult(_options);
        }

        public async Task<int> UpdateReservationStatusesAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var gracePeriodEnd = now.AddMinutes(-_options.GracePeriodMinutes);
                
                // Get all active reservations that have passed their end time (including grace period)
                var expiredReservations = await _unitOfWork.Reservations.GetExpiredActiveReservationsAsync(gracePeriodEnd);
                
                // Get all scheduled reservations that should now be active (if enabled)
                var scheduledReservations = _options.AutoActivateScheduledReservations 
                    ? await _unitOfWork.Reservations.GetScheduledReservationsToActivateAsync(now)
                    : Enumerable.Empty<Domain.Entities.Reservation>();

                var totalUpdated = 0;

                // Update expired reservations to completed
                if (expiredReservations.Any())
                {
                    _logger.LogInformation($"Found {expiredReservations.Count()} expired reservations to complete");

                    foreach (var reservation in expiredReservations)
                    {
                        reservation.Status = "Completed";
                        _unitOfWork.Reservations.Update(reservation);
                        
                        _logger.LogDebug($"Updated reservation {reservation.ReservationId} from Active to Completed");
                    }
                    totalUpdated += expiredReservations.Count();
                }

                // Update scheduled reservations to active
                if (scheduledReservations.Any())
                {
                    _logger.LogInformation($"Found {scheduledReservations.Count()} scheduled reservations to activate");

                    foreach (var reservation in scheduledReservations)
                    {
                        reservation.Status = "Active";
                        _unitOfWork.Reservations.Update(reservation);
                        
                        _logger.LogDebug($"Updated reservation {reservation.ReservationId} from Scheduled to Active");
                    }
                    totalUpdated += scheduledReservations.Count();
                }

                if (totalUpdated > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Successfully updated {totalUpdated} reservations");
                }

                return totalUpdated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reservations");
                throw;
            }
        }
    }
}
