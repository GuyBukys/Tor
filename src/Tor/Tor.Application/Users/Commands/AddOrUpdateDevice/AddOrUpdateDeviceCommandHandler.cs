using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Application.Users.Commands.AddOrUpdateDevice;

internal sealed class AddOrUpdateDeviceCommandHandler : IRequestHandler<AddOrUpdateDeviceCommand, Result<List<Device>>>
{
    private readonly ITorDbContext _context;

    public AddOrUpdateDeviceCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<Device>>> Handle(AddOrUpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
            .FirstOrDefaultAsync(x => x.UserToken == request.UserToken, cancellationToken);

        if (user is null)
        {
            return Result.Fail<List<Device>>(
                new NotFoundError($"could not find user with token {request.UserToken}"));
        }

        bool isDeviceAlreadyExists = user.Devices.Any(x => x.Token == request.DeviceToken);
        if (isDeviceAlreadyExists)
        {
            return user.Devices;
        }

        user.Devices.Add(new(request.DeviceToken));
        user.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return user.Devices;
    }
}
