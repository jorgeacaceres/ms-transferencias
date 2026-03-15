using MediatR;
using ms_transferencias.Application.Common.Interfaces;
using ms_transferencias.Application.Common.Models;
using OneOf;

namespace ms_transferencias.Application.Commands.UpdateTransfer;

public class UpdateTransferCommandHandler : IRequestHandler<UpdateTransferCommand, OneOf<Unit, ErrorResponse>>
{
    private readonly ILogger<UpdateTransferCommandHandler> _logger;
    private readonly ITransferRepository _transferRepository;
    public UpdateTransferCommandHandler(ILogger<UpdateTransferCommandHandler> logger,
                                        ITransferRepository transferRepository)
    {
        _logger = logger;
        _transferRepository = transferRepository;
    }

    public async Task<OneOf<Unit, ErrorResponse>> Handle(UpdateTransferCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Inicia petición para actualizar la transferencia {@command}", command);
        var transferencia = await _transferRepository.GetByExternalIdAsync(command.ExternalOperationId, ct);
        _logger.LogInformation("Transferencia obtenida {@transferencia}", transferencia);
        if (transferencia is null)
        {
            return new ErrorResponse { Message = "No existe transferencia" };
        }
        transferencia.Status = command.Status;
        transferencia.UpdatedAt = DateTime.UtcNow;
        var updateTransfer = await _transferRepository.UpdateTransferAsync(transferencia, ct);
        _logger.LogInformation("Se proceso la solicitud {updateTransfer}", updateTransfer);
        if (transferencia is null)
        {
            return new ErrorResponse { Message = "No se pudo actualizar la transferencia" };
        }
        return default;
    }
}