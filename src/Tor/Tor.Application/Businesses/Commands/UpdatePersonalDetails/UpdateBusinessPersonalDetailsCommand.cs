using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Businesses.Commands.UpdatePersonalDetails;

public record UpdateBusinessPersonalDetailsCommand(
    Guid BusinessId,
    string Name,
    string Description,
    string Email,
    List<PhoneNumber> PhoneNumbers,
    List<SocialMedia> SocialMedias) : IRequest<Result>;
