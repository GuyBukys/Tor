using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tor.Api.Extensions;
using Tor.Application.WaitingLists.Commands.JoinWaitingList;
using Tor.Application.WaitingLists.Queries.GetByClient;
using Tor.Contracts.WaitingList;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class WaitingListController
{
    private readonly ISender _mediator;

    public WaitingListController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> JoinWaitingList(JoinWaitingListCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetWaitingListsResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetByClient(Guid clientId, CancellationToken cancellationToken)
    {
        var query = new GetWaitingListsByClientQuery(clientId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetWaitingListsResponse>());
    }
}
