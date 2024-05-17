using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Queries.GetById;

public record GetByIdQuery(Guid Id) : IRequest<Result<Business>>;
