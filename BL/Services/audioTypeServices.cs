using myCollections.Data.SqlLite;

namespace myCollections.BL.Services
{
    static class AudioTypeServices
    {
        private static void Add(AudioType objItem)
        {
            Dal.GetInstance.AddAudioType(objItem);
        }
        public static AudioType Get(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            AudioType objItem = Dal.GetInstance.GetAudioType(value);
            if (objItem == null || string.IsNullOrWhiteSpace(objItem.Id))
                objItem = new AudioType(value);

            return objItem;
        }
    }
}
