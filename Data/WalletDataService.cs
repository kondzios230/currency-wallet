using Microsoft.EntityFrameworkCore;
using Wallet.Api.Data.Interfaces;
using Wallet.Api.Data.Models;
using Wallet.Api.DTOs;

namespace Wallet.Api.Data;

public sealed class WalletDataService : IWalletDataService
{
    private readonly AppDbContext _context;

    public WalletDataService(AppDbContext context)
    {
        _context = context;
    }

    public Task SaveWallet(WalletDto data)
    {
        throw new NotImplementedException();
    }
}
