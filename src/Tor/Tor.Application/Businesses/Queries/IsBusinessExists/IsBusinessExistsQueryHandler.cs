using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Queries.IsBusinessExists;

internal sealed class IsBusinessExistsQueryHandler : IRequestHandler<IsBusinessExistsQuery, Result<bool>>
{
    private readonly ITorDbContext _context;

    public IsBusinessExistsQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(IsBusinessExistsQuery request, CancellationToken cancellationToken)
    {
        Business? business = await _context.Businesses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.Email == request.Email)
            .FirstOrDefaultAsync(cancellationToken);

        return business is not null;
    }
}
