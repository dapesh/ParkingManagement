using ParkingManagement.Application.DTOs;
using ParkingManagement.Application.DTOs.Auth;

namespace ParkingManagement.Application.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    }
}
