using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate.Entities;

namespace Tor.Application.Services.Commands.AddService;

internal sealed class AddServiceCommandHandler : IRequestHandler<AddServiceCommand, Result<Guid>>
{
    private readonly ITorDbContext _context;

    public AddServiceCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(AddServiceCommand request, CancellationToken cancellationToken)
    {
        bool isStaffMemberExists = await _context.StaffMembers
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (!isStaffMemberExists)
        {
            return Result.Fail<Guid>(
                new NotFoundError($"could not find staff member with id {request.StaffMemberId}"));
        }

        bool isServiceAlreadyExistsForStaffMember = await _context.Services
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .AnyAsync(x => x.Name == request.Name, cancellationToken);

        if (isServiceAlreadyExistsForStaffMember)
        {
            return Result.Fail<Guid>(
                new ConflictError($"service with name '{request.Name}' already exists for staff member"));
        }


        Service service = new(Guid.NewGuid())
        {
            StaffMemberId = request.StaffMemberId,
            Name = request.Name,
            Description = request.Description,
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow,
            Amount = request.Amount,
            Durations = request.Durations,
        };

        await _context.Services.AddAsync(service, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return service.Id;
    }
}
