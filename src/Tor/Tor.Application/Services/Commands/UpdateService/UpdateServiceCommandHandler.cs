using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.Services.Commands.UpdateService;

internal sealed class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Result<Service>>
{
    private readonly ITorDbContext _context;

    public UpdateServiceCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Service>> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        Service? service = await _context.Services
            .FirstOrDefaultAsync(x => x.Id == request.ServiceId, cancellationToken);

        if (service is null)
        {
            return Result.Fail<Service>(
                new NotFoundError($"could not find service with id {request.ServiceId}"));
        }

        UpdateProperties(ref service, request);
        await _context.SaveChangesAsync(cancellationToken);

        return service;
    }

    private static void UpdateProperties(ref Service service, UpdateServiceCommand request)
    {
        service.Name = request.Name;
        service.Description = request.Description;
        service.Amount = request.Amount;
        service.Durations = request.Durations;
        service.UpdatedDateTime = DateTime.UtcNow;
    }
}
