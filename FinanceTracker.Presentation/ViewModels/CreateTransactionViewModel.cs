using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Presentation.ViewModels;


public class CreateTransactionViewModel
{
    public int Id { get; set; }

    [Required]
    public int WalletId { get; set; }

    [Required]
    public string Type { get; set; } = "Expense";

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime TransactionDate { get; set; }

    public List<string> ExpenseCategories { get; set; } = new();
    public List<string> IncomeCategories { get; set; } = new();
}