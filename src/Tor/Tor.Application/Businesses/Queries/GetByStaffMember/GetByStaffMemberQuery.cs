using Tor.Domain.BusinessAggregate;
using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Queries.GetByStaffMember;

public record GetByStaffMemberQuery(Guid StaffMemberId) : IRequest<Result<Business>>;
