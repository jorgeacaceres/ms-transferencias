using MediatR;

namespace ms_transferencias.Application.Commands.PublishMessage;

public class PublishMessageCommand : IRequest
{
    public string Topico { get; set; }
    public Object Mensaje { get; set; }
}