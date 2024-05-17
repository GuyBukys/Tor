using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.CustomWorkingDays.Queries.GetCustomWorkingDays;

internal sealed class GetCustomWorkingDaysQueryHandler : IRequestHandler<GetCustomWorkingDaysQuery, Result<List<CustomWorkingDay>>>
{
    private readonly ITorDbContext _context;
    private readonly DateOnly _defaultFromDate;
    private readonly DateOnly _defaultUntilDate;

    public GetCustomWorkingDaysQueryHandler(ITorDbContext context)
    {
        _context = context;
        _defaultFromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _defaultUntilDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddMonths(1));
    }

    public async Task<Result<List<CustomWorkingDay>>> Handle(GetCustomWorkingDaysQuery request, CancellationToken cancellationToken)
    {
        StaffMember? staffMember = await _context.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staffMember is null)
        {
            return Result.Fail<List<CustomWorkingDay>>(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        DateOnly from = request.From ?? _defaultFromDate;
        DateOnly until = request.Until ?? _defaultUntilDate;

        var customWorkingDaysInRange = staffMember.CustomWorkingDays
            .Where(x => x.AtDate >= from && x.AtDate <= until)
            .ToList();

        return customWorkingDaysInRange;
    }
}
