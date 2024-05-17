using FluentAssertions;
using Tor.Application.Businesses.Commands.UpdateSettings;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateBusinessSettingsCommandHandlerTests : UnitTestBase
{
    private readonly UpdateBusinessSettingsCommandHandler _sut;

    public UpdateBusinessSettingsCommandHandlerTests()
        : base()
    {
        _sut = new UpdateBusinessSettingsCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdateBusinessSettings()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate() with { BusinessId = business.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(business).ReloadAsync();
        business.Settings.Should().Be(command.BusinessSettings);
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Businesses.UpdateBusinessSettingsCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with id {command.BusinessId}"));
    }

}
