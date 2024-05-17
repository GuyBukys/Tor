using Tor.Domain.BusinessAggregate.Entities;
using FluentResults;
using MediatR;

namespace Tor.Application.Services.Queries.GetServices;

public record GetServicesQuery(Guid StaffMemberId) : IRequest<Result<List<Service>>>;
