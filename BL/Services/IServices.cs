using myCollections.Data.SqlLite;
using System.Collections;
using System.Collections.Generic;

namespace myCollections.BL.Services
{

    public interface IServices
    {
        void Add(IMyCollectionsData objItem);
        
        IMyCollectionsData Get(string id);
        IList GetAll();
        int GetCountByType(string type);
        IMyCollectionsData GetFirst();
        IList GetItemTypes(IEnumerable<string> thumbItem);
        IList GetByMedia(string mediaName);
        IList GetTypesName();
        void GetInfoFromWeb(IMyCollectionsData objEntity);

        int ImportFromXml(string filepath);
    }
}
