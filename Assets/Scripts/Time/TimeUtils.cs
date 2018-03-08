
using System;

public static class TimeUtils
{
    public static string TimeAgo(this DateTime dateTime)
    {
        string result = string.Empty;
        var timeSpan = DateTime.Now.Subtract(dateTime);

        if (timeSpan <= TimeSpan.FromSeconds(60))
        {
            result = string.Format("{0} seconds ago", timeSpan.Seconds);
        }
        else if (timeSpan <= TimeSpan.FromMinutes(60))
        {
            result = timeSpan.Minutes > 1 ?
                String.Format("About {0} minutes ago", timeSpan.Minutes) :
                "about a minute ago";
        }
        else if (timeSpan <= TimeSpan.FromHours(24))
        {
            result = timeSpan.Hours > 1 ?
                String.Format("About {0} hours ago", timeSpan.Hours) :
                "About an hour ago";
        }
        else if (timeSpan <= TimeSpan.FromDays(30))
        {
            result = timeSpan.Days > 1 ?
                String.Format("About {0} days ago", timeSpan.Days) :
                "Yesterday";
        }
        else if (timeSpan <= TimeSpan.FromDays(365))
        {
            result = timeSpan.Days > 30 ?
                String.Format("About {0} months ago", timeSpan.Days / 30) :
                "About a month ago";
        }
        else
        {
            result = timeSpan.Days > 365 ?
                String.Format("About {0} years ago", timeSpan.Days / 365) :
                "About a year ago";
        }

        return result;
    }
}