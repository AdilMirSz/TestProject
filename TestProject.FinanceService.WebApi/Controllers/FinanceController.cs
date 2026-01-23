using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestProject.FinanceService.Application.Rates;

namespace TestProject.FinanceService.WebApi.Controllers;

[ApiController]
[Route("api/finance")]
public sealed class FinanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinanceController(IMediator mediator) => _mediator = mediator;

    [Authorize]
    [HttpGet("rates")]
    public async Task<ActionResult<IReadOnlyList<CurrencyRateDto>>> GetMyRates(CancellationToken ct)
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !long.TryParse(raw, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetMyRatesQuery(userId), ct);
        return Ok(result);
    }
}