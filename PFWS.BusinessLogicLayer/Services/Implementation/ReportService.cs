using PFWS.BusinessLogicLayer.DTOs.Report;

namespace PFWS.BusinessLogicLayer.Services.Implementation;

public class ReportService : IReportService
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;

    public ReportService(ITransactionService transactionService, IAccountService accountService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    public async Task<ReportDto> GetReportOnAccount(int accountId, string username)
    {
        var account = await _accountService.GetAccountById(accountId, username);
        var transactions = await _transactionService.GetTransactionsByAccountId(accountId, username);

        var report = new ReportDto
        {
            AccountId = account.Id,
            AccountName = account.Name,
            Balance = account.Balance,
            Income = new SummaryByCategoryType { Total = 0, CategoriesSummary = new List<CategorySummary>() },
            Expenses = new SummaryByCategoryType { Total = 0, CategoriesSummary = new List<CategorySummary>() }
        };

        var incomeGroups = transactions
            .Where(t => t.ToAccountId == accountId && t.IncomeCategory != null)
            .GroupBy(t => t.IncomeCategory.Id);
        var expenseGroups = transactions
            .Where(t => t.FromAccountId == accountId && t.ExpenseCategory != null)
            .GroupBy(t => t.ExpenseCategory.Id);

        foreach (var group in incomeGroups)
        {
            var totalAmount = group.Sum(t => t.Amount);
            report.Income.Total += totalAmount;
            report.Income.CategoriesSummary.Add(new CategorySummary
            {
                CategoryId = group.Key,
                CategoryName = group.First().IncomeCategory.Name,
                Total = totalAmount
            });
        }

        foreach (var group in expenseGroups)
        {
            var totalAmount = group.Sum(t => t.Amount);
            report.Expenses.Total += totalAmount;
            report.Expenses.CategoriesSummary.Add(new CategorySummary
            {
                CategoryId = group.Key,
                CategoryName = group.First().ExpenseCategory.Name,
                Total = totalAmount
            });
        }

        report.Total = report.Income.Total - report.Expenses.Total;

        return report;
    }
}
