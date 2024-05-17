using FluentAssertions;
using MediatR;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Clients.Commands.Deactivate;
using Tor.Application.Clients.Notifications.ClientDeactivated;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.ClientAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Clients;

public class DeactivateClientCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IClientRepository> _repositoryMock;
    private readonly Mock<IPublisher> _publisherMock;
    private readonly DeactivateClientCommandHandler _sut;

    public DeactivateClientCommandHandlerTests()
        : base()
    {
        _repositoryMock = new Mock<IClientRepository>();
        _publisherMock = new Mock<IPublisher>();

        _sut = new DeactivateClientCommandHandler(Context, _repositoryMock.Object, _publisherMock.Object);
    }

    [Fact]
    public async Task Deactivate_WhenValidCommand_ShouldDeactivateClientSuccessfully()
    {
        Client client = Fakers.Clients.ClientFaker.Generate();
        client.User!.EntityId = client.Id;
        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        var command = new DeactivateClientCommand(client.Id);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.Deactivate(command.ClientId, It.IsAny<CancellationToken>()), Times.Once);
        List<string> deviceTokens = client.User!.Devices.ConvertAll(x => x.Token);
        _publisherMock.Verify(x =>
            x.Publish(
                It.Is<ClientDeactivatedNotification>(x => x.DeviceTokens.SequenceEqual(deviceTokens)),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task Deactivate_WhenClientDoesntExist_ShouldReturnNotFoundError()
    {
        var command = new DeactivateClientCommand(Guid.NewGuid());

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find client with id {command.ClientId}"));
    }

    [Fact]
    public async Task Deactivate_WhenRepositoryFailed_ShouldNotPublishNotification()
    {
        Client client = Fakers.Clients.ClientFaker.Generate();
        client.User!.EntityId = client.Id;
        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        _repositoryMock.Setup(x => x.Deactivate(client.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());
        var command = new DeactivateClientCommand(client.Id);

        var func = async () => await _sut.Handle(command, default);

        await func.Should().ThrowAsync<InvalidOperationException>();
        _publisherMock.Verify(x =>
            x.Publish(
                It.IsAny<ClientDeactivatedNotification>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
    }
}
