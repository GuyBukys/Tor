using FluentResults;
using MediatR;
using Tor.Domain.CategoryAggregate.Enums;

namespace Tor.Application.Services.Queries.GetDefaultServices;

public record GetDefaultServicesQuery(CategoryType Category) : IRequest<Result<List<DefaultService>>>;