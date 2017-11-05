
namespace myCollections.Data.SqlLite
{
    public class Audio
    {
        public string Id { get; set; }
        public AudioType AudioType { get; set; }
        public Language Language { get; set; }
        public bool IsOld { get; set; }

    }
}
