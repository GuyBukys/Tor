using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.StaffMembers.Queries.GetById;

public record GetStaffMemberByIdQuery(Guid Id) : IRequest<Result<StaffMember>>;
