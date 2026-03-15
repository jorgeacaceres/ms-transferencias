using MediatR;
using ms_transferencias.Application.Common.Models;
using OneOf;

namespace ms_transferencias.Application.Commands.CreateTransfer;

public class CreateTransferCommand : IRequest<OneOf<CreateTransferResponse, ErrorResponse>>
{
    public string CustomerId { get; set; }
    public string ServiceProviderId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
}