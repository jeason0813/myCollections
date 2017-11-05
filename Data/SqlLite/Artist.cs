using System;
using System.Collections.Generic;

namespace myCollections.Data.SqlLite
{
    public class Artist
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDay { get; set; }
        public byte[] Picture { get; set; }
        public string Bio { get; set; }
        public string PlaceBirth { get; set; }
        public string WebSite { get; set; }
        public string YearsActive { get; set; }
        public string Ethnicity { get; set; }
        public string Breast { get; set; }
        public string FulleName { get; set; }
        public string Aka { get; set; }
        public bool Sex { get; set; }
        public Job Job { get; set; }
        public DateTime Added { get; set; }
        public bool IsOld { get; set; }

        public IList<ArtistCredits> ArtistCredits { get; set; }
       
    }
}
