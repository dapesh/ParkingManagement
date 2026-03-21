using Microsoft.EntityFrameworkCore;
using ParkingManagement.Application.DTOs;
using ParkingManagement.Application.Interfaces;
using ParkingManagement.Domain.Entities;

namespace ParkingManagement.Application.Services
{
    public interface IParkingService
    {
        Task<ApiResponse<object>> ProcessPlateAsync(string plateNumber);
    }

    public class ParkingService : IParkingService
    {
        private readonly IApplicationDbContext _context;

        public ParkingService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object>> ProcessPlateAsync(string plateNumber)
        {
            // Normalize plate number
            var normalizedPlate = plateNumber.Trim().ToUpper();

            // 1. Check if vehicle is registered
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.PlateNumber == normalizedPlate);

            if (vehicle == null)
            {
                return ApiResponse<object>.CreateFailure($"Vehicle with plate [{normalizedPlate}] is not registered in the system.", 403);
            }

            // 2. Check for the MOST RECENT active parking record (where ExitTime is null)
            var activeRecord = await _context.ParkingRecords
                .Where(r => r.VehicleId == vehicle.Id && r.ExitTime == null)
                .OrderByDescending(r => r.EntryTime)
                .FirstOrDefaultAsync();

            if (activeRecord == null)
            {
                // New Entry
                var newRecord = new ParkingRecord
                {
                    VehicleId = vehicle.Id,
                    EntryTime = DateTime.UtcNow
                };

                _context.ParkingRecords.Add(newRecord);
                await _context.SaveChangesAsync();

                return ApiResponse<object>.CreateSuccess(new
                {
                    Vehicle = vehicle.PlateNumber,
                    Owner = vehicle.OwnerName,
                    Apartment = vehicle.ApartmentNumber,
                    Action = "Entry",
                    EntryTime = newRecord.EntryTime,
                    Status = "Parking Started"
                }, $"Welcome {vehicle.OwnerName}. Entry recorded for {vehicle.PlateNumber}.");
            }
            else
            {
                // Exit
                activeRecord.ExitTime = DateTime.UtcNow;
                
                // Calculate duration
                var duration = activeRecord.ExitTime.Value - activeRecord.EntryTime;
                activeRecord.StayDurationMinutes = Math.Max(0, duration.TotalMinutes);

                await _context.SaveChangesAsync();

                return ApiResponse<object>.CreateSuccess(new
                {
                    Vehicle = vehicle.PlateNumber,
                    Owner = vehicle.OwnerName,
                    Apartment = vehicle.ApartmentNumber,
                    Action = "Exit",
                    EntryTime = activeRecord.EntryTime,
                    ExitTime = activeRecord.ExitTime,
                    DurationMinutes = Math.Round(activeRecord.StayDurationMinutes.Value, 2),
                    Status = "Parking Ended"
                }, $"Goodbye {vehicle.OwnerName}. Plate: {vehicle.PlateNumber}. Stay duration: {Math.Round(activeRecord.StayDurationMinutes.Value, 2)} minutes.");
            }
        }
    }
}
