namespace ParkingManagement.Domain.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public bool IsRegistered { get; set; } = true;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ParkingRecord> ParkingRecords { get; set; } = new List<ParkingRecord>();
    }
}
