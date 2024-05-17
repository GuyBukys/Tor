using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Clients.Commands.Create;
using Tor.Application.Clients.Notifications.ClientCreated;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Clients;

public class CreateClientCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IStorageManager> _storageManagerMock;
    private readonly Mock<IPublisher> _publisherMock;
    private readonly CreateClientCommandHandler _sut;

    public CreateClientCommandHandlerTests()
        : base()
    {
        _storageManagerMock = new Mock<IStorageManager>();
        _publisherMock = new Mock<IPublisher>();

        _sut = new CreateClientCommandHandler(Context, _storageManagerMock.Object, _publisherMock.Object);
    }

    [Fact]
    public async Task Create_WhenValidCommand_ShouldCreateClientSuccessfully()
    {
        User user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.ClientApp;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var command = Fakers.Clients.CreateClientCommandFaker.Generate() with
        {
            UserId = user.Id,
        };
        _storageManagerMock.Setup(x => x.IsFileExists(command.ProfileImage!.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        var clientFromDb = await Context.Clients.FirstOrDefaultAsync();
        clientFromDb.Should().NotBeNull();
        clientFromDb.Should().BeEquivalentTo(command, cfg => cfg.ExcludingMissingMembers());
        await Context.Entry(user).ReloadAsync();
        user.EntityId.Should().Be(clientFromDb!.Id);
        _publisherMock.Verify(x => x.Publish(It.Is<ClientCreatedNotification>(x => x.ClientId == clientFromDb!.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WhenUserAlreadyHasAnEntityId_ShouldReturnDomainError()
    {
        var user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.ClientApp;
        user.EntityId = Guid.NewGuid();
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var command = Fakers.Clients.CreateClientCommandFaker.Generate() with
        {
            UserId = user.Id,
        };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(DomainError) && x.Message.Contains($"user alrady in use with id {user.EntityId.Value}"));
    }

    [Fact]
    public async Task Create_WhenImageNotInStorage_ShouldReturnDomainError()
    {
        User user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.ClientApp;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var command = Fakers.Clients.CreateClientCommandFaker.Generate() with
        {
            UserId = user.Id,
        };
        _storageManagerMock.Setup(x => x.IsFileExists(command.ProfileImage!.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(DomainError) && x.Message.Contains("profile image does not exist in storage"));
    }

    [Fact]
    public async Task Create_WhenUserDoesntExist_ShouldReturnDomainError()
    {
        var command = Fakers.Clients.CreateClientCommandFaker.Generate();
        _storageManagerMock.Setup(x => x.IsFileExists(command.ProfileImage!.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(DomainError) && x.Message.Contains($"user id {command.UserId} doesnt exist"));
    }

    [Fact]
    public async Task Create_WhenUserIsNotClientApp_ShouldReturnDomainError()
    {
        User user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.BusinessApp;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var command = Fakers.Clients.CreateClientCommandFaker.Generate() with
        {
            UserId = user.Id,
        };
        _storageManagerMock.Setup(x => x.IsFileExists(command.ProfileImage!.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(DomainError) && x.Message.Contains("user is not a ClientApp user type"));
    }
}
