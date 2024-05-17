using FluentResults;
using MediatR;
using Tor.Domain.AppointmentAggregate;

namespace Tor.Application.Businesses.Queries.GetAppointmentsByClient;

public record GetAppointmentsByClientQuery(Guid BusinessId, Guid ClientId) : IRequest<Result<List<Appointment>>>;
