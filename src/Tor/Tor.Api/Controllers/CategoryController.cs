using Tor.Api.Extensions;
using Tor.Application.Categories.Queries.GetCategories;
using Tor.Contracts.Category;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CategoryController
{
    private readonly ISender _mediator;

    public CategoryController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetCategoriesResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetCategoriesResponse>());
    }
}
