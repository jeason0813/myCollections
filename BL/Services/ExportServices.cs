
namespace myCollections.BL.Services
{
    public class ExportServices : IExportServices
    {
        private int _totalCount;
        private const string AllTitle = "All";
        private const string AppTitle = "Apps";
        private const string BookTitle = "Books";
        private const string GameTitle = "Games";
        private const string MovieTitle = "Movies";
        private const string MusicTitle = "Music";
        private const string NdsTitle = "Nds";
        private const string SeriesTitle = "Series";
        private const string XxxTitle = "XXX";

        public int GetCountExportItems(string item)
        {
            if (item == AppTitle || item == AllTitle)
                _totalCount += AppServices.Gets().Count;

            if (item == BookTitle || item == AllTitle)
                _totalCount += BookServices.Gets().Count;

            if (item == GameTitle || item == AllTitle)
                _totalCount += GameServices.Gets().Count;

            if (item == MovieTitle || item == AllTitle)
                _totalCount += MovieServices.Gets().Count;

            if (item == MusicTitle || item == AllTitle)
                _totalCount += MusicServices.Gets().Count;

            if (item == NdsTitle || item == AllTitle)
                _totalCount += NdsServices.Gets().Count;

            if (item == SeriesTitle || item == AllTitle)
                _totalCount += SerieServices.Gets().Count;

            if (item == XxxTitle || item == AllTitle)
                _totalCount += XxxServices.Gets().Count;

            return _totalCount;
        }
    }

}
