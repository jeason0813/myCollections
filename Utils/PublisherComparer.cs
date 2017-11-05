using System.Collections.Generic;
using myCollections.Data.SqlLite;

namespace myCollections.Utils
{
    class PublisherComparer : IEqualityComparer<Publisher>
    {
        public bool Equals(Publisher x, Publisher y)
        {
            return x.Name.Trim().ToUpper() == y.Name.Trim().ToUpper();

        }

        public int GetHashCode(Publisher obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
