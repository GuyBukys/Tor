using FluentResults;
using MediatR;

namespace Tor.Application.StaffMembers.Commands.DeleteStaffMember;

public record DeleteStaffMemberCommand(Guid StaffMemberId) : IRequest<Result>;
