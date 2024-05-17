using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.StaffMembers.Notifications.StaffMemberAdded;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.Common.Enums;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;

namespace Tor.Application.StaffMembers.Commands.AddStaffMember;

internal sealed class AddStaffMemberCommandHandler : IRequestHandler<AddStaffMemberCommand, Result<StaffMember>>
{
    private readonly ITorDbContext _context;
    private readonly IPublisher _publisher;
    private readonly IStorageManager _storageManager;

    public AddStaffMemberCommandHandler(
        ITorDbContext context,
        IPublisher publisher,
        IStorageManager storageManager)
    {
        _context = context;
        _publisher = publisher;
        _storageManager = storageManager;
    }

    public async Task<Result<StaffMember>> Handle(AddStaffMemberCommand request, CancellationToken cancellationToken)
    {
        Business? business = await _context.Businesses
            .Include(x => x.StaffMembers)
            .FirstOrDefaultAsync(x => x.Id == request.BusinessId, cancellationToken);

        if (business is null)
        {
            return Result.Fail<StaffMember>(
                new NotFoundError($"could not find business with id {request.BusinessId}"));
        }

        User? user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Fail<StaffMember>(
                new DomainError($"user id {request.UserId} doesnt exist"));
        }

        if (user.AppType != AppType.BusinessApp)
        {
            return Result.Fail<StaffMember>(
                new DomainError("staff member user is not a BusinessApp user type"));
        }

        StaffMember newStaffMember = new(Guid.NewGuid())
        {
            WeeklySchedule = request.WeeklySchedule,
            BusinessId = business.Id,
            UserId = request.UserId,
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow,
            Name = request.Name,
            Email = request.Email,
            IsActive = true,
            Position = PositionType.RegularStaffMember,
            PhoneNumber = request.PhoneNumber,
            ProfileImage = request.ProfileImage ?? _storageManager.GetDefaultImage(ImageType.Profile, EntityType.StaffMember),
            Services = request.Services.ConvertAll(service => new Service(Guid.NewGuid())
            {
                Amount = service.Amount,
                Description = string.Empty,
                Name = service.Name,
                Durations = service.Durations,
                CreatedDateTime = DateTime.UtcNow,
                UpdatedDateTime = DateTime.UtcNow,
                Location = LocationType.PhysicalLocation,
            }),
        };

        user.EntityId = newStaffMember.Id;

        business.StaffMembers.Add(newStaffMember);
        await _context.SaveChangesAsync(cancellationToken);

        var notification = new StaffMemberAddedNotification(newStaffMember.Id);
        await _publisher.Publish(notification, cancellationToken);

        return newStaffMember;
    }
}
