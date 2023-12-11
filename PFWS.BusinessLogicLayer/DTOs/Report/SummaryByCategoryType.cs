namespace PFWS.BusinessLogicLayer.DTOs.Report;

public class SummaryByCategoryType
{
    public decimal Total { get; set; }
    public List<CategorySummary> CategoriesSummary { get; set; }
}
