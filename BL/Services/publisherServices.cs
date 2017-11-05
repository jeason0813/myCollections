using System.Collections;
using myCollections.Data.SqlLite;

namespace myCollections.BL.Services
{
    class PublisherServices
    {
        public static void CreatePublisher(string tableName, Publisher publisher)
        {
            Dal.GetInstance.AddPublisher(tableName,publisher);
        }
        public static void DeletePublisher(string entityId, string parentTable, string foreignKey, string childTable, string entityName)
        {
            Dal.GetInstance.UpdateMain(parentTable, entityId, foreignKey, null);
            Dal.GetInstance.DeleteById(childTable, "Name", entityName);
        }
        public static Publisher GetPublisher(string publisher, out bool isNew, string tableName)
        {
            isNew = false;
            if (string.IsNullOrWhiteSpace(publisher.Trim()) == false)
            {
                Publisher objStudio = Dal.GetInstance.GetPublisher(publisher.Trim(), tableName, "Name");
                if (objStudio == null)
                {
                    objStudio = new Publisher();
                    objStudio.Name = publisher.Trim();
                    isNew = true;
                }

                return objStudio;
            }

            return null;
        }
        public static IList GetPublishers(string tableName)
        {
            return Dal.GetInstance.GetPublishers(tableName);
        }
    }
}
