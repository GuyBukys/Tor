using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.MessageBlasts.Commands.Activate;
using Tor.Application.MessageBlasts.Commands.BulkSendNotification;
using Tor.Application.MessageBlasts.Commands.Deactivate;
using Tor.Contracts.Business;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.ClientAggregate;
using Tor.Domain.MessageBlastAggregate;
using Tor.Domain.MessageBlastAggregate.Entities;
using Tor.Domain.UserAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.MessageBlasts;

public class MessageBlastsControllerTests : BaseIntegrationTest
{
    private readonly Mock<IPushNotificationSender> _pushNotificationSenderMock;

    public MessageBlastsControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
        _pushNotificationSenderMock = factory.PushNotificationSenderMock;
    }

    [Fact]
    public async Task GetMessageBlasts_WhenValidRequest_ShouldGetMessageBlastsForBusiness()
    {
        Business business = await TestUtils.SetupBusiness(Context);
        string requestUri = $"{MessageBlastsControllerConstants.GetMessageBlastsUri}?businessId={business.Id}";

        var result = await Client.GetFromJsonAsync<GetMessageBlastsResponse>(requestUri);

        result.Should().NotBeNull();
        var messageBlasts = await Context.MessageBlasts
            .Where(x => x.Businesses.Contains(business))
            .ToListAsync();
        result!.MessageBlasts.Should().HaveCount(messageBlasts.Count);
        foreach (var messageBlastResponse in result!.MessageBlasts)
        {
            messageBlasts.Should().Contain(x =>
                x.Id == messageBlastResponse.Id &&
                x.Name == messageBlastResponse.Name &&
                x.Description == messageBlastResponse.Description &&
                x.DisplayName == messageBlastResponse.DisplayName &&
                x.DisplayDescription == messageBlastResponse.DisplayDescription &&
                x.IsActive == messageBlastResponse.IsActive &&
                x.TemplateBody == messageBlastResponse.TemplateBody &&
                x.CanEditBody == messageBlastResponse.CanEditBody);
            var businessMessageBlast = await Context.BusinessMessageBlasts
                .Where(x => x.MessageBlastId == messageBlastResponse.Id)
                .Where(x => x.BusinessId == business.Id)
                .FirstAsync();
            businessMessageBlast.Body.Should().Be(messageBlastResponse.BusinessBody);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ActivateMessageBlast_WhenValidCommand_ShouldActivateMessageBlastForBusiness(bool isActive)
    {
        Business business = await TestUtils.SetupBusiness(Context);
        MessageBlast messageBlast = Fakers.MessageBlasts.MessageBlastFaker.Generate();
        await Context.AddAsync(messageBlast);
        await Context.SaveChangesAsync();
        BusinessMessageBlast businessMessageBlast = Fakers.MessageBlasts.BusinessMessageBlastFaker.Generate();
        businessMessageBlast.BusinessId = business.Id;
        businessMessageBlast.MessageBlastId = messageBlast.Id;
        businessMessageBlast.IsActive = isActive;
        await Context.AddAsync(businessMessageBlast);
        await Context.SaveChangesAsync();
        var command = new ActivateMessageBlastCommand(business.Id, messageBlast.Id, "body");

        var result = await Client.PostAsJsonAsync(MessageBlastsControllerConstants.ActivateMessageBlastUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(businessMessageBlast).ReloadAsync();
        businessMessageBlast.IsActive.Should().BeTrue();
        businessMessageBlast.Body.Should().Be(command.Body);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeactivateMessageBlast_WhenValidCommand_ShouldDeactivateMessageBlastForBusiness(bool isActive)
    {
        Business business = await TestUtils.SetupBusiness(Context);
        MessageBlast messageBlast = Fakers.MessageBlasts.MessageBlastFaker.Generate();
        await Context.AddAsync(messageBlast);
        await Context.SaveChangesAsync();
        BusinessMessageBlast businessMessageBlast = Fakers.MessageBlasts.BusinessMessageBlastFaker.Generate();
        businessMessageBlast.BusinessId = business.Id;
        businessMessageBlast.MessageBlastId = messageBlast.Id;
        businessMessageBlast.IsActive = isActive;
        await Context.AddAsync(businessMessageBlast);
        await Context.SaveChangesAsync();
        var command = new DeactivateMessageBlastCommand(business.Id, messageBlast.Id);

        var result = await Client.PostAsJsonAsync(MessageBlastsControllerConstants.DeactivateMessageBlastUri, command);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await Context.Entry(businessMessageBlast).ReloadAsync();
        businessMessageBlast.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task BulkSendNotification_WhenValidRequest_ShouldSendNotificationToAllClientsAndStaffMembers()
    {
        var business = await TestUtils.SetupBusiness(Context);
        var staffMember = await TestUtils.SetupStaffMember(Context);
        staffMember.BusinessId = business.Id;
        await Context.SaveChangesAsync();
        Client client = await TestUtils.SetupClient(Context);
        business.Clients.Add(client);
        await Context.SaveChangesAsync();
        BulkSendNotificationCommand command = new(business.Id, "title", "message");

        var result = await Client.PostAsJsonAsync(MessageBlastsControllerConstants.BulkSendNotificationUri, command);

        User clientUser = await Context.Clients
            .Where(x => x.Id == client.Id)
            .Select(x => x.User)
            .FirstAsync();
        User staffMemberUser = await Context.StaffMembers
            .Where(x => x.Id == staffMember.Id)
            .Select(x => x.User)
            .FirstAsync();
        List<string> deviceTokens = clientUser.Devices.Select(x => x.Token)
            .Union(staffMemberUser.Devices.Select(x => x.Token))
            .ToList();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        _pushNotificationSenderMock.Verify(x => x.Send(
            It.Is<SendPushNotificationRequest>(request =>
                request.Title == command.Title &&
                request.Message == command.Message &&
                request.DeviceTokens.SequenceEqual(deviceTokens)),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
