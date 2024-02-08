// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.EService.Gateway.Authorization;

public static class Roles
{
    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>register for E-Voting</item>
    ///     <item>unregister from E-Voting</item>
    ///     <item>read E-Voting status</item>
    /// </list>
    /// </summary>
    public const string EVoting = "EVoting";
}
