using FluentAssertions;
using Tor.Application.Businesses.Commands.UpdatePersonalDetails;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Businesses;

public class UpdateBusinessPersonalDetailsCommandHandlerTests : UnitTestBase
{
    private readonly UpdateBusinessPersonalDetailsCommandHandler _sut;

    public UpdateBusinessPersonalDetailsCommandHandlerTests()
        : base()
    {
        _sut = new UpdateBusinessPersonalDetailsCommandHandler(Context);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdatePersonalDetails()
    {
        Business business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate() with { BusinessId = business.Id };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(business).ReloadAsync();
        business.Name.Should().Be(command.Name);
        business.Description.Should().Be(command.Description);
        business.Email.Should().Be(command.Email);
        business.PhoneNumbers.Should().BeEquivalentTo(command.PhoneNumbers);
        business.SocialMedias.Should().BeEquivalentTo(command.SocialMedias);
    }

    [Fact]
    public async Task Handle_WhenBusinessDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find Business with id {command.BusinessId}"));
    }

    [Fact]
    public async Task Handle_WhenEmailExistsInAnotherActiveBusiness_ShouldReturnConflictError()
    {
        Business business1 = Fakers.Businesses.BusinessFaker.Generate();
        Business business2 = Fakers.Businesses.BusinessFaker.Generate();
        await Context.Businesses.AddAsync(business1);
        await Context.Businesses.AddAsync(business2);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate() with
        {
            BusinessId = business1.Id,
            Email = business2.Email,
        };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(ConflictError) && x.Message.Contains($"business with email {command.Email} already exists in another business"));
    }

    [Fact]
    public async Task Handle_WhenEmailExistsInAnotherNonActiveBusiness_ShouldUpdateSuccessfully()
    {
        Business business1 = Fakers.Businesses.BusinessFaker.Generate();
        Business business2 = Fakers.Businesses.BusinessFaker.Generate();
        business2.IsActive = false;
        await Context.Businesses.AddAsync(business1);
        await Context.Businesses.AddAsync(business2);
        await Context.SaveChangesAsync();
        var command = Fakers.Businesses.UpdateBusinessPersonalDetailsCommandFaker.Generate() with
        {
            BusinessId = business1.Id,
            Email = business2.Email,
        };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await Context.Entry(business1).ReloadAsync();
        business1.Name.Should().Be(command.Name);
        business1.Description.Should().Be(command.Description);
        business1.Email.Should().Be(command.Email);
        business1.PhoneNumbers.Should().BeEquivalentTo(command.PhoneNumbers);
        business1.SocialMedias.Should().BeEquivalentTo(command.SocialMedias);
    }
}
