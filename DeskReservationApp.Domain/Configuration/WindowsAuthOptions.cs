namespace DeskReservationApp.Domain.Configuration
{
    /// <summary>
    /// Configuration options for Windows Authentication
    /// </summary>
    public class WindowsAuthOptions
    {
        public const string SectionName = "WindowsAuth";

        /// <summary>
        /// Default domain for email generation if not provided
        /// </summary>
        public string DefaultEmailDomain { get; set; } = "company.com";

        /// <summary>
        /// Enable automatic user creation on first login
        /// </summary>
        public bool AutoCreateUsers { get; set; } = true;

        /// <summary>
        /// Default role assigned to new users
        /// </summary>
        public string DefaultUserRole { get; set; } = "User";

        /// <summary>
        /// List of predefined roles in the system
        /// </summary>
        public string[] PredefinedRoles { get; set; } = { "User", "TeamLead", "Admin" };

        /// <summary>
        /// Enable detailed logging for authentication operations
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = true;
    }
}
