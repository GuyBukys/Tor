using FluentResults;
using MediatR;

namespace Tor.Application.WaitingLists.Commands.JoinWaitingList;

public record JoinWaitingListCommand(
    Guid StaffMemberId,
    Guid ClientId,
    DateOnly AtDate) : IRequest<Result>;
