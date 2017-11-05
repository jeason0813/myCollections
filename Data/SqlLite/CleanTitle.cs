

namespace myCollections.Data.SqlLite
{
    public class CleanTitle
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string Category { get; set; }

        public override bool Equals(object obj)
        {
            CleanTitle cleanTitle = obj as CleanTitle;
            if (cleanTitle == null)
                return false;
            else if (Id == cleanTitle.Id && Value == cleanTitle.Value && Category == cleanTitle.Category)
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
