using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeskReservationApp.Application.DTOs.Floor;
using DeskReservationApp.Application.Interfaces;

namespace DeskReservationApp.API.Controllers
{
    /// <summary>
    /// Floor controller for managing floor operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FloorController : ControllerBase
    {
        private readonly IFloorService _floorService;

        public FloorController(IFloorService floorService)
        {
            _floorService = floorService;
        }

        /// <summary>
        /// Get all floors
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllFloors()
        {
            var response = await _floorService.GetAllFloorsAsync();
            return Ok(response);
        }

        /// <summary>
        /// Get floor by ID
        /// </summary>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetFloorById(int id)
        {
            var response = await _floorService.GetFloorByIdAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Create a new floor
        /// </summary>
        [HttpPost("create")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> CreateFloor([FromBody] CreateFloorRequestDTO request)
        {
            var floorId = await _floorService.CreateFloorAsync(request);
            return CreatedAtAction(nameof(GetFloorById), new { id = floorId }, null);
        }

        /// <summary>
        /// Update a floor
        /// </summary>
        [HttpPut("{id}/update")]
        [Authorize(Policy = "TeamLeadOrAdmin")]
        public async Task<IActionResult> UpdateFloor(int id, [FromBody] UpdateFloorRequestDTO request)
        {
            await _floorService.UpdateFloorAsync(id, request);
            return NoContent();
        }

        /// <summary>
        /// Delete a floor
        /// </summary>
        [HttpDelete("{id}/delete")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteFloor(int id)
        {
            await _floorService.DeleteFloorAsync(id);
            return NoContent();
        }
    }
}
