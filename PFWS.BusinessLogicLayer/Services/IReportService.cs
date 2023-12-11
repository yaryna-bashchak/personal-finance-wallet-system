using PFWS.BusinessLogicLayer.DTOs.Report;

namespace PFWS.BusinessLogicLayer.Services;

public interface IReportService
{
    public Task<ReportDto> GetReportOnAccount(int accountId, string username);
}
