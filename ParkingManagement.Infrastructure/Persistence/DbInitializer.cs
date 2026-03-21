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

            var vehicles = new List<Vehicle>
            {
                new Vehicle { PlateNumber = "ABC 123", OwnerName = "John Doe", ApartmentNumber = "101" },
                new Vehicle { PlateNumber = "XYZ 789", OwnerName = "Jane Smith", ApartmentNumber = "202" },
                new Vehicle { PlateNumber = "KBP 456", OwnerName = "Alice Brown", ApartmentNumber = "303" }
            };

            context.Vehicles.AddRange(vehicles);
            await context.SaveChangesAsync();
        }
    }
}
