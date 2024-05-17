using FluentResults;
using MediatR;

namespace Tor.Application.WaitingLists.Queries.GetByClient;

public record GetWaitingListsByClientQuery(Guid ClientId) : IRequest<Result<List<WaitingListResult>>>;
