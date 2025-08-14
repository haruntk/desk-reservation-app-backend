namespace DeskReservationApp.Application.DTOs.User
{
    /// <summary>
    /// User data transfer object
    /// </summary>
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
