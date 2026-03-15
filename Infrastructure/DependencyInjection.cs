using Microsoft.EntityFrameworkCore;
using ms_transferencias.Application.Common.Interfaces;
using ms_transferencias.Infrastructure.Messaging;
using ms_transferencias.Infrastructure.Persistence;
using ms_transferencias.Infrastructure.Persistence.Repositories;

namespace ms_transferencias.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddCap(x =>
        {
            x.UseEntityFramework<ApplicationDbContext>();
            x.UseKafka(configuration.GetValue<string>("Configuration:KafkaBootstrapServers"));
        });
        services.AddTransient<ITransferRepository, TransferRepository>();
        services.AddTransient<IMessagePublisher, MessagePublisher>();

        return services;
    }
}