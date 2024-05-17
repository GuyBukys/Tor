using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Users.Commands.AddOrUpdateDevice;
using Tor.Domain.UserAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Users;

public class AddOrUpdateDeviceCommandHandlerTests : UnitTestBase
{
    private readonly AddOrUpdateDeviceCommandHandler _sut;

    public AddOrUpdateDeviceCommandHandlerTests()
        : base()
    {
        _sut = new AddOrUpdateDeviceCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenDeviceDoesntExist_ShouldAddDeviceToUser()
    {
        User user = Fakers.Users.UserFaker.Generate();
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        string deviceToken = Guid.NewGuid().ToString();
        var command = new AddOrUpdateDeviceCommand(user.UserToken, deviceToken);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(x => x.Token == deviceToken);
        await Context.Entry(user).ReloadAsync();
        user.Devices.Should().Contain(x => x.Token == deviceToken);
    }

    [Fact]
    public async Task Handle_WhenDeviceExists_ShouldDoNothing()
    {
        User user = Fakers.Users.UserFaker.Generate();
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        string deviceToken = user.Devices.First().Token;
        var command = new AddOrUpdateDeviceCommand(user.UserToken, deviceToken);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(x => x.Token == deviceToken);
        await Context.Entry(user).ReloadAsync();
        user.Devices.SingleOrDefault(x => x.Token == deviceToken).Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenUserDoesntExist_ShouldReturnNotFoundError()
    {
        var command = new AddOrUpdateDeviceCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find user with token {command.UserToken}"));
    }
}
