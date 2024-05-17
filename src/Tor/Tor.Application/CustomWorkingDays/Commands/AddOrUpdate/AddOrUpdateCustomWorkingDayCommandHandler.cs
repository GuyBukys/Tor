using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.CustomWorkingDays.Commands.AddOrUpdate;

internal sealed class AddOrUpdateCustomWorkingDayCommandHandler : IRequestHandler<AddOrUpdateCustomWorkingDayCommand, Result>
{
    private readonly ITorDbContext _context;

    public AddOrUpdateCustomWorkingDayCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AddOrUpdateCustomWorkingDayCommand request, CancellationToken cancellationToken)
    {
        StaffMember? staffMember = await _context.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staffMember is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        CustomWorkingDay? existingCustomWorkingDay = staffMember.CustomWorkingDays.FirstOrDefault(x => x.AtDate == request.CustomWorkingDay.AtDate);
        if (existingCustomWorkingDay is not null)
        {
            staffMember.CustomWorkingDays.Remove(existingCustomWorkingDay);
        }

        staffMember.CustomWorkingDays.Add(request.CustomWorkingDay);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
