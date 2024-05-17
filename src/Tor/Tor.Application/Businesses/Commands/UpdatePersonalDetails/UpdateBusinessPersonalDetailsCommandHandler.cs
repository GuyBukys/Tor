using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Commands.UpdatePersonalDetails;

internal sealed class UpdateBusinessPersonalDetailsCommandHandler : IRequestHandler<UpdateBusinessPersonalDetailsCommand, Result>
{
    private readonly ITorDbContext _context;

    public UpdateBusinessPersonalDetailsCommandHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateBusinessPersonalDetailsCommand request, CancellationToken cancellationToken)
    {
        Business? business = await _context.Businesses
            .FirstOrDefaultAsync(x => x.Id == request.BusinessId, cancellationToken);
        if (business is null)
        {
            return Result.Fail(
                new NotFoundError($"could not find Business with id {request.BusinessId}"));
        }

        bool isEmailExistsInOtherBusiness = await _context.Businesses
            .Where(x => x.IsActive)
            .Where(x => x.Email == request.Email)
            .Where(x => x.Id != request.BusinessId)
            .AnyAsync(cancellationToken);
        if (isEmailExistsInOtherBusiness)
        {
            return Result.Fail(
                new ConflictError($"business with email {request.Email} already exists in another business"));
        }

        business.Name = request.Name;
        business.Description = request.Description;
        business.Email = request.Email;
        business.PhoneNumbers = request.PhoneNumbers;
        business.SocialMedias = request.SocialMedias;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
