using System.Collections;
using System.Collections.Generic;
using System.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Services
{
    class MetaDataServices
    {
        public static void Add(MetaData objItem)
        {
            Dal.GetInstance.AddMetaData(objItem);
        }
        public static IList Get(EntityType entityType)
        {
            return Dal.GetInstance.GetMetaDataName(entityType);
        }
        public static IList<MetaData> GetAll()
        {
            return Dal.GetInstance.GetAllMetaDatas();
        }
        public static void Link(IMyCollectionsData entity, IEnumerable<string> metadas)
        {
            if (metadas.Any())
            {
                foreach (string metadata in metadas)
                {
                    string metaDataId = Dal.GetInstance.GetMetaDataId(metadata, entity.ObjectType);
                    if (string.IsNullOrWhiteSpace(metaDataId) == false)
                        Dal.GetInstance.LinkMetaData(entity, metaDataId);
                }

            }
        }

        public static void Delete(MetaData item)
        {
            Dal.GetInstance.DeleteById("MetaData", "Id", item.Id);
        }
    }
}
