using DeskReservationApp.Application.DTOs.Reservation;
using DeskReservationApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeskReservationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Get all reservations (Admin or teamlead only)
        /// </summary>
        [HttpGet("get-all")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> GetAllReservations()
        {
            var response = await _reservationService.GetAllReservationsAsync();
            return Ok(response);
        }

        /// <summary>
        /// Get reservation by ID
        /// </summary>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var response = await _reservationService.GetReservationByIdAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Get current user's reservations
        /// </summary>
        [HttpGet("my-reservations")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _reservationService.GetUserReservationsAsync(userId);
            return Ok(response);
        }

        /// <summary>
        /// Get reservations by desk ID
        /// </summary>
        [HttpGet("desk/{deskId}")]
        public async Task<ActionResult<IEnumerable<ReservationResponseDTO>>> GetReservationsByDesk(int deskId)
        {
            var reservations = await _reservationService.GetReservationsByDeskIdAsync(deskId);
            return Ok(reservations);
        }

        /// <summary>
        /// Get all active reservations
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ReservationResponseDTO>>> GetActiveReservations()
        {
            var reservations = await _reservationService.GetActiveReservationsAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// Get user's past reservations
        /// </summary>
        [HttpGet("past")]
        public async Task<ActionResult<IEnumerable<ReservationResponseDTO>>> GetPastReservations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var reservations = await _reservationService.GetPastReservationsAsync(userId);
            return Ok(reservations);
        }

        /// <summary>
        /// Get user's upcoming reservations
        /// </summary>
        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<ReservationResponseDTO>>> GetUpcomingReservations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var reservations = await _reservationService.GetUpcomingReservationsAsync(userId);
            return Ok(reservations);
        }

        /// <summary>
        /// Create a new reservation
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateReservation(CreateReservationRequestDTO createReservationRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var reservationId = await _reservationService.CreateReservationAsync(userId, createReservationRequest);
            return CreatedAtAction(nameof(GetReservation), new { id = reservationId }, null);
        }

        /// <summary>
        /// Update an existing reservation
        /// </summary>
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ReservationResponseDTO>> UpdateReservation(int id, UpdateReservationRequestDTO updateReservationRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _reservationService.UpdateReservationAsync(id, userId, updateReservationRequest);
            return NoContent();
        }

        /// <summary>
        /// Update reservation status
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ReservationResponseDTO>> UpdateReservationStatus(int id, UpdateReservationStatusRequestDTO updateStatusRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _reservationService.UpdateReservationStatusAsync(id, userId, updateStatusRequest);
            return NoContent();
        }

        /// <summary>
        /// Cancel a reservation
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _reservationService.CancelReservationAsync(id, userId);
            return NoContent();
        }

        /// <summary>
        /// Delete a reservation (Admin only)
        /// </summary>
        [HttpDelete("{id}/delete")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteReservation(int id)
        {
            await _reservationService.DeleteReservationAsync(id);
            return NoContent();
        }
    }
}
