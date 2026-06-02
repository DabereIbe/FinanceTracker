namespace FinanceTracker.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int WalletId { get; set; }
    public TransactionType Type { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReceiptUrl { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Wallet? Wallet { get; set; }
}

public enum TransactionType
{
    Income,
    Expense
}

public static class TransactionCategories
{
    public static readonly string[] ExpenseCategories = 
    {
        "Food & Dining",
        "Transportation",
        "Utilities",
        "Entertainment",
        "Shopping",
        "Healthcare",
        "Education",
        "Other"
    };

    public static readonly string[] IncomeCategories = 
    {
        "Salary",
        "Freelance",
        "Investment",
        "Bonus",
        "Other"
    };
}