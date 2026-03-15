using MediatR;
using ms_transferencias.Application.Common.Interfaces;
using ms_transferencias.Application.Common.Models;
using OneOf;

namespace ms_transferencias.Application.Queries.GetTransferById;

public class GetTransferByIdQueryHandler : IRequestHandler<GetTransferByIdQuery, OneOf<GetTransferByIdResponse, ErrorResponse>>
{
    private readonly ILogger<GetTransferByIdQueryHandler> _logger;
    private readonly ITransferRepository _transferRepository;
    private readonly ITransferMapper _transferMapper;
    public GetTransferByIdQueryHandler(ILogger<GetTransferByIdQueryHandler> logger,
                                       ITransferRepository transferRepository,
                                       ITransferMapper transferMapper)
    {
        _logger = logger;
        _transferRepository = transferRepository;
        _transferMapper = transferMapper;
    }

    public async Task<OneOf<GetTransferByIdResponse, ErrorResponse>> Handle(GetTransferByIdQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Inicia petición para la consulta de la transferencia {@request}", request);
        if (!Guid.TryParse(request.ExternalOperationId, out var id) || id == Guid.Empty)
        {
            _logger.LogWarning("Formato de GUID inválido: {ExternalId}", request.ExternalOperationId);
            return new ErrorResponse
            {
                Message = $"Formato de GUID inválido: {request.ExternalOperationId}"
            };
        }
        var result = await _transferRepository.GetByExternalIdAsync(id, ct);
        _logger.LogInformation("Transferencia obtenida {@result}", result);
        if (result is null)
        {
            _logger.LogWarning("No existe transferencia {ExternalOperationId}", request.ExternalOperationId);
            return default;
        }
        return _transferMapper.EntityToResponseQuery(result);
    }
}