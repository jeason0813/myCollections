using System.Collections.Generic;
using myCollections.Data.SqlLite;

namespace myCollections.Utils
{
    class LanguageComparer : IEqualityComparer<Language>
    {
        public bool Equals(Language x, Language y)
        {
            return x.DisplayName == y.DisplayName;
            
        }

        public int GetHashCode(Language obj)
        {
            return obj.DisplayName.GetHashCode();
        }
    }
}
