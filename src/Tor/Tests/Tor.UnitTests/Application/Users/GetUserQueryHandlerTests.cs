using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Users.Queries.Get;
using Tor.Domain.UserAggregate.Enum;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Users;

public class GetUserQueryHandlerTests : UnitTestBase
{
    private readonly GetUserQueryHandler _sut;

    public GetUserQueryHandlerTests()
        : base()
    {
        _sut = new GetUserQueryHandler(Context);
    }

    [Theory]
    [InlineData(AppType.BusinessApp)]
    [InlineData(AppType.ClientApp)]
    public async Task Handle_WhenUserDoesntExists_ShouldReturnFailedResult(AppType appType)
    {
        string userTokenThatDoesntExists = "user token";
        var query = new GetUserQuery(userTokenThatDoesntExists, appType);

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
        x.GetType() == typeof(NotFoundError));
    }

    [Theory]
    [InlineData(AppType.BusinessApp)]
    [InlineData(AppType.ClientApp)]
    public async Task Handle_WhenUserExists_ShouldReturnUser(AppType appType)
    {
        var user = Fakers.Users.UserFaker.Generate();
        user.AppType = appType;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var query = new GetUserQuery(user.UserToken, appType);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task Handle_WhenUserExistsButNotForTheSameAppType_ShouldReturnNotFound()
    {
        var user = Fakers.Users.UserFaker.Generate();
        user.AppType = AppType.BusinessApp;
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        var query = new GetUserQuery(user.UserToken, AppType.ClientApp);

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
        x.GetType() == typeof(NotFoundError));
    }
}
