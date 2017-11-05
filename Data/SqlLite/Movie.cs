using System.Collections.Generic;
using myCollections.BL.Services;
using myCollections.Utils;
namespace myCollections.Data.SqlLite
{
    public class Movie : MyCollectionsData, IMyCollectionsData
    {
        private string _alloCine;
        public string AlloCine
        {
            get { return _alloCine; }
            set
            {
                if (value != _alloCine)
                {
                    _alloCine = value;
                    NotifyPropertyChanged("AlloCine");
                }
            }
        }

        string IMyCollectionsData.CoverTheme
        {
            get { return MySettings.TvixThemeMovie; }
        }

        private string _imdb;
        public string Imdb
        {
            get { return _imdb; }
            set
            {
                if (value != _imdb)
                {
                    _imdb = value;
                    NotifyPropertyChanged("Imdb");
                }
            }
        }
        public string Goofs { get; set; }

        EntityType IMyCollectionsData.ObjectType
        {
            get { return EntityType.Movie; }
        }

        private double? _publicRating;
        public double? PublicRating
        {
            get
            {
                if (_publicRating == null)
                    _publicRating = MovieServices.CalculateMovieRating(this);

                return _publicRating;
            }
            set
            {
                if (value != _publicRating)
                {
                    _publicRating = value;
                    NotifyPropertyChanged("PublicRating");
                }
            }
        }

        private string _tagLine;
        public string Tagline
        {
            get { return _tagLine; }
            set
            {
                if (value != _tagLine)
                {
                    _tagLine = value;
                    NotifyPropertyChanged("Tagline");
                }
            }
        }
    }
}