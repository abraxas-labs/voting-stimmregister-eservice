// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Iam.AuthenticationScheme;
using Voting.Lib.Iam.ServiceTokenHandling;

namespace Voting.Stimmregister.EService.Gateway.Configuration;

public class AppOptions
{
    public const string EServiceServiceAccountName = nameof(EServiceServiceAccount);

    /// <summary>
    /// Gets or sets the identity provider api.
    /// </summary>
    public Uri? IdentityAccessManagementApi { get; set; }

    public SecureConnectOptions SecureConnect { get; set; } = new();

    public SecureConnectServiceAccountOptions EServiceServiceAccount { get; set; } = new();

    public PortOptions Ports { get; set; } = new();

    /// <summary>
    /// Gets or sets the port configuration for the metric endpoint.
    /// </summary>
    public ushort MetricPort { get; set; } = 9090;

    public HashSet<string> LowPriorityHealthCheckNames { get; set; } = new();
}
