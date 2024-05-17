using FluentAssertions;
using Tor.Application.Abstractions.Models;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Infrastructure.Persistence.Repositories;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Infrastructure;

public class BusinessRepositoryTests : UnitTestBase
{
    private readonly BusinessRepository _sut;

    public BusinessRepositoryTests()
        : base()
    {
        _sut = new BusinessRepository(Context);
    }

    [Fact]
    public async Task GetAll_WhenValidInput_ShouldGetAllBusinesses()
    {
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(5);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(Guid.NewGuid(), 1, 5, null, null, null);

        var result = await _sut.GetAll(input, default);

        result.Items.Count.Should().Be(businesses.Count);
        businesses.Should().BeEquivalentTo(result.Items, cfg => cfg.ExcludingMissingMembers());
        result.TotalCount.Should().Be(businesses.Count);
        result.Page.Should().Be(input.Page);
        result.PageSize.Should().Be(input.PageSize);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAll_WhenAllBusinessInactive_ShouldReturnEmptyList()
    {
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(5);
        businesses.ForEach(x => x.IsActive = false);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(Guid.NewGuid(), 1, 5, null, null, null);

        var result = await _sut.GetAll(input, default);

        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_WhenSomeBusinessesBlockedClient_ShouldGetAllBusinessesExceptBlocked()
    {
        Guid clientId = Guid.NewGuid();
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(2);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        await Context.BlockedClients.AddAsync(new BlockedClient
        {
            BusinessId = businesses.First().Id,
            ClientId = clientId,
        });
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(clientId, 1, 2, null, null, null);

        var result = await _sut.GetAll(input, default);

        result.Items.Count.Should().Be(1);
        businesses.Last().Should().BeEquivalentTo(result.Items.First(), cfg => cfg.ExcludingMissingMembers());
    }

    [Theory]
    [InlineData("name")]
    public async Task GetAll_WhenSortByColumnAndNoSortOrder_ShouldSortByColumnAndDefaultAscending(string sortColumn)
    {
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(10);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(Guid.NewGuid(), 1, 10, null, null, sortColumn);

        var result = await _sut.GetAll(input, default);

        result.Items.Count.Should().Be(businesses.Count);
        result.Items.Should().BeInAscendingOrder(x => x.Name);
    }

    [Fact]
    public async Task GetAll_WhenNoSortByColumnAndSortOrderDescending_ShouldSortByIdDescending()
    {
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(10);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(Guid.NewGuid(), 1, 10, null, "desc", null);

        var result = await _sut.GetAll(input, default);

        result.Items.Count.Should().Be(businesses.Count);
        result.Items.Should().BeInDescendingOrder(x => x.Id);
    }

    [Fact]
    public async Task GetAll_WhenInputPageIsOneAndThereIsMoreThanOnePage_ShouldReturnFirstPage()
    {
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(10);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(Guid.NewGuid(), 1, 5, null, null, null);

        var result = await _sut.GetAll(input, default);

        result.Items.Count.Should().Be(input.PageSize);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAll_WhenInputPageIsAfterOneAndThereIsMoreThanOnePage_ShouldReturnTheRequestedPage()
    {
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(12);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(Guid.NewGuid(), 2, 4, null, null, null);

        var result = await _sut.GetAll(input, default);

        result.Items.Count.Should().Be(input.PageSize);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_WhenLastPage_ShouldReturnLastPage()
    {
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(10);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(Guid.NewGuid(), 2, 5, null, null, null);

        var result = await _sut.GetAll(input, default);

        result.Items.Count.Should().Be(input.PageSize);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_WhenLastPageHasLessThanPageSize_ShouldReturnRemainingItems()
    {
        List<Business> businesses = Fakers.Businesses.BusinessFaker.Generate(10);
        await Context.Businesses.AddRangeAsync(businesses);
        await Context.SaveChangesAsync();
        GetAllBusinessesInput input = new(Guid.NewGuid(), 2, 6, null, null, null);

        var result = await _sut.GetAll(input, default);

        result.Items.Count.Should().Be(4);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeTrue();
    }
}
