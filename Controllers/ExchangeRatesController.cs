using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.DTOs;
using Wallet.Api.Services.Interfaces;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/ExchangeRates")]
[AllowAnonymous]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRatesService _exchangeRatesService;

    public ExchangeRatesController(IExchangeRatesService exchangeRatesService) => _exchangeRatesService = exchangeRatesService;

    [HttpGet("GetExchangeRates")]
    public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> GetExchangeRates()
    {
        var list = await _exchangeRatesService.GetExchangeRates();
        return Ok(list);
    }

    [HttpGet("RefreshExchangeRates")]
    public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> RefreshExchangeRates()
    {
        var list = await _exchangeRatesService.RefreshExchangeRates();
        return Ok(list);
    }
}
