using MediatR;
using ms_transferencias.Application.Common.Models;
using OneOf;

namespace ms_transferencias.Application.Commands.UpdateTransfer;

public class UpdateTransferCommand : IRequest<OneOf<Unit, ErrorResponse>>
{
    public Guid ExternalOperationId { get; set; }
    public string Status { get; set; }
}