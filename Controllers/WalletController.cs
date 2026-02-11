using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.DTOs;
using Wallet.Api.Services.Interfaces;

namespace Wallet.Api.Controllers;

[ApiController]
[Route("api/Wallet")]
[AllowAnonymous]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService) => _walletService = walletService;

    [HttpPost("CreateWallet")]
    public async Task<ActionResult<WalletDto>> CreateWallet([FromBody] WalletDto data)
    {
        try
        {
            var wallet = await _walletService.CreateWallet(data);
            return Ok(wallet);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}


