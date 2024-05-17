using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Services.Commands.UpdateService;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Services;

public class UpdateServiceCommandHandlerTests : UnitTestBase
{
    private readonly UpdateServiceCommandHandler _sut;

    public UpdateServiceCommandHandlerTests()
        : base()
    {
        _sut = new UpdateServiceCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldAddService()
    {
        Service service = Fakers.Services.ServiceFaker.Generate();
        await Context.Services.AddAsync(service);
        await Context.SaveChangesAsync();
        var command = Fakers.Services.UpdateServiceCommandFaker.Generate();
        command = command with { ServiceId = service.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        var serviceFromDb = await Context.Services.FirstOrDefaultAsync(x => x.Id == service.Id);
        serviceFromDb!.Name.Should().BeEquivalentTo(command.Name);
        serviceFromDb!.Description.Should().BeEquivalentTo(command.Description);
        serviceFromDb!.Amount.Should().BeEquivalentTo(command.Amount);
        serviceFromDb!.Durations.Should().BeEquivalentTo(command.Durations);
    }

    [Fact]
    public async Task Handle_WhenServiceDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Services.UpdateServiceCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find service with id {command.ServiceId}"));
    }
}
