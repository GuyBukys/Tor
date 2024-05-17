using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Services.Commands.AddService;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Tor.UnitTests.Application.Services;

public class AddServiceCommandHandlerTests : UnitTestBase
{
    private readonly AddServiceCommandHandler _sut;

    public AddServiceCommandHandlerTests()
        : base()
    {
        _sut = new AddServiceCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldAddService()
    {
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.Services.AddServiceCommandFaker.Generate();
        command = command with { StaffMemberId = staffMember.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var staffMemberFromDb = await Context.StaffMembers
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.Id == staffMember.Id);
        staffMemberFromDb.Should().NotBeNull();
        staffMember.Services.Should().Contain(x => x.Id == result.Value);
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Services.AddServiceCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {command.StaffMemberId}"));
    }

    [Fact]
    public async Task Handle_WhenServiceAlreadyExistForStaffMember_ShouldReturnConflictError()
    {
        var command = Fakers.Services.AddServiceCommandFaker.Generate();
        StaffMember staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Services.First().Name = command.Name;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        command = command with { StaffMemberId = staffMember.Id };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(ConflictError) && x.Message.Contains($"service with name '{command.Name}' already exists for staff member"));
    }
}
