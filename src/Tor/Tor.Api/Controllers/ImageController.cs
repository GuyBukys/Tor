using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tor.Api.Extensions;
using Tor.Application.Images.Commands.Upload;
using Tor.Contracts.Images;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ImageController
{
    private readonly ISender _mediator;

    public ImageController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UploadImageResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UploadImage([FromBody] UploadImageCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<UploadImageResponse>());
    }
}
