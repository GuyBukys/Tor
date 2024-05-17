using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.StaffMembers.Commands.UpdateStaffMemberSettings;

internal sealed class UpdateStaffMemberSettingsCommandHandler : IRequestHandler<UpdateStaffMemberSettingsCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateStaffMemberSettingsCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateStaffMemberSettingsCommand request, CancellationToken cancellationToken)
    {
        bool isStaffMemberExists = await _context.StaffMembers
            .AnyAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (!isStaffMemberExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        await _context.StaffMembers
            .Where(x => x.Id == request.StaffMemberId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.Settings.SendNotificationsWhenAppointmentScheduled, request.Settings.SendNotificationsWhenAppointmentScheduled)
                .SetProperty(p => p.Settings.SendNotificationsWhenAppointmentCanceled, request.Settings.SendNotificationsWhenAppointmentCanceled)
                .SetProperty(p => p.UpdatedDateTime, DateTime.UtcNow),
                cancellationToken);

        return Result.Ok();
    }
}
