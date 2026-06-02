using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Presentation.Controllers;


[Authorize]
public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;
    private readonly UserManager<User> _userManager;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger, UserManager<User> userManager)
    {
        _transactionService = transactionService;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? walletId, DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var userId = GetCurrentUserId();
            startDate ??= DateTime.UtcNow.AddMonths(-1);
            endDate ??= DateTime.UtcNow;

            var transactions = await _transactionService.GetUserTransactionsAsync(userId, walletId, startDate, endDate);

            var model = new TransactionListViewModel
            {
                Transactions = transactions.ToList(),
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                WalletId = walletId
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading transactions");
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateTransactionViewModel
        {
            TransactionDate = DateTime.UtcNow,
            ExpenseCategories = TransactionCategories.ExpenseCategories.ToList(),
            IncomeCategories = TransactionCategories.IncomeCategories.ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTransactionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ExpenseCategories = TransactionCategories.ExpenseCategories.ToList();
            model.IncomeCategories = TransactionCategories.IncomeCategories.ToList();
            return View(model);
        }

        try
        {
            var userId = GetCurrentUserId();
            var dto = new CreateTransactionDto
            {
                WalletId = model.WalletId,
                Type = model.Type,
                Category = model.Category,
                Amount = model.Amount,
                Description = model.Description,
                TransactionDate = model.TransactionDate
            };

            await _transactionService.CreateTransactionAsync(userId, dto);
            _logger.LogInformation($"Transaction created for user {userId}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            ModelState.AddModelError(string.Empty, "Error creating transaction");
            model.ExpenseCategories = TransactionCategories.ExpenseCategories.ToList();
            model.IncomeCategories = TransactionCategories.IncomeCategories.ToList();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionAsync(id);
            if (transaction == null)
                return NotFound();

            var model = new CreateTransactionViewModel
            {
                Id = transaction.Id,
                WalletId = 0, // Set from transaction
                Type = transaction.Type,
                Category = transaction.Category,
                Amount = transaction.Amount,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                ExpenseCategories = TransactionCategories.ExpenseCategories.ToList(),
                IncomeCategories = TransactionCategories.IncomeCategories.ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading transaction");
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateTransactionViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            model.ExpenseCategories = TransactionCategories.ExpenseCategories.ToList();
            model.IncomeCategories = TransactionCategories.IncomeCategories.ToList();
            return View(model);
        }

        try
        {
            var dto = new CreateTransactionDto
            {
                WalletId = model.WalletId,
                Type = model.Type,
                Category = model.Category,
                Amount = model.Amount,
                Description = model.Description,
                TransactionDate = model.TransactionDate
            };

            await _transactionService.UpdateTransactionAsync(id, dto);
            _logger.LogInformation($"Transaction updated: {id}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction");
            ModelState.AddModelError(string.Empty, "Error updating transaction");
            model.ExpenseCategories = TransactionCategories.ExpenseCategories.ToList();
            model.IncomeCategories = TransactionCategories.IncomeCategories.ToList();
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _transactionService.DeleteTransactionAsync(id);
            _logger.LogInformation($"Transaction deleted: {id}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction");
            return RedirectToAction(nameof(Index));
        }
    }

    private string GetCurrentUserId()
    {
        var user = _userManager.GetUserAsync(User).Result;
        return user!.Id;
    }
}