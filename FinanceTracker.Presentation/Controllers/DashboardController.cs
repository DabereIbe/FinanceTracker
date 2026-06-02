using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Presentation.Controllers;


[Authorize]
public class DashboardController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly IBudgetService _budgetService;
    private readonly ILogger<DashboardController> _logger;
    private readonly UserManager<User> _userManager;

    public DashboardController(ITransactionService transactionService, IBudgetService budgetService, ILogger<DashboardController> logger, UserManager<User> userManager)
    {
        _transactionService = transactionService;
        _budgetService = budgetService;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = GetCurrentUserId();
            var startDate = DateTime.UtcNow.AddMonths(-1);
            var endDate = DateTime.UtcNow;

            var summary = await _transactionService.GetTransactionSummaryAsync(userId, startDate, endDate);
            var budgets = await _budgetService.GetUserBudgetsAsync(userId);
            var overBudgetItems = await _budgetService.GetOverBudgetCategoriesAsync(userId);

            var model = new DashboardViewModel
            {
                Summary = summary,
                Budgets = budgets.ToList(),
                OverBudgetItems = overBudgetItems.ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return View("Error");
        }
    }

    private string GetCurrentUserId()
    {
        var user = _userManager.GetUserAsync(User).Result;
        return user!.Id;
    }
}