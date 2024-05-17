using FluentResults;
using MediatR;

namespace Tor.Application.Clients.Queries.GetAppointments;

public record GetClientAppointmentsQuery(Guid ClientId) : IRequest<Result<List<ClientAppointmentResult>>>;
