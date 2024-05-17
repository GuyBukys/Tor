using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;

namespace Tor.Application.CustomWorkingDays.Commands.AddOrUpdate;

public record AddOrUpdateCustomWorkingDayCommand(
    Guid StaffMemberId,
    CustomWorkingDay CustomWorkingDay) : IRequest<Result>;