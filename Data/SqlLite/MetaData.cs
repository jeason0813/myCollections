namespace myCollections.Data.SqlLite
{
    public class MetaData
    {
        public string Id { get; set; }
        public string ItemId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public override bool Equals(object obj)
        {
            MetaData metaData = obj as MetaData;
            if (metaData == null)
                return false;
            else if (Id == metaData.Id && Name == metaData.Name && Category==metaData.Category)
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
