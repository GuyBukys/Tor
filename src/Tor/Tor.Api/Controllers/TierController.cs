using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tor.Api.Extensions;
using Tor.Application.Tiers.Commands.UpdateTier;
using Tor.Application.Tiers.Queries.ValidateTier;
using Tor.Contracts.Tier;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TierController
{
    private readonly ISender _mediator;

    public TierController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateTier(UpdateTierCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpGet]
    [ProducesResponseType(typeof(ValidateTierResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> ValidateTier(Guid staffMemberId, string? externalReference, CancellationToken cancellationToken)
    {
        ValidateTierQuery query = new(staffMemberId, externalReference);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<ValidateTierResponse>());
    }
}
