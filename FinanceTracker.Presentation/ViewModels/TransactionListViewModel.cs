using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Presentation.ViewModels;


public class TransactionListViewModel
{
    public List<TransactionDto> Transactions { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? WalletId { get; set; }
}