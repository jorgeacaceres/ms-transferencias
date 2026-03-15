using MediatR;
using ms_transferencias.Application.Common.Models;
using OneOf;

namespace ms_transferencias.Application.Queries.GetTransferById;

public class GetTransferByIdQuery : IRequest<OneOf<GetTransferByIdResponse, ErrorResponse>>
{
    public string ExternalOperationId { get; set; }
}