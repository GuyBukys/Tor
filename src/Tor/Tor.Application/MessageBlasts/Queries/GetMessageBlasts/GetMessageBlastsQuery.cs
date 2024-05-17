using FluentResults;
using MediatR;
using Tor.Application.MessageBlasts.Common;

namespace Tor.Application.MessageBlasts.Queries.GetMessageBlasts;

public record GetMessageBlastsQuery(Guid BusinessId) : IRequest<Result<List<MessageBlastResult>>>;
