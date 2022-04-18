using ApplicationCore;
using WorkerService;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {
        services.AddHostedService<Worker>();
        services.AddApplicationCore(context.Configuration);
    })
    .UseSerilog((_, loggerConfiguration) => loggerConfiguration.WriteTo.Console());

IHost host = builder.Build();

await host.RunAsync();
