namespace PFWS.BusinessLogicLayer.DTOs.Report;

public class ReportDto
{
    public int AccountId { get; set; }
    public string AccountName { get; set; }
    public decimal Total { get; set; }
    public decimal Balance { get; set; }
    public SummaryByCategoryType Income { get; set; }
    public SummaryByCategoryType Expenses { get; set; }
}
