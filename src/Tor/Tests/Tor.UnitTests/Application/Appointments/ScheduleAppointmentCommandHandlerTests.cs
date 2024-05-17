using FluentAssertions;
using Medallion.Threading;
using MediatR;
using Moq;
using Tor.Application;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Appointments.Commands.ScheduleAppointment;
using Tor.Application.Appointments.Common;
using Tor.Application.Appointments.Notifications.AppointmentScheduled;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Common.Extensions;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.AppointmentAggregate.Enums;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.Common.ValueObjects;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.UnitTests.Application.Appointments;
public class ScheduleAppointmentCommandHandlerTests : UnitTestBase
{
    private readonly ScheduleAppointmentCommandHandler _sut;
    private readonly Mock<IAppointmentRepository> _repositoryMock;
    private readonly Mock<IPublisher> _publisherMock;
    private readonly Mock<IAvailableTimesCalculator> _availableTimesCalculatorMock;
    private readonly Mock<IDistributedLockProvider> _lockProviderMock;
    private readonly Mock<IDistributedLock> _mockLock;

    public ScheduleAppointmentCommandHandlerTests()
        : base()
    {
        _repositoryMock = new Mock<IAppointmentRepository>();
        _publisherMock = new Mock<IPublisher>();
        _availableTimesCalculatorMock = new Mock<IAvailableTimesCalculator>();
        _lockProviderMock = new Mock<IDistributedLockProvider>();

        var mockHandle = new Mock<IDistributedSynchronizationHandle>();
        _mockLock = new Mock<IDistributedLock>();
        _mockLock.Setup(x => x.TryAcquireAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockHandle.Object);
        _lockProviderMock.Setup(x => x.CreateLock(It.IsAny<string>())).Returns(_mockLock.Object);

        _sut = new ScheduleAppointmentCommandHandler(
            Context,
            _repositoryMock.Object,
            _publisherMock.Object,
            _availableTimesCalculatorMock.Object,
            _lockProviderMock.Object);
    }

    [Theory]
    [InlineData(AppointmentType.Regular)]
    [InlineData(AppointmentType.Manual)]
    public async Task Handle_WhenValidCommmand_ShouldScheduleAppointment(AppointmentType type)
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        var business = Fakers.Businesses.BusinessFaker.Generate();
        staffMember.Business = business;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            Type = type,
        };
        SetupAvailableTimes(command, staffMember);
        _repositoryMock.Setup(x => x.Create(
            It.Is<CreateAppointmentInput>(x => x.StaffMemberId == command.StaffMemberId && x.ScheduledFor == command.ScheduledFor),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Fakers.Appointments.AppointmentFaker.Generate());

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.Create(It.Is<CreateAppointmentInput>(i => i.StaffMemberId == command.StaffMemberId), It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(x =>
            x.Publish(
                It.Is<AppointmentScheduledNotification>(x => x.ClientId == command.ClientId),
                It.IsAny<CancellationToken>()),
                type == AppointmentType.Regular ? Times.Once() : Times.Never());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task Handle_WhenRegualrAppointmentAndExceededMaximumNumberOfAppointments_ShouldReturnError(int amountToAdd)
    {
        var client = Fakers.Clients.ClientFaker.Generate();
        await Context.Clients.AddAsync(client);
        await Context.SaveChangesAsync();
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        var business = Fakers.Businesses.BusinessFaker.Generate();
        staffMember.Business = business;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        List<Appointment> appointments = Fakers.Appointments.AppointmentFaker.Generate(business.Settings.MaximumAppointmentsForClient + amountToAdd);
        appointments.ForEach(x =>
        {
            x.ScheduledFor = DateTimeOffset.UtcNow.AddDays(1);
            x.ClientId = client.Id;
            x.StaffMember = staffMember;
            x.StaffMemberId = staffMember.Id;
        });
        await Context.Appointments.AddRangeAsync(appointments);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            ClientId = client.Id,
            StaffMemberId = staffMember.Id,
            Type = AppointmentType.Regular,
        };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(CustomError) && x.Message == Constants.ErrorMessages.ExceededMaximumAppointments);
        _repositoryMock.Verify(x => x.Create(It.IsAny<CreateAppointmentInput>(), It.IsAny<CancellationToken>()), Times.Never);
        _publisherMock.Verify(x =>
            x.Publish(
                It.IsAny<AppointmentScheduledNotification>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRegularAppointmentAndNoAvailableTimes_ShouldReturnConflict()
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        var business = Fakers.Businesses.BusinessFaker.Generate();
        staffMember.Business = business;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            Type = AppointmentType.Regular,
        };
        SetupAvailableTimes(command, staffMember, isAvailableTimes: false);

        var result = await _sut.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.GetType() == typeof(CustomError) && x.Message.Contains(Constants.ErrorMessages.AppointmentTaken));
    }

    [Fact]
    public async Task Handle_WhenStaffMemberDoesntExist_ShouldReturnNotFoundError()
    {
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate();

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find staff member with id {command.StaffMemberId}"));
    }

    [Fact]
    public async Task Handle_WhenClientIdIsNotNullAndDoesntExist_ShouldReturnNotFoundError()
    {
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        var business = Fakers.Businesses.BusinessFaker.Generate();
        staffMember.Business = business;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            ClientId = Guid.NewGuid(),
        };

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(NotFoundError) && x.Message.Contains($"could not find client with id {command.ClientId}"));
    }

    [Fact]
    public async Task Handle_WhenRegularAppointmentAndCantGetLock_ShouldReturnDomainError()
    {
        _mockLock.Setup(x => x.TryAcquireAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>())).ReturnsAsync((IDistributedSynchronizationHandle?)null);
        var staffMember = Fakers.StaffMembers.StaffMemberFaker.Generate();
        var business = Fakers.Businesses.BusinessFaker.Generate();
        staffMember.Business = business;
        await Context.StaffMembers.AddAsync(staffMember);
        await Context.SaveChangesAsync();
        var command = Fakers.Appointments.ScheduleAppointmentCommandFaker.Generate() with
        {
            StaffMemberId = staffMember.Id,
            Type = AppointmentType.Regular,
        };
        SetupAvailableTimes(command, staffMember);

        var result = await _sut.Handle(command, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            x => x.GetType() == typeof(DomainError) && x.Message.Contains("could not acquire distributed lock"));
    }

    private void SetupAvailableTimes(ScheduleAppointmentCommand command, StaffMember staffMember, bool isAvailableTimes = true)
    {
        DateOnly atDate = DateOnly.FromDateTime(command.ScheduledFor.ToIsraelTime());
        TimeSpan duration = TimeSpan.FromMinutes(command.ServiceDetails.Durations.First().ValueInMinutes);

        TimeOnly scheduledForTime = TimeOnly.FromDateTime(command.ScheduledFor.ToIsraelTime());
        TimeOnly scheduledForTimeWithDuration = TimeOnly.FromDateTime(command.ScheduledFor.ToIsraelTime() + duration);

        List<TimeRange> timeRanges = isAvailableTimes ? [new(scheduledForTime, scheduledForTimeWithDuration)] : [];

        _availableTimesCalculatorMock.Setup(x =>
            x.CalculateAvailableTimes(
                atDate,
                duration,
                staffMember.Business.Settings,
                staffMember.WeeklySchedule,
                staffMember.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeRanges);
    }
}
