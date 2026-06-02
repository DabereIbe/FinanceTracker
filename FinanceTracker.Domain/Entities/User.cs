using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.Domain.Entities;

public class User : IdentityUser
{
    //public int Id { get; set; }
    //public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    //public string PasswordHash { get; set; } = string.Empty;
    public string PreferredCurrency { get; set; } = "USD";
    public decimal SavingsTarget { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();
}

public enum UserRole
{
    Admin,
    User
}