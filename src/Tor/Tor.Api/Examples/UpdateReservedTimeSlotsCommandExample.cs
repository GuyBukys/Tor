using Swashbuckle.AspNetCore.Filters;
using Tor.Application.ReservedTimeSlots.Commands.Update;

namespace Tor.Api.Examples;

public class UpdateReservedTimeSlotsCommandExample : IExamplesProvider<UpdateReservedTimeSlotCommand>
{
    public UpdateReservedTimeSlotCommand GetExamples()
    {
        return new(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow),
            new(new TimeOnly(8, 0), new TimeOnly(10, 0)),
            "reason for break");
    }
}
