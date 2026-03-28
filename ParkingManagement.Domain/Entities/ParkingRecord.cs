namespace ParkingManagement.Domain.Entities
{
    public class ParkingRecord
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public double? DurationInMinutes { get; set; }

        public int? SlotId { get; set; }
        public ParkingSlot? Slot { get; set; }

        public int? ApartmentId { get; set; }
        public Apartment? Apartment { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
    }
}
