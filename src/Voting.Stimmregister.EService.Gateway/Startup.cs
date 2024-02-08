// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Prometheus;
using Serilog;
using Voting.Stimmregister.EService.Gateway.Authorization;
using Voting.Stimmregister.EService.Gateway.Configuration;
using Voting.Stimmregister.EService.Gateway.Middleware;

namespace Voting.Stimmregister.EService.Gateway;

public class Startup
{
    private const string ReverseProxySection = "ReverseProxy";

    private readonly AppOptions _appConfig;
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _appConfig = configuration.Get<AppOptions>();
        _configuration = configuration;
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_appConfig);
        services.AddCors(_configuration);

        services.AddSecureConnectServiceAccount(AppOptions.EServiceServiceAccountName, _appConfig.EServiceServiceAccount);

        services
            .AddReverseProxy()
            .LoadFromConfig(_configuration.GetSection(ReverseProxySection));

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                PolicyTypes.RequiresEvoterRole,
                policy => policy.RequireClaim(ClaimTypes.Role, Roles.EVoting));
        });

        services.AddHealthChecks()
                .ForwardToPrometheus();
        ConfigureAuthentication(services.AddVotingLibIam(new() { BaseUrl = _appConfig.IdentityAccessManagementApi }));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMetricServer(_appConfig.MetricPort);
        app.UseHttpMetrics();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapVotingHealthChecks(_appConfig.LowPriorityHealthCheckNames);
            endpoints.MapReverseProxy(ProxyMiddleware.Builder);
        });
    }

    protected virtual void ConfigureAuthentication(AuthenticationBuilder builder)
        => builder.AddSecureConnectScheme(options =>
        {
            options.Audience = _appConfig.SecureConnect.Audience;
            options.Authority = _appConfig.SecureConnect.Authority;
            options.FetchRoleToken = true;
            options.LimitRolesToAppHeaderApps = false;
            options.ServiceAccount = _appConfig.SecureConnect.ServiceAccount;
            options.ServiceAccountPassword = _appConfig.SecureConnect.ServiceAccountPassword;
            options.ServiceAccountScopes = _appConfig.SecureConnect.ServiceAccountScopes;
        });
}
