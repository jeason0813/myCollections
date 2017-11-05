using System;
using System.Collections.ObjectModel;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Services
{
    static class LoanServices
    {
        public static Loan Get(string strItemId)
        {
            return Dal.GetInstance.GetLoanByItemId(strItemId);
        }

        public static int SetBackLoan(Collection<string> lstId)
        {
            foreach (string item in lstId)
            {
                Loan objLoan = Dal.GetInstance.GetLoanByItemId(item);
                objLoan.IsBack = true;
                objLoan.BackDate = DateTime.Now;
                Dal.GetInstance.AddLoan(objLoan);
            }

            return lstId.Count;
        }

        public static void Update(Loan objLoan, string strFriend, EntityType selectedItem)
        {
            objLoan.Friend = Dal.GetInstance.GetFriendByName(strFriend);
            objLoan.ItemTypeId = Dal.GetInstance.GetItemType(selectedItem.ToString());
            Dal.GetInstance.AddLoan(objLoan);
        }
    }
}
