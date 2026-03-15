namespace ms_transferencias.Infrastructure.Entities;

public class Transfer
{
    public Guid ExternalOperationId { get; set; }
    public string CustomerId { get; set; }
    public string ServiceProviderId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}