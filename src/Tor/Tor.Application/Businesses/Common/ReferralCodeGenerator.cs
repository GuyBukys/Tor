using System.Text.RegularExpressions;

namespace Tor.Application.Businesses.Common;

internal static partial class ReferralCodeGenerator
{
    public static string Generate(string businessName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(businessName);

        string sanitized = LettersRegex().Replace(businessName, string.Empty);
        int randomNumber = Random.Shared.Next(1, 999);

        return $"{sanitized}{randomNumber}";
    }

    [GeneratedRegex(@"[^\p{L}]")]
    private static partial Regex LettersRegex();
}
