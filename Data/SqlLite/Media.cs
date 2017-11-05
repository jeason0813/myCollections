using System;
using myCollections.Utils;

namespace myCollections.Data.SqlLite
{
    public class Media
    {
        public string Id { get; set; }
        public MediaType MediaType { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public byte[] Image { get; set; }
        public int? FreeSpace { get; set; }
        public int? TotalSpace { get; set; }
        public DateTime? LastUpdate { get; set; }
        public EntityType EntityType { get; set; }
        public string LastPattern { get; set; }
        public bool SearchSub { get; set; }
        public bool LocalImage { get; set; }
        public bool UseNfo { get; set; }
        public bool CleanTitle { get; set; }
        public bool IsOld { get; set; }
    }
}
