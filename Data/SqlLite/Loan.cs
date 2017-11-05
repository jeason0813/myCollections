using System;

namespace myCollections.Data.SqlLite
{
    public class Loan
    {
        public string Id { get; set; }
        public int ItemTypeId { get; set; }
        public string ItemId { get; set; }
        public Friends Friend { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime BackDate { get; set; }
        public bool IsBack { get; set; }
        public bool IsOld { get; set; }
    }
}
