// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration.Json;
using Serilog;
using Voting.Stimmregister.EService.Gateway.Configuration;

namespace Voting.Stimmregister.EService.Gateway;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                // we deploy our config with the docker image, no need to watch for changes
                foreach (var source in config.Sources.OfType<JsonConfigurationSource>())
                {
                    source.ReloadOnChange = false;
                }
            })
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
                webBuilder
                .UseStartup<Startup>()
                .ConfigureKestrel(server =>
                {
                    var config = server.ApplicationServices.GetRequiredService<AppOptions>();
                    server.ListenAnyIP(config.Ports.Http, o => o.Protocols = HttpProtocols.Http1);
                    server.ListenAnyIP(config.Ports.Http2, o => o.Protocols = HttpProtocols.Http2);
                    server.ListenAnyIP(config.MetricPort, o => o.Protocols = HttpProtocols.Http1);
                }));
}
