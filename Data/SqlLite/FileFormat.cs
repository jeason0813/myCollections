namespace myCollections.Data.SqlLite
{
    public class FileFormat
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public bool IsOld  {get; set; }
    }
}
