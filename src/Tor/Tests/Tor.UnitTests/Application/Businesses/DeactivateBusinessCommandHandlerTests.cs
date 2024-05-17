using FluentAssertions;
using MediatR;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Commands.Deactivate;
using Tor.Application.Businesses.Notifications.BusinessDeactivated;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class DeactivateBusinessCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IBusinessRepository> _repositoryMock;
    private readonly Mock<IPublisher> _publisherMock;
    private readonly DeactivateBusinessCommandHandler _sut;

    public DeactivateBusinessCommandHandlerTests()
        : base()
    {
        _repositoryMock = new Mock<IBusinessRepository>();
        _publisherMock = new Mock<IPublisher>();

        _sut = new DeactivateBusinessCommandHandler(Context, _repositoryMock.Object, _publisherMock.Object);
    }

    [Fact]
    public async Task Deactivate_WhenValidCommand_ShouldDeactivateBusinessSuccessfully()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.User = Fakers.Users.UserFaker.Generate();
        business.StaffMembers.Add(staffMember);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        List<string> deviceTokens = staffMember.User.Devices.ConvertAll(x => x.Token);
        var command = new DeactivateBusinessCommand(business.Id);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.Deactivate(command.BusinessId, It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(x =>
            x.Publish(
                It.Is<BusinessDeactivatedNotification>(x => x.DeviceTokens.SequenceEqual(deviceTokens)),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task Deactivate_WhenBusinessDoesntExist_ShouldReturnNotFoundError()
    {
        var command = new DeactivateBusinessCommand(Guid.NewGuid());

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with id {command.BusinessId}"));
    }

    [Fact]
    public async Task Deactivate_WhenRepositoryFailed_ShouldNotPublishNotification()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.User = Fakers.Users.UserFaker.Generate();
        business.StaffMembers.Add(staffMember);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = new DeactivateBusinessCommand(business.Id);
        _repositoryMock.Setup(x => x.Deactivate(command.BusinessId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        var func = async () => await _sut.Handle(command, default);

        await func.Should().ThrowAsync<InvalidOperationException>();
        _publisherMock.Verify(x =>
            x.Publish(
                It.IsAny<BusinessDeactivatedNotification>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
    }
}
