namespace ParkingManagement.Application.DTOs
{
    public class ComplaintRequest
    {
        public string PlateNumber { get; set; } = string.Empty;
        public string ReportedSlot { get; set; } = string.Empty;
        public string Description { get; set; } = "Vehicle parked in my assigned slot.";
    }
}
