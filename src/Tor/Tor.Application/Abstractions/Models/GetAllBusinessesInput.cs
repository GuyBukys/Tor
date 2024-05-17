namespace Tor.Application.Abstractions.Models;

public record GetAllBusinessesInput(
    Guid ClientId,
    int Page,
    int PageSize,
    string? FreeText,
    string? SortOrder,
    string? SortColumn);
