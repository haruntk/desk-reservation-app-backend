using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskReservationApp.Domain.Entities
{
    public class Role
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public Role()
        {
        }

        public Role(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }
    }
}
