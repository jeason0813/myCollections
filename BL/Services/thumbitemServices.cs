using myCollections.Data;
namespace myCollections.BL.Services
{
    static class ThumbitemServices
    {
        public static bool WithoutCover(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.HasCover == false && thumitem.Deleted == false))
                return true;
            else
                return false;
        }
        public static bool WithCover(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.HasCover == true && thumitem.Deleted == false))
                return true;
            else
                return false;
        }
        public static bool ViewAll(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            return thumitem != null && !thumitem.Deleted;
        }
        public static bool ViewToBeDeleted(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.ToBeDeleted == true && thumitem.Deleted == false))
                return true;
            else
                return false;
        }
        public static bool ViewDeleted(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && thumitem.Deleted == true)
                return true;
            else
                return false;
        }
        public static bool ViewToBuy(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.ToBuy == true && thumitem.Deleted == false))
                return true;
            else
                return false;
        }
        public static bool ViewComplete(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.IsComplete == true && thumitem.Deleted == false))
                return true;
            else
                return false;
        }
        public static bool ViewNotComplete(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.IsComplete == false && thumitem.Deleted == false))
                return true;
            else
                return false;
        }
        public static bool ViewSeen(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.Seen == true && thumitem.Deleted == false))
                return true;
            else
                return false;
        }
        public static bool ViewNotSeen(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.Seen == false && thumitem.Deleted == false))
                return true;
            else
                return false;
        }
        public static bool ToWatch(object item)
        {
            ThumbItem thumitem = item as ThumbItem;
            if (thumitem != null && (thumitem.ToWatch == true 
                                        && thumitem.Deleted == false))
                return true;
            else
                return false;
        } 
    }
}
