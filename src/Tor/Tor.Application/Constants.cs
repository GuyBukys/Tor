namespace Tor.Application;

public static class Constants
{
    public const string ServiceName = "Tor";
    public static readonly TimeSpan FreeTrialDuration = TimeSpan.FromDays(30);

    public static class Groups
    {
        public const string BusinessesNotificationGroup = "businesses";
        public const string ClientsNotificationGroup = "clients";
    }

    public static class Cache
    {
        public static class Tags
        {
            public const string GetAllBusinessesTag = "GetAllBusinesses";
        }
    }

    public static class MessageBlasts
    {
        public const string ScheduleAppointmentReminderName = "ScheduleAppointmentReminder";
        public const string ScheduleAppointmentReminderDisplayName = "תזכורת לקביעת תור";
    }
    internal static class ErrorMessages
    {
        public const string AppointmentTaken = "לצערנו התור נתפס :( יש לנסות מועד אחר";
        public const string CannotCancelAppointment = "לצערנו, כבר לא ניתן לבטל את התור";
        public const string ExceededMaximumAppointments = "עברת את מספר התורים המוגבל שאפשר לקבוע לעסק זה";
    }
}
