using Aspire.ServiceDefaults;
using Chronicles.Application;
using Chronicles.Infrastructure;
using Chronicles.Worker;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services
    .AddOptions<ChroniclesWorkerOptions>()
    .Bind(builder.Configuration.GetSection(ChroniclesWorkerOptions.SectionName))
    .Validate(options => options.BatchSize > 0, "ChroniclesWorker:BatchSize must be greater than zero.")
    .Validate(options => options.PollIntervalSeconds > 0, "ChroniclesWorker:PollIntervalSeconds must be greater than zero.")
    .ValidateOnStart();

builder.Services
    .AddChroniclesApplication()
    .AddChroniclesInfrastructure(builder.Configuration)
    .AddHostedService<ChroniclesProcessingWorker>();

IHost host = builder.Build();

await host.RunAsync();
