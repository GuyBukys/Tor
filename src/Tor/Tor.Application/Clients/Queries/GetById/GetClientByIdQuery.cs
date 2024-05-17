using FluentResults;
using MediatR;
using Tor.Domain.ClientAggregate;

namespace Tor.Application.Clients.Queries.GetById;

public record GetClientByIdQuery(Guid Id) : IRequest<Result<Client>>;
