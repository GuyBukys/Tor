using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.StaffMembers.Commands.UpdateStaffMemberAddress;

public record UpdateStaffMemberAddressCommand(
    Guid StaffMemberId,
    Address Address) : IRequest<Result>;
