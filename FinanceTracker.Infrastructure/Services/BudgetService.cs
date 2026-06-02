using FinanceTracker.Application.DTOs;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Application.Services;


public class BudgetService : IBudgetService
{
    private readonly IRepository<Budget> _budgetRepository;
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(
        IRepository<Budget> budgetRepository,
        IRepository<Transaction> transactionRepository,
        ILogger<BudgetService> logger)
    {
        _budgetRepository = budgetRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<BudgetDto> CreateBudgetAsync(string userId, CreateBudgetDto dto)
    {
        try
        {
            if (!Enum.TryParse<BudgetPeriod>(dto.Period, out var period))
                throw new ArgumentException("Invalid budget period");

            var now = DateTime.UtcNow;
            var (startDate, endDate) = GetPeriodDates(now, period);

            var budget = new Budget
            {
                UserId = userId,
                Category = dto.Category,
                Limit = dto.Limit,
                Period = period,
                StartDate = startDate,
                EndDate = endDate,
                AlertEnabled = dto.AlertEnabled,
                AlertThreshold = dto.AlertThreshold,
                CurrentSpent = 0
            };

            await _budgetRepository.AddAsync(budget);
            await _budgetRepository.SaveChangesAsync();

            _logger.LogInformation($"Budget created: {budget.Id}");
            return MapToDto(budget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            throw;
        }
    }

    public async Task<IEnumerable<BudgetDto>> GetUserBudgetsAsync(string userId)
    {
        var budgets = await _budgetRepository.FindAsync(b => b.UserId == userId);
        return budgets.Select(MapToDto);
    }

    public async Task<BudgetDto> GetBudgetAsync(int budgetId)
    {
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
            throw new KeyNotFoundException("Budget not found");

        return MapToDto(budget);
    }

    public async Task<bool> UpdateBudgetAsync(int budgetId, CreateBudgetDto dto)
    {
        try
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget == null)
                return false;

            if (Enum.TryParse<BudgetPeriod>(dto.Period, out var period))
            {
                budget.Period = period;
                var (startDate, endDate) = GetPeriodDates(DateTime.UtcNow, period);
                budget.StartDate = startDate;
                budget.EndDate = endDate;
            }

            budget.Category = dto.Category;
            budget.Limit = dto.Limit;
            budget.AlertEnabled = dto.AlertEnabled;
            budget.AlertThreshold = dto.AlertThreshold;

            await _budgetRepository.UpdateAsync(budget);
            _logger.LogInformation($"Budget updated: {budgetId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget");
            throw;
        }
    }

    public async Task<bool> DeleteBudgetAsync(int budgetId)
    {
        try
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget == null)
                return false;

            await _budgetRepository.DeleteAsync(budget);
            _logger.LogInformation($"Budget deleted: {budgetId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting budget");
            throw;
        }
    }

    public async Task<IEnumerable<BudgetDto>> GetOverBudgetCategoriesAsync(string userId)
    {
        var budgets = await _budgetRepository.FindAsync(b => b.UserId == userId);
        return budgets
            .Where(b => b.CurrentSpent > b.Limit)
            .Select(MapToDto);
    }

    private static (DateTime start, DateTime end) GetPeriodDates(DateTime now, BudgetPeriod period)
    {
        return period switch
        {
            BudgetPeriod.Weekly => (now.AddDays(-(int)now.DayOfWeek), now.AddDays(6 - (int)now.DayOfWeek)),
            BudgetPeriod.Monthly => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month))),
            BudgetPeriod.Yearly => (new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31)),
            _ => (now, now.AddDays(30))
        };
    }

    private static BudgetDto MapToDto(Budget budget)
    {
        var percentageUsed = budget.Limit > 0 ? (budget.CurrentSpent / budget.Limit) * 100 : 0;
        return new BudgetDto
        {
            Id = budget.Id,
            Category = budget.Category,
            Limit = budget.Limit,
            CurrentSpent = budget.CurrentSpent,
            Period = budget.Period.ToString(),
            PercentageUsed = (decimal)percentageUsed,
            IsOverBudget = budget.CurrentSpent > budget.Limit
        };
    }
}