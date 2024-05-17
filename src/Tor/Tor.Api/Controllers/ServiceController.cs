using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tor.Api.Extensions;
using Tor.Application.Services.Commands.AddService;
using Tor.Application.Services.Commands.DeleteService;
using Tor.Application.Services.Commands.UpdateService;
using Tor.Application.Services.Queries.GetDefaultServices;
using Tor.Application.Services.Queries.GetServices;
using Tor.Contracts.Service;
using Tor.Domain.CategoryAggregate.Enums;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ServiceController
{
    private readonly ISender _mediator;

    public ServiceController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetServicesResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetServices([FromQuery] Guid staffMemberId, CancellationToken cancellationToken)
    {
        var query = new GetServicesQuery(staffMemberId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetServicesResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetDefaultServicesResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetDefaultServices([FromQuery] CategoryType category, CancellationToken cancellationToken)
    {
        var query = new GetDefaultServicesQuery(category);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetDefaultServicesResponse>());
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddServiceResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> AddService([FromBody] AddServiceCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(new AddServiceResponse { ServiceId = result.Value });
    }

    [HttpPut]
    [ProducesResponseType(typeof(ServiceResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateService([FromBody] UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<ServiceResponse>());
    }

    [HttpDelete]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> DeleteService([FromQuery] Guid serviceId, CancellationToken cancellationToken)
    {
        var command = new DeleteServiceCommand(serviceId);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

}
