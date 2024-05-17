using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.UserAggregate;

namespace Tor.Application.Users.Queries.Get;

internal sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<User>>
{
    private readonly ITorDbContext _context;

    public GetUserQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(x => x.UserToken == request.UserToken)
            .Where(x => x.AppType == request.AppType)
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? Result.Fail<User>(new NotFoundError($"could not find user with token {request.UserToken} and app type {request.AppType}"))
            : user;
    }
}
