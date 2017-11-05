
using myCollections.Utils;
namespace myCollections.Data
{
    class SearchInfo
    {
        public SearchMode SearchMode { get; set; }
        public Provider? Provider { get; set; }
        public string Id { get; set; }
        public string Search { get; set; }
        public string Message { get; set; }
        public string Artist { get; set; }
        public bool IsBarcode { get; set; }
        public AmazonIndex AmazonIndex { get; set; }
        public AmazonCountry AmazonCountry { get; set; }
        public AmazonBrowserNode AmazonBrowserNode { get; set; }
        public LanguageType LanguageType { get; set; }
        public string SeasonNumber { get; set; }
        public bool AllFind { get; set; }
        public string ErrorMessage { get; set; }
        public string BarcodeType { get; set; }
    }
}
