namespace ms_transferencias.Application.Common.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync(string topic, object message);
}
