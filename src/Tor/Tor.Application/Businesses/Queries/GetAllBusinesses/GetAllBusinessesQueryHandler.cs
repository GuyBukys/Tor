using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Common.Errors.ErrorTypes;
using Tor.Application.Common.Models;

namespace Tor.Application.Businesses.Queries.GetAllBusinesses;

internal sealed class GetAllBusinessesQueryHandler : IRequestHandler<GetAllBusinessesQuery, Result<PagedList<BusinessSummary>>>
{
    private readonly ITorDbContext _context;
    private readonly IBusinessRepository _repository;

    public GetAllBusinessesQueryHandler(
        ITorDbContext context,
        IBusinessRepository repository)
    {
        _context = context;
        _repository = repository;
    }

    public async Task<Result<PagedList<BusinessSummary>>> Handle(GetAllBusinessesQuery request, CancellationToken cancellationToken)
    {
        bool isClientExists = await _context.Clients
            .AnyAsync(x => x.Id == request.ClientId, cancellationToken);
        if (!isClientExists)
        {
            return Result.Fail<PagedList<BusinessSummary>>(
                new NotFoundError($"could not find client with id {request.ClientId}"));
        }

        GetAllBusinessesInput input = new(
            request.ClientId,
            request.Page,
            request.PageSize,
            request.FreeText,
            request.SortOrder,
            request.SortColumn);
        PagedList<BusinessOutput> businesses = await _repository.GetAll(input, cancellationToken);

        List<Guid> favoriteBusinessIds = await _context.FavoriteBusinesses
            .Where(x => x.ClientId == request.ClientId)
            .Select(x => x.BusinessId)
            .ToListAsync(cancellationToken);

        List<BusinessSummary> businessSummaries = businesses.Items.ConvertAll(business =>
        {
            return new BusinessSummary(
                business.Id,
                business.Name,
                business.Description,
                business.Logo,
                business.Cover,
                business.Address,
                business.PhoneNumber,
                false,
                favoriteBusinessIds.Contains(business.Id));
        });

        return new PagedList<BusinessSummary>(businessSummaries, businesses.Page, businesses.PageSize, businesses.TotalCount);
    }
}
