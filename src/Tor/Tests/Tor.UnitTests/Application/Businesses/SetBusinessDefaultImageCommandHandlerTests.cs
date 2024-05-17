using FluentAssertions;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Commands.SetDefaultImage;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.Common.Enums;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class SetBusinessDefaultImageCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IStorageManager> _mockStorageManager;
    private readonly SetBusinessDefaultImageCommandHandler _sut;

    public SetBusinessDefaultImageCommandHandlerTests()
        : base()
    {
        _mockStorageManager = new Mock<IStorageManager>();
        _sut = new SetBusinessDefaultImageCommandHandler(Context, _mockStorageManager.Object);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task Handle_WhenValidCommand_ShouldSetDefaultImage(bool setLogo, bool setCover)
    {
        var defaultImage = Fakers.Images.ImageFaker.Generate();
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        _mockStorageManager.Setup(x => x.GetDefaultImage(
            It.IsAny<ImageType>(),
            It.IsAny<EntityType>(),
            setCover ? business.Categories.First().Type : null))
            .Returns(defaultImage);
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.SetBusinessDefaultImageCommandFaker.Generate() with
        {
            BusinessId = business.Id,
            SetLogo = setLogo,
            SetCover = setCover,
        };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(business).ReloadAsync();
        business.Logo.Should().Be(setLogo ? defaultImage : business.Logo);
        business.Cover.Should().Be(setCover ? defaultImage : business.Cover);
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Businesses.SetBusinessDefaultImageCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find Business with id {command.BusinessId}"));
    }

}
