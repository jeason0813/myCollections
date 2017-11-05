using System.Collections.Generic;
using myCollections.Data.Rest;

namespace myCollections.Utils
{
    public class AlloCineComparer : IEqualityComparer<AlloCine>
    {
        bool IEqualityComparer<AlloCine>.Equals(AlloCine x, AlloCine y)
        {
            if (x.Id == y.Id)
                return true;
            else
                return false;
        }

        int IEqualityComparer<AlloCine>.GetHashCode(AlloCine obj)
        {
            return obj.Id.GetHashCode();
        }
    }

}
