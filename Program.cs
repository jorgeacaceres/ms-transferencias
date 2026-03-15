using FluentValidation;
using ms_transferencias.Application;
using ms_transferencias.Endpoints;
using ms_transferencias.Infrastructure;
using Serilog;

try
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    var builder = WebApplication.CreateBuilder(args);
    Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
    builder.Host.UseSerilog();
    builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
    builder.Services.AddApplication(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.MapEndpoints();
    app.UseSerilogRequestLogging();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fallo en inicio del microservicio");
}
finally
{
    Log.CloseAndFlush();
}