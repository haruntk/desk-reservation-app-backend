namespace DeskReservationApp.Application.DTOs.Authentication
{
    /// <summary>
    /// Response DTO for user login
    /// </summary>
    public class LoginResponseDTO
    {
        public string JwtToken { get; set; } = string.Empty;
    }
}
