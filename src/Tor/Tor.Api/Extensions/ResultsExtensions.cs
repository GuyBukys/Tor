using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tor.Application.Common.Errors.ErrorTypes;

namespace Tor.Api.Extensions;

public static class ResultExtensions
{
    private static readonly string _generalErrorMessage = "זה לא אתה זה אנחנו! נתקלנו בבעיה זמנית. אנא נסה שנית מאוחר יותר";

    public static IResult ToProblem(this IResultBase failedResult)
    {
        if (failedResult.Reasons.Any(x => x.GetType() == typeof(CustomError)))
        {
            return Results.Problem(new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = failedResult.Reasons.First().Message,
            });
        }

        if (failedResult.Reasons.All(x => x.GetType() == typeof(ValidationError)))
        {
            IDictionary<string, string[]> errors = failedResult.Errors
                .Cast<ValidationError>()
                .ToDictionary(k => k.PropertyName, v => new string[] { v.Message });

            return Results.ValidationProblem(errors, title: _generalErrorMessage);
        }

        HttpStatusCode statusCode = failedResult.Reasons.First() switch
        {
            NotFoundError => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        string reasons = string.Join(' ', failedResult.Reasons.Select(x => x.Message));
        return Results.Problem(new ProblemDetails
        {
            Status = (int)statusCode,
            Detail = reasons,
            Title = _generalErrorMessage,
        });
    }
}
