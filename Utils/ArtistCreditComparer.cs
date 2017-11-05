using System.Collections.Generic;
using myCollections.Data.SqlLite;

namespace myCollections.Utils
{
    class ArtistCreditComparer : IEqualityComparer<ArtistCredits>
    {
        bool IEqualityComparer<ArtistCredits>.Equals(ArtistCredits x, ArtistCredits y)
        {
            if (x.Title.Trim().ToUpper()== y.Title.Trim().ToUpper())
                return true;
            else
                return false;
        }

        int IEqualityComparer<ArtistCredits>.GetHashCode(ArtistCredits artist_credits)
        {
            //Check whether the object is null
            if (ReferenceEquals(artist_credits, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashTitle = artist_credits.Title == null ? 0 : artist_credits.Title.GetHashCode();

            
            //Calculate the hash code for the product.
            return hashTitle;
        }
    }
}
