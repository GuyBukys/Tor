using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Commands.Create;
using Tor.Application.Businesses.Common;
using Tor.Application.Businesses.Notifications.BusinessCreated;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.Common.Enums;
using Tor.Domain.TierAggregate;
using Tor.Domain.TierAggregate.Enums;
using Tor.Domain.UserAggregate;
using Tor.Domain.UserAggregate.Enum;

namespace Tor.Application.Users.Commands.Create;

internal sealed class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, Result<Business>>
{
    private readonly ITorDbContext _context;
    private readonly IStorageManager _storageManager;
    private readonly IPublisher _publisher;

    public CreateBusinessCommandHandler(
        ITorDbContext context,
        IStorageManager storageManager,
        IPublisher publisher)
    {
        _context = context;
        _storageManager = storageManager;
        _publisher = publisher;
    }

    public async Task<Result<Business>> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .Where(x => request.CategoryIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
        if (categories.Count != request.CategoryIds.Count)
        {
            IEnumerable<Guid> categoryIdsThatDoesntExist = request.CategoryIds
                .Except(categories.Select(x => x.Id));

            return Result.Fail<Business>(
                new DomainError($"category ids ({string.Join(',', categoryIdsThatDoesntExist)})  doesnt exist"));
        }

        bool isStaffMemberAlreadyExists = await _context.StaffMembers
            .AsNoTracking()
            .AnyAsync(x => x.UserId == request.BusinessOwner.UserId, cancellationToken);
        if (isStaffMemberAlreadyExists)
        {
            return Result.Fail<Business>(
                new ConflictError($"staff member with user id {request.BusinessOwner.UserId} already exists"));
        }

        User? user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == request.BusinessOwner.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Fail<Business>(
                new DomainError($"user id {request.BusinessOwner.UserId} doesnt exist"));
        }

        if (user.AppType != AppType.BusinessApp)
        {
            return Result.Fail<Business>(
                new DomainError("business owner user is not a BusinessApp user type"));
        }

        List<string> missingImages = await CheckForNonExistingImagesInStorage(request, cancellationToken);
        if (missingImages.Count > 0)
        {
            return Result.Fail<Business>(
                new DomainError($"some images do not exist in storage bucket. file names: {string.Join(',', missingImages)}"));
        }

        Tier tier = await _context.Tiers
            .FirstAsync(x => x.Type == TierType.Premium, cancellationToken);

        Guid businessOwnerId = Guid.NewGuid();
        user.EntityId = businessOwnerId;
        Business business = new(Guid.NewGuid())
        {
            ReferringBusinessId = request.ReferringBusinessId,
            Name = request.Name,
            Description = request.Description,
            Email = request.Email,
            Categories = categories,
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow,
            IsActive = true,
            InvitationId = Ulid.NewUlid().ToString(),
            ReferralCode = ReferralCodeGenerator.Generate(request.Name),
            Address = request.Address,
            PhoneNumbers = request.PhoneNumbers,
            Logo = request.Logo ?? _storageManager.GetDefaultImage(ImageType.Logo, EntityType.Business),
            Cover = request.Cover ?? _storageManager.GetDefaultImage(ImageType.Cover, EntityType.Business, categories.First().Type),
            Locations = [new(LocationType.PhysicalLocation),],
            Tier = tier,
            StaffMembers =
            [
                new(businessOwnerId)
                {
                    WeeklySchedule = request.BusinessOwner.WeeklySchedule,
                    Name = request.BusinessOwner.Name,
                    Email = request.BusinessOwner.Email,
                    UserId = request.BusinessOwner.UserId,
                    CreatedDateTime = DateTime.UtcNow,
                    UpdatedDateTime = DateTime.UtcNow,
                    ProfileImage = request.BusinessOwner.ProfileImage ?? _storageManager.GetDefaultImage(ImageType.Profile, EntityType.StaffMember),
                    PhoneNumber = request.BusinessOwner.PhoneNumber,
                    IsActive = true,
                    Position = PositionType.BusinessOwner,
                    Services = request.BusinessOwner.Services
                            .ConvertAll(s => new Service(Guid.NewGuid())
                            {
                                Amount = s.Amount,
                                Name = s.Name,
                                CreatedDateTime = DateTime.UtcNow,
                                UpdatedDateTime = DateTime.UtcNow,
                                Location = LocationType.PhysicalLocation,
                                Durations = s.Durations,
                            }),
                }
            ],
        };

        await _context.Businesses.AddAsync(business, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        BusinessCreatedNotification notification = new(business.Id, business.Name, user.Devices);
        await _publisher.Publish(notification, cancellationToken);

        return business;
    }

    private async Task<List<string>> CheckForNonExistingImagesInStorage(
        CreateBusinessCommand request,
        CancellationToken cancellationToken)
    {
        List<string> nonExistingFileNames = [];

        if (request.Cover is not null)
        {
            bool isBusinessOwnerProfileImageExists = await _storageManager.IsFileExists(request.Cover.Name, cancellationToken);
            if (!isBusinessOwnerProfileImageExists)
            {
                nonExistingFileNames.Add(request.Cover.Name);
            }
        }

        return nonExistingFileNames;
    }
}
