using myCollections.Utils;
namespace myCollections.Data.SqlLite
{
    public class XXX : MyCollectionsData,IMyCollectionsData
    {
        string IMyCollectionsData.CoverTheme
        {
            get { return MySettings.TvixThemeXXX; }
        }

        EntityType IMyCollectionsData.ObjectType
        {
            get { return EntityType.XXX; }
        }
       
    }

}