using Microsoft.EntityFrameworkCore;
using ParkingManagement.Domain.Entities;
using ParkingManagement.Infrastructure.Persistence;

namespace ParkingManagement.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                };
                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RoleId = adminRole.Id
                });
                await context.SaveChangesAsync();
            }

            if (!await context.Apartments.AnyAsync())
            {
                var apt1 = new Apartment { Name = "Antigravity Apartments", Address = "Gate 1, High St" };
                context.Apartments.Add(apt1);
                await context.SaveChangesAsync();

                // Seed 5 slots
                var slots = new List<ParkingSlot>();
                for (int i = 1; i <= 5; i++)
                {
                    slots.Add(new ParkingSlot { SlotNumber = $"P-{i:00}", ApartmentId = apt1.Id, IsOccupied = false });
                }
                context.ParkingSlots.AddRange(slots);
                await context.SaveChangesAsync();
            }

            if (!await context.Vehicles.AnyAsync())
            {
                var slot1 = await context.ParkingSlots.FirstAsync(s => s.SlotNumber == "P-01");
                var slot2 = await context.ParkingSlots.FirstAsync(s => s.SlotNumber == "P-02");

                context.Vehicles.AddRange(
                    new Vehicle 
                    { 
                        PlateNumber = "KBP 456", 
                        OwnerName = "Dipesh", 
                        ApartmentNumber = "101", 
                        PhoneNumber = "9800000001",
                        AssignedSlotId = slot1.Id
                    },
                    new Vehicle 
                    { 
                        PlateNumber = "BAG 123", 
                        OwnerName = "Sita", 
                        ApartmentNumber = "202", 
                        PhoneNumber = "9800000002",
                        AssignedSlotId = slot2.Id
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
