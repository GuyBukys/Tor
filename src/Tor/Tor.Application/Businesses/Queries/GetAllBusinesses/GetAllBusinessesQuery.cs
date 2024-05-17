using FluentResults;
using MediatR;
using Tor.Application.Common.Models;

namespace Tor.Application.Businesses.Queries.GetAllBusinesses;

public record GetAllBusinessesQuery(
    Guid ClientId,
    int Page,
    int PageSize,
    string? FreeText,
    string? SortOrder,
    string? SortColumn) : IRequest<Result<PagedList<BusinessSummary>>>;