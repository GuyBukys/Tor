using FluentResults;
using MediatR;
using Tor.Domain.BusinessAggregate.Enums;
using Tor.Domain.BusinessAggregate.ValueObjects;
using Tor.Domain.CategoryAggregate.Enums;

namespace Tor.Application.Services.Queries.GetDefaultServices;

internal sealed class GetDefaultServicesQueryHandler : IRequestHandler<GetDefaultServicesQuery, Result<List<DefaultService>>>
{
    private static readonly Dictionary<CategoryType, List<DefaultService>> _servicesToCategory = new()
    {
        {
            CategoryType.Barbershop,
            [
                new DefaultService("תספורת", new(50, "ILS"), [new Duration(1, 60, DurationType.Work)]),
                new DefaultService("תספורת + זקן", new(60, "ILS"), [new Duration(1, 60, DurationType.Work)]),
            ]
        },
        {
            CategoryType.NailSalon,
            [
                new DefaultService("בניית ציפורניים באקריל", new(130, "ILS"), [new Duration(1, 60, DurationType.Work)]),
            ]
        },
        {
            CategoryType.Makeup,
            [
                new DefaultService("איפור ערב", new(250, "ILS"), [new Duration(1, 60, DurationType.Work)]),
            ]
        },
        {
            CategoryType.Massage,
            [
                new DefaultService("עיסוי רפואי", new(250, "ILS"), [new Duration(1, 45, DurationType.Work)]),
            ]
        },
        {
            CategoryType.EyebrowsAndLashes,
            [
                new DefaultService("מיקרובליידינג", new(700, "ILS"), [new Duration(1, 90, DurationType.Work)]),
                new DefaultService("הרמת ריסים", new(200, "ILS"), [new Duration(1, 45, DurationType.Work)]),
            ]
        },
        {
            CategoryType.PersonalTrainer,
            [
                new DefaultService("אימון אישי", new(150, "ILS"), [new Duration(1, 60, DurationType.Work)]),
            ]
        },
        {
            CategoryType.Piercing,
            [
                new DefaultService("הליקס", new(120, "ILS"), [new Duration(1, 10, DurationType.Work)]),
                new DefaultService("טראגוס", new(150, "ILS"), [new Duration(1, 15, DurationType.Work)]),
            ]
        },
        {
            CategoryType.PetServices,
            [
                new DefaultService("תספורת לכלב", new(150, "ILS"), [new Duration(1, 90, DurationType.Work)]),
            ]
        },
        {
            CategoryType.HairSalon,
            [
                new DefaultService("החלקה אורגנית", new(600, "ILS"), [new Duration(1, 180, DurationType.Work)]),
            ]
        },
        {
            CategoryType.PrivateTutor,
            [
                new DefaultService("שיעור פרטי", new(100, "ILS"), [new Duration(1, 60, DurationType.Work)]),
            ]
        },
        {
            CategoryType.CosmeticsAndBeauty,
            [
                new DefaultService("טיפול פנים בסיסי", new(350, "ILS"), [new Duration(1, 90, DurationType.Work)]),
            ]
        },
    };

    public Task<Result<List<DefaultService>>> Handle(GetDefaultServicesQuery request, CancellationToken cancellationToken)
    {
        return !_servicesToCategory.TryGetValue(request.Category, out List<DefaultService>? defaultServices)
            ? Task.FromResult(Result.Ok(new List<DefaultService>()))
            : Task.FromResult(Result.Ok(defaultServices));
    }
}
