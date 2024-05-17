using Tor.Application.Categories.Queries.GetCategories;
using Tor.TestsInfra;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tor.UnitTests.Application.Categories;

public class GetCategoriesQueryHandlerTests : UnitTestBase
{
    private readonly GetCategoriesQueryHandler _sut;

    public GetCategoriesQueryHandlerTests()
        : base()
    {
        _sut = new GetCategoriesQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenCategoriesExist_ShouldReturnCategories()
    {
        var result = await _sut.Handle(new GetCategoriesQuery(), default);

        result.IsSuccess.Should().BeTrue();
        var categories = await Context.Categories.ToListAsync();
        result.Value.Should().BeEquivalentTo(categories);
    }

    [Fact]
    public async Task Handle_WhenNoCategoriesExist_ShouldReturnEmptyResult()
    {
        Context.Categories.RemoveRange(await Context.Categories.ToListAsync());
        await Context.SaveChangesAsync();

        var result = await _sut.Handle(new GetCategoriesQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
