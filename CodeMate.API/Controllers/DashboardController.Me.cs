using CodeMate.Contracts.Dashboard.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CodeMate.API.Controllers;

public partial class DashboardController
{
    [HttpGet("me")]
    public async Task<ActionResult<MyDashboardResponse>> GetMyDashboard()
    {
        var result = await _myDashboardService.GetMyStatsAsync();
        return Ok(result);
    }
}