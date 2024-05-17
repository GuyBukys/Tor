using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tor.Application.Abstractions;
using Tor.Application.Abstractions.Models;
using Tor.Application.Common.Extensions;
using Tor.Application.Common.Models;
using Tor.Domain.BusinessAggregate;

namespace Tor.Infrastructure.Persistence.Repositories;

internal sealed class BusinessRepository : IBusinessRepository
{
    private readonly TorDbContext _context;

    public BusinessRepository(TorDbContext context)
    {
        _context = context;
    }

    public Task<Business> Create(CreateBusinessInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task Deactivate(Guid businessId, CancellationToken cancellationToken)
    {
        using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        await _context.Businesses
            .Where(x => x.Id == businessId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.IsActive, false)
                .SetProperty(p => p.UpdatedDateTime, DateTime.UtcNow),
                cancellationToken);

        List<Guid> userIds = await _context.StaffMembers
            .Where(x => x.BusinessId == businessId)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        await _context.StaffMembers
            .Where(x => x.BusinessId == businessId)
            .ExecuteDeleteAsync(cancellationToken);

        await _context.Users
            .Where(x => userIds.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<PagedList<BusinessOutput>> GetAll(GetAllBusinessesInput input, CancellationToken cancellationToken)
    {
        IQueryable<Business> businessesQuery = _context.Businesses
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (!string.IsNullOrEmpty(input.FreeText))
        {
            businessesQuery = businessesQuery.Where(x => x.Name.Contains(input.FreeText));
        }

        List<Guid> businessIdsWhoBlockedClient = await _context.BlockedClients
            .Where(x => x.ClientId == input.ClientId)
            .Select(x => x.BusinessId)
            .ToListAsync(cancellationToken);

        businessesQuery = businessesQuery.Where(x => !businessIdsWhoBlockedClient.Contains(x.Id));

        Expression<Func<Business, object>> orderBySelector = input.SortColumn?.ToLower() switch
        {
            Sorting.SortColumns.Name => business => business.Name,
            Sorting.SortColumns.CreationDate => business => business.CreatedDateTime,
            _ => business => business.Id,
        };

        businessesQuery = input.SortOrder?.ToLower() switch
        {
            Sorting.SortOrder.Asc => businessesQuery.OrderBy(orderBySelector),
            Sorting.SortOrder.Desc => businessesQuery.OrderByDescending(orderBySelector),
            _ => businessesQuery.OrderBy(orderBySelector),
        };

        return await businessesQuery
            .Select(x => new BusinessOutput(
                x.Id,
                x.Name,
                x.Description,
                x.Logo!,
                x.Cover!,
                x.Address,
                x.PhoneNumbers.First(),
                null))
            .ToPagedListAsync(input.Page, input.PageSize, cancellationToken);
    }

    public async Task<Business?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Businesses
            .AsNoTracking()
            .Include(x => x.Tier)
            .Include(x => x.StaffMembers)
                .ThenInclude(x => x.Services)
            .Where(x => x.IsActive)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Business?> GetByInvitation(string invitationId, CancellationToken cancellationToken)
    {
        return await _context.Businesses
            .AsNoTracking()
            .Include(x => x.Tier)
            .Include(x => x.StaffMembers)
                .ThenInclude(x => x.Services)
            .Where(x => x.IsActive)
            .Where(x => x.InvitationId == invitationId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Business?> GetByStaffMember(Guid staffMemberId, CancellationToken cancellationToken)
    {
        return await _context.Businesses
            .AsNoTracking()
            .Include(x => x.Tier)
            .Include(x => x.StaffMembers)
                .ThenInclude(x => x.Services)
            .Where(x => x.IsActive)
            .Where(x => x.StaffMembers.Any(s => s.Id == staffMemberId))
            .FirstOrDefaultAsync(cancellationToken);
    }
}

file static class Sorting
{
    public static class SortOrder
    {
        public const string Asc = "asc";
        public const string Desc = "desc";
    }

    public static class SortColumns
    {
        public const string Name = "name";
        public const string CreationDate = "created";
    }
}
