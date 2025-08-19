using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeskReservationApp.Infrastructure.Services
{
    /// <summary>
    /// Background service to automatically update reservation statuses based on time
    /// </summary>
    public class ReservationStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationStatusBackgroundService> _logger;
        private readonly ReservationStatusOptions _options;
        private readonly TimeSpan _interval;

        public ReservationStatusBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ReservationStatusBackgroundService> logger,
            IOptions<ReservationStatusOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
            _interval = TimeSpan.FromMinutes(_options.BackgroundServiceIntervalMinutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.EnableBackgroundService)
            {
                _logger.LogInformation("Reservation Status Background Service is disabled");
                return;
            }

            _logger.LogInformation($"Reservation Status Background Service started (interval: {_options.BackgroundServiceIntervalMinutes} minutes)");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateExpiredReservationsAsync();
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating reservation statuses");
                    // Continue running even if an error occurs
                    await Task.Delay(_interval, stoppingToken);
                }
            }

            _logger.LogInformation("Reservation Status Background Service stopped");
        }

        private async Task UpdateExpiredReservationsAsync()
        {
            // Singleton service'e Scoped service inject edilemez dolayisiyla DI yerine bu implementasyon yapildi.
            using var scope = _serviceProvider.CreateScope();
            var statusService = scope.ServiceProvider.GetRequiredService<IReservationStatusService>();

            try
            {
                var updatedCount = await statusService.UpdateReservationStatusesAsync();
                if (updatedCount > 0)
                {
                    _logger.LogInformation($"Background service updated {updatedCount} reservations");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background service while updating reservations");
                throw;
            }
        }
    }
}
