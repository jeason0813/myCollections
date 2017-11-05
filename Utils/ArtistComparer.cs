using System.Collections.Generic;
using myCollections.Data.SqlLite;

namespace myCollections.Utils
{
    class ArtistComparer : IEqualityComparer<Artist>
    {
        bool IEqualityComparer<Artist>.Equals(Artist x, Artist y)
        {
            if (x.Id == y.Id)
                return true;
            else
                return false;
        }

        int IEqualityComparer<Artist>.GetHashCode(Artist obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
