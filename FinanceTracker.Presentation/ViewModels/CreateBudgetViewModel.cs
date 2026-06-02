using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Presentation.ViewModels;


public class CreateBudgetViewModel
{
    public int Id { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Limit must be greater than 0")]
    public decimal Limit { get; set; }

    [Required]
    public string Period { get; set; } = "Monthly";

    public bool AlertEnabled { get; set; } = true;

    [Range(0, 100, ErrorMessage = "Threshold must be between 0 and 100")]
    public decimal AlertThreshold { get; set; } = 80;

    public List<string> Categories { get; set; } = new();
    public List<string> Periods { get; set; } = new();
}