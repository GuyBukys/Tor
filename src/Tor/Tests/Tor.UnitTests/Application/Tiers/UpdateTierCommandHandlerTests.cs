using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Tiers.Commands.UpdateTier;
using Tor.Domain.TierAggregate.Enums;
using Tor.TestsInfra;

namespace Tor.UnitTests.Application.Tiers;

public class UpdateTierCommandHandlerTests : UnitTestBase
{
    private readonly UpdateTierCommandHandler _sut;

    public UpdateTierCommandHandlerTests()
        : base()
    {
        _sut = new UpdateTierCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnFailedResult()
    {
        var query = new UpdateTierCommand(Guid.NewGuid(), TierType.Premium);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find business with id {query.BusinessId}"));
    }
}
