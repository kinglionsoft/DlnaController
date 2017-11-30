using System.Collections.Generic;

namespace SV.UPnPLite.Core
{
    public class MediaItemComparer<TMedia> : IEqualityComparer<TMedia> where TMedia : MediaItem
    {
        public bool Equals(TMedia x, TMedia y)
        {
            if (x == null || y == null) return false;
            return x.Title.Equals(y.Title);
        }

        public int GetHashCode(TMedia obj)
        {
            if (!string.IsNullOrEmpty(obj.Title)) return obj.Title.GetHashCode();
            return obj.GetHashCode();
        }
    }
}