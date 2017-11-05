using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using myCollections.Data;

namespace myCollections.Utils
{
    class PartialMatchComparer : IEqualityComparer<PartialMatche>
    {
        bool IEqualityComparer<PartialMatche>.Equals(PartialMatche x, PartialMatche y)
        {
            if ((x.Title == y.Title) && (x.Artist==y.Artist))
                return true;
            else
                return false;
        }

        int IEqualityComparer<PartialMatche>.GetHashCode(PartialMatche obj)
        {
            return obj.Link.GetHashCode();
        }
    }
}
