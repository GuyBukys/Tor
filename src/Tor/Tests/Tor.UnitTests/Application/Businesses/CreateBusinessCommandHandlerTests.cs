using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Commands.Create;
using Tor.Application.Businesses.Notifications.BusinessCreated;
using Tor.Application.Users.Commands.Create;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.CategoryAggregate;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;
public class CreateBusinessCommandHandlerTests : UnitTestBase
{
    private readonly CreateBusinessCommandHandler _sut;
    private readonly Mock<IStorageManager> _mockStorageManager;
    private readonly Mock<IPublisher> _mockPublisher;

    public CreateBusinessCommandHandlerTests()
        : base()
    {
        _mockStorageManager = new Mock<IStorageManager>();
        _mockPublisher = new Mock<IPublisher>();

        _sut = new CreateBusinessCommandHandler(Context, _mockStorageManager.Object, _mockPublisher.Object);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldCreateBusiness()
    {
        CreateBusinessCommand command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        await Context.Categories.AddRangeAsync(command.CategoryIds.Select(x => new Category(x)));
        await Context.Users.AddAsync(new User(command.BusinessOwner.UserId) { AppType = AppType.BusinessApp, PhoneNumber = Fakers.Common.PhoneNumberFaker.Generate() });
        await Context.SaveChangesAsync();
        _mockStorageManager.Setup(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        _mockStorageManager.Verify(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mockPublisher.Verify(x => x.Publish(It.IsAny<BusinessCreatedNotification>(), It.IsAny<CancellationToken>()), Times.Once());
        var businessFromDb = await Context.Businesses.FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        businessFromDb.Should().NotBeNull();
        businessFromDb!.ReferralCode.Should().NotBeEmpty();
        businessFromDb!.Logo.Should().Be(command.Logo);
        businessFromDb!.Cover.Should().Be(command.Cover);
    }

    [Fact]
    public async Task Handle_WhenValidCommandAndHasReferringBusiness_ShouldCreateBusinessAndAddReferringBusiness()
    {
        var referringBusiness = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(referringBusiness);
        await Context.SaveChangesAsync();
        CreateBusinessCommand command = Fakers.Businesses.CreateBusinessCommandFaker.Generate() with
        {
            ReferringBusinessId = referringBusiness.Id,
        };
        await Context.Categories.AddRangeAsync(command.CategoryIds.Select(x => new Category(x)));
        await Context.Users.AddAsync(new User(command.BusinessOwner.UserId) { AppType = AppType.BusinessApp, PhoneNumber = Fakers.Common.PhoneNumberFaker.Generate() });
        await Context.SaveChangesAsync();

        _mockStorageManager.Setup(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        _mockStorageManager.Verify(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        _mockPublisher.Verify(x => x.Publish(It.IsAny<BusinessCreatedNotification>(), It.IsAny<CancellationToken>()), Times.Once());
        var businessFromDb = await Context.Businesses
            .Include(x => x.ReferringBusiness)
            .FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        businessFromDb.Should().NotBeNull();
        businessFromDb!.ReferralCode.Should().NotBeEmpty();
        businessFromDb!.ReferringBusinessId.Should().Be(referringBusiness.Id);
        businessFromDb!.ReferringBusiness.Should().BeEquivalentTo(referringBusiness);
    }

    [Fact]
    public async Task Handle_WhenCategoriesDoesntExist_ShouldReturnFailedResult()
    {
        CreateBusinessCommand command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        await Context.Users.AddAsync(new User(command.BusinessOwner.UserId) { AppType = AppType.BusinessApp, PhoneNumber = Fakers.Common.PhoneNumberFaker.Generate() });
        await Context.SaveChangesAsync();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x => x.Message.Contains("category ids"));
    }

    [Fact]
    public async Task Handle_WhenBusinessOwnerUserDoesntExist_ShouldReturnFailedResult()
    {
        CreateBusinessCommand command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        await Context.Categories.AddRangeAsync(command.CategoryIds.Select(x => new Category(x)));
        await Context.SaveChangesAsync();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x => x.Message.Contains($"user id {command.BusinessOwner.UserId} doesnt exist"));
    }

    [Fact]
    public async Task Handle_WhenBusinessOwnerUserIsNotAStaffMemberType_ShouldReturnFailedResult()
    {
        CreateBusinessCommand command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        await Context.Categories.AddRangeAsync(command.CategoryIds.Select(x => new Category(x)));
        await Context.Users.AddAsync(new User(command.BusinessOwner.UserId) { AppType = AppType.ClientApp, PhoneNumber = Fakers.Common.PhoneNumberFaker.Generate() });
        await Context.SaveChangesAsync();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x => x.Message.Contains("business owner user is not a BusinessApp user type"));
    }

    [Fact]
    public async Task Handle_WhenStaffMemberAlreadyExists_ShouldReturnFailedResult()
    {
        CreateBusinessCommand command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        await Context.Users.AddAsync(new User(command.BusinessOwner.UserId) { AppType = AppType.BusinessApp, PhoneNumber = Fakers.Common.PhoneNumberFaker.Generate() });
        await Context.Categories.AddRangeAsync(command.CategoryIds.Select(x => new Category(x)));
        await Context.StaffMembers.AddAsync(new StaffMember(Guid.NewGuid()) { UserId = command.BusinessOwner.UserId });
        await Context.SaveChangesAsync();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x => x.Message.Contains($"staff member with user id {command.BusinessOwner.UserId} already exists"));
    }

    [Fact]
    public async Task Handle_WhenSomeImagesDoesntExist_ShouldReturnFailedResult()
    {
        CreateBusinessCommand command = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        await Context.Categories.AddRangeAsync(command.CategoryIds.Select(x => new Category(x)));
        await Context.Users.AddAsync(new User(command.BusinessOwner.UserId) { AppType = AppType.BusinessApp, PhoneNumber = Fakers.Common.PhoneNumberFaker.Generate() });
        await Context.SaveChangesAsync();
        _mockStorageManager.Setup(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x => x.Message.Contains("some images do not exist in storage bucket"));
    }
}
