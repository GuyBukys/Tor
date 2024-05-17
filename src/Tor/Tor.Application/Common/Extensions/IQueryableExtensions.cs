using Microsoft.EntityFrameworkCore;
using Tor.Application.Common.Models;

namespace Tor.Application.Common.Extensions;

public static class IQueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new(items, page, pageSize, totalCount);
    }
}
