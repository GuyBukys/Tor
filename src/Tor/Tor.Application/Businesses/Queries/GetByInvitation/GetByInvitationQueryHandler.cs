using FluentResults;
using MediatR;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Queries.GetByInvitation;

internal sealed class GetByInvitationQueryHandler : IRequestHandler<GetByInvitationQuery, Result<Business>>
{
    private readonly IBusinessRepository _repository;

    public GetByInvitationQueryHandler(IBusinessRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Business>> Handle(GetByInvitationQuery request, CancellationToken cancellationToken)
    {
        Business? business = await _repository.GetByInvitation(request.InvitationId, cancellationToken);

        if (business is null)
        {
            return Result.Fail<Business>(
                new NotFoundError($"could not find business with invitation id {request.InvitationId}"));
        }

        return business!;
    }
}
