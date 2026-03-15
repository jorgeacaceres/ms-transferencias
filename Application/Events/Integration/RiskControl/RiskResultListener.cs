using DotNetCore.CAP;
using MediatR;
using ms_transferencias.Application.Commands.UpdateTransfer;

namespace ms_transferencias.Application.Events.Integration.RiskControl;

public class RiskResultListener : ICapSubscribe
{
    private readonly ILogger<RiskResultListener> _logger;
    private readonly IMediator _mediator;

    public RiskResultListener(ILogger<RiskResultListener> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [CapSubscribe("risk-evaluation-response")]
    public async Task ReceiveRiskResult(RiskResultEvent @event, CancellationToken ct)
    {
        _logger.LogInformation("Inicia proceso de actualización de estado {@event}", @event);
        if (@event.ExternalOperationId == Guid.Empty)
        {
            _logger.LogWarning("Formato de GUID inválido: {ExternalId}", @event.ExternalOperationId);
        }
        await _mediator.Send(new UpdateTransferCommand
        {
            ExternalOperationId = @event.ExternalOperationId,
            Status = @event.Status
        }, ct);
    }
}