using FinanceTracker.Application.DTOs;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using FinanceTracker.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.Application.Services;



public class AuthenticationService : IAuthenticationService
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthenticationService(IRepository<User> userRepository, ILogger<AuthenticationService> logger, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userRepository = userRepository;
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Success, string Message, string? Token)> RegisterAsync(RegisterUserDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return (false, "Email and password are required", null);

            if (dto.Password != dto.ConfirmPassword)
                return (false, "Passwords do not match", null);

            var existingUser = (await _userRepository.FindAsync(u => u.Email == dto.Email)).FirstOrDefault();
            if (existingUser != null)
                return (false, "Email already registered", null);

            //var passwordHash = HashPassword(dto.Password);
            var user = new User
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError(errors);
                return (false, $"Registration failed: {errors}", null);
            }
            else
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation($"User registered and signed in: {dto.Email}");
                return (true, "Registration successful", user.Id.ToString());
            }

            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return (false, "Registration failed", null);
        }
    }

    public async Task<(bool Success, string Message, string? Token)> LoginAsync(LoginUserDto dto)
    {
        try
        {
            /*var user = (await _userRepository.FindAsync(u => u.Email == dto.Email)).FirstOrDefault();
            if (user == null)
                return (false, "Invalid email or password", null);

            if (!VerifyPassword(dto.Password, user.PasswordHash))
                return (false, "Invalid email or password", null);

            if (!user.IsActive)
                return (false, "Account is inactive", null);*/

            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User logged in: {dto.Email}");
                return (true, "Login successful", null);
            }
            else
            {
                _logger.LogWarning($"Failed login attempt: {dto.Email}");
                return (false, "Invalid email or password", null);
            }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return (false, "Login failed", null);
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token) || !int.TryParse(token, out var userId))
                return false;

            var user = await _userRepository.GetByIdAsync(userId);
            return user?.IsActive ?? false;
        }
        catch
        {
            return false;
        }
    }

    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput.Equals(hash);
    }
}