using Domain;
using Tor.Domain.BusinessAggregate;

namespace Tor.Domain.ClientAggregate.Entities;

public class FavoriteBusiness : Entity<Guid>
{
    public FavoriteBusiness(Guid id) : base(id)
    {
    }

    public Guid BusinessId { get; set; }
    public Guid ClientId { get; set; }
    public bool MuteNotifications { get; set; }

    public Business Business { get; set; } = default!;
    public Client Client { get; set; } = default!;
}
