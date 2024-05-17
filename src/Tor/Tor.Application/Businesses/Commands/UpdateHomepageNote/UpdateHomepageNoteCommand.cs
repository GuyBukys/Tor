using FluentResults;
using MediatR;

namespace Tor.Application.Businesses.Commands.UpdateHomepageNote;

public record UpdateHomepageNoteCommand(Guid BusinessId, string HomepageNote) : IRequest<Result>;

