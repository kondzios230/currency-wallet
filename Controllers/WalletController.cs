using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Controllers.Models;
using Wallet.Api.Services.Interfaces;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[AllowAnonymous]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IExchangeRatesService _exchangeRatesService;

    public WalletController(IWalletService walletService, IExchangeRatesService exchangeRatesService)
    {
        _walletService = walletService;
        _exchangeRatesService = exchangeRatesService;
    }

    [HttpPost]
    public async Task<ActionResult<WalletModel>> CreateWallet([FromBody] WalletModel data)
    {
        try
        {
            var wallet = await _walletService.CreateWallet(data.WalletName);
            return Ok(wallet);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{walletId:guid}")]
    public async Task<ActionResult> RemoveWallet([FromRoute] Guid walletId)
    {
        try
        {
            await _walletService.RemoveWallet(walletId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(ex.Message)
                : BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<WalletRowModel>> TopUpWallet([FromBody] ModifyRowModel request)
    {
        try
        {
            if (request.Amount <= 0)
                throw new InvalidOperationException("Amount must be positive.");

            if (!await _exchangeRatesService.DoesCurrencyExists(request.CurrencyCode))
                throw new InvalidOperationException("Currency code is invalid.");

            var row = await _walletService.TopUpWallet(request.WalletId, request.CurrencyCode, request.Amount);
            return Ok(row);
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(ex.Message)
                : BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<WalletRowModel>> WithdrawFromWallet([FromBody] ModifyRowModel request)
    {
        try
        {
            if (request.Amount <= 0)
                throw new InvalidOperationException("Amount must be positive.");

            if (!await _exchangeRatesService.DoesCurrencyExists(request.CurrencyCode))
                throw new InvalidOperationException("Currency code is invalid.");

            var row = await _walletService.WithdrawFromWallet(request.WalletId, request.CurrencyCode, request.Amount);
            return row == null ? NoContent() : Ok(row);
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(ex.Message)
                : BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> Exchange([FromBody] ExchangeRequestModel request)
    {
        try
        {
            if (request.SourceAmount <= 0)
                throw new InvalidOperationException("Source amount must be positive.");

            await _walletService.ExchangeAsync(
                request.WalletId,
                request.SourceCurrencyCode,
                request.TargetCurrencyCode,
                request.SourceAmount);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(ex.Message)
                : BadRequest(ex.Message);
        }
    }
}
