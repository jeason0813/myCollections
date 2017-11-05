using myCollections.Utils;
namespace myCollections.Data.SqlLite
{
    public class Gamez : MyCollectionsData,IMyCollectionsData
    {
        string IMyCollectionsData.CoverTheme
        {
            get { return MySettings.TvixThemeGames; }
        }

        EntityType IMyCollectionsData.ObjectType
        {
            get { return EntityType.Games; }
        }
        public int? Price { get; set; }

    }
}