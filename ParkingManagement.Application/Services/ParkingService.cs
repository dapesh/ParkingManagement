using Microsoft.EntityFrameworkCore;
using ParkingManagement.Application.DTOs;
using ParkingManagement.Application.Interfaces;
using ParkingManagement.Domain.Entities;

namespace ParkingManagement.Application.Services
{
    public interface IParkingService
    {
        Task<ApiResponse<object>> ProcessPlateAsync(string plateNumber);
        Task<ApiResponse<object>> RegisterVehicleAsync(VehicleRegistrationRequest request);
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
                .Include(v => v.AssignedSlot)
                .FirstOrDefaultAsync(v => v.PlateNumber == normalizedPlate || v.PlateNumber == normalizedPlate.Replace(" ", ""));

            if (vehicle == null)
            {
                return ApiResponse<object>.CreateFailure($"Vehicle {normalizedPlate} is not registered in this system.", 404);
            }

            // 2. Determine if it's an Entry or Exit
            var activeRecord = await _context.ParkingRecords
                .Where(r => r.VehicleId == vehicle.Id && r.ExitTime == null)
                .OrderByDescending(r => r.EntryTime)
                .FirstOrDefaultAsync();

            if (activeRecord == null)
            {
                // ENTRY LOGIC (At Gate)
                var welcomeMessage = $"Welcome {normalizedPlate}! Your assigned parking slot is: {vehicle.AssignedSlot?.SlotNumber ?? "Unassigned"}";
                
                var newRecord = new ParkingRecord
                {
                    VehicleId = vehicle.Id,
                    EntryTime = DateTime.UtcNow,
                    SlotId = vehicle.AssignedSlotId
                };

                // Mark slot as occupied
                if (vehicle.AssignedSlot != null)
                {
                    vehicle.AssignedSlot.IsOccupied = true;
                }

                _context.ParkingRecords.Add(newRecord);
                await _context.SaveChangesAsync();

                return ApiResponse<object>.CreateSuccess(new
                {
                    Action = "Entry",
                    Owner = vehicle.OwnerName,
                    Plate = normalizedPlate,
                    AssignedSlot = vehicle.AssignedSlot?.SlotNumber,
                    Message = welcomeMessage
                }, welcomeMessage);
            }
            else
            {
                // EXIT LOGIC (At Gate)
                activeRecord.ExitTime = DateTime.UtcNow;
                activeRecord.DurationInMinutes = (activeRecord.ExitTime.Value - activeRecord.EntryTime).TotalMinutes;

                // Mark slot as free
                var slot = await _context.ParkingSlots.FirstOrDefaultAsync(s => s.Id == vehicle.AssignedSlotId);
                if (slot != null)
                {
                    slot.IsOccupied = false;
                }

                await _context.SaveChangesAsync();

                var goodbyeMessage = $"Goodbye {normalizedPlate}! Slot {vehicle.AssignedSlot?.SlotNumber ?? "Unassigned"} is now free.";

                return ApiResponse<object>.CreateSuccess(new
                {
                    Action = "Exit",
                    Owner = vehicle.OwnerName,
                    Plate = normalizedPlate,
                    Duration = activeRecord.DurationInMinutes,
                    Message = goodbyeMessage
                }, goodbyeMessage);
            }
        }

        public async Task<ApiResponse<object>> RegisterVehicleAsync(VehicleRegistrationRequest request)
        {
            // 1. Find the first available slot in the specified apartment (or default to the first apartment)
            var apartment = await _context.Apartments
                .Include(a => a.ParkingSlots)
                .FirstOrDefaultAsync(); // Simplifying to first apartment for now

            if (apartment == null)
            {
                return ApiResponse<object>.CreateFailure("No apartments found in the system.", 404);
            }

            var freeSlot = apartment.ParkingSlots.FirstOrDefault(s => !s.IsOccupied && _context.Vehicles.All(v => v.AssignedSlotId != s.Id));

            if (freeSlot == null)
            {
                return ApiResponse<object>.CreateFailure("No free parking slots available.", 400);
            }

            // 2. Create the vehicle
            var vehicle = new Vehicle
            {
                PlateNumber = request.PlateNumber.ToUpper().Trim(),
                OwnerName = request.OwnerName,
                PhoneNumber = request.PhoneNumber,
                ApartmentNumber = request.ApartmentNumber,
                AssignedSlotId = freeSlot.Id,
                IsRegistered = true
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var registrationMessage = $"User {vehicle.OwnerName} ({vehicle.PlateNumber}) has been registered. You have been provided parking slot: {freeSlot.SlotNumber}";

            return ApiResponse<object>.CreateSuccess(new
            {
                Owner = vehicle.OwnerName,
                Plate = vehicle.PlateNumber,
                AssignedSlot = freeSlot.SlotNumber,
                Message = registrationMessage
            }, registrationMessage);
        }
    }
}
