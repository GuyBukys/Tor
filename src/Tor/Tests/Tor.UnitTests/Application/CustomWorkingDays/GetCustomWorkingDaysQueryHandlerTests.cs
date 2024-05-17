using FluentAssertions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.CustomWorkingDays.Queries.GetCustomWorkingDays;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.CustomWorkingDays;

public class GetCustomWorkingDaysQueryHandlerTests : UnitTestBase
{
    private readonly GetCustomWorkingDaysQueryHandler _sut;

    public GetCustomWorkingDaysQueryHandlerTests()
        : base()
    {
        _sut = new GetCustomWorkingDaysQueryHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenCustomWorkingDaysExistsWithNoUntilDate_ShouldReturnCustomWorkingDaysUntilOneMonth()
    {
        StaffMember staffMember = await SetupStaffMember();
        List<CustomWorkingDay> customWorkingDays = await SetupCustomWorkingDays(staffMember);
        GetCustomWorkingDaysQuery query = new(
            staffMember.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
            null);

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var customWorkingDaysUntilOneMonth = customWorkingDays.Where(x => x.AtDate <= DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)));
        result.Value.Should().BeEquivalentTo(customWorkingDaysUntilOneMonth);
    }

    [Fact]
    public async Task Handle_WhenCustomWorkingDaysExistsWithNoFromDate_ShouldReturnCustomWorkingDaysFromNow()
    {
        StaffMember staffMember = await SetupStaffMember();
        List<CustomWorkingDay> customWorkingDays = await SetupCustomWorkingDays(staffMember);
        GetCustomWorkingDaysQuery query = new(
            staffMember.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var CustomWorkingDaysFromNow = customWorkingDays.Where(x => x.AtDate >= DateOnly.FromDateTime(DateTime.UtcNow));
        result.Value.Should().BeEquivalentTo(CustomWorkingDaysFromNow);
    }

    [Fact]
    public async Task Handle_WhenCustomWorkingDaysExistsWithFromDate_ShouldReturnCustomWorkingDaysFromDateGiven()
    {
        StaffMember staffMember = await SetupStaffMember();
        List<CustomWorkingDay> customWorkingDays = await SetupCustomWorkingDays(staffMember);
        GetCustomWorkingDaysQuery query = new(
            staffMember.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var customWorkingDaysFromDateGiven = customWorkingDays.Where(x => x.AtDate >= query.From);
        result.Value.Should().BeEquivalentTo(customWorkingDaysFromDateGiven);
    }

    [Fact]
    public async Task Handle_WhenCustomWorkingDaysExistsWithUntilDate_ShouldReturnCustomWorkingDaysUntilTheDateGiven()
    {
        StaffMember staffMember = await SetupStaffMember();
        List<CustomWorkingDay> customWorkingDays = await SetupCustomWorkingDays(staffMember);
        GetCustomWorkingDaysQuery query = new(
            staffMember.Id,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().BeEquivalentTo(customWorkingDays.Where(x => x.AtDate <= query.Until));
    }

    [Fact]
    public async Task Handle_WhenNoCustomWorkingDaysExists_ShouldReturnEmptyList()
    {
        StaffMember staffMember = await SetupStaffMember();
        GetCustomWorkingDaysQuery query = new(
            staffMember.Id,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)));

        var result = await _sut.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnFailedResult()
    {
        GetCustomWorkingDaysQuery query = new(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)));

        var result = await _sut.Handle(query, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {query.StaffMemberId}"));
    }

    private async Task<StaffMember> SetupStaffMember()
    {
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();

        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();

        return staffMember;
    }

    private async Task<List<CustomWorkingDay>> SetupCustomWorkingDays(StaffMember staffMember)
    {
        int day = 1;
        var customWorkingDays = Fakers.CustomWorkingDays.CustomWorkingDayFaker.Generate(20);
        customWorkingDays.ForEach(x =>
        {
            x.AtDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(day));
            day++;
        });

        staffMember.CustomWorkingDays.AddRange(customWorkingDays);
        await Context.SaveChangesAsync();

        return customWorkingDays;
    }
}
