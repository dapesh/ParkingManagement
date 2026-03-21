namespace ParkingManagement.Domain.Entities
{
    public class ParkingRecord
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public double? StayDurationMinutes { get; set; }

        public virtual Vehicle? Vehicle { get; set; }
    }
}
