using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Queries.CanAddStaffMember;

public record CanAddStaffMemberQuery(Guid BusinessId) : IRequest<Result<bool>>;
