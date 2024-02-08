// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.EService.Gateway.Configuration;

public class PortOptions
{
    public ushort Http { get; set; } = 5000;

    public ushort Http2 { get; set; } = 5001;
}
