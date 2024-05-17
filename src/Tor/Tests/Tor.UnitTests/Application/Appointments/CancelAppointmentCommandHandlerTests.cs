using FluentAssertions;
using MediatR;
using Moq;
using Tor.Application;
using Tor.Application.Abstractions;
using Tor.Application.Appointments.Commands.CancelAppointment;
using Tor.Application.Appointments.Notifications.AppointmentCanceled;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.AppointmentAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Appointments;

public class CancelAppointmentCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IPublisher> _publisherMock;
    private readonly Mock<IAppointmentRepository> _repositoryMock;
    private readonly CancelAppointmentCommandHandler _sut;

    public CancelAppointmentCommandHandlerTests()
        : base()
    {
        _publisherMock = new Mock<IPublisher>();
        _repositoryMock = new Mock<IAppointmentRepository>();
        _sut = new CancelAppointmentCommandHandler(Context, _repositoryMock.Object, _publisherMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidCommandAndCanCancel_ShouldCancelAppointment()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        business.Settings = business.Settings with { CancelAppointmentMinimumTimeInMinutes = 1 };
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.BusinessId = business.Id;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        appointment.ScheduledFor = DateTime.UtcNow.AddDays(1);
        appointment.StaffMember = staffMember;
        await Context.Appointments.AddAsync(appointment);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.CancelAppointmentCommandFaker.Generate() with
        {
            AppointmentId = appointment.Id,
        };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.Cancel(appointment.Id, It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(x => x.Publish(
            It.Is<AppointmentCanceledNotification>(x => x.StaffMemberId == appointment.StaffMemberId && x.ClientId == appointment.ClientId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenScheduledForExceededMinimumTimeToCancel_ShouldReturnConflictError()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        business.Settings = business.Settings with { CancelAppointmentMinimumTimeInMinutes = int.MaxValue };
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.BusinessId = business.Id;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        appointment.ScheduledFor = DateTime.UtcNow.AddDays(1);
        appointment.StaffMember = staffMember;
        await Context.Appointments.AddAsync(appointment);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.CancelAppointmentCommandFaker.Generate() with
        {
            AppointmentId = appointment.Id,
        };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(CustomError) && x.Message.Contains(Constants.ErrorMessages.CannotCancelAppointment));
    }

    [Fact]
    public async Task Handle_WhenScheduledForExceededMinimumTimeToCancelButForceCancel_ShouldCancelAppointment()
    {
        var business = Fakers.Businesses.BusinessFaker.Generate();
        business.Settings = business.Settings with { CancelAppointmentMinimumTimeInMinutes = int.MaxValue };
        await Context.Businesses.AddAsync(business);
        await Context.SaveChangesAsync();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.BusinessId = business.Id;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        Appointment appointment = Fakers.Appointments.AppointmentFaker.Generate();
        appointment.ScheduledFor = DateTime.UtcNow.AddDays(1);
        appointment.StaffMember = staffMember;
        await Context.Appointments.AddAsync(appointment);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.CancelAppointmentCommandFaker.Generate() with
        {
            ForceCancel = true,
            AppointmentId = appointment.Id,
        };

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.Cancel(appointment.Id, It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(x => x.Publish(
            It.Is<AppointmentCanceledNotification>(x => x.StaffMemberId == appointment.StaffMemberId && x.ClientId == appointment.ClientId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAppointmentDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Appointments.CancelAppointmentCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find appointment with id {command.AppointmentId}"));
    }
}
