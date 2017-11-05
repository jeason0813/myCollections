using System;
using System.Collections.ObjectModel;
using myCollections.Utils;

namespace myCollections.Data
{
    public class ThumbItem : NotifyChanged
    {
        private DateTime _added;
        public DateTime Added
        {
            get { return _added; }
            set
            {
                if (_added != value)
                {
                    _added = value;
                    FirePropertyChanged("Added");
                }
            }
        }

        private string _aka;
        public string Aka
        {
            get { return _aka; }
            set
            {
                if (_aka != value)
                {
                    _aka = value;
                    FirePropertyChanged("Aka");
                }
            }
        }
        private string _album;
        public string Album
        {
            get { return _album; }
            set
            {
                if (_album != value)
                {
                    _album = value;
                    FirePropertyChanged("Album");
                }
            }
        }
        private string _artist;
        public string Artist
        {
            get { return _artist; }
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    FirePropertyChanged("Artist");
                }
            }
        }
        private Byte[] _cover;
        public Byte[] Cover
        {
            get { return _cover; }
            set
            {
                if (_cover != value)
                {
                    _cover = value;
                    FirePropertyChanged("Cover");
                }
            }
        }
        private bool _deleted;
        public bool Deleted
        {
            get { return _deleted; }
            set
            {
                if (_deleted != value)
                {
                    _deleted = value;
                    FirePropertyChanged("Deleted");
                }
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    FirePropertyChanged("Description");
                }
            }
        }
        public EntityType EType { get; set; }

        private string _fullPath;
        public string FullPath
        {
            get { return _fullPath; }
            set
            {
                if (_fullPath != value)
                {
                    _fullPath = value;
                    FirePropertyChanged("FullPath");
                }
            }
        }

        private bool _hasCover;
        public bool HasCover
        {
            get { return _hasCover; }
            set
            {
                if (_hasCover != value)
                {
                    _hasCover = value;
                    FirePropertyChanged("HasCover");
                }
            }
        }
        public string Id { get; set; }

        private bool _iscomplete;
        public bool IsComplete
        {
            get { return _iscomplete; }
            set
            {
                if (_iscomplete != value)
                {
                    _iscomplete = value;
                    FirePropertyChanged("IsComplete");
                }
            }
        }

        private string _media;
        public string Media
        {
            get { return _media; }
            set
            {
                if (_media != value)
                {
                    _media = value;
                    FirePropertyChanged("Media");
                }
            }
        }
        private double? _myrating;
        public double? MyRating
        {
            get { return _myrating; }
            set
            {
                if (_myrating != value)
                {
                    _myrating = value;
                    FirePropertyChanged("MyRating");
                }
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    //Fix bug on version 2.3.2.0 with groupbyType
                    if (HasCover == false && _cover==null)
                        Cover = Util.CreateCover(_name);

                    FirePropertyChanged("Name");
                }
            }
        }
        private string _serieName;
        public string SerieName
        {
            get { return _serieName; }
            set
            {
                if (_serieName != value)
                {
                    _serieName = value;
                    FirePropertyChanged("SerieName");
                }
            }
        }
        private string _originalTitle;
        public string OriginalTitle
        {
            get { return _originalTitle; }
            set
            {
                if (_originalTitle != value)
                {
                    _originalTitle = value;
                    FirePropertyChanged("OriginalTitle");
                }
            }
        }
        private double? _publicRating;
        public double? PublicRating
        {
            get { return _publicRating; }
            set
            {
                if (_publicRating != value)
                {
                    _publicRating = value;
                    FirePropertyChanged("PublicRating");
                }
            }
        }
        private DateTime? _releasedate;
        public DateTime? ReleaseDate
        {
            get { return _releasedate; }
            set
            {
                if (_releasedate != value)
                {
                    _releasedate = value;
                    FirePropertyChanged("ReleaseDate");
                }
            }
        }
        private Int32? _runtime;
        public Int32? Runtime
        {
            get { return _runtime; }
            set
            {
                if (_runtime != value)
                {
                    _runtime = value;
                    FirePropertyChanged("Runtime");
                }
            }
        }
        private bool _seen;
        public bool Seen
        {
            get { return _seen; }
            set
            {
                if (_seen != value)
                {
                    _seen = value;
                    FirePropertyChanged("Seen");
                }
            }
        }
        private bool _tobedeleted;
        public bool ToBeDeleted
        {
            get { return _tobedeleted; }
            set
            {
                if (_tobedeleted != value)
                {
                    _tobedeleted = value;
                    FirePropertyChanged("ToBeDeleted");
                }
            }
        }
        private bool _tobuy;
        public bool ToBuy
        {
            get { return _tobuy; }
            set
            {
                if (_tobuy != value)
                {
                    _tobuy = value;
                    FirePropertyChanged("ToBuy");
                }
            }
        }
        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    FirePropertyChanged("Type");
                }
            }
        }
        private int _artistcredits;
        public int ArtistCredits
        {
            get { return _artistcredits; }
            set
            {
                if (_artistcredits != value)
                {
                    _artistcredits = value;
                    FirePropertyChanged("ArtistCredits");
                }
            }
        }
        private bool _toWatch;
        public bool ToWatch
        {
            get { return _toWatch; }
            set
            {
                if (_toWatch != value)
                {
                    _toWatch = value;
                    FirePropertyChanged("ToWatch");
                }
            }
        }

        private bool _showMe;
        public bool ShowMe
        {
            get { return _showMe; }
            set
            {
                if (_showMe != value)
                {
                    _showMe = value;
                    FirePropertyChanged("ShowMe");
                }
            }
        }

        private int? _numId;
        public int? NumId
        {
            get { return _numId; }
            set
            {
                if (_numId != value)
                {
                    _numId = value;
                    FirePropertyChanged("NumId");
                }
            }
        }

        public ThumbItem()
        {

        }
        public ThumbItem(string name, string id, Byte[] cover,
                       DateTime added, Int32? runtime, string originaltitle, bool deleted,
                      EntityType etype, double? myrating, double? publicrating, string description,
                      bool tobedeleted, string media, bool tobuy, bool iscomplete, bool seen,
                      string type, string album, string artist, int artistCredits, bool towatch, string fullPath, int? numId)
            : this(name, id, cover, added, runtime, originaltitle, deleted, etype, myrating,
                publicrating, description, tobedeleted, media, tobuy, iscomplete, seen,
                type, album, artist, towatch, fullPath, numId)
        {

            ArtistCredits = artistCredits;

        }
        public ThumbItem(string name, string id, Byte[] cover,
                         DateTime added, Int32? runtime, string originaltitle, bool deleted,
                        EntityType etype, double? myrating, double? publicrating, string description,
                        bool tobedeleted, string media, bool tobuy, bool iscomplete, bool seen,
                        string type, string album, string artist, bool towatch, string fullPath, int? numId)
        {
            Name = name;
            Id = id;
            if (cover != null)
            {
                Cover = cover;
                HasCover = true;
            }
            else
            {
                Cover = Util.CreateCover(Name);
                HasCover = false;
            }
            Added = added;
            Album = album;
            Artist = artist;
            Runtime = runtime;
            if (string.IsNullOrWhiteSpace(originaltitle))
                OriginalTitle = string.Empty;
            else
                OriginalTitle = originaltitle;
            Deleted = deleted;
            EType = etype;

            if (myrating != null)
                MyRating = myrating / 4;
            else
                MyRating = null;

            if (publicrating != null)
                PublicRating = publicrating / 4;
            else
                PublicRating = null;

            Description = description;
            ToBeDeleted = tobedeleted;
            Media = media;
            ToBuy = tobuy;
            IsComplete = iscomplete;
            Seen = seen;
            Type = type;
            ToWatch = towatch;
            FullPath = fullPath;
            NumId = numId;
        }

        public ThumbItem(string name, string aka, string id, Byte[] cover,
                      EntityType etype, string description, DateTime? releaseDate)
        {
            Name = name;
            Id = id;
            if (cover != null)
            {
                Cover = cover;
                HasCover = true;
            }
            else
            {
                Cover = Util.CreateCover(Name);
                HasCover = false;
            }

            EType = etype;
            Description = description;
            ReleaseDate = releaseDate;
            Aka = aka;
        }
    }
    public class ThumbItems : ObservableCollection<ThumbItem> { }
}
