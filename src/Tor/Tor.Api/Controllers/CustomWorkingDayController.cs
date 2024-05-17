using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tor.Api.Extensions;
using Tor.Application.CustomWorkingDays.Commands.AddOrUpdate;
using Tor.Application.CustomWorkingDays.Queries.GetCustomWorkingDays;
using Tor.Contracts.StaffMember;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CustomWorkingDayController
{
    private readonly ISender _mediator;

    public CustomWorkingDayController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> AddOrUpdate([FromBody] AddOrUpdateCustomWorkingDayCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetCustomWorkingDaysResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetCustomWorkingDays(
        Guid staffMemberId,
        CancellationToken cancellationToken,
        DateOnly? from = null,
        DateOnly? until = null)
    {
        var query = new GetCustomWorkingDaysQuery(staffMemberId, from, until);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetCustomWorkingDaysResponse>());
    }
}
