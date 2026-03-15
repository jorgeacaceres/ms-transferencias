using ms_transferencias.Application.Commands.CreateTransfer;
using ms_transferencias.Application.Commands.UpdateTransfer;
using ms_transferencias.Application.Common.Interfaces;
using ms_transferencias.Application.Common.Models;
using ms_transferencias.Application.Events.Integration.RiskControl;
using ms_transferencias.Application.Queries.GetTransferById;
using ms_transferencias.Infrastructure.Entities;
using Riok.Mapperly.Abstractions;

namespace ms_transferencias.Application.Common.Mappings;

[Mapper]
public partial class TransferMapper : ITransferMapper
{
    public partial Transfer CommandToEntity(CreateTransferCommand command);
    public partial CreateTransferResponse EntityToResponse(Transfer command);
    public partial GetTransferByIdResponse EntityToResponseQuery(Transfer command);
    public partial RiskControl EntityToRiskModel(Transfer command);
}