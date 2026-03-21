using Microsoft.EntityFrameworkCore;
using ParkingManagement.Application.DTOs;
using ParkingManagement.Application.DTOs.Auth;
using ParkingManagement.Application.Interfaces;
using ParkingManagement.Domain.Entities;

namespace ParkingManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IApplicationDbContext context, IJwtProvider jwtProvider)
        {
            _context = context;
            _jwtProvider = jwtProvider;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponse>.CreateFailure("Invalid username or password.", 401);
            }

            var token = _jwtProvider.Generate(user);

            return ApiResponse<AuthResponse>.CreateSuccess(new AuthResponse
            {
                Username = user.Username,
                Token = token,
                Role = user.Role?.Name ?? "User"
            }, "Login successful.");
        }

        public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return ApiResponse<AuthResponse>.CreateFailure("Username already exists.", 400);
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Reload user with Role info for token generation
            user = await _context.Users.Include(u => u.Role).FirstAsync(u => u.Id == user.Id);
            var token = _jwtProvider.Generate(user);

            return ApiResponse<AuthResponse>.CreateSuccess(new AuthResponse
            {
                Username = user.Username,
                Token = token,
                Role = user.Role?.Name ?? "User"
            }, "Registration successful.", 201);
        }
    }
}
