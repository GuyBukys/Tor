using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.StaffMembers.Commands.UpdateProfileImage;

public record UpdateStaffMemberProfileImageCommand(
    Guid StaffMemberId,
    Image? ProfileImage) : IRequest<Result>;
