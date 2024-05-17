using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Queries.GetAllClients;

public record GetAllClientsQuery(Guid Id) : IRequest<Result<GetAllClientsResult>>;
