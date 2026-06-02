namespace FinanceTracker.Domain.Entities;

public class Budget
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public decimal CurrentSpent { get; set; }
    public BudgetPeriod Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AlertEnabled { get; set; } = true;
    public decimal AlertThreshold { get; set; } = 80;

    public User? User { get; set; }
}

public enum BudgetPeriod
{
    Weekly,
    Monthly,
    Yearly
}