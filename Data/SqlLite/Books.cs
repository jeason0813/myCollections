using myCollections.Utils;
namespace myCollections.Data.SqlLite
{
    public class Books : MyCollectionsData, IMyCollectionsData
    {
        private string _isbn;
        public string Isbn
        {
            get { return _isbn; }
            set
            {
                if (value != _isbn)
                {
                    _isbn = value;
                    NotifyPropertyChanged("Isbn");
                }
            }
        }
       
        private int _nbrPages;
        public int NbrPages
        {
            get { return _nbrPages; }
            set
            {
                if (value != _nbrPages)
                {
                    _nbrPages = value;
                    NotifyPropertyChanged("NbrPages");
                }
            }
        }

        EntityType IMyCollectionsData.ObjectType
        {
            get { return EntityType.Books; }
        }
       
    }
}