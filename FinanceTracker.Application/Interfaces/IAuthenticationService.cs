using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Application.Interfaces;


public interface IAuthenticationService
{
    Task<(bool Success, string Message, string? Token)> RegisterAsync(RegisterUserDto dto);
    Task<(bool Success, string Message, string? Token)> LoginAsync(LoginUserDto dto);
    Task LogoutAsync();
    Task<bool> ValidateTokenAsync(string token);
}