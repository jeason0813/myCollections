using System.Collections.Generic;
using System.Linq;
using myCollections.Data.SqlLite;

namespace myCollections.BL.Services
{
    class SubTitleServices
    {
        public static void Add(IEnumerable<string> subs, IMyCollectionsData objEntity)
        {
            foreach (string item in subs)
            {
                string sub = item.Trim();
                if (string.IsNullOrWhiteSpace(sub) == false)
                {

                    Language lang = LanguageServices.GetLanguage(sub, true);

                    bool bFind = objEntity.Subtitles.Any(movieSub => movieSub.DisplayName == lang.DisplayName);

                    if (bFind == false)
                    {
                        lang = new Language();
                        lang.ShortName = sub;
                        lang.DisplayName = sub;

                        objEntity.Subtitles.Add(lang);
                    }
                }
            }
        }

    }
}
