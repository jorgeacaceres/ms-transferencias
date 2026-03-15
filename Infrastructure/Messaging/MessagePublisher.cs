using DotNetCore.CAP;
using ms_transferencias.Application.Common.Interfaces;

namespace ms_transferencias.Infrastructure.Messaging;

public class MessagePublisher : IMessagePublisher
{
    private readonly ICapPublisher _capPublisher;

    public MessagePublisher(ICapPublisher capPublisher) => _capPublisher = capPublisher;

    public async Task PublishAsync(string topic, object message)
    {
        await _capPublisher.PublishAsync(topic, message);
    }
}