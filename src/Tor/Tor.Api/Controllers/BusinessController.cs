using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Tor.Api.Extensions;
using Tor.Application;
using Tor.Application.Businesses.Commands.Block;
using Tor.Application.Businesses.Commands.Create;
using Tor.Application.Businesses.Commands.Deactivate;
using Tor.Application.Businesses.Commands.SetDefaultImage;
using Tor.Application.Businesses.Commands.Unblock;
using Tor.Application.Businesses.Commands.UpdateAddress;
using Tor.Application.Businesses.Commands.UpdateHomepageNote;
using Tor.Application.Businesses.Commands.UpdateImages;
using Tor.Application.Businesses.Commands.UpdatePersonalDetails;
using Tor.Application.Businesses.Commands.UpdateSettings;
using Tor.Application.Businesses.Commands.UpdateWeeklySchedule;
using Tor.Application.Businesses.Queries.CanAddStaffMember;
using Tor.Application.Businesses.Queries.GetAllBusinesses;
using Tor.Application.Businesses.Queries.GetAllClients;
using Tor.Application.Businesses.Queries.GetAppointmentsByClient;
using Tor.Application.Businesses.Queries.GetById;
using Tor.Application.Businesses.Queries.GetByInvitation;
using Tor.Application.Businesses.Queries.GetByReferralCode;
using Tor.Application.Businesses.Queries.GetByStaffMember;
using Tor.Application.Businesses.Queries.IsBusinessExists;
using Tor.Contracts.Business;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BusinessController
{
    private readonly ISender _mediator;
    private readonly IOutputCacheStore _cache;

    public BusinessController(ISender mediator, IOutputCacheStore cache)
    {
        _mediator = mediator;
        _cache = cache;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateBusinessResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Create([FromBody] CreateBusinessCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<CreateBusinessResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetBusinessResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetByStaffMember([FromQuery] Guid staffMemberId, CancellationToken cancellationToken)
    {
        var query = new GetByStaffMemberQuery(staffMemberId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetBusinessResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetAppointmentsByClientResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetAppointmentsByClient(Guid businessId, Guid clientId, CancellationToken cancellationToken)
    {
        var query = new GetAppointmentsByClientQuery(businessId, clientId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetAppointmentsByClientResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetByReferralCodeResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetByReferralCode([FromQuery] string referralCode, CancellationToken cancellationToken)
    {
        var query = new GetByReferralCodeQuery(referralCode);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetByReferralCodeResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetBusinessResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetByInvitation([FromQuery] string invitationId, CancellationToken cancellationToken)
    {
        var query = new GetByInvitationQuery(invitationId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetBusinessResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetBusinessResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdQuery(id);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetBusinessResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetAllBusinessesResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetAll(
        Guid clientId,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = int.MaxValue,
        string? freeText = null,
        string? sortOrder = null,
        string? sortColumn = null)
    {
        var query = new GetAllBusinessesQuery(clientId, page, pageSize, freeText, sortOrder, sortColumn);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetAllBusinessesResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetAllClientsResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetAllClients(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAllClientsQuery(id);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetAllClientsResponse>());
    }

    [HttpGet]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> IsBusinessExists(string email, CancellationToken cancellationToken)
    {
        var query = new IsBusinessExistsQuery(email);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> CanAddStaffMember(Guid businessId, CancellationToken cancellationToken)
    {
        var query = new CanAddStaffMemberQuery(businessId);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Deactivate([FromQuery] Guid businessId, CancellationToken cancellationToken)
    {
        var command = new DeactivateBusinessCommand(businessId);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

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
    public async Task<IResult> UpdateAddress(UpdateBusinessAddressCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdatePersonalDetails(UpdateBusinessPersonalDetailsCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateSettings(UpdateBusinessSettingsCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [Obsolete("will remove after weekly schedule move to staff member")]
    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateBusinessWeeklySchedule([FromBody] UpdateBusinessWeeklyScheduleCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateHomepageNote([FromBody] UpdateHomepageNoteCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateImages([FromBody] UpdateBusinessImagesCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> SetDefaultImage([FromBody] SetBusinessDefaultImageCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Block([FromBody] BlockClientCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> Unblock([FromBody] UnblockClientCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        await _cache.EvictByTagAsync(Constants.Cache.Tags.GetAllBusinessesTag, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }
}
