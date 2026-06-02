using FinanceTracker.Application.DTOs;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Application.Services;


public class TransactionService : ITransactionService
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IRepository<Budget> _budgetRepository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        IRepository<Transaction> transactionRepository,
        IRepository<Wallet> walletRepository,
        IRepository<Budget> budgetRepository,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _budgetRepository = budgetRepository;
        _logger = logger;
    }

    public async Task<TransactionDto> CreateTransactionAsync(string userId, CreateTransactionDto dto)
    {
        try
        {
            var wallet = await _walletRepository.GetByIdAsync(dto.WalletId);
            if (wallet?.UserId != userId)
                throw new UnauthorizedAccessException("Invalid wallet");

            if (!Enum.TryParse<TransactionType>(dto.Type, out var transactionType))
                throw new ArgumentException("Invalid transaction type");

            var transaction = new Transaction
            {
                UserId = userId,
                WalletId = dto.WalletId,
                Type = transactionType,
                Category = dto.Category,
                Amount = dto.Amount,
                Description = dto.Description,
                TransactionDate = dto.TransactionDate,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);
            
            // Update wallet balance
            if (transactionType == TransactionType.Income)
                wallet.Balance += dto.Amount;
            else
                wallet.Balance -= dto.Amount;

            await _walletRepository.UpdateAsync(wallet);
            await _transactionRepository.SaveChangesAsync();

            // Update budget if expense
            if (transactionType == TransactionType.Expense)
                await UpdateBudgetAsync(userId, dto.Category, dto.Amount);

            _logger.LogInformation($"Transaction created: {transaction.Id}");

            return MapToDto(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            throw;
        }
    }

    public async Task<TransactionDto> GetTransactionAsync(int transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null)
            throw new KeyNotFoundException("Transaction not found");

        return MapToDto(transaction);
    }

    public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(string userId, int? walletId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = (await _transactionRepository.FindAsync(t => t.UserId == userId)).AsQueryable();

        if (walletId.HasValue)
            query = query.Where(t => t.WalletId == walletId.Value);

        if (startDate.HasValue)
            query = query.Where(t => t.TransactionDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.TransactionDate <= endDate.Value);

        return query.OrderByDescending(t => t.TransactionDate).Select(MapToDto);
    }

    public async Task<bool> UpdateTransactionAsync(int transactionId, CreateTransactionDto dto)
    {
        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                return false;

            var oldAmount = transaction.Amount;
            var oldType = transaction.Type;

            transaction.Category = dto.Category;
            transaction.Description = dto.Description;
            transaction.Amount = dto.Amount;
            transaction.TransactionDate = dto.TransactionDate;

            await _transactionRepository.UpdateAsync(transaction);

            // Adjust wallet balance
            var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
            if (wallet != null)
            {
                if (oldType == TransactionType.Income)
                    wallet.Balance -= oldAmount;
                else
                    wallet.Balance += oldAmount;

                if (Enum.TryParse<TransactionType>(dto.Type, out var newType))
                {
                    if (newType == TransactionType.Income)
                        wallet.Balance += dto.Amount;
                    else
                        wallet.Balance -= dto.Amount;
                }

                await _walletRepository.UpdateAsync(wallet);
            }

            _logger.LogInformation($"Transaction updated: {transactionId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction");
            throw;
        }
    }

    public async Task<bool> DeleteTransactionAsync(int transactionId)
    {
        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
                return false;

            // Reverse wallet balance
            var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
            if (wallet != null)
            {
                if (transaction.Type == TransactionType.Income)
                    wallet.Balance -= transaction.Amount;
                else
                    wallet.Balance += transaction.Amount;

                await _walletRepository.UpdateAsync(wallet);
            }

            await _transactionRepository.DeleteAsync(transaction);
            _logger.LogInformation($"Transaction deleted: {transactionId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction");
            throw;
        }
    }

    public async Task<TransactionSummaryDto> GetTransactionSummaryAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var transactions = (await _transactionRepository.FindAsync(
            t => t.UserId == userId && 
                 t.TransactionDate >= startDate && 
                 t.TransactionDate <= endDate)).ToList();

        var summary = new TransactionSummaryDto
        {
            TotalIncome = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount),
            TotalExpense = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount),
            ExpenseByCategory = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount))
        };

        summary.NetBalance = summary.TotalIncome - summary.TotalExpense;
        return summary;
    }

    private async Task UpdateBudgetAsync(string userId, string category, decimal amount)
    {
        var budgets = (await _budgetRepository.FindAsync(
            b => b.UserId == userId && 
                 b.Category == category)).ToList();

        foreach (var budget in budgets)
        {
            budget.CurrentSpent += amount;
            await _budgetRepository.UpdateAsync(budget);
        }
    }

    private static TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            Type = transaction.Type.ToString(),
            Category = transaction.Category,
            Amount = transaction.Amount,
            Description = transaction.Description,
            TransactionDate = transaction.TransactionDate,
            ReceiptUrl = transaction.ReceiptUrl
        };
    }
}