namespace Tor.Contracts.Service;

public class GetServicesResponse
{
    public List<ServiceResponse> Services { get; set; } = new List<ServiceResponse>();
}
