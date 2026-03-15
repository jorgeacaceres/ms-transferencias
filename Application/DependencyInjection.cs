using ms_transferencias.Application.Common.Behaviors;
using ms_transferencias.Application.Common.Interfaces;
using ms_transferencias.Application.Common.Mappings;
using ms_transferencias.Application.Common.Models;
using ms_transferencias.Application.Events.Integration.RiskControl;

namespace ms_transferencias.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.Configure<AppSettings>(configuration.GetSection("Configuration"));
        services.AddTransient<RiskResultListener>();
        services.AddTransient<ITransferMapper, TransferMapper>();
        return services;
    }
}