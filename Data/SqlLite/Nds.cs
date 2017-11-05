using myCollections.Utils;
namespace myCollections.Data.SqlLite
{
    public class Nds : MyCollectionsData,IMyCollectionsData
    {
        EntityType IMyCollectionsData.ObjectType
        {
            get { return EntityType.Nds; }
        }
    }
}