using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.StaffMembers.Commands.UpdatePersonalDetails;

internal sealed class UpdateStaffMemberPersonalDetailsCommandHandler : IRequestHandler<UpdateStaffMemberPersonalDetailsCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateStaffMemberPersonalDetailsCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateStaffMemberPersonalDetailsCommand request, CancellationToken cancellationToken)
    {
        var staffMember = await _context.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staffMember is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        staffMember.Name = request.Name;
        staffMember.Description = request.Description;
        staffMember.Email = request.Email;
        staffMember.BirthDate = request.BirthDate;
        staffMember.PhoneNumber = request.PhoneNumber;
        staffMember.Address = request.Address;
        staffMember.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
