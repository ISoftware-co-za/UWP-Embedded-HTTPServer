using System;

namespace ISoftware.UWP.HTMLLogViewer
{
    public static class Formatting
    {
        public static string Format(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss");
        }
    }
}
