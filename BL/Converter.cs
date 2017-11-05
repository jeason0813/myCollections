using System.Collections;

namespace myCollections.BL
{
    static class Converter
    {
        public static string[] ToStringArray(IList objList)
        {
            string[] objResults = new string[objList.Count];
            int i = 0;
            foreach (string item in objList)
            {
                objResults[i] = item;
                i++;
            }
            return objResults;
        }
    }
}
