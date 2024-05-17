using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.StaffMembers.Commands.UpdateStaffMemberAddress;

internal sealed class UpdateStaffMemberAddressCommandHandler : IRequestHandler<UpdateStaffMemberAddressCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateStaffMemberAddressCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateStaffMemberAddressCommand request, CancellationToken cancellationToken)
    {
        StaffMember? staffMember = await _context.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staffMember is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        staffMember.Address = request.Address;
        staffMember.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
