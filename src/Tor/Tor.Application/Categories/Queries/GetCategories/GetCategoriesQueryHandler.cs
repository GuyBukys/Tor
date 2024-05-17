using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tor.Application.Abstractions;
using Tor.Domain.CategoryAggregate;

namespace Tor.Application.Categories.Queries.GetCategories;

internal sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<Category>>>
{
    private readonly ITorDbContext _context;

    public GetCategoriesQueryHandler(ITorDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<Category>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderByDescending(x => x.Type)
            .ToArrayAsync(cancellationToken);

        return categories;
    }
}
