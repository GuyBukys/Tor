using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tor.Api.Extensions;
using Tor.Application.ReservedTimeSlots.Commands.Add;
using Tor.Application.ReservedTimeSlots.Commands.Delete;
using Tor.Application.ReservedTimeSlots.Commands.Update;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ReservedTimeSlotController
{
    private readonly ISender _mediator;

    public ReservedTimeSlotController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Update(UpdateReservedTimeSlotCommand request, CancellationToken cancellationToken)
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
    public async Task<IResult> Add(AddReservedTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpDelete]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteReservedTimeSlotCommand(id);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }
}
