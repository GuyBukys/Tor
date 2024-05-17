using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.StaffMembers.Commands.DeleteStaffMember;

internal sealed class DeleteStaffMemberCommandHandler : IRequestHandler<DeleteStaffMemberCommand, Result>
{
    private readonly ITorDbContext _context;

    public DeleteStaffMemberCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteStaffMemberCommand request, CancellationToken cancellationToken)
    {
        StaffMember? staffMember = await _context.StaffMembers
            .Include(x => x.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (staffMember is null)
        {
            return Result.Ok();
        }

        using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        _context.Users.Remove(staffMember.User);
        _context.StaffMembers.Remove(staffMember);
        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
