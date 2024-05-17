using FluentAssertions;
using MediatR;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.StaffMembers.Commands.AddStaffMember;
using Tor.Application.StaffMembers.Notifications.StaffMemberAdded;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.CategoryAggregate.Enums;
using Tor.Domain.Common.Enums;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.StaffMembers;

public class AddStaffMemberCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IPublisher> _mockPublisher;
    private readonly Mock<IStorageManager> _mockStorageManager;
    private readonly AddStaffMemberCommandHandler _sut;

    public AddStaffMemberCommandHandlerTests()
        : base()
    {
        _mockPublisher = new Mock<IPublisher>();
        _mockStorageManager = new Mock<IStorageManager>();

        _sut = new AddStaffMemberCommandHandler(Context, _mockPublisher.Object, _mockStorageManager.Object);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenValidCommand_ShouldAddStaffMemberToBusiness(bool addProfileImage)
    {
        var defaultImage = Fakers.Images.ImageFaker.Generate();
        _mockStorageManager.Setup(x => x.GetDefaultImage(ImageType.Profile, EntityType.StaffMember, It.IsAny<CategoryType?>()))
            .Returns(defaultImage);
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        User user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.BusinessApp;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate() with
        {
            BusinessId = business.Id,
            UserId = user.Id,
            ProfileImage = addProfileImage ? Fakers.Images.ImageFaker.Generate() : null,
        };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await Context.Entry(business).ReloadAsync();
        business.StaffMembers.Should().Contain(result.Value);
        business.StaffMembers.First(x => x.Id == result.Value.Id).ProfileImage
            .Should()
            .Be(addProfileImage ? command.ProfileImage : defaultImage);
        await Context.Entry(user).ReloadAsync();
        user.EntityId.Should().Be(result.Value.Id);
        _mockPublisher.Verify(x => x.Publish(It.IsAny<StaffMemberAddedNotification>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with id {command.BusinessId}"));
        _mockPublisher.Verify(x => x.Publish(It.IsAny<StaffMemberAddedNotification>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task Handle_WhenUserDoesntExist_ShouldReturnDomainError()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate() with
        {
            BusinessId = business.Id,
            UserId = Guid.NewGuid(),
        };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(DomainError) && x.Message.Contains($"user id {command.UserId} doesnt exist"));
        _mockPublisher.Verify(x => x.Publish(It.IsAny<StaffMemberAddedNotification>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task Handle_WhenUserAppTypeIsNotBusinessApp_ShouldReturnDomainError()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        User user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.ClientApp;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var command = Fakers.StaffMembers.AddStaffMemberCommandFaker.Generate() with
        {
            BusinessId = business.Id,
            UserId = user.Id,
        };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(DomainError) && x.Message.Contains("staff member user is not a BusinessApp user type"));
        _mockPublisher.Verify(x => x.Publish(It.IsAny<StaffMemberAddedNotification>(), It.IsAny<CancellationToken>()), Times.Never());
    }
}
