using System;
using System.Collections.Generic;
using System.ComponentModel;
using myCollections.Utils;

namespace myCollections.Data.SqlLite
{
    public abstract class MyCollectionsData : IMyCollectionsData
    {
        private DateTime _addedDate;
        public DateTime AddedDate
        {
            get { return _addedDate; }
            set
            {
                if (value != _addedDate)
                {
                    _addedDate = value;
                    NotifyPropertyChanged("AddedDate");
                }
            }
        }

        private IList<Artist> _artists;
        public IList<Artist> Artists
        {
            get { return _artists; }
            set
            {
                if (value != _artists)
                {
                    _artists = value;
                    NotifyPropertyChanged("Artists");
                }
            }
        }

        private AspectRatio _aspectRatio;
        public AspectRatio AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                if (value != _aspectRatio)
                {
                    _aspectRatio = value;
                    NotifyPropertyChanged("AspectRatio");
                }
            }
        }
        private IList<Audio> _audios;
        public IList<Audio> Audios
        {
            get { return _audios; }
            set
            {
                if (value != _audios)
                {
                    _audios = value;
                    NotifyPropertyChanged("Audios");
                }
            }
        }

        private string _barCode;
        public string BarCode
        {
            get { return _barCode; }
            set
            {
                if (value != _barCode)
                {
                    _barCode = value;
                    NotifyPropertyChanged("BarCode");
                }
            }
        }

        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set
            {
                if (value != _comments)
                {
                    _comments = value;
                    NotifyPropertyChanged("Comments");
                }
            }
        }

        private byte[] _cover;
        public byte[] Cover
        {
            get { return _cover; }
            set
            {
                if (value != _cover)
                {
                    _cover = value;
                    NotifyPropertyChanged("Cover");
                }
            }
        }

        public string CoverTheme { get; set; }

        private string _country;
        public string Country
        {
            get { return _country; }
            set
            {
                if (value != _country)
                {
                    _country = value;
                    NotifyPropertyChanged("Country");
                }
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private FileFormat _fileFormat;
        public FileFormat FileFormat
        {
            get { return _fileFormat; }
            set
            {
                if (value != _fileFormat)
                {
                    _fileFormat = value;
                    NotifyPropertyChanged("FileFormat");
                }
            }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (value != _fileName)
                {
                    _fileName = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (value != _filePath)
                {
                    _filePath = value;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }
        public IList<Genre> Genres { get; set; }
        public string Id { get; set; }

        private bool _isComplete;
        public bool IsComplete
        {
            get { return _isComplete; }
            set
            {
                if (value != _isComplete)
                {
                    _isComplete = value;
                    NotifyPropertyChanged("IsComplete");
                }
            }
        }

        private bool _isDeleted;
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                if (value != _isDeleted)
                {
                    _isDeleted = value;
                    NotifyPropertyChanged("IsDeleted");
                }
            }
        }

        private bool _isWhish;
        public bool IsWhish
        {
            get { return _isWhish; }
            set
            {
                if (value != _isWhish)
                {
                    _isWhish = value;
                    NotifyPropertyChanged("IsWhish");
                }
            }
        }

        private Language _language;
        public Language Language
        {
            get { return _language; }
            set
            {
                if (value != _language)
                {
                    _language = value;
                    NotifyPropertyChanged("Language");
                }
            }
        }

        public IList<Links> Links { get; set; }

        private Media _media;
        public Media Media
        {
            get { return _media; }
            set
            {
                if (value != _media)
                {
                    _media = value;
                    NotifyPropertyChanged("Media");
                }
            }
        }

        public IList<MetaData> MetaDatas { get; set; }

        private string _rated;
        public string Rated
        {
            get { return _rated; }
            set
            {
                if (value != _rated)
                {
                    _rated = value;
                    NotifyPropertyChanged("Rated");
                }
            }
        }

        private int? _myRating;
        public int? MyRating
        {
            get { return _myRating; }
            set
            {
                if (value != _myRating)
                {
                    _myRating = value;
                    NotifyPropertyChanged("MyRating");
                }
            }
        }

        public EntityType ObjectType { get; set; }

        private string _originalTitle;
        public string OriginalTitle
        {
            get { return _originalTitle; }
            set
            {
                if (value != _originalTitle)
                {
                    _originalTitle = value;
                    NotifyPropertyChanged("OriginalTitle");
                }
            }
        }

        private int? _numId;
        public int? NumId
        {
            get
            {
                return _numId;
            }
            set
            {
                if (value != _numId)
                {
                    _numId = value;
                    NotifyPropertyChanged("NumId");
                }
            }
        }

        private Platform _platform;
        public Platform Platform
        {
            get { return _platform; }
            set
            {
                if (value != _platform)
                {
                    _platform = value;
                    NotifyPropertyChanged("Platform");
                }
            }
        }

        private Publisher _publisher;
        public Publisher Publisher
        {
            get { return _publisher; }
            set
            {
                if (value != _publisher)
                {
                    _publisher = value;
                    NotifyPropertyChanged("Publisher");
                }
            }
        }

        private double? _publicRating;
        public double? PublicRating
        {
            get { return _publicRating; }
            set
            {
                if (value != _publicRating)
                {
                    _publicRating = value;
                    NotifyPropertyChanged("PublicRating");
                }
            }
        }

        private DateTime? _releaseDate;
        public DateTime? ReleaseDate
        {
            get { return _releaseDate; }
            set
            {
                if (value != _releaseDate)
                {
                    _releaseDate = value;
                    NotifyPropertyChanged("ReleaseDate");
                }
            }
        }
        public bool RemoveCover { get; set; }
        public IList<Ressource> Ressources { get; set; }

        private int? _runtime;
        public int? Runtime
        {
            get { return _runtime; }
            set
            {
                if (value != _runtime)
                {
                    _runtime = value;
                    NotifyPropertyChanged("Runtime");
                }
            }
        }
        private IList<Language> _subtitles;
        public IList<Language> Subtitles
        {
            get { return _subtitles; }
            set
            {
                if (value != _subtitles)
                {
                    _subtitles = value;
                    NotifyPropertyChanged("Subtitles");
                }
            }
        }

        public IList<Track> Tracks { get; set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private bool _toBeDeleted;
        public bool ToBeDeleted
        {
            get { return _toBeDeleted; }
            set
            {
                if (value != _toBeDeleted)
                {
                    _toBeDeleted = value;
                    NotifyPropertyChanged("ToBeDeleted");
                }
            }
        }

        private bool _toWatch;
        public bool ToWatch
        {
            get { return _toWatch; }
            set
            {
                if (value != _toWatch)
                {
                    _toWatch = value;
                    NotifyPropertyChanged("ToWatch");
                }
            }
        }

        private bool _watched;
        public bool Watched
        {
            get { return _watched; }
            set
            {
                if (value != _watched)
                {
                    _watched = value;
                    NotifyPropertyChanged("Watched");
                }
            }
        }

        public string SerieId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
