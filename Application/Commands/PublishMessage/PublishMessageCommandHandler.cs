using MediatR;
using ms_transferencias.Application.Common.Interfaces;

namespace ms_transferencias.Application.Commands.PublishMessage;

public class PublishMessageCommandHandler : IRequestHandler<PublishMessageCommand>

{
    private readonly ILogger<PublishMessageCommandHandler> _logger;
    private readonly IMessagePublisher _publisher;

    public PublishMessageCommandHandler(ILogger<PublishMessageCommandHandler> logger, IMessagePublisher publisher)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Handle(PublishMessageCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Enviando evento al tópico {@Topic}", request);
        await _publisher.PublishAsync(request.Topico, request.Mensaje);
        _logger.LogInformation("Evento publicado exitosamente.");
    }
}
