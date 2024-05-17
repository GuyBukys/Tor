using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tor.Api.Extensions;
using Tor.Application.Users.Commands.AddOrUpdateDevice;
using Tor.Application.Users.Commands.Create;
using Tor.Application.Users.Queries.Get;
using Tor.Contracts.User;
using Tor.Domain.UserAggregate.Enum;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController
{
    private readonly ISender _mediator;

    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Create([FromBody] CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<CreateUserResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetUserResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Get([FromQuery] string userToken, [FromQuery] AppType appType, CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(userToken, appType);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetUserResponse>());
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddOrUpdateDeviceResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> AddOrUpdateDevice([FromBody] AddOrUpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<AddOrUpdateDeviceResponse>());
    }
}
