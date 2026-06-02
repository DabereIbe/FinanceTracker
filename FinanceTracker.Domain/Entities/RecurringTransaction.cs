namespace FinanceTracker.Domain.Entities;

public class RecurringTransaction
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int WalletId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public RecurrencePattern Pattern { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastExecuted { get; set; }

    public User? User { get; set; }
}

public enum RecurrencePattern
{
    Daily,
    Weekly,
    BiWeekly,
    Monthly,
    Quarterly,
    Yearly
}