using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Businesses.Commands.Create;

public record CreateBusinessCommand(
    string Name,
    string Description,
    string Email,
    List<Guid> CategoryIds,
    BusinessOwnerCommand BusinessOwner,
    WeeklySchedule? WeeklySchedule,
    Address Address,
    List<PhoneNumber> PhoneNumbers,
    Image? Cover,
    Image? Logo,
    Guid? ReferringBusinessId = null) : IRequest<Result<Business>>;

public record BusinessOwnerCommand(
    Guid UserId,
    string Name,
    string Email,
    PhoneNumber PhoneNumber,
    WeeklySchedule WeeklySchedule,
    List<ServiceCommand> Services,
    Image? ProfileImage);

public record ServiceCommand(
    string Name,
    AmountDetails Amount,
    List<Duration> Durations);
