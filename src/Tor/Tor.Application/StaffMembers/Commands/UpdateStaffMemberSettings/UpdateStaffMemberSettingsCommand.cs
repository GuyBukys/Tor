using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.StaffMembers.Commands.UpdateStaffMemberSettings;
public record UpdateStaffMemberSettingsCommand(
    Guid StaffMemberId,
    StaffMemberSettings Settings) : IRequest<Result>;
