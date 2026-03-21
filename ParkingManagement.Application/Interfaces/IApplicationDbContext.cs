using ParkingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ParkingManagement.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Vehicle> Vehicles { get; }
        DbSet<ParkingRecord> ParkingRecords { get; }
        DbSet<Role> Roles { get; }
        DbSet<User> Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
