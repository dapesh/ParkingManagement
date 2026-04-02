namespace ParkingManagement.Application.DTOs
{
    public class VehicleRegistrationRequest
    {
        public string PlateNumber { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public int? AssignedSlotId { get; set; }


    }
}
