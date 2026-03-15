namespace ms_transferencias.Application.Queries.GetTransferById;

public class GetTransferByIdResponse
{
    public string ExternalOperationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
}