using System.Collections.Generic;
using System.Linq;
using myCollections.Data.SqlLite;

namespace myCollections.BL.Services
{
    class AudioServices
    {
        public static void Add(IEnumerable<Audio> audios, IMyCollectionsData objEntity)
        {

            foreach (Audio item in audios)
            {
                if (item != null)
                {
                    bool bFind = false;
                    for (int i = 0; i < objEntity.Audios.Count; i++)
                    {

                        Audio movieAudio = objEntity.Audios.ElementAt(i);
                        if (movieAudio.Language.DisplayName == item.Language.DisplayName && movieAudio.AudioType.Name == item.AudioType.Name)
                        {
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind == false)
                    {
                        if (objEntity.Audios.Count > 0)
                            foreach (Audio audio in objEntity.Audios)
                            {
                                if (audio.Language.DisplayName == item.Language.DisplayName && audio.AudioType.Name == item.AudioType.Name)
                                {
                                    bFind = true;
                                    break;
                                }
                            }
                    }
                    if (bFind == false)
                        objEntity.Audios.Add(item);
                }
            }
        }

    }
}
