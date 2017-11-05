

namespace myCollections.Data.SqlLite
{
    public class Publisher
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public bool IsOld { get; set; }

        public override bool Equals(object obj)
        {
            Publisher publisher = obj as Publisher;
            if (publisher == null)
                return false;
            else if (Name == publisher.Name)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
