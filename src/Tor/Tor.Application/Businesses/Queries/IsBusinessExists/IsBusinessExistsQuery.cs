using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Queries.IsBusinessExists;
public record IsBusinessExistsQuery(string Email) : IRequest<Result<bool>>;
