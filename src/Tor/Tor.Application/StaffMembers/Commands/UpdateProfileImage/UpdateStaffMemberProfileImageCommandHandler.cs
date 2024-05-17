using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.Common.Enums;

namespace Tor.Application.StaffMembers.Commands.UpdateProfileImage;

internal sealed class UpdateStaffMemberProfileImageCommandHandler : IRequestHandler<UpdateStaffMemberProfileImageCommand, Result>
{
    private readonly ITorDbContext _context;
    private readonly IStorageManager _storageManager;

    public UpdateStaffMemberProfileImageCommandHandler(ITorDbContext context, IStorageManager storageManager)
    {
        _context = context;
        _storageManager = storageManager;
    }

    public async Task<Result> Handle(UpdateStaffMemberProfileImageCommand request, CancellationToken cancellationToken)
    {
        var staffMember = await _context.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staffMember is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        if (request.ProfileImage is not null &&
            !await _storageManager.IsFileExists(request.ProfileImage.Name, cancellationToken))
        {
            return Result.Fail(
                new NotFoundError($"profile image {request.ProfileImage.Name} doesnt exist in storage"));
        }

        staffMember.ProfileImage = request.ProfileImage ?? _storageManager.GetDefaultImage(ImageType.Profile, EntityType.StaffMember);
        staffMember.UpdatedDateTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
