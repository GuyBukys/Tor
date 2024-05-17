using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Users.Commands.Create;
using Tor.Domain.UserAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Users;

public class CreateUserCommandHandlerTests : UnitTestBase
{
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
        : base()
    {
        _sut = new CreateUserCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenUserExist_ShouldReturnConflictError()
    {
        User user = Fakers.Users.UserFaker.Generate();
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var command = Fakers.Users.CreateUserCommandFaker.Generate() with
        {
            UserToken = user.UserToken,
            AppType = user.AppType
        };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(ConflictError) && x.Message.Contains($"user with token {command.UserToken} and app type {command.AppType} already exists"));
    }

    [Fact]
    public async Task Handle_WhenUserDoesntExist_ShouldCreateAndReturnNewUser()
    {
        Context.Users.RemoveRange(await Context.Users.ToListAsync());
        var command = Fakers.Users.CreateUserCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserToken.Should().Be(command.UserToken);
        result.Value.AppType.Should().Be(command.AppType);
        result.Value.Devices.Should().Contain(x => x.Token == command.DeviceToken);
        result.Value.PhoneNumber.Should().Be(command.PhoneNumber);
        var userFromDb = await Context.Users.FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        userFromDb.Should().NotBeNull();
    }
}
