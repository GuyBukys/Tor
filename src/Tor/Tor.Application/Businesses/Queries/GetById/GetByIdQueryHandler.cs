using FluentResults;
using MediatR;
using Tor.Application.Abstractions;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Businesses.Queries.GetById;

internal sealed class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, Result<Business>>
{
    private readonly IBusinessRepository _repository;

    public GetByIdQueryHandler(IBusinessRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Business>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        Business? business = await _repository.GetById(request.Id, cancellationToken);

        if (business is null)
        {
            return Result.Fail<Business>(
                new NotFoundError($"could not find business with id {request.Id}"));
        }

        return business!;
    }
}
