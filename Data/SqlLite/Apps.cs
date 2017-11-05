using myCollections.Utils;
namespace myCollections.Data.SqlLite
{
    public class Apps : MyCollectionsData,IMyCollectionsData
    {
        EntityType IMyCollectionsData.ObjectType
        {
            get { return EntityType.Apps; }
        }
        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                if (value != _version)
                {
                    _version = value;
                    NotifyPropertyChanged("Version");
                }
            }
        }
       
    }
}