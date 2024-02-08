// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.Net.Http.Headers;
using Voting.Lib.Iam.ServiceTokenHandling;
using Voting.Stimmregister.EService.Gateway.Configuration;

namespace Voting.Stimmregister.EService.Gateway.Middleware;

public static class ProxyMiddleware
{
    public static Action<IReverseProxyApplicationBuilder> Builder => proxyPipeline =>
    {
        proxyPipeline.Use(async (context, next) =>
        {
            var requestPath = context.Request.Path.Value ?? string.Empty;

            if (requestPath.StartsWith("/v2/", StringComparison.OrdinalIgnoreCase))
            {
                await AddServiceTokenAuthentication(context);
            }

            await next();
        });
    };

    private static async Task AddServiceTokenAuthentication(HttpContext context)
    {
        var serviceTokenHandlerFactory = context.RequestServices.GetRequiredService<IServiceTokenHandlerFactory>();
        var handler = serviceTokenHandlerFactory.CreateHandler(AppOptions.EServiceServiceAccountName);
        var serviceToken = await handler.GetServiceToken();
        AddTokenToAuthorizationHeader(context, serviceToken);
    }

    private static void AddTokenToAuthorizationHeader(HttpContext context, string token)
    {
        context.Request.Headers[HeaderNames.Authorization] = $"Bearer {token}";
    }
}
