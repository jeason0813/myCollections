using System;

namespace myCollections.Data.SqlLite
{
    public class Friends
    {
        public string Id { get; set; }
        public string Alias { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Sex { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Adresse { get; set; }
        public string PhoneNumber { get; set; }
        public string EMail { get; set; }
        public int NbrCurrentLoan { get; set; }
        public int NbrMaxLoan { get; set; }
        public byte[] Picture { get; set; }
        public string Comments { get; set; }
        public bool IsOld { get; set; }
    }
}
