using Tor.Domain.CategoryAggregate;
using FluentResults;
using MediatR;

namespace Tor.Application.Categories.Queries.GetCategories;
public record GetCategoriesQuery() : IRequest<Result<IEnumerable<Category>>>;
