namespace ms_transferencias.Application.Events.Integration.RiskControl;

public record RiskResultEvent(Guid ExternalOperationId, string Status);