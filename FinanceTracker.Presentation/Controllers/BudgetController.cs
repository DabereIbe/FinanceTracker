using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Presentation.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.Presentation.Controllers;


[Authorize]
public class BudgetController : Controller
{
    private readonly IBudgetService _budgetService;
    private readonly ILogger<BudgetController> _logger;
    private readonly UserManager<User> _userManager;

    public BudgetController(IBudgetService budgetService, ILogger<BudgetController> logger, UserManager<User> userManager)
    {
        _budgetService = budgetService;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = GetCurrentUserId();
            var budgets = await _budgetService.GetUserBudgetsAsync(userId);

            return View(budgets.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading budgets");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateBudgetViewModel
        {
            AlertEnabled = true,
            AlertThreshold = 80,
            Categories = TransactionCategories.ExpenseCategories.ToList(),
            Periods = new List<string> { "Weekly", "Monthly", "Yearly" }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBudgetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = TransactionCategories.ExpenseCategories.ToList();
            model.Periods = new List<string> { "Weekly", "Monthly", "Yearly" };
            return View(model);
        }

        try
        {
            var userId = GetCurrentUserId();
            var dto = new CreateBudgetDto
            {
                Category = model.Category,
                Limit = model.Limit,
                Period = model.Period,
                AlertEnabled = model.AlertEnabled,
                AlertThreshold = model.AlertThreshold
            };

            await _budgetService.CreateBudgetAsync(userId, dto);
            _logger.LogInformation($"Budget created for user {userId}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            ModelState.AddModelError(string.Empty, "Error creating budget");
            model.Categories = TransactionCategories.ExpenseCategories.ToList();
            model.Periods = new List<string> { "Weekly", "Monthly", "Yearly" };
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var budget = await _budgetService.GetBudgetAsync(id);
            if (budget == null)
                return NotFound();

            var model = new CreateBudgetViewModel
            {
                Id = budget.Id,
                Category = budget.Category,
                Limit = budget.Limit,
                Period = budget.Period,
                AlertEnabled = true,
                AlertThreshold = budget.AlertThreshold,
                Categories = TransactionCategories.ExpenseCategories.ToList(),
                Periods = new List<string> { "Weekly", "Monthly", "Yearly" }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading budget");
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateBudgetViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            model.Categories = TransactionCategories.ExpenseCategories.ToList();
            model.Periods = new List<string> { "Weekly", "Monthly", "Yearly" };
            return View(model);
        }

        try
        {
            var dto = new CreateBudgetDto
            {
                Category = model.Category,
                Limit = model.Limit,
                Period = model.Period,
                AlertEnabled = model.AlertEnabled,
                AlertThreshold = model.AlertThreshold
            };

            await _budgetService.UpdateBudgetAsync(id, dto);
            _logger.LogInformation($"Budget updated: {id}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget");
            ModelState.AddModelError(string.Empty, "Error updating budget");
            model.Categories = TransactionCategories.ExpenseCategories.ToList();
            model.Periods = new List<string> { "Weekly", "Monthly", "Yearly" };
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _budgetService.DeleteBudgetAsync(id);
            _logger.LogInformation($"Budget deleted: {id}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting budget");
            return RedirectToAction(nameof(Index));
        }
    }

    private string GetCurrentUserId()
    {
        var user = _userManager.GetUserAsync(User).Result;
        return user!.Id;
    }
}