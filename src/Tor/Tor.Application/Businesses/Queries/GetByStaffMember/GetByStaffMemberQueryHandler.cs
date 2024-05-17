using FluentResults;
using MediatR;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Queries.GetByStaffMember;

internal sealed class GetByStaffMemberQueryHandler : IRequestHandler<GetByStaffMemberQuery, Result<Business>>
{
    private readonly IBusinessRepository _repository;

    public GetByStaffMemberQueryHandler(IBusinessRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Business>> Handle(GetByStaffMemberQuery request, CancellationToken cancellationToken)
    {
        Business? business = await _repository.GetByStaffMember(request.StaffMemberId, cancellationToken);

        if (business is null)
        {
            return Result.Fail<Business>(
                new NotFoundError($"could not find business with staff member id {request.StaffMemberId}"));
        }

        return business!;
    }
}
