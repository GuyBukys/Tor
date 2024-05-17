using Geolocation;
using Microsoft.AspNetCore.Mvc;
using Tor.Application.Abstractions;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class GeolocationController
{
    private readonly IMapsService _service;

    public GeolocationController(IMapsService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<IResult> Autocomplete([FromQuery] string input, CancellationToken cancellationToken)
    {
        var predictions = await _service.AutocompleteAddress(input, cancellationToken);

        return Results.Ok(predictions);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Coordinate), 200)]
    public async Task<IResult> GetByAddress([FromQuery] string address, CancellationToken cancellationToken)
    {
        var coordinate = await _service.GetByAddress(address, cancellationToken);

        return Results.Ok(coordinate);
    }
}
