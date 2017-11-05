
using myCollections.Utils;
namespace myCollections.Data
{
    class MediaPlayerDevices
    {
        public SupportedDevice Name { get; set; }
        public string BGFileName { get; set; }
        public int BGWidth { get; set; }
        public int BGHeight { get; set; }
        public double BGRatio { get; set; }

        public string FolderFileName { get; set; }
        public float FolderWidth { get; set; }
        public float FolderHeight { get; set; }

        public string Folder2FileName { get; set; }
        public float Folder2Width { get; set; }
        public float Folder2Height { get; set; }

        public string XmlFileName { get; set; }


    }
}
