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
        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();

        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        public User(string userName, string email) : this()
        {
            UserName = userName;
            Email = email;
        }

        public void AssignRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (Roles.Any(ur => ur.RoleId == role.Id)) return;
            
            Roles.Add(new UserRole { UserId = Id, RoleId = role.Id, User = this, Role = role });
        }

        public void RemoveRole(Role role)
        {
            if (role == null) return;
            
            var userRole = Roles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (userRole != null)
            {
                Roles.Remove(userRole);
            }
        }

        public bool HasRole(string roleName)
        {
            return Roles.Any(ur => ur.Role?.Name?.Equals(roleName, StringComparison.OrdinalIgnoreCase) == true);
        }
    }
}
