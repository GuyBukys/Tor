using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.StaffMembers.Commands.UpdateWeeklySchedule;

internal sealed class UpdateStaffMemberWeeklyScheduleCommandHandler : IRequestHandler<UpdateStaffMemberWeeklyScheduleCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateStaffMemberWeeklyScheduleCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateStaffMemberWeeklyScheduleCommand request, CancellationToken cancellationToken)
    {
        StaffMember? staffMember = await _context.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staffMember is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        staffMember.WeeklySchedule = request.WeeklySchedule;
        staffMember.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
