using PFWS.BusinessLogicLayer.DTOs.Category;
using PFWS.BusinessLogicLayer.DTOs.Report;
using PFWS.BusinessLogicLayer.DTOs.Transactions;

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

        var incomeGroups = transactions
            .Where(t => t.ToAccountId == accountId && t.IncomeCategory != null)
            .GroupBy(t => t.IncomeCategory.Id);
        var expenseGroups = transactions
            .Where(t => t.FromAccountId == accountId && t.ExpenseCategory != null)
            .GroupBy(t => t.ExpenseCategory.Id);

        var report = new ReportDto
        {
            AccountId = account.Id,
            AccountName = account.Name,
            Balance = account.Balance,
            Income = SummarizeTransactions(incomeGroups, t => t.IncomeCategory),
            Expenses = SummarizeTransactions(expenseGroups, t => t.ExpenseCategory)
        };

        report.Total = report.Income.Total - report.Expenses.Total;

        return report;
    }

    private SummaryByCategoryType SummarizeTransactions(IEnumerable<IGrouping<int, GetTransactionDto>> transactionGroups, Func<GetTransactionDto, GetCategoryDtoShort> categorySelector)
    {
        var summary = new SummaryByCategoryType
        {
            Total = 0,
            CategoriesSummary = new List<CategorySummary>()
        };

        foreach (var group in transactionGroups)
        {
            var totalAmount = group.Sum(t => t.Amount);
            summary.Total += totalAmount;
            summary.CategoriesSummary.Add(new CategorySummary
            {
                CategoryId = group.Key,
                CategoryName = categorySelector(group.First()).Name,
                Total = totalAmount
            });
        }

        return summary;
    }

}
