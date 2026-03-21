using ParkingManagement.Domain.Entities;

namespace ParkingManagement.Application.Interfaces
{
    public interface IJwtProvider
    {
        string Generate(User user);
    }
}
