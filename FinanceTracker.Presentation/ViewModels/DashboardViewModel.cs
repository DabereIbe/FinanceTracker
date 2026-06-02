using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Presentation.ViewModels;


public class DashboardViewModel
{
    public TransactionSummaryDto Summary { get; set; } = new();
    public List<BudgetDto> Budgets { get; set; } = new();
    public List<BudgetDto> OverBudgetItems { get; set; } = new();
}