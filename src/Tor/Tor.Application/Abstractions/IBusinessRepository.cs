using Tor.Application.Abstractions.Models;
using Tor.Application.Common.Models;
using Tor.Domain.BusinessAggregate;

namespace Tor.Application.Abstractions;

public interface IBusinessRepository
{
    Task<Business> Create(CreateBusinessInput input, CancellationToken cancellationToken);
    Task Deactivate(Guid businessId, CancellationToken cancellationToken);
    Task<Business?> GetById(Guid id, CancellationToken cancellationToken);
    Task<PagedList<BusinessOutput>> GetAll(GetAllBusinessesInput input, CancellationToken cancellationToken);
    Task<Business?> GetByStaffMember(Guid staffMemberId, CancellationToken cancellationToken);
    Task<Business?> GetByInvitation(string invitationId, CancellationToken cancellationToken);
}
