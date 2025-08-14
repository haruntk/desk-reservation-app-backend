using DeskReservationApp.Domain.Entities;

namespace DeskReservationApp.Application.Interfaces
{
    /// <summary>
    /// Token service interface for JWT operations
    /// </summary>
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string email, IList<string> roles);
    }
}
