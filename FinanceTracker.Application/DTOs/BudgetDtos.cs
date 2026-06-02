namespace FinanceTracker.Application.DTOs;

public class CreateBudgetDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public string Period { get; set; } = "Monthly";
    public bool AlertEnabled { get; set; } = true;
    public decimal AlertThreshold { get; set; } = 80;
}

public class BudgetDto
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public decimal CurrentSpent { get; set; }
    public string Period { get; set; } = string.Empty;
    public decimal PercentageUsed { get; set; }
    public decimal AlertThreshold { get; set; }
    public bool IsOverBudget { get; set; }
}