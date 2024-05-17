using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tor.Application.Appointments.Commands.ApproveAppointment;
public record ApproveAppointmentCommand(Guid AppointmentId) : IRequest<Result>;
