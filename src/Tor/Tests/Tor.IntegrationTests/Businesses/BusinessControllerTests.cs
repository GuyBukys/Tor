using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Commands.Block;
using Tor.Application.Businesses.Commands.Create;
using Tor.Application.Businesses.Commands.Unblock;
using Tor.Application.Businesses.Commands.UpdateSettings;
using Tor.Contracts.Business;
using Tor.Contracts.Service;
using Tor.Contracts.StaffMember;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.ClientAggregate;
using Tor.Domain.ClientAggregate.Entities;
using Tor.Domain.TierAggregate;
using Tor.Domain.TierAggregate.Enums;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.Businesses;

public class BusinessControllerTests : BaseIntegrationTest
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly Mock<IStorageManager> _storageManagerMock;

    public BusinessControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
        _factory = factory;
        _storageManagerMock = factory.StorageManagerMock;
    }

    [Fact]
    public async Task GetAll_WhenValidQuery_ShouldReturnBusinesses()
    {
        Business business = await TestUtils.SetupBusiness(Context);
        Client client = await TestUtils.SetupClient(Context);
        string requestUri = $"{BusinessControllerConstants.GetAllBusinessesUri}?clientId={client.Id}";

        var result = await Client.GetFromJsonAsync<GetAllBusinessesResponse>(requestUri);

        result.Should().NotBeNull();
        result!.Businesses.Items.Should().HaveCount(1);
        business.Should().BeEquivalentTo(result.Businesses.Items.First(), cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetAppointmentsByClient_WhenValidQuery_ShouldReturnAppointmentsByClient()
    {
        Client client = await TestUtils.SetupClient(Context);
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        var appointments = await TestUtils.SetupAppointments(staffMember, Context, client.Id);
        string requestUri = $"{BusinessControllerConstants.GetAppointmentsByClientUri}?businessId={staffMember.BusinessId}&clientId={client.Id}";

        var result = await Client.GetFromJsonAsync<GetAppointmentsByClientResponse>(requestUri);

        result.Should().NotBeNull();
        result!.Appointments.Should().NotBeEmpty();
        appointments.Should().BeEquivalentTo(result.Appointments, cfg => cfg
            .ExcludingMissingMembers()
            .Excluding(x => x.ScheduledFor));
    }

    [Fact]
    public async Task Create_WhenValidRequest_ShouldCreateBusiness()
    {
        Business referringBusiness = await TestUtils.SetupBusiness(Context);
        Guid userId = await TestUtils.SetupUser(Context);
        var categories = await Context.Categories.ToListAsync();
        _storageManagerMock.Setup(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        CreateBusinessCommand request = Fakers.Businesses.CreateBusinessCommandFaker.Generate();
        var validRequest = request with
        {
            ReferringBusinessId = referringBusiness.Id,
            CategoryIds = categories.ConvertAll(x => x.Id),
            BusinessOwner = request.BusinessOwner with
            {
                UserId = userId,
            }
        };

        var result = await Client.PostAsJsonAsync(BusinessControllerConstants.CreateUri, validRequest);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var businessFromDb = await Context.Businesses
            .Where(x => x.Id != referringBusiness.Id)
            .Include(x => x.StaffMembers)
                .ThenInclude(x => x.Services)
            .FirstOrDefaultAsync();
        businessFromDb.Should().NotBeNull();
        businessFromDb.Should().BeEquivalentTo(validRequest, cfg => cfg.ExcludingMissingMembers());
        businessFromDb!.StaffMembers.First().Should().BeEquivalentTo(validRequest.BusinessOwner, cfg => cfg.ExcludingMissingMembers());
        businessFromDb!.Logo.Should().Be(request.Logo);
        var messageBlastIds = await Context.MessageBlasts.Select(x => x.Id).ToListAsync();
        var businessMessageBlastIds = await Context.BusinessMessageBlasts
            .Where(x => x.BusinessId == businessFromDb.Id)
            .Select(x => x.MessageBlastId)
            .ToListAsync();
        messageBlastIds.Should().NotBeEmpty();
        businessMessageBlastIds.Should().BeEquivalentTo(messageBlastIds);
        var userFromDb = await Context.Users.FirstAsync(x => x.Id == userId);
        await Context.Entry(userFromDb).ReloadAsync();
        userFromDb.EntityId.Should().Be(businessFromDb!.StaffMembers.First().Id);
        var response = await result.Content.ReadFromJsonAsync<CreateBusinessResponse>();
        response.Should().NotBeNull();
        businessFromDb!.Should().BeEquivalentTo(response!, cfg => cfg.ExcludingMissingMembers());
        businessFromDb!.ReferringBusinessId.Should().Be(referringBusiness.Id);
        Tier premiumTier = await Context.Tiers.FirstAsync(x => x.Type == TierType.Premium);
        businessFromDb!.Tier.Should().BeEquivalentTo(premiumTier);
    }

    [Fact]
    public async Task Create_WhenInvalidRequest_ShouldReturnValidationErrors()
    {
        CreateBusinessCommand request = Fakers.Businesses.CreateBusinessCommandFaker.Generate() with
        {
            BusinessOwner = null!,
        };

        var result = await Client.PostAsJsonAsync(BusinessControllerConstants.CreateUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var businessFromDb = await Context.Businesses.FirstOrDefaultAsync();
        businessFromDb.Should().BeNull();
    }

    [Fact]
    public async Task Deactivate_WhenValidRequest_ShouldDeactivateBusiness()
    {
        var business = await TestUtils.SetupBusiness(Context);
        var userId = await TestUtils.SetupUser(Context);
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.BusinessId = business.Id;
        staffMember.UserId = userId;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var waitingList = Fakers.WaitingLists.WaitingListFaker.Generate();
        waitingList.StaffMemberId = staffMember.Id;
        await Context.WaitingLists.AddAsync(waitingList);
        await Context.SaveChangesAsync();
        string requestUri = $"{BusinessControllerConstants.DeactivateUri}?businessId={business.Id}";

        var result = await Client.PostAsync(requestUri, null);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(business).ReloadAsync();
        business.IsActive.Should().BeFalse();
        var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == staffMember.UserId);
        user.Should().BeNull();
        var staffMembersFromDb = await Context.StaffMembers.ToListAsync();
        staffMembersFromDb.Should().BeEmpty();
        var servicesFromDb = await Context.Services.ToListAsync();
        servicesFromDb.Should().BeEmpty();
        var waitingListFromDb = await Context.WaitingLists.FirstOrDefaultAsync(x => x.Id == waitingList.Id);
        waitingListFromDb.Should().BeNull();
    }

    [Fact]
    public async Task GetByStaffMember_WhenValidRequest_ShouldGetBusiness()
    {
        var business = await TestUtils.SetupBusiness(Context);
        Guid userId = await TestUtils.SetupUser(Context);
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Position = PositionType.BusinessOwner;
        staffMember.UserId = userId;
        staffMember.BusinessId = business.Id;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        string requestUri = $"{BusinessControllerConstants.GetByStaffMemberUri}?staffMemberId={staffMember.Id}";

        var result = await Client.GetFromJsonAsync<GetBusinessResponse>(requestUri);

        result.Should().NotBeNull();
        await Context.Entry(business).ReloadAsync();
        AssertValues(business, result!);
    }

    [Fact]
    public async Task GetByReferralCode_WhenValidRequest_ShouldGetByReferralCode()
    {
        var business = await TestUtils.SetupBusiness(Context);
        string requestUri = $"{BusinessControllerConstants.GetByReferralCodeUri}?referralCode={business.ReferralCode}";

        var result = await Client.GetFromJsonAsync<GetByReferralCodeResponse>(requestUri);

        result.Should().NotBeNull();
        result!.BusinessId.Should().Be(business.Id);
        result!.BusinessName.Should().Be(business.Name);

    }

    [Fact]
    public async Task GetByInvitation_WhenValidRequest_ShouldGetBusiness()
    {
        var business = await TestUtils.SetupBusiness(Context);
        string requestUri = $"{BusinessControllerConstants.GetByInvitationUri}?invitationId={business.InvitationId}";

        var result = await Client.GetFromJsonAsync<GetBusinessResponse>(requestUri);

        result.Should().NotBeNull();
        await Context.Entry(business).ReloadAsync();
        AssertValues(business, result!);
    }

    [Fact]
    public async Task GetById_WhenValidRequest_ShouldGetBusiness()
    {
        var business = await TestUtils.SetupBusiness(Context);
        string requestUri = $"{BusinessControllerConstants.GetByIdUri}?id={business.Id}";

        var result = await Client.GetFromJsonAsync<GetBusinessResponse>(requestUri);

        result.Should().NotBeNull();
        await Context.Entry(business).ReloadAsync();
        AssertValues(business, result!);
    }

    [Fact]
    public async Task IsBusinessExists_WhenValidRequest_ShouldReturnTrue()
    {
        var business = await TestUtils.SetupBusiness(Context);
        string requestUri = $"{BusinessControllerConstants.IsBusinessExistsUri}?email={business.Email}";

        var result = await Client.GetAsync(requestUri);

        result.Should().NotBeNull();
        bool value = JsonConvert.DeserializeObject<bool>(await result.Content.ReadAsStringAsync());
        value.Should().BeTrue();
    }

    [Fact]
    public async Task CanAddStaffMember_WhenValidRequest_ShouldReturnTrue()
    {
        var staffMember = await TestUtils.SetupStaffMember(Context);
        var business = await Context.Businesses.FirstAsync(x => x.Id == staffMember.BusinessId);
        business.Tier.MaximumStaffMembers = 3;
        await Context.SaveChangesAsync();
        string requestUri = $"{BusinessControllerConstants.CanAddStaffMemberUri}?businessId={business.Id}";

        var result = await Client.GetAsync(requestUri);

        result.Should().NotBeNull();
        bool value = JsonConvert.DeserializeObject<bool>(await result.Content.ReadAsStringAsync());
        value.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllClients_WhenValidRequest_ShouldGetAllClients()
    {
        var business = await TestUtils.SetupBusiness(Context);
        List<Client> clients = Enumerable.Range(1, 5).Select(_ => Fakers.Clients.ClientFaker.Generate()).ToList();
        List<Client> blockedClients = Enumerable.Range(1, 5).Select(_ => Fakers.Clients.ClientFaker.Generate()).ToList();
        List<Client> favoriteClients = Enumerable.Range(1, 5).Select(_ => Fakers.Clients.ClientFaker.Generate()).ToList();
        await Context.Clients.AddRangeAsync([.. clients, .. blockedClients, .. favoriteClients]);
        await Context.SaveChangesAsync();
        await Context.FavoriteBusinesses.AddRangeAsync(favoriteClients.ConvertAll(x => new FavoriteBusiness(Guid.NewGuid()) { BusinessId = business.Id, ClientId = x.Id }));
        business.Clients.AddRange(clients);
        business.BlockedClients.AddRange(blockedClients);
        await Context.SaveChangesAsync();
        string requestUri = $"{BusinessControllerConstants.GetAllClientsUri}?id={business.Id}";

        var result = await Client.GetFromJsonAsync<GetAllClientsResponse>(requestUri);

        result.Should().NotBeNull();
        await Context.Entry(business).ReloadAsync();
        clients.Should().BeEquivalentTo(result!.ClientsWhoBookedAnAppointment, cfg => cfg.ExcludingMissingMembers());
        blockedClients.Should().BeEquivalentTo(result!.BlockedClients, cfg => cfg.ExcludingMissingMembers());
        favoriteClients.Should().BeEquivalentTo(result!.ClientsWhoMarkedAsFavorite, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetAllClients_WhenNoClients_ShouldReturnEmptyLists()
    {
        var business = await TestUtils.SetupBusiness(Context);
        string requestUri = $"{BusinessControllerConstants.GetAllClientsUri}?id={business.Id}";

        var result = await Client.GetFromJsonAsync<GetAllClientsResponse>(requestUri);

        result.Should().NotBeNull();
        result!.ClientsWhoBookedAnAppointment.Should().BeEmpty();
        result!.ClientsWhoMarkedAsFavorite.Should().BeEmpty();
        result!.BlockedClients.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateHomepageNote_WhenValidRequest_ShouldUpdateHomepageNote()
    {
        var business = await TestUtils.SetupBusiness(Context);
        var command = Fakers.Businesses.UpdateHomepageNoteCommandFaker.Generate() with { BusinessId = business.Id };

        var result = await Client.PutAsJsonAsync(BusinessControllerConstants.UpdateHomepageNoteUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(business).ReloadAsync();
        business.HomepageNote.Should().Be(command.HomepageNote);
    }

    [Fact]
    public async Task UpdateImages_WhenValidRequest_ShouldUpdateImages()
    {
        var business = await TestUtils.SetupBusiness(Context);
        var command = Fakers.Businesses.UpdateBusinessImagesCommandFaker.Generate() with { BusinessId = business.Id };

        var result = await Client.PutAsJsonAsync(BusinessControllerConstants.UpdateImagesUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITorDbContext>();
        var businessFromDb = await context.Businesses.FirstOrDefaultAsync(x => x.Id == business.Id);
        businessFromDb.Should().NotBeNull();
        businessFromDb!.Logo.Should().Be(command.Logo);
        businessFromDb!.Cover.Should().Be(command.Cover);
        businessFromDb!.Portfolio.Should().BeEquivalentTo(command.Portfolio);
    }

    [Fact]
    public async Task UpdateSettings_WhenValidRequest_ShouldUpdateSettings()
    {
        var business = await TestUtils.SetupBusiness(Context);
        var command = new UpdateBusinessSettingsCommand(business.Id, new BusinessSettings(29, 30, 31, 32, 33, 24));

        var result = await Client.PutAsJsonAsync(BusinessControllerConstants.UpdateBusinessSettingsUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITorDbContext>();
        var businessFromDb = await context.Businesses.FirstOrDefaultAsync(x => x.Id == business.Id);
        businessFromDb!.Settings.Should().Be(command.BusinessSettings);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Block_WhenValidRequest_ShouldAddIfNotExistsBlockedClient(bool isExists)
    {
        var business = await TestUtils.SetupBusiness(Context);
        var client = await TestUtils.SetupClient(Context);
        if (isExists)
        {
            await Context.BlockedClients.AddAsync(new BlockedClient
            {
                BusinessId = business.Id,
                ClientId = client.Id,
            });
            await Context.SaveChangesAsync();
        }
        var command = new BlockClientCommand(business.Id, client.Id);

        var result = await Client.PostAsJsonAsync(BusinessControllerConstants.BlockUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITorDbContext>();
        var blockedClient = await context.BlockedClients.SingleOrDefaultAsync(x =>
            x.BusinessId == business.Id && x.ClientId == client.Id);
        blockedClient.Should().NotBeNull();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Unblock_WhenValidRequest_ShouldDeleteBlockedClientEntity(bool isExists)
    {
        var business = await TestUtils.SetupBusiness(Context);
        var client = await TestUtils.SetupClient(Context);
        if (isExists)
        {
            await Context.BlockedClients.AddAsync(new BlockedClient
            {
                BusinessId = business.Id,
                ClientId = client.Id,
            });
            await Context.SaveChangesAsync();
        }
        var command = new UnblockClientCommand(business.Id, client.Id);

        var result = await Client.PostAsJsonAsync(BusinessControllerConstants.UnblockUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITorDbContext>();
        var blockedClient = await context.BlockedClients.SingleOrDefaultAsync(x =>
            x.BusinessId == business.Id && x.ClientId == client.Id);
        blockedClient.Should().BeNull();
    }

    private static void AssertValues(Business business, GetBusinessResponse result)
    {
        business.Id.Should().Be(result.Id);
        business.Name.Should().Be(result.Name);
        business.Description.Should().Be(result.Description);
        business.Email.Should().Be(result.Email);
        business.Address.Should().Be(result.Address);
        business.PhoneNumbers.Should().BeEquivalentTo(result.PhoneNumbers);
        business.Logo.Should().BeEquivalentTo(result.Logo);
        business.Cover.Should().BeEquivalentTo(result.Cover);
        business.HomepageNote.Should().Be(result.HomepageNote);
        business.SocialMedias.Should().BeEquivalentTo(result.SocialMedias);
        business.ReferralCode.Should().BeEquivalentTo(result.ReferralCode);
        business.Tier.Should().BeEquivalentTo(result.Tier, cfg => cfg.ExcludingMissingMembers());

        foreach (var staffMember in business.StaffMembers)
        {
            GetStaffMemberResponse? resultStaffMember = result.StaffMembers.FirstOrDefault(x => x.Id == staffMember.Id);
            resultStaffMember.Should().NotBeNull();
            staffMember.Name.Should().Be(resultStaffMember!.Name);
            staffMember.Email.Should().Be(resultStaffMember.Email);
            staffMember.Address.Should().Be(resultStaffMember.Address);
            staffMember.PhoneNumber.Should().Be(resultStaffMember.PhoneNumber);
            staffMember.Position.Should().Be(resultStaffMember.Position);
            staffMember.ProfileImage.Should().BeEquivalentTo(resultStaffMember.ProfileImage);
            staffMember.BirthDate.Should().Be(resultStaffMember.BirthDate);
            staffMember.WeeklySchedule.Should().Be(resultStaffMember.WeeklySchedule);

            foreach (var service in staffMember.Services)
            {
                ServiceResponse? resultService = resultStaffMember.Services.FirstOrDefault(x => x.Id == service.Id);
                resultService.Should().NotBeNull();
                service.Name.Should().Be(resultService!.Name);
                service.Description.Should().Be(resultService!.Description);
                service.Amount.Should().Be(resultService!.Amount);
                service.Durations.Should().BeEquivalentTo(resultService!.Durations);
            }
        }
    }
}
