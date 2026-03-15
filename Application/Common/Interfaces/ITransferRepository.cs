using ms_transferencias.Infrastructure.Entities;

namespace ms_transferencias.Application.Common.Interfaces;

public interface ITransferRepository
{
    Task<Transfer?> GetByExternalIdAsync(Guid externalId, CancellationToken ct);
    Task<Transfer> AddTransferAsync(Transfer transfer, CancellationToken ct);
    Task<int> UpdateTransferAsync(Transfer transfer, CancellationToken ct);
}