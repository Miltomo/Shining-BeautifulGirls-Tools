using System;

namespace MHTools
{
    public static class TimeTool
    {
        public static string FormatMS(int milliseconds, bool inEnglish = false)
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);

            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            string f = $"{seconds}{(inEnglish ? "s" : "秒")}";
            if (minutes > 0)
                f = $"{minutes}{(inEnglish ? "m" : "分")}" + f;
            if (hours > 0)
                f = $"{hours}{(inEnglish ? "h" : "时")}" + f;

            return f;
        }
    }
}
