using FluentAssertions;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Commands.UpdateImages;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateBusinessImagesCommandHandlerTests : UnitTestBase
{
    private readonly UpdateBusinessImagesCommandHandler _sut;
    private readonly Mock<IStorageManager> _storageManagerMock;

    public UpdateBusinessImagesCommandHandlerTests()
        : base()
    {
        _storageManagerMock = new Mock<IStorageManager>();
        _sut = new UpdateBusinessImagesCommandHandler(Context, _storageManagerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdateBusinessImages()
    {
        _storageManagerMock.Setup(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.UpdateBusinessImagesCommandFaker.Generate() with { BusinessId = business.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(business).ReloadAsync();
        business.Logo.Should().Be(command.Logo);
        business.Cover.Should().Be(command.Cover);
        business.Portfolio.Should().BeEquivalentTo(command.Portfolio);
    }

    [Theory]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    public async Task Handle_WhenImageIsNull_ShouldNotUpdate(bool updateLogo, bool updateCover, bool updatePortfolio)
    {
        _storageManagerMock.Setup(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.UpdateBusinessImagesCommandFaker.Generate() with
        {
            BusinessId = business.Id,
            Logo = updateLogo ? Fakers.Images.ImageFaker.Generate() : null!,
            Cover = updateCover ? Fakers.Images.ImageFaker.Generate() : null!,
            Portfolio = updatePortfolio ? [Fakers.Images.ImageFaker.Generate()] : null!,
        };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(business).ReloadAsync();
        business.Logo.Should().Be(updateLogo ? command.Logo : business.Logo);
        business.Cover.Should().Be(updateCover ? command.Cover : business.Cover);
        business.Portfolio.Should().BeEquivalentTo(updatePortfolio ? command.Portfolio : business.Portfolio);
    }

    [Theory]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    public async Task Handle_WhenSomeImagesDoesntExistInStorage_ShouldReturnNotFoundError(bool missingLogo, bool missingCover, bool missingPortfolio)
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.UpdateBusinessImagesCommandFaker.Generate() with
        {
            BusinessId = business.Id,
        };
        SetupStorageManager(command, missingLogo, missingCover, missingPortfolio);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains(
                missingLogo ? command.Logo!.Name :
                missingCover ? command.Cover!.Name :
                missingPortfolio ? command.Portfolio!.First().Name : string.Empty));
    }

    private void SetupStorageManager(UpdateBusinessImagesCommand command, bool missingLogo, bool missingCover, bool missingPortfolio)
    {
        _storageManagerMock.Setup(x => x.IsFileExists(command.Logo!.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(!missingLogo);
        _storageManagerMock.Setup(x => x.IsFileExists(command.Cover!.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(!missingCover);
        foreach (var portfolio in command.Portfolio!)
        {
            _storageManagerMock.Setup(x => x.IsFileExists(portfolio.Name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(!missingPortfolio);
        }
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Businesses.UpdateBusinessImagesCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with id {command.BusinessId}"));
    }

}
