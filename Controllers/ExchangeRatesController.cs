using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Controllers.Models;
using Wallet.Api.Services.Interfaces;
using Wallet.Api.Services.Models;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[AllowAnonymous]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRatesService _exchangeRatesService;

    public ExchangeRatesController(IExchangeRatesService exchangeRatesService) => _exchangeRatesService = exchangeRatesService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExchangeRateModel>>> GetExchangeRates(CancellationToken cancellationToken)
    {
        var list = await _exchangeRatesService.GetExchangeRates(cancellationToken);
        return Ok(list.Select(r => ConvertToModel(r)).ToList());
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExchangeRateModel>>> RefreshExchangeRates(CancellationToken cancellationToken)
    {
        var list = await _exchangeRatesService.RefreshExchangeRates(cancellationToken);
        return Ok(list.Select(r => ConvertToModel(r)).ToList());
    }

    private static ExchangeRateModel ConvertToModel(ExchangeRateDto dto)
    {
        return new ExchangeRateModel
        {
            CurrencyCode = dto.CurrencyCode,
            ExchangeRate = dto.ExchangeRate
        };
    }
}
