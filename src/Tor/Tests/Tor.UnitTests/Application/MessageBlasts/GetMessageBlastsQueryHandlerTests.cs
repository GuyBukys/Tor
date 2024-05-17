using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tor.Application.MessageBlasts.Queries.GetMessageBlasts;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.MessageBlastAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.MessageBlasts;
public class GetMessageBlastsQueryHandlerTests : UnitTestBase
{
    private readonly GetMessageBlastsQueryHandler _sut;

    public GetMessageBlastsQueryHandlerTests()
        : base()
    {
        _sut = new GetMessageBlastsQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldGetMessageBlasts()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var messageBlasts = await Context.MessageBlasts.Select(x => x.Id).ToListAsync();
        var businessMessageBlasts = messageBlasts.ConvertAll(x => new BusinessMessageBlast
        {
            MessageBlastId = x,
            BusinessId = business.Id,
            IsActive = true,
        });
        await Context.BusinessMessageBlasts.AddRangeAsync(businessMessageBlasts);
        await Context.SaveChangesAsync();
        var query = new GetMessageBlastsQuery(business.Id);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(businessMessageBlasts.Count);
        foreach (var messageBlastResult in result.Value)
        {
            businessMessageBlasts.Should().Contain(x =>
                x.BusinessId == business.Id &&
                x.IsActive == messageBlastResult.IsActive &&
                x.Body == messageBlastResult.BusinessBody);
        }
    }
}
