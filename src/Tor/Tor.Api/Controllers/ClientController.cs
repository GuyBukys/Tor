using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Tor.Api.Extensions;
using Tor.Application;
using Tor.Application.Clients.Commands.Create;
using Tor.Application.Clients.Commands.Deactivate;
using Tor.Application.Clients.Commands.UpdateFavoriteBusiness;
using Tor.Application.Clients.Commands.UpdatePersonalDetails;
using Tor.Application.Clients.Commands.UpdateProfileImage;
using Tor.Application.Clients.Queries.GetAppointments;
using Tor.Application.Clients.Queries.GetById;
using Tor.Contracts.Client;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ClientController
{
    private readonly ISender _mediator;
    private readonly IOutputCacheStore _cache;

    public ClientController(ISender mediator, IOutputCacheStore cache)
    {
        _mediator = mediator;
        _cache = cache;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateClientResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Create([FromBody] CreateClientCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<CreateClientResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(ClientResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetClientByIdQuery(id);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<ClientResponse>());
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Deactivate([FromQuery] Guid clientId, CancellationToken cancellationToken)
    {
        var command = new DeactivateClientCommand(clientId);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdatePersonalDetails([FromBody] UpdateClientPersonalDetailsCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateProfileImage([FromBody] UpdateClientProfileImageCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateFavoriteBusiness([FromBody] UpdateFavoriteBusinessCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetClientAppointmentsResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetClientAppointments(Guid clientId, CancellationToken cancellationToken)
    {
        var query = new GetClientAppointmentsQuery(clientId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetClientAppointmentsResponse>());
    }
}
