using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.DTOs;
using Wallet.Api.Services;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/ExchangeRates")]
[AllowAnonymous]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRatesService _ExchangeRatesService;

    public ExchangeRatesController(IExchangeRatesService ExchangeRatesService) => _ExchangeRatesService = ExchangeRatesService;

    [HttpGet("GetExchangeRates")]
    public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> GetExchangeRates()
    {
        var list = await _ExchangeRatesService.GetExchangeRates();
        return Ok(list);
    }
}
