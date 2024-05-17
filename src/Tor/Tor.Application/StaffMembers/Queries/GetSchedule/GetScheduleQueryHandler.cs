using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Common.Extensions;
using Tor.Domain.AppointmentAggregate;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.StaffMembers.Queries.GetSchedule;

internal sealed class GetScheduleQueryHandler : IRequestHandler<GetScheduleQuery, Result<GetScheduleResult>>
{
    private readonly ITorDbContext _context;
    private readonly DateTimeOffset _defaultFrom;

    private const int _untilThresholdInDays = 7;

    public GetScheduleQueryHandler(ITorDbContext context)
    {
        _context = context;
        _defaultFrom = DateTimeOffset.UtcNow.Date;
    }

    public async Task<Result<GetScheduleResult>> Handle(GetScheduleQuery request, CancellationToken cancellationToken)
    {
        bool isStaffMemberExists = await _context.StaffMembers
            .AnyAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (!isStaffMemberExists)
        {
            return Result.Fail<GetScheduleResult>(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        // the appointments are in utc date time offset
        DateTimeOffset from = request.From ?? _defaultFrom;
        DateTimeOffset until = request.Until ?? from.AddDays(_untilThresholdInDays);
        List<Appointment> appointments = await _context.Appointments
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .Where(x => x.ScheduledFor >= from && x.ScheduledFor <= until)
            .ToListAsync(cancellationToken);

        // the reserved time slots are in israel date
        DateOnly fromInIsraelDate = DateOnly.FromDateTime(from.ToIsraelTime());
        DateOnly untilInIsraelDate = DateOnly.FromDateTime(until.ToIsraelTime());
        List<ReservedTimeSlot> reservedTimeSlots = await _context.ReservedTimeSlots
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .Where(x => x.AtDate >= fromInIsraelDate && x.AtDate <= untilInIsraelDate)
            .ToListAsync(cancellationToken);

        return new GetScheduleResult(appointments, reservedTimeSlots);
    }
}
