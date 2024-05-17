using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Services.Commands.AddService;
using Tor.Application.Services.Commands.UpdateService;
using Tor.Contracts.Service;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Infrastructure.Persistence;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.Services;

public class ServiceControllerTests : BaseIntegrationTest
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public ServiceControllerTests(IntegrationTestWebApplicationFactory factory)
    : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetServices_WhenValidRequest_ShouldReturnServices()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        string requestUri = $"{ServiceControllerConstants.GetServicesUri}?staffMemberId={staffMember.Id}";

        var result = await Client.GetFromJsonAsync<GetServicesResponse>(requestUri);

        result.Should().NotBeNull();
        result!.Services.Should().NotBeEmpty();
        result!.Services.Count.Should().Be(staffMember.Services.Count);
    }

    [Fact]
    public async Task GetServices_WhenNoInvalidRequest_ShouldReturnBadRequest()
    {
        string requestUri = $"{ServiceControllerConstants.GetServicesUri}?staffMemberId={Guid.Empty}";

        var result = await Client.GetAsync(requestUri);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetServices_WhenNoServiceForStaffMember_ShouldReturnEmptyList()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        staffMember.Services.Clear();
        await Context.SaveChangesAsync();
        string requestUri = $"{ServiceControllerConstants.GetServicesUri}?staffMemberId={staffMember.Id}";

        var result = await Client.GetFromJsonAsync<GetServicesResponse>(requestUri);

        result.Should().NotBeNull();
        result!.Services.Should().BeEmpty();
    }

    [Fact]
    public async Task AddService_WhenValidRequest_ShouldAddService()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        AddServiceCommand request = Fakers.Services.AddServiceCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
        };

        var result = await Client.PostAsJsonAsync(ServiceControllerConstants.AddServiceUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await result.Content.ReadFromJsonAsync<AddServiceResponse>();
        response.Should().NotBeNull();
        response!.ServiceId.Should().NotBeEmpty();
        var staffMemberFromDb = await Context.StaffMembers
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.Id == staffMember.Id);
        staffMemberFromDb.Should().NotBeNull();
        staffMemberFromDb!.Services.Should().Contain(x => x.Id == response.ServiceId);
    }

    [Fact]
    public async Task AddService_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        AddServiceCommand request = Fakers.Services.AddServiceCommandFaker.Generate() with
        {
            StaffMemberId = Guid.Empty,
        };

        var result = await Client.PostAsJsonAsync(ServiceControllerConstants.AddServiceUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateService_WhenValidRequest_ShouldAddService()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Service service = staffMember.Services.First();
        UpdateServiceCommand request = Fakers.Services.UpdateServiceCommandFaker.Generate() with
        {
            ServiceId = service.Id,
        };

        var result = await Client.PutAsJsonAsync(ServiceControllerConstants.UpdateServiceUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await result.Content.ReadFromJsonAsync<ServiceResponse>();
        response.Should().NotBeNull();
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TorDbContext>();
        var serviceFromDb = await context.Services.FirstOrDefaultAsync(x => x.Id == service.Id);
        serviceFromDb.Should().NotBeNull();
        AssertValues(serviceFromDb!, response!);
    }

    [Fact]
    public async Task UpdateService_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        UpdateServiceCommand request = Fakers.Services.UpdateServiceCommandFaker.Generate() with
        {
            ServiceId = Guid.Empty,
        };

        var result = await Client.PutAsJsonAsync(ServiceControllerConstants.UpdateServiceUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteService_WhenValidRequest_ShouldDeleteService()
    {
        StaffMember staffMember = await TestUtils.SetupStaffMember(Context);
        Service service = staffMember.Services.First();
        string requestUri = $"{ServiceControllerConstants.DeleteServiceUri}?serviceId={service.Id}";

        var result = await Client.DeleteAsync(requestUri);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        Service? serviceFromDb = await Context.Services.FirstOrDefaultAsync(x => x.Id == service.Id);
        serviceFromDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteService_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        string requestUri = $"{ServiceControllerConstants.DeleteServiceUri}?serviceId={Guid.Empty}";

        var result = await Client.DeleteAsync(requestUri);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteService_WhenServiceDoesntExist_ShouldDoNothing()
    {
        string requestUri = $"{ServiceControllerConstants.DeleteServiceUri}?serviceId={Guid.NewGuid()}";

        var result = await Client.DeleteAsync(requestUri);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static void AssertValues(Service serviceFromDb, ServiceResponse response)
    {
        serviceFromDb.Id.Should().Be(response.Id);
        serviceFromDb.Name.Should().BeEquivalentTo(response.Name);
        serviceFromDb.Description.Should().BeEquivalentTo(response.Description);
        serviceFromDb.Amount.Should().BeEquivalentTo(response.Amount);
        serviceFromDb.Durations.Should().BeEquivalentTo(response.Durations);
    }
}
