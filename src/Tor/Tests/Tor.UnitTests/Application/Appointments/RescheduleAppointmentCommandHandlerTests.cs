using FluentAssertions;
using MediatR;
using Moq;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Commands.RescheduleAppointment;
using Tor.Application.Appointments.Notifications.AppointmentScheduled;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.AppointmentAggregate;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Appointments;
public class RescheduleAppointmentCommandHandlerTests : UnitTestBase
{
    private readonly Mock<IAppointmentRepository> _repositoryMock;
    private readonly Mock<IPublisher> _publisherMock;

    private readonly RescheduleAppointmentCommandHandler _sut;

    public RescheduleAppointmentCommandHandlerTests()
        : base()
    {
        _repositoryMock = new Mock<IAppointmentRepository>();
        _publisherMock = new Mock<IPublisher>();

        _sut = new RescheduleAppointmentCommandHandler(Context, _repositoryMock.Object, _publisherMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidCommmand_ShouldRescheduleAppointment()
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        staffMember.Business = Fakers.Businesses.BusinessFaker.Generate();
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var appointment = Fakers.Appointments.AppointmentFaker.Generate();
        appointment.StaffMemberId = staffMember.Id;
        await Context.Appointments.AddAsync(appointment);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.RescheduleAppointmentCommandFaker.Generate() with
        {
            AppointmentId = appointment.Id,
            StaffMemberId = staffMember.Id,
        };
        Appointment newAppointment = Fakers.Appointments.AppointmentFaker.Generate();
        newAppointment.StaffMemberId = staffMember.Id;
        _repositoryMock.Setup(x => x.Create(It.Is<CreateAppointmentInput>(i => i.ScheduledFor == command.ScheduledFor), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newAppointment);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(newAppointment);
        _repositoryMock.Verify(x => x.Cancel(appointment.Id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.Create(It.Is<CreateAppointmentInput>(i => i.ScheduledFor == command.ScheduledFor), It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(x => x.Publish(It.Is<AppointmentScheduledNotification>(n =>
            n.BusinessId == staffMember.BusinessId &&
            n.ClientId == newAppointment.ClientId &&
            n.StaffMemberId == newAppointment.StaffMemberId &&
            n.NotifyClient == command.NotifyClient &&
            n.NotifyStaffMember == false),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAppointmentDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Appointments.RescheduleAppointmentCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find appointment with id {command.AppointmentId}"));
    }
}
