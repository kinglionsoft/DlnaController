using System;

namespace DlnaController.Abstractions
{
    public static class TimeSpanExtensions
    {
        public static string ToDuration(this TimeSpan ts)
        {
            string result;
            if (ts.Days > 0)
            {
                result = $"{ts.Days}天{ts.Hours}小时{ts.Minutes}分";
            }
            else if (ts.Hours > 0)
            {
                result = $"{ts.Hours}小时{ts.Minutes}分";
            }
            else
            {
                result = $"{ts.Minutes}分";
            }
            return result;
        }
    }
}