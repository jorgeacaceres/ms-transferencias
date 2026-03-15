namespace ms_transferencias.Application.Common.Models;

public class AppSettings
{
    public string KafkaBootstrapServers { get; set; }
    public string TopicRiskEvaluationRequest { get; set; }
    public string TopicRiskEvaluationResponse { get; set; }
}