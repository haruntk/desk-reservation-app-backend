namespace DeskReservationApp.Application.DTOs.Role
{
    /// <summary>
    /// Response DTO for role assignment
    /// </summary>
    public class AssignRoleResponseDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
