using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.ClientAggregate;

namespace Tor.Application.WaitingLists.Queries.GetByClient;

internal sealed class GetWaitingListsByClientQueryHandler : IRequestHandler<GetWaitingListsByClientQuery, Result<List<WaitingListResult>>>
{
    private readonly ITorDbContext _context;

    public GetWaitingListsByClientQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<WaitingListResult>>> Handle(GetWaitingListsByClientQuery request, CancellationToken cancellationToken)
    {
        Client? client = await _context.Clients
            .AsNoTracking()
            .Include(x => x.WaitingLists)
            .ThenInclude(x => x.StaffMember)
            .ThenInclude(x => x.Business)
            .Where(x => x.Id == request.ClientId)
            .FirstOrDefaultAsync(cancellationToken);

        if (client is null)
        {
            return Result.Fail<List<WaitingListResult>>(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        List<WaitingListResult> result = client.WaitingLists
            .Where(x => x.AtDate >= DateOnly.FromDateTime(DateTime.UtcNow))
            .Select(x => new WaitingListResult(
                x.AtDate,
                x.StaffMemberId,
                x.StaffMember.BusinessId,
                x.StaffMember.Business.Name,
                x.StaffMember.Business.Logo!))
            .ToList();

        return result;
    }
}
