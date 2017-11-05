
using System.Collections.Generic;
using myCollections.Data.SqlLite;
using System.IO;
using myCollections.Utils;
using myCollections.Pages;
using myCollections.BL.Imports;
namespace myCollections.BL.Services
{
    static class FriendServices
    {
        public static void Delete(Friends objFriend)
        {
            if (objFriend == null) return;

            Dal.GetInstance.DeleteById("Friends", "Id",objFriend.Id);
        }

        public static Friends Get(string strAlias)
        {
            return Dal.GetInstance.GetFriendByName(strAlias);
        }
        public static IEnumerable<Friends> Gets()
        {
            return Dal.GetInstance.GetFriends();
        }

        public static int ImportFromCsv(string filepath)
        {
            List<string> lines = new List<string>();

            using (StreamReader sr = new StreamReader(filepath, Util.GetFileEncoding(filepath)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    if (string.IsNullOrWhiteSpace(line) == false)
                        lines.Add(line);
            }

            int added = ImportsLines(lines);

            return added;
        }
        private static int ImportsLines(List<string> lines)
        {
            int added = 0;
            if (lines != null)
            {

                ProgressBar progressWindow = new ProgressBar(
                                               new ImportLoaners(lines));
                progressWindow.ShowDialog();
                added = progressWindow.AddedItem;

            }
            return added;
        }

        public static void SplitName(string strName, out string strFirstName, out string strLastName)
        {

            strFirstName = null;
            strLastName = null;

            if (string.IsNullOrEmpty(strName))
                return;

            strLastName = " ";

            foreach (string item in strName.Split(' '))
            {
                if (string.IsNullOrEmpty(strFirstName))
                    strFirstName = item;
                else
                    strLastName += item + " ";
            }
            if (strFirstName != null)
                strFirstName = strFirstName.Trim();

            strLastName = strLastName.Trim();
        }

        public static void Update(Friends objFriend)
        {
            Dal.GetInstance.AddFriend(objFriend);
        }
    }
}
