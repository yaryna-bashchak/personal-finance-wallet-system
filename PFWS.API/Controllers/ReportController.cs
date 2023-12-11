using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFWS.API.Controllers.Controllers;
using PFWS.BusinessLogicLayer.DTOs.Report;
using PFWS.BusinessLogicLayer.Services;

namespace PFWS.API.Controllers;

[Authorize]
public class ReportController : BaseApiController
{
    private readonly IReportService _reportService;
    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("{accountId}")]
    public async Task<ActionResult<ReportDto>> GetReportOnAccount(int accountId)
    {
        try
        {
            var username = User.Identity.Name;
            var report = await _reportService.GetReportOnAccount(accountId, username);
            return Ok(report);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
