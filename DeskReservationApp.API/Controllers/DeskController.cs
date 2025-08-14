using DeskReservationApp.Application.DTOs.Desk;
using DeskReservationApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeskReservationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeskController : ControllerBase
    {
        private readonly IDeskService _deskService;

        public DeskController(IDeskService deskService)
        {
            _deskService = deskService;
        }

        /// <summary>
        /// Get all desks
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllDesks()
        {
            var response = await _deskService.GetAllDesksAsync();
            return Ok(response);
        }

        /// <summary>
        /// Get desk by ID
        /// </summary>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetDesk(int id)
        {
            var response = await _deskService.GetDeskByIdAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Get desks by floor ID
        /// </summary>
        [HttpGet("floor/{floorId}")]
        public async Task<IActionResult> GetDesksByFloor(int floorId)
        {
            var response = await _deskService.GetDesksByFloorIdAsync(floorId);
            return Ok(response);
        }

        /// <summary>
        /// Get available desks for a time period
        /// </summary>
        [HttpPost("available")]
        public async Task<IActionResult> GetAvailableDesks(DeskAvailabilityRequestDTO availabilityRequest)
        {
            var response = await _deskService.GetAvailableDesksAsync(availabilityRequest);
            return Ok(response);
        }

        /// <summary>
        /// Create a new desk
        /// </summary>
        [HttpPost("create")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> CreateDesk(CreateDeskRequestDTO createDeskRequest)
        {
            var deskId = await _deskService.CreateDeskAsync(createDeskRequest);
            return CreatedAtAction(nameof(GetDesk), new { id = deskId }, null);
        }

        /// <summary>
        /// Update an existing desk
        /// </summary>
        [HttpPut("{id}/update")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> UpdateDesk(int id, UpdateDeskRequestDTO updateDeskRequest)
        {
            await _deskService.UpdateDeskAsync(id, updateDeskRequest);
            return NoContent();
        }

        /// <summary>
        /// Delete a desk
        /// </summary>
        [HttpDelete("{id}/delete")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteDesk(int id)
        {
            await _deskService.DeleteDeskAsync(id);
            return NoContent();
        }
    }
}
