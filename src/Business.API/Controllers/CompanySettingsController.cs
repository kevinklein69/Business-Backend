using Business.Application.Features.CompanySettings;
using Business.Application.Features.CompanySettings.GetCompanySettings;
using Business.Application.Features.CompanySettings.UpdateCompanySettings;
using Business.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

[ApiController]
[Authorize]
[Route("api/company-settings")]
public class CompanySettingsController(ISender sender) : ControllerBase
{
    public record UpdateCompanySettingsBody(GermanState State);

    /// Visible to every authenticated user — employees should be able to see which
    /// Bundesland (and therefore which public holidays) applies to the company.
    [HttpGet]
    public async Task<ActionResult<CompanySettingsDto>> Get(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCompanySettingsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CompanySettingsDto>> Update(UpdateCompanySettingsBody request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateCompanySettingsCommand(request.State), cancellationToken);
        return Ok(result);
    }
}
