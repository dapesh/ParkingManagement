namespace ParkingManagement.Domain.Entities
{
    public class ParkingSlot
    {
        public int Id { get; set; }
        public string SlotNumber { get; set; } = string.Empty;
        public int ApartmentId { get; set; }
        public bool IsOccupied { get; set; }

        // Navigation properties
        public Apartment Apartment { get; set; } = null!;
        public Vehicle? AssignedVehicle { get; set; }
    }
}
