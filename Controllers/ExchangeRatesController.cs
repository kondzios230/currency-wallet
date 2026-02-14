using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Controllers.Models;
using Wallet.Api.Services.Interfaces;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[AllowAnonymous]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRatesService _exchangeRatesService;

    public ExchangeRatesController(IExchangeRatesService exchangeRatesService) => _exchangeRatesService = exchangeRatesService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExchangeRateModel>>> GetExchangeRates()
    {
        var list = await _exchangeRatesService.GetExchangeRates();
        return Ok(list);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExchangeRateModel>>> RefreshExchangeRates()
    {
        var list = await _exchangeRatesService.RefreshExchangeRates();
        return Ok(list);
    }
}
