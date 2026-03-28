using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.Application.DTOs;
using ParkingManagement.Application.Interfaces;
using ParkingManagement.Domain.Entities;

namespace ParkingManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintsController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public ComplaintsController(IApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> LogComplaint([FromBody] ComplaintRequest request)
        {
            // 1. Find the vehicle
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.PlateNumber == request.PlateNumber.ToUpper().Trim());

            if (vehicle == null)
            {
                return NotFound(ApiResponse<object>.CreateFailure($"Vehicle {request.PlateNumber} not found.", 404));
            }

            // 2. Log the complaint
            var complaint = new Complaint
            {
                VehicleId = vehicle.Id,
                PlateNumber = vehicle.PlateNumber,
                ReportedSlot = request.ReportedSlot,
                Description = request.Description
            };

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            var warningMessage = $"⚠️ You have parked in a different slot! Complaint logged for slot: {request.ReportedSlot}";

            return Ok(ApiResponse<object>.CreateSuccess(new
            {
                ComplaintId = complaint.Id,
                Message = warningMessage
            }, warningMessage));
        }
    }
}
