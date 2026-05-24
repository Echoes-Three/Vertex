using System.Security.RightsManagement;

namespace Vertex.Data.Services;

public static class ValidateReminder
{
    public static (bool IsValid, string Message) Content(string title)
    {
        var isValid = !string.IsNullOrEmpty(title);
        var warning = isValid ? "" : "- Content must not be empty." ;
        
        return  (isValid, warning);
    }

    public static (bool IsValid, string Message) Hour(int hour ,int minute, string meridiem)
    {
        var hour24 = meridiem switch
        {
            "AM" => hour == 12 ? 0 : hour,
            "PM" => hour == 12 ? 12 : hour + 12,
                _=> hour
        };
        
        var correctHour = new DateTime(
            DateTime.Today.Year,
            DateTime.Today.Month,
            DateTime.Today.Day,
            hour24,
            minute,
            0
        );;
        
        var isValidHour = correctHour > DateTime.Now;
        var warning = isValidHour ? "" : $"- Picked hour must be greater than current time: {DateTime.Now:yyyy-MM-dd hh:mm tt}.";
        return (isValidHour, warning);
    }

    public static (bool IsValid, string Message) Date(DateTime? date)
    {
        var isValidDate = date != null;
        var warning = isValidDate ? "" : "- Date must not be empty." ;
        return (isValidDate, warning);
    }
}