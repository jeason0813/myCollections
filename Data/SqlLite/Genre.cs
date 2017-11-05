
namespace myCollections.Data.SqlLite
{
    public class Genre
    {
        public string Id { get; set; }
        public string RealName { get; set; }
        public string DisplayName { get; set; }
        public byte[] Image { get; set; }
        public bool IsOld { get; set; }

        public Genre(string realName="",string displayName="")
        {
            RealName = realName;
            DisplayName = displayName;
        }

        public Genre()
        {
        }

        public override bool Equals(object obj)
        {
            Genre genre = obj as Genre;
            if (genre == null)
                return false;
            else if (Id == genre.Id)
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
