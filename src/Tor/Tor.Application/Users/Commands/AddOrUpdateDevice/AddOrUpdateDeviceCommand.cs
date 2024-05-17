using FluentResults;
using MediatR;
using Tor.Domain.UserAggregate.ValueObjects;

namespace Tor.Application.Users.Commands.AddOrUpdateDevice;

public record AddOrUpdateDeviceCommand(string UserToken, string DeviceToken) : IRequest<Result<List<Device>>>;
