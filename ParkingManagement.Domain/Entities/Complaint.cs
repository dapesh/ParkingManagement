namespace ParkingManagement.Domain.Entities
{
    public class Complaint
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string ReportedSlot { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Vehicle? Vehicle { get; set; }
    }
}
