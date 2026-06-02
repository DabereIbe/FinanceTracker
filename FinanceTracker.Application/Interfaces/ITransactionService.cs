using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Application.Interfaces;


public interface ITransactionService
{
    Task<TransactionDto> CreateTransactionAsync(string userId, CreateTransactionDto dto);
    Task<TransactionDto> GetTransactionAsync(int transactionId);
    Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(string userId, int? walletId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> UpdateTransactionAsync(int transactionId, CreateTransactionDto dto);
    Task<bool> DeleteTransactionAsync(int transactionId);
    Task<TransactionSummaryDto> GetTransactionSummaryAsync(string userId, DateTime startDate, DateTime endDate);
}