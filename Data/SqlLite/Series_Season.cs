using System.Globalization;
using myCollections.Utils;
namespace myCollections.Data.SqlLite
{
    public class SeriesSeason : MyCollectionsData,IMyCollectionsData
    {
        public int? AvailableEpisodes { get; set; }

        string IMyCollectionsData.CoverTheme
        {
            get { return MySettings.TvixThemeSerie; }
        }
        public bool IsInProduction { get; set; }
        public int? MissingEpisodes { get; set; }

        EntityType IMyCollectionsData.ObjectType
        {
            get { return EntityType.Series; }
        }
        public string OfficialWebSite { get; set; }

        string IMyCollectionsData.OriginalTitle
        {
            get { return "Season : " + Season.ToString(CultureInfo.InvariantCulture); }
            set { }
        }
        public int Season { get; set; }
        public int? TotalEpisodes { get; set; }
    }
}