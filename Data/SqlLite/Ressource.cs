
namespace myCollections.Data.SqlLite
{
    public class Ressource
    {
        public string Id { get; set; }
        public string ItemId { get; set; }
        public ResourcesType ResourcesType { get; set; }
        public string Link { get; set; }
        public byte[] Value { get; set; }
        public bool IsDefault { get; set; }
        public bool IsOld { get; set; }
    }
}
