using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Application.Interfaces;


public interface IBudgetService
{
    Task<BudgetDto> CreateBudgetAsync(string userId, CreateBudgetDto dto);
    Task<IEnumerable<BudgetDto>> GetUserBudgetsAsync(string userId);
    Task<BudgetDto> GetBudgetAsync(int budgetId);
    Task<bool> UpdateBudgetAsync(int budgetId, CreateBudgetDto dto);
    Task<bool> DeleteBudgetAsync(int budgetId);
    Task<IEnumerable<BudgetDto>> GetOverBudgetCategoriesAsync(string userId);
}