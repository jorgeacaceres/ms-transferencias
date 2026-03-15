namespace ms_transferencias.Application.Commands.CreateTransfer;

public class CreateTransferResponse
{
    public string ExternalOperationId { get; set; }
    public string CustomerId { get; set; }
    public string ServiceProviderId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
}