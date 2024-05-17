using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tor.Api.Extensions;
using Tor.Application.StaffMembers.Commands.AddStaffMember;
using Tor.Application.StaffMembers.Commands.DeleteStaffMember;
using Tor.Application.StaffMembers.Commands.UpdatePersonalDetails;
using Tor.Application.StaffMembers.Commands.UpdateProfileImage;
using Tor.Application.StaffMembers.Commands.UpdateStaffMemberAddress;
using Tor.Application.StaffMembers.Commands.UpdateStaffMemberSettings;
using Tor.Application.StaffMembers.Commands.UpdateWeeklySchedule;
using Tor.Application.StaffMembers.Queries.GetById;
using Tor.Application.StaffMembers.Queries.GetSchedule;
using Tor.Contracts.StaffMember;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Infrastructure.Filters;

namespace Tor.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class StaffMemberController
{
    private readonly ISender _mediator;

    public StaffMemberController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetStaffMemberResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetStaffMemberByIdQuery(id);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetStaffMemberResponse>());
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddStaffMemberResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> AddStaffMember(AddStaffMemberCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<AddStaffMemberResponse>());
    }

    [HttpDelete]
    [Permission(PositionType.BusinessOwner)]
    public async Task<IResult> DeleteStaffMember(Guid staffMemberId, CancellationToken cancellationToken)
    {
        var command = new DeleteStaffMemberCommand(staffMemberId);

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
    public async Task<IResult> UpdateAddress(UpdateStaffMemberAddressCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> UpdateStaffMemberWeeklySchedule([FromBody] UpdateStaffMemberWeeklyScheduleCommand request, CancellationToken cancellationToken)
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
    public async Task<IResult> UpdatePersonalDetails([FromBody] UpdateStaffMemberPersonalDetailsCommand request, CancellationToken cancellationToken)
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
    public async Task<IResult> UpdateProfileImage([FromBody] UpdateStaffMemberProfileImageCommand request, CancellationToken cancellationToken)
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
    public async Task<IResult> UpdateStaffMemberSettings([FromBody] UpdateStaffMemberSettingsCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok();
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetScheduleResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IResult> GetSchedule(
        Guid staffMemberId,
        CancellationToken cancellationToken,
        DateTimeOffset? from = null,
        DateTimeOffset? until = null)
    {
        var query = new GetScheduleQuery(staffMemberId, from, until);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsFailed ?
            result.ToProblem() :
            Results.Ok(result.Value.Adapt<GetScheduleResponse>());
    }
}
