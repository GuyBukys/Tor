using FluentResults;
using MediatR;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.StaffMembers.Commands.UpdatePersonalDetails;

public record UpdateStaffMemberPersonalDetailsCommand(
    Guid StaffMemberId,
    string Name,
    string? Description,
    string Email,
    DateOnly? BirthDate,
    PhoneNumber PhoneNumber,
    Address? Address) : IRequest<Result>;
