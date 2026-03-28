using Microsoft.EntityFrameworkCore;
using ParkingManagement.Application.Interfaces;
using ParkingManagement.Domain.Entities;

namespace ParkingManagement.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<ParkingRecord> ParkingRecords => Set<ParkingRecord>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Apartment> Apartments => Set<Apartment>();
        public DbSet<ParkingSlot> ParkingSlots => Set<ParkingSlot>();
        public DbSet<Complaint> Complaints => Set<Complaint>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Apartment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<ParkingSlot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Apartment)
                    .WithMany(p => p.ParkingSlots)
                    .HasForeignKey(d => d.ApartmentId);
                
                entity.HasOne(d => d.AssignedVehicle)
                    .WithOne(p => p.AssignedSlot)
                    .HasForeignKey<Vehicle>(d => d.AssignedSlotId);
            });

            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Vehicle)
                    .WithMany()
                    .HasForeignKey(d => d.VehicleId);
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlateNumber).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.PlateNumber).IsUnique();
            });

            modelBuilder.Entity<ParkingRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.ParkingRecords)
                    .HasForeignKey(d => d.VehicleId);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
