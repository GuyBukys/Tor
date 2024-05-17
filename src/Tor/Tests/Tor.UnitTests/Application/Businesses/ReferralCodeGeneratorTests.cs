using FluentAssertions;
using System.Text.RegularExpressions;
using Tor.Application.Businesses.Common;

namespace Tor.UnitTests.Application.Businesses;

public class ReferralCodeGeneratorTests
{
    [Theory]
    [InlineData("Example Business")]
    [InlineData("123 Corporation")]
    [InlineData("Special$Chars")]
    [InlineData("LongBusinessNameWithSpacesAndNumbers123")]
    [InlineData("12345")]
    [InlineData("mybusiness!@#$%^&*()")]
    [InlineData("  המספרה של אופק  ")]
    public void Generate_WhenValidInput_ShouldReturnsValidCode(string businessName)
    {
        string referralCode = ReferralCodeGenerator.Generate(businessName);

        referralCode.Should().NotBeNull();
        referralCode.Should().MatchRegex("^[a-zA-Z0-9א-ת]+$");
        string digits = Regex.Match(referralCode, @"\d+").Value;
        digits.Length.Should().BeInRange(1, 3);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null!)]
    [InlineData("     ")]
    public void Generate_WhenInvalidInput_ShouldThrowArgumentException(string businessName)
    {
        Action action = () => ReferralCodeGenerator.Generate(businessName);

        action.Should().Throw<ArgumentException>();
    }
}
