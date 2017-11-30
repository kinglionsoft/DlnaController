using System;

namespace DlnaController.Abstractions
{
    public static class NumberExtensions
    {
        /// <summary>
        /// print sizes in powers of 1024 (e.g., 1023M)
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ToHumanString(this double size)
        {
            var k = Math.Floor(size / 1024);
            if (k < 1)
            {
                return $"{size:N0}B";
            }
            var m = Math.Floor(size / 1024 / 1024);
            if (m < 1)
            {
                return $"{k}KB";
            }
            if (m < 1024)
            {
                return $"{m}MB";
            }
            return $"{(m / 1024.0):N2}GB";
        }

        /// <summary>
        /// print sizes in powers of 1024 (e.g., 1023M)
        /// </summary>
        public static string ToHumanString(this ulong size) => ToHumanString((double) size);
    }
}