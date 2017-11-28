using System;

namespace DlnaController.Abstractions
{
    public static class AppConstants
    {
        #region Cache Prefix

        public static string GetMediasCacheKey(string mediaType, string udn) => mediaType + "_" + udn;

        #endregion
    }
}
