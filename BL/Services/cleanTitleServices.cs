using System.Collections.Generic;
using myCollections.Data.SqlLite;

namespace myCollections.BL.Services
{
    static class CleanTitleServices
    {
        public static void Add(CleanTitle objItem)
        {
            Dal.GetInstance.AddCleanTitle(objItem);
        }
        public static void Delete(CleanTitle item)
        {
            Dal.GetInstance.DeleteById("CleanTitle", "Id",item.Id);
        }
        public static IList<CleanTitle> GetAll()
        {
            return Dal.GetInstance.GetAllCleanTitle();
        }

    }
}
