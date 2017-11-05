using System.Collections.Generic;
using System.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Services
{
    static class LanguageServices
    {
        public static void Add(Language objItem)
        {
            Dal.GetInstance.AddLanguage(objItem);
        }
        public static Language GetLanguage(string strLanguage, bool save)
        {
            if (string.IsNullOrEmpty(strLanguage))
                return null;

            Language objLanguage = Dal.GetInstance.GetLanguageByShortName(strLanguage);
            if (objLanguage == null || string.IsNullOrWhiteSpace(objLanguage.Id))
            {
                objLanguage = new Language();
                objLanguage.ShortName= strLanguage;
                objLanguage.DisplayName = strLanguage;
                if (save==true)
                    Dal.GetInstance.AddLanguage(objLanguage);
            }

            return objLanguage;
        }
        public static IEnumerable<Language> GetLanguages()
        {
            return Dal.GetInstance.GetLanguages().Distinct(new LanguageComparer()).ToList();
        }
        public static IList<Language> GetAllLanguages()
        {
            return Dal.GetInstance.GetAllLanguages();
        }

        public static void Delete(Language item)
        {
            Dal.GetInstance.DeleteById("Language", "Id",item.Id);
        }
       
    }
}
