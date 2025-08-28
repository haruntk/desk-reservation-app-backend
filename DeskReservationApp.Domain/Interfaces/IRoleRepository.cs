using DeskReservationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskReservationApp.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(string id);
        Task<Role> GetByNameAsync(string roleName);
        Task AddAsync(Role role);
        Task<List<Role>> GetAllAsync();
    }
}
