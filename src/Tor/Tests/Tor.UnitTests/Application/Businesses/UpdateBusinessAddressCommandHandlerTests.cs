using FluentAssertions;
using Tor.Application.Businesses.Commands.UpdateAddress;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateBusinessAddressCommandHandlerTests : UnitTestBase
{
    private readonly UpdateBusinessAddressCommandHandler _sut;

    public UpdateBusinessAddressCommandHandlerTests()
        : base()
    {
        _sut = new UpdateBusinessAddressCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdateAddress()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.UpdateBusinessAddressCommandFaker.Generate() with { BusinessId = business.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(business).ReloadAsync();
        business.Address.Should().Be(command.Address);
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Businesses.UpdateBusinessAddressCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find Business with id {command.BusinessId}"));
    }
}
