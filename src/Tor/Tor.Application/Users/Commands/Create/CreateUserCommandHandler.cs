using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Application.Users.Commands.Create;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<User>>
{
    private readonly ITorDbContext _context;

    public CreateUserCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
            .AsNoTracking()
            .Where(x => x.AppType == request.AppType)
            .Where(x => x.UserToken == request.UserToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is not null)
        {
            return Result.Fail<User>(
                new ConflictError($"user with token {request.UserToken} and app type {request.AppType} already exists"));
        }

        User newUser = new(Guid.NewGuid())
        {
            UserToken = request.UserToken,
            AppType = request.AppType,
            PhoneNumber = request.PhoneNumber,
            FirstLogin = true,
            Devices = [new Device(request.DeviceToken)],
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow,
            IsActive = true,
        };

        await _context.Users.AddAsync(newUser, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return newUser;
    }
}
