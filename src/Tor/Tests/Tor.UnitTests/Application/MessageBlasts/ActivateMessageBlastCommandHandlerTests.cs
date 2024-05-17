using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.MessageBlasts.Commands.Activate;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.MessageBlastAggregate;
using Tor.Domain.MessageBlastAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.MessageBlasts;
public class ActivateMessageBlastCommandHandlerTests : UnitTestBase
{
    private readonly ActivateMessageBlastCommandHandler _sut;

    public ActivateMessageBlastCommandHandlerTests()
        : base()
    {
        _sut = new ActivateMessageBlastCommandHandler(Context);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenValidCommand_ShouldActivateMessageBlastForBusiness(bool isActive)
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        MessageBlast messageBlast = Fakers.MessageBlasts.MessageBlastFaker.Generate();
        BusinessMessageBlast businessMessageBlast = Fakers.MessageBlasts.BusinessMessageBlastFaker.Generate();
        businessMessageBlast.BusinessId = business.Id;
        businessMessageBlast.MessageBlastId = messageBlast.Id;
        businessMessageBlast.IsActive = isActive;
        await Context.AddAsync(business);
        await Context.AddAsync(messageBlast);
        await Context.AddAsync(businessMessageBlast);
        await Context.SaveChangesAsync();
        var command = new ActivateMessageBlastCommand(business.Id, messageBlast.Id, "body");

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(businessMessageBlast).ReloadAsync();
        businessMessageBlast.IsActive.Should().BeTrue();
        businessMessageBlast.Body.Should().Be(command.Body);
    }

    [Fact]
    public async Task Handle_WhenBusinessMessageBlastDoesntExist_ShouldReturnNotFoundError()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        MessageBlast messageBlast = Fakers.MessageBlasts.MessageBlastFaker.Generate();
        await Context.AddAsync(business);
        await Context.AddAsync(messageBlast);
        await Context.SaveChangesAsync();
        var command = new ActivateMessageBlastCommand(business.Id, messageBlast.Id, "body");

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains(
                $"could not find message blast for busines {command.BusinessId} and message blast id {command.MessageBlastId}"));
    }
}
