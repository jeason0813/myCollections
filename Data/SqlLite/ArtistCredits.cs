using System;
using myCollections.Utils;

namespace myCollections.Data.SqlLite
{
    public class ArtistCredits
    {
        public string Id { get; set; }
        public string ArtistId { get; set; }
        public EntityType EntityType { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string BuyLink { get; set; }
        public bool IsOld { get; set; }
    }
}
