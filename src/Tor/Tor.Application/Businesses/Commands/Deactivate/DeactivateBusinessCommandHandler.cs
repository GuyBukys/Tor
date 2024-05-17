using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Notifications.BusinessDeactivated;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Application.Businesses.Commands.Deactivate;

internal sealed class DeactivateBusinessCommandHandler : IRequestHandler<DeactivateBusinessCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IBusinessRepository _repository;
    private readonly IPublisher _publisher;

    public DeactivateBusinessCommandHandler(ITorDbContext context, IBusinessRepository repository, IPublisher publisher)
    {
        _context = context;
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result> Handle(DeactivateBusinessCommand request, CancellationToken cancellationToken)
    {
        bool isBusinessExists = await _context.Businesses
            .AnyAsync(x => x.Id == request.BusinessId, cancellationToken);
        if (!isBusinessExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        List<string> allStaffMembersDeviceTokens = (await _context.StaffMembers
            .AsNoTracking()
            .Where(x => x.BusinessId == request.BusinessId)
            .Select(x => x.User.Devices)
            .FirstOrDefaultAsync(cancellationToken))!
            .ConvertAll(x => x.Token);

        await _repository.Deactivate(request.BusinessId, cancellationToken);

        await _publisher.Publish(new BusinessDeactivatedNotification(allStaffMembersDeviceTokens), cancellationToken);

        return Result.Ok();
    }
}
