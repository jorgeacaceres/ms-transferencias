using MediatR;
using Microsoft.Extensions.Options;
using ms_transferencias.Application.Commands.PublishMessage;
using ms_transferencias.Application.Common.Interfaces;
using ms_transferencias.Application.Common.Models;
using OneOf;

namespace ms_transferencias.Application.Commands.CreateTransfer;

public class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand,
                                                            OneOf<CreateTransferResponse, ErrorResponse>>
{
    private readonly ILogger<CreateTransferCommandHandler> _logger;
    private readonly ITransferRepository _transferRepository;
    private readonly ITransferMapper _transferMapper;
    private readonly IMediator _mediator;
    private readonly AppSettings _config;
    public CreateTransferCommandHandler(ILogger<CreateTransferCommandHandler> logger,
                                        ITransferRepository transferRepository,
                                        ITransferMapper transferMapper,
                                        IMediator mediator,
                                        IOptions<AppSettings> config)
    {
        _logger = logger;
        _transferRepository = transferRepository;
        _transferMapper = transferMapper;
        _mediator = mediator;
        _config = config.Value;
    }
    public async Task<OneOf<CreateTransferResponse, ErrorResponse>> Handle(CreateTransferCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Inicia petición para la inserción de transferencia {@command}", command);
        var request = _transferMapper.CommandToEntity(command);
        _logger.LogInformation("Transferencia a insertar {@request}", request);
        var result = await _transferRepository.AddTransferAsync(request, ct);
        _logger.LogInformation("Transferencia insertada {@result}", result);
        if (result?.ExternalOperationId == null || result?.ExternalOperationId == Guid.Empty)
        {
            _logger.LogWarning("No se pudo insertar el registro");
            return new ErrorResponse { Message = "No se pudo insertar el registro" };
        }
        _logger.LogInformation("Registro insertado correctamente");
        var message = _transferMapper.EntityToRiskModel(result);
        await _mediator.Send(new PublishMessageCommand
        {
            Topico = _config.TopicRiskEvaluationRequest,
            Mensaje = message
        }, ct);
        return _transferMapper.EntityToResponse(result);
    }
}