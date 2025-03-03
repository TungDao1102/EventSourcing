using EventSourcing.Projections.Database;
using EventSourcing.Projections.Projections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder();

builder.ConfigureHostConfiguration(config =>
{
    config.AddJsonFile("appsettings.json", false);
    config.AddEnvironmentVariables();
});

builder.ConfigureServices((_, services) =>
{
    services.RegisterDataConnections();
    services.RegisterProjections();
});

var app = builder.Build();

app.Run();
