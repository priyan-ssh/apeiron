using Apeiron.Application;
using Apeiron.Infrastructure;
using Apeiron.Api.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    //adding serilog configs
    builder.Host.UseSerilog((context,service,configuration)=>
    configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(service)
    .Enrich.FromLogContext()
    );

    // 1. Add Layers (The Clean Architecture Wiring)
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // 2. Add API Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // 3. The Middleware Pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.UseExceptionHandler();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}



