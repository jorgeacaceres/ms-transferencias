using ms_transferencias.Application.Commands.CreateTransfer;
using ms_transferencias.Application.Common.Models;
using ms_transferencias.Application.Queries.GetTransferById;
using ms_transferencias.Infrastructure.Entities;

namespace ms_transferencias.Application.Common.Interfaces;

public interface ITransferMapper
{
    Transfer CommandToEntity(CreateTransferCommand command);
    CreateTransferResponse EntityToResponse(Transfer command);
    GetTransferByIdResponse EntityToResponseQuery(Transfer command);
    RiskControl EntityToRiskModel(Transfer command);
}