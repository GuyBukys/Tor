using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.ClientAggregate;
using Tor.Domain.WaitingListAggregate;

namespace Tor.Application.WaitingLists.Commands.JoinWaitingList;

internal sealed class JoinWaitingListCommandHandler : IRequestHandler<JoinWaitingListCommand, Result>
{
    private readonly ITorDbContext _context;

    public JoinWaitingListCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(JoinWaitingListCommand request, CancellationToken cancellationToken)
    {
        bool isStaffMemberExists = await _context.StaffMembers
            .AnyAsync(x => x.Id == request.StaffMemberId, cancellationToken);
        if (!isStaffMemberExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        Client? client = await _context.Clients
            .FirstOrDefaultAsync(x => x.Id == request.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        WaitingList waitingList = await GetOrAdd(request, cancellationToken);
        waitingList.Clients.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    private async Task<WaitingList> GetOrAdd(JoinWaitingListCommand request, CancellationToken cancellationToken)
    {
        WaitingList? waitingList = await _context.WaitingLists
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .Where(x => x.AtDate == request.AtDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (waitingList is not null)
        {
            return waitingList;
        }

        WaitingList newWaitingList = new(Guid.NewGuid())
        {
            StaffMemberId = request.StaffMemberId,
            AtDate = request.AtDate,
        };

        await _context.WaitingLists.AddAsync(newWaitingList, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return newWaitingList;
    }
}
