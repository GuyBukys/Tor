namespace Tor.Contracts.Client;

public class GetClientAppointmentsResponse
{
    public List<ClientAppointmentResponse> Appointments { get; set; } = [];
}
