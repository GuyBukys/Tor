using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.MessageBlasts.Common;

namespace Tor.Application.MessageBlasts.Queries.GetMessageBlasts;

internal sealed class GetMessageBlastsQueryHandler : IRequestHandler<GetMessageBlastsQuery, Result<List<MessageBlastResult>>>
{
    private readonly ITorDbContext _context;

    public GetMessageBlastsQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MessageBlastResult>>> Handle(GetMessageBlastsQuery request, CancellationToken cancellationToken)
    {
        bool isBusinessExists = await _context.Businesses
            .AnyAsync(x => x.Id == request.BusinessId, cancellationToken);
        if (!isBusinessExists)
        {
            return Result.Fail<List<MessageBlastResult>>(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        var businessMessageBlasts = await _context.BusinessMessageBlasts
            .AsNoTracking()
            .Include(x => x.MessageBlast)
            .Where(x => x.BusinessId == request.BusinessId)
            .ToListAsync(cancellationToken);

        List<MessageBlastResult> result = businessMessageBlasts.ConvertAll(MessageBlastResult.FromBusinessMessageBlast);

        return result;
    }
}
