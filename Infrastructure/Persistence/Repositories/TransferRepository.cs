using Microsoft.EntityFrameworkCore;
using ms_transferencias.Application.Common.Interfaces;
using ms_transferencias.Infrastructure.Entities;

namespace ms_transferencias.Infrastructure.Persistence.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly ApplicationDbContext _context;

    public TransferRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transfer?> GetByExternalIdAsync(Guid externalId, CancellationToken ct)
    {
        return await _context.Transfers.FirstOrDefaultAsync(t => t.ExternalOperationId == externalId, ct);
    }

    public async Task<Transfer> AddTransferAsync(Transfer transfer, CancellationToken ct)
    {
        await _context.Transfers.AddAsync(transfer, ct);
        await _context.SaveChangesAsync(ct);
        return transfer;
    }

    public async Task<int> UpdateTransferAsync(Transfer transfer, CancellationToken ct)
    {
        _context.Transfers.Update(transfer);
        return await _context.SaveChangesAsync(ct);
    }
}