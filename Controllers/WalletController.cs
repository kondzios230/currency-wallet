using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Controllers.Models;
using Wallet.Api.Services.Interfaces;
using Wallet.Api.Services.Models;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WalletModel>>> GetWallets()
    {
        var result = await _walletService.GetAllWallets();
        return Ok(result.Select(r => ConvertWalletToModel(r)));
    }

    [HttpGet("{walletId:guid}")]
    public async Task<ActionResult<WalletModel>> GetWallet([FromRoute] Guid walletId)
    {
        var wallet = await _walletService.GetWallet(walletId);
        if (wallet == null)
            return NotFound("Wallet not found.");
        return Ok(ConvertWalletToModel(wallet));
    }

    [HttpPost]
    public async Task<ActionResult<WalletModel>> CreateWallet([FromBody] WalletModel data)
    {
        try
        {
            var wallet = await _walletService.CreateWallet(data.WalletName);
            return Ok(ConvertWalletToModel(wallet));
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
            if (!await _exchangeRatesService.DoesCurrencyExists(request.CurrencyCode))
                throw new InvalidOperationException("Currency code is invalid.");

            var row = await _walletService.TopUpWallet(request.WalletId, request.CurrencyCode, request.Amount);
            return Ok(ConvertWalletRowToModel(row));
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
            if (!await _exchangeRatesService.DoesCurrencyExists(request.CurrencyCode))
                throw new InvalidOperationException("Currency code is invalid.");

            var row = await _walletService.WithdrawFromWallet(request.WalletId, request.CurrencyCode, request.Amount);
            return row == null ? NoContent() : Ok(ConvertWalletRowToModel(row));
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

    private WalletModel ConvertWalletToModel(WalletDto dto)
    {
        return new WalletModel
        {
            Id = dto.Id,
            WalletName = dto.WalletName,
            Rows = dto.Rows.Select(r => ConvertWalletRowToModel(r)).ToList()
        };
    }

    private WalletRowModel ConvertWalletRowToModel(WalletRowDto dto)
    {
        return new WalletRowModel
        {
            Id = dto.Id,
            WalletId = dto.WalletId,
            CurrencyCode = dto.CurrencyCode,
            Amount = dto.Amount
        };
    }
}
