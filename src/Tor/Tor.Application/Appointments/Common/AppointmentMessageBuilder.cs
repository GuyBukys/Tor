using System.Text;
using Tor.Application.Common.Extensions;

namespace Tor.Application.Appointments.Common;

internal static class AppointmentMessageBuilder
{
    public static string BuildClientAppointmentScheduledMessage(string businessName, DateTimeOffset scheduledFor, string? notes)
    {
        string date = DateOnly.FromDateTime(scheduledFor.ToIsraelTime()).ToString("dd/MM/yyyy");
        string time = TimeOnly.FromDateTime(scheduledFor.ToIsraelTime()).ToShortTimeString();

        string message = $"תודה שקבעת אצלנו תור ב{businessName}! התור נקבע לתאריך {date} בשעה {time}. מתרגשים לראותך :)";

        if (!string.IsNullOrEmpty(notes))
        {
            message += $"הערות: {notes}";
        }

        return message;
    }

    public static string BuildStaffMemberAppointmentScheduledMessage(DateTimeOffset scheduledFor, string clientName)
    {
        string date = DateOnly.FromDateTime(scheduledFor.ToIsraelTime()).ToString("dd/MM/yyyy");
        string time = TimeOnly.FromDateTime(scheduledFor.ToIsraelTime()).ToShortTimeString();

        string message = $"נקבע אליך תור לתאריך {date} בשעה {time}. שם הלקוח: {clientName}";

        return message;
    }

    public static string BuildClientAppointmentCanceledMessage(DateTimeOffset scheduledFor, string serviceName, string? reason)
    {
        string date = DateOnly.FromDateTime(scheduledFor.ToIsraelTime()).ToString("dd/MM/yyyy");
        string time = TimeOnly.FromDateTime(scheduledFor.ToIsraelTime()).ToShortTimeString();

        string message = $"התור שקבעת אצלנו ל{serviceName} בתאריך {date} בשעה {time} בוטל.";

        if (!string.IsNullOrEmpty(reason))
        {
            message += $"סיבה: {reason}";
        }

        return message;
    }

    public static string BuildStaffMemberAppointmentCanceledMessage(DateTimeOffset scheduledFor, string clientName)
    {
        string date = DateOnly.FromDateTime(scheduledFor.ToIsraelTime()).ToString("dd/MM/yyyy");
        string time = TimeOnly.FromDateTime(scheduledFor.ToIsraelTime()).ToShortTimeString();

        string message = $"התבטל תור שנקבע אליך בתאריך {date} בשעה {time}. שם הלקוח: {clientName}";

        return message;
    }

    public static string BuildWaitingListMessage(string serviceName, DateOnly date)
    {
        string dateAsString = date.ToString("dd/MM/yyyy");

        return $"התפנה מקום ברשימת המתנה לשירות {serviceName} בתאריך {dateAsString}. יש להיכנס עכשיו בכדי לקבוע תור לפני שייתפס שוב :)";
    }

    public static string BuildAppointmentReminderMessage(
        string clientName,
        string serviceName,
        DateTime scheduledFor,
        string? notes)
    {
        DateOnly date = DateOnly.FromDateTime(scheduledFor);
        TimeOnly time = TimeOnly.FromDateTime(scheduledFor);

        StringBuilder builder = new();

        builder.AppendFormat("היי {0}. ", clientName);
        builder.AppendFormat("טרם אישרת הגעה לתור שנקבע לך ל{0} ", serviceName);

        if (date == DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
        {
            builder.Append("מחר ");
        }
        else if (date == DateOnly.FromDateTime(DateTime.UtcNow))
        {
            builder.Append("היום ");
        }
        else
        {
            builder.AppendFormat("בתאריך {0} ", date);
        }

        builder.AppendFormat("בשעה {0}. ", time);
        builder.Append("יש ללחוץ כאן בכדי לאשר הגעה. ");

        builder.Append("מתרגשים לראותך :) ");

        if (!string.IsNullOrEmpty(notes))
        {
            builder.AppendFormat("הערות: {0}. ", notes);
        }

        return builder.ToString();
    }
}
