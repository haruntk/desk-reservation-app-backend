namespace DeskReservationApp.Infrastructure.Persistance.Configurations
{
    /// <summary>
    /// JWT configuration options
    /// Migrated from old project Services/JwtOptions.cs
    /// </summary>
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int AccessTokenMinutes { get; set; }
    }
}
