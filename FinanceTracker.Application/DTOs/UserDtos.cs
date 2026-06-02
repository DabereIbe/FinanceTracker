using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.DTOs;

public class RegisterUserDto
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginUserDto
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PreferredCurrency { get; set; } = "USD";
    public decimal SavingsTarget { get; set; }
}

public class UpdateProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string PreferredCurrency { get; set; } = string.Empty;
    public decimal SavingsTarget { get; set; }
}