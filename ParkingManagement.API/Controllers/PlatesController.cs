using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Application.DTOs;
using ParkingManagement.Application.Services;

namespace ParkingManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlatesController : ControllerBase
    {
        private readonly IParkingService _parkingService;

        public PlatesController(IParkingService parkingService)
        {
            _parkingService = parkingService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPlate([FromBody] PlateDetectionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PlateNumber))
            {
                return BadRequest(ApiResponse<object>.CreateFailure("Plate number is required."));
            }

            var result = await _parkingService.ProcessPlateAsync(request.PlateNumber.Trim().ToUpper());

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }
    }
}
