using myCollections.Utils;

namespace myCollections.Data.SqlLite
{
    public class Music : MyCollectionsData,IMyCollectionsData
    {
        public string Album { get; set; }
        public int? BitRate { get; set; }

        string IMyCollectionsData.CoverTheme
        {
            get { return MySettings.TvixThemeMusic; }
        }

        EntityType IMyCollectionsData.ObjectType
        {
            get { return EntityType.Music; }
        }

    }
}