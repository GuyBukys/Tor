using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.AppointmentAggregate.Enums;

namespace Tor.Application.Appointments.Commands.ApproveAppointment;
internal sealed class ApproveAppointmentCommandHandler : IRequestHandler<ApproveAppointmentCommand, Result>
{
    private readonly ITorDbContext _context;

    public ApproveAppointmentCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ApproveAppointmentCommand request, CancellationToken cancellationToken)
    {
        bool isAppointmentExists = await _context.Appointments
            .AnyAsync(x => x.Id == request.AppointmentId, cancellationToken);
        if (!isAppointmentExists)
        {
            return Result.Fail(
                new NotFoundError($"could not find appointment with id {request.AppointmentId}"));
        }

        await _context.Appointments
            .Where(x => x.Id == request.AppointmentId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.Status, AppointmentStatusType.Approved),
                cancellationToken);

        return Result.Ok();
    }
}
