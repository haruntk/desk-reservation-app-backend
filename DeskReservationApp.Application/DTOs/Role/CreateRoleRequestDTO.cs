using System.ComponentModel.DataAnnotations;

namespace DeskReservationApp.Application.DTOs.Role
{
    /// <summary>
    /// Request DTO for creating a new role
    /// </summary>
    public class CreateRoleRequestDTO
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
