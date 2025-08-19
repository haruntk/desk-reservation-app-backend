namespace DeskReservationApp.Domain.Configuration
{
    /// <summary>
    /// Configuration options for reservation status management
    /// </summary>
    public class ReservationStatusOptions
    {
        public const string SectionName = "ReservationStatus";

        /// <summary>
        /// Interval in minutes for background service to check and update reservation statuses
        /// Default: 5 minutes
        /// </summary>
        public int BackgroundServiceIntervalMinutes { get; set; } = 5;

        /// <summary>
        /// Whether to enable automatic status updates via background service
        /// Default: true
        /// </summary>
        public bool EnableBackgroundService { get; set; } = true;

        /// <summary>
        /// Grace period in minutes after reservation end time before marking as completed
        /// This allows for late check-outs. Default: 15 minutes
        /// </summary>
        public int GracePeriodMinutes { get; set; } = 15;

        /// <summary>
        /// Whether to automatically activate scheduled reservations when their start time arrives
        /// Default: true
        /// </summary>
        public bool AutoActivateScheduledReservations { get; set; } = true;

        /// <summary>
        /// Whether to allow creating reservations in the past (for admin purposes)
        /// Default: false
        /// </summary>
        public bool AllowPastReservations { get; set; } = false;

        /// <summary>
        /// Maximum days in advance a reservation can be made
        /// Default: 30 days
        /// </summary>
        public int MaxAdvanceReservationDays { get; set; } = 30;
    }
}
