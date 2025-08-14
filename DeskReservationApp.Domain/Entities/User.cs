namespace DeskReservationApp.Domain.Entities
{
    /// <summary>
    /// Domain entity representing a user.
    /// </summary>
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
