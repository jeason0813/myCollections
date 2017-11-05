using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using IdSharp.Tagging.ID3v2;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using Luminescence.Xiph;
using Genre = myCollections.Data.SqlLite.Genre;

namespace myCollections.BL
{
    internal sealed class AddItem : IProgressOperation, IDisposable
    {
        private readonly bool _bFile;
        private readonly bool _bGetImage;
        private readonly bool _bParseNfo;
        private readonly bool _cleanTitle;
        private readonly EntityType _entityType;
        private readonly Hashtable _mapping;
        private readonly int _nbrBaseParsing;
        private readonly string[] _selectedItems;
        private readonly string[] _selectedPath;
        private readonly string _strMediaName;
        private readonly string _mediaType;
        private readonly string _strPattern;
        private readonly string _path;
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private readonly bool _useSubFolder;
        private readonly string _patternType;
        private string _message;
        private readonly string[] _seriesSeasonValue =
        { "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10", 
            "S01","S02","S03","S04","S05","S06","S07","S08","S09","S10", "SAISON",
            "Season 1","Season 2","Season 3","Season 4","Season 5","Season 6","Season 7","Season 8","Season 9","Season 10" };

        public AddItem(IList selectedItems, Hashtable mapping,
                       int nbrBaseParsing, EntityType entityType,
                       string strPattern, string strPath,
                       string strMediaName, string mediaType, bool bGetImage,
                       bool bParseNfo, bool cleanTitle, bool useSubFolder, string patternType)
        {
            _current = 1;
            _entityType = entityType;

            _selectedItems = new string[selectedItems.Count];
            _selectedPath = new string[selectedItems.Count];

            int i = 0;
            foreach (ListBoxItem item in selectedItems)
            {
                _selectedItems[i] = item.Content.ToString();

                DirectoryInfo info = item.Content as DirectoryInfo;
                if (info != null)
                {
                    DirectoryInfo objTemp = info;
                    _selectedPath[i] = objTemp.FullName.Replace(objTemp.Name, "");
                }
                else
                {
                    FileInfo temp = item.Content as FileInfo;
                    if (temp != null)
                    {
                        FileInfo objTemp = temp;
                        if (objTemp.DirectoryName != null)
                            _selectedPath[i] = objTemp.DirectoryName.Replace(objTemp.Name, "");
                        _bFile = true;
                    }
                    else
                        _selectedPath[i] = strPath;
                }

                i++;
            }

            _mapping = mapping;
            _nbrBaseParsing = nbrBaseParsing;
            _entityType = entityType;
            _strPattern = strPattern;
            _strMediaName = strMediaName;
            _mediaType = mediaType;
            _bGetImage = bGetImage;
            _bParseNfo = bParseNfo;
            _cleanTitle = cleanTitle;
            _path = strPath;
            _useSubFolder = useSubFolder;
            _patternType = patternType;
        }

        public AddItem(IList selectedItems, EntityType entityType,
                       string strPath, string strMediaName, string mediaType,
                       bool bGetImage, bool bParseNfo, bool cleanTitle, bool useSubFolder, string patternType)
        {
            _current = 1;
            _entityType = entityType;

            _selectedItems = new string[selectedItems.Count];
            _selectedPath = new string[selectedItems.Count];

            int i = 0;
            foreach (var item in selectedItems)
            {
                DirectoryInfo info = item as DirectoryInfo;
                if (info != null)
                {
                    DirectoryInfo objTemp = info;
                    _selectedPath[i] = objTemp.FullName.Replace(objTemp.Name, "");
                    _selectedItems[i] = item.ToString();
                }
                else
                {
                    FileInfo temp = item as FileInfo;
                    if (temp != null)
                    {
                        FileInfo objTemp = temp;
                        if (objTemp.DirectoryName != null)
                            _selectedPath[i] = objTemp.DirectoryName.Replace(objTemp.Name, "");
                        _selectedItems[i] = item.ToString();
                        _bFile = true;
                    }
                    else
                        _selectedPath[i] = strPath;
                }

                i++;
            }

            _mapping = null;
            _nbrBaseParsing = 0;
            _entityType = entityType;
            _strPattern = string.Empty;
            _strMediaName = strMediaName;
            _mediaType = mediaType;
            _bGetImage = bGetImage;
            _bParseNfo = bParseNfo;
            _cleanTitle = cleanTitle;
            _path = strPath;
            _useSubFolder = useSubFolder;
            _patternType = patternType;
        }
        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;

        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        public int Total
        {
            get { return _total; }
            private set
            {
                _total = value;
                OnProgressTotalChanged(EventArgs.Empty);
            }
        }
        public int AddedItem
        {
            get { return _intAddedItem; }
        }
        public int NotAddedItem
        {
            get { return _intNotAddedItem; }
        }
        public int Current
        {
            get { return _current; }
            private set
            {
                _current = value;
                OnProgressChanged(EventArgs.Empty);
            }
        }
        public IList RemovedItems
        {
            get { return null; }
        }
        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;
                OnMessageChanged(EventArgs.Empty);
            }
        }
        public void CancelAsync()
        {
            _isCancelationPending = true;
        }

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Total = _selectedItems.Length;

                string[] strTemp = null;
                IList<string> dirtyTags = null;

                if (_cleanTitle == true)
                    dirtyTags = Dal.GetInstance.GetDirtyTags(_entityType);

                for (int i = 0; i < _selectedItems.Length; i++)
                {
                    //exit if the user cancels
                    if (_isCancelationPending == true)
                        return;

                    #region Common

                    string strTitle;
                    if (_mapping != null)
                    {
                        strTemp = ParseValue(_strPattern, _selectedItems[i]);
                        strTitle = Util.ConstructString(strTemp, (string[])_mapping["Title"], _nbrBaseParsing);
                    }
                    else
                        strTitle = _selectedItems[i];

                    if (string.IsNullOrEmpty(strTitle))
                        strTitle = _selectedItems[i];

                    if (dirtyTags != null)
                        strTitle = dirtyTags.Aggregate(strTitle, (current, item) => Regex.Replace(current,item, " ",RegexOptions.IgnoreCase));

                    strTitle = strTitle.Replace("  ", " ");
                    strTitle = strTitle.Replace("_", " ");
                    strTitle = strTitle.Replace("-", " ");
                    strTitle = strTitle.Replace(".", " ");
                    strTitle = strTitle.Replace("   ", " ");
                    strTitle = strTitle.Replace("  ", " ");
                    strTitle = Util.CleanExtensions(strTitle);
                    strTitle = Util.RemoveDate(strTitle);
                    strTitle = strTitle.Trim();
                    strTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strTitle);

                    #endregion
                    switch (_entityType)
                    {
                        case EntityType.Apps:
                            AddApps(strTemp, strTitle, _selectedItems[i], _selectedPath[i]);
                            break;
                        case EntityType.Books:
                            AddBooks(strTemp, strTitle, _selectedItems[i], _selectedPath[i]);
                            break;
                        case EntityType.Games:
                            AddGamez(strTemp, strTitle, _selectedItems[i], _selectedPath[i]);
                            break;
                        case EntityType.Movie:
                            AddMovies(strTitle, _selectedItems[i], _selectedPath[i]);
                            break;
                        case EntityType.Music:
                            AddMusic(strTemp, strTitle, _selectedItems[i], _selectedPath[i]);
                            break;
                        case EntityType.Nds:
                            AddNds(strTemp, strTitle, _selectedItems[i], _selectedPath[i]);
                            break;
                        case EntityType.Series:
                            AddSeries(strTitle, _selectedItems[i], _selectedPath[i]);
                            break;
                        case EntityType.XXX:
                            AddXxx(strTemp, strTitle, _selectedItems[i], _selectedPath[i]);
                            break;
                    }
                    Current++;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }
        private void AddApps(string[] strTemp, string strTitle, string strFileName, string strFilePath)
        {
            Media objMedia = MediaServices.Get(_strMediaName.Trim(), _mediaType, _path, _cleanTitle, _entityType, _patternType, _useSubFolder, _bGetImage, _bParseNfo, true);

            Apps objApps = new Apps();
            objApps.Title = strTitle;
            objApps.AddedDate = DateTime.Now;
            #region File

            objApps.FileName = strFileName;
            objApps.FilePath = strFilePath;

            #endregion
            if (_mapping != null)
            {
                string strLanguage = Util.ConstructString(strTemp, (string[])_mapping["Language"], _nbrBaseParsing);
                objApps.Language = LanguageServices.GetLanguage(strLanguage, true);

                string strVersion = Util.ConstructString(strTemp, (string[])_mapping["Version"], _nbrBaseParsing);
                objApps.Version = strVersion;
            }
            objApps.Media = objMedia;
            #region Cover
            if (_bGetImage == true)
                RessourcesServices.AddImage(Util.GetLocalImage(objApps.FilePath, objApps.FileName, _bFile), objApps, true);
            #endregion
            if (string.IsNullOrEmpty(objApps.Title) == false)
            {
                bool bExist = false;
                if (Dal.GetInstance.GetApps(objApps.Media.Name, objApps.FilePath, objApps.FileName) != null)
                    bExist = true;

                if (bExist == false)
                {
                    #region ParseNfo

                    if (_bParseNfo == true)
                    {
                        string errorMessage;
                        AppServices.ParseNfo(objApps, out errorMessage);
                    }
                    #endregion
                    Dal.GetInstance.AddApps(objApps);
                    _intAddedItem++;
                }
                else
                    _intNotAddedItem++;
            }
        }
        private void AddSeries(string strTitle, string strFileName, string strFilePath)
        {
            if (IsRootFolder(strFileName, strFilePath) == false)
            {
                bool create;
                SeriesSeason objEntity = IsSeasonFolder(strFileName, strFilePath, out create);
                if (create == true)
                {
                    if (objEntity == null)
                    {
                        objEntity = new SeriesSeason();
                        objEntity.SerieId = SerieServices.GetSerieId(strTitle);
                        objEntity.Title = strTitle;
                        objEntity.Season = 1;
                    }

                    Media objMedia = MediaServices.Get(_strMediaName.Trim(), _mediaType, _path, _cleanTitle, _entityType, _patternType, _useSubFolder, _bGetImage, _bParseNfo, true);

                    objEntity.Media = objMedia;
                    objEntity.Watched = false;
                    objEntity.IsComplete = false;
                    objEntity.ToBeDeleted = false;
                    objEntity.ToWatch = false;
                    objEntity.AddedDate = DateTime.Now;
                    objEntity.Ressources = new List<Ressource>();
                    objEntity.Genres = new List<Genre>();
                    objEntity.Audios = new List<Audio>();
                    objEntity.Subtitles = new List<Language>();


                    #region File

                    objEntity.FilePath = Path.Combine(strFilePath, strFileName);

                    #endregion

                    #region Cover

                    if (_bGetImage == true)
                        RessourcesServices.AddImage(Util.GetLocalImage(objEntity.FilePath, string.Empty, _bFile), objEntity,
                                               true);

                    #endregion

                    if (string.IsNullOrEmpty(objEntity.Title) == false)
                    {
                        bool bExist = false;
                        if (Dal.GetInstance.GetSeriesSeason(objEntity.Media.Name, objEntity.FilePath, objEntity.Season) != null)
                            bExist = true;

                        if (bExist == false)
                        {
                            #region MediaInfo
                            FileInfo file = new FileInfo(Path.Combine(strFilePath, strFileName));
                            if (file.Attributes == FileAttributes.Directory)
                            {
                                if (Directory.Exists(file.FullName) == true)
                                {
                                    string[] lstFile = Util.GetFiles(file.FullName, new[] { "*.mkv", "*.mp4", "*.avi", "*.divx" });

                                    if (lstFile.Any())
                                        SerieServices.Fill(MediaInfoService.getInfo(lstFile[0], objEntity.Id), objEntity);
                                }
                            }
                            else if (file.Attributes == FileAttributes.Normal)
                                SerieServices.Fill(MediaInfoService.getInfo(file.FullName, objEntity.Id), objEntity);

                            #endregion

                            #region ParseNfo

                            if (_bParseNfo == true)
                            {
                                string errorMessage;
                                SerieServices.ParseNfo(objEntity, out errorMessage);
                            }
                            #endregion
                            Dal.GetInstance.AddSeriesSeason(objEntity);
                            _intAddedItem++;
                        }
                        else
                            _intNotAddedItem++;
                    }
                }
            }
        }

        private SeriesSeason IsSeasonFolder(string folderName, string folderPath, out bool create)
        {
            create = true;
            var season = from items in _seriesSeasonValue
                         where folderName.ToUpper() == items.ToUpper()
                         select items;

            DirectoryInfo folder = new DirectoryInfo(Path.Combine(folderPath, folderName));

            if (season.Any())
            {
                if (folder.Exists == true)
                {
                    int seasonNumber;
                    if (int.TryParse(Regex.Match(folderName, @"\d+").Value, out seasonNumber) == true)
                    {
                        if (folder.Parent != null)
                        {
                            bool outCreate;
                            if (
                                IsSeasonFolder(folder.Parent.Name,
                                               folder.Parent.FullName.Substring(0,
                                                                                folder.Parent.FullName.IndexOf(folder.Parent.Name, StringComparison.Ordinal)), out outCreate) == null)
                            {
                                if (outCreate == false)
                                {
                                    create = false;
                                    return null;
                                }

                                SeriesSeason objEntity = new SeriesSeason();
                                objEntity.SerieId = SerieServices.GetSerieId(folder.Parent.Name);
                                objEntity.Title = folder.Parent.Name;
                                objEntity.Season = seasonNumber;

                                return objEntity;
                            }
                            else
                                create = false;
                        }
                    }
                }
            }
            else
            {
                if (folder.Exists == true)
                {
                    if (folder.Parent != null)
                    {
                        int seasonNumber;
                        season = from items in _seriesSeasonValue
                                 where folder.Parent.Name.ToUpper() == items.ToUpper()
                                 select items;

                        if (season.Any() && int.TryParse(Regex.Match(folder.Parent.Name, @"\d+").Value, out seasonNumber) == true)
                            create = false;
                    }

                }
            }

            return null;
        }
        private bool IsRootFolder(string folderName, string folderPath)
        {
            DirectoryInfo folder = new DirectoryInfo(Path.Combine(folderPath, folderName));

            //Fix since 2.5.5.0
            if (folder.Exists == true)
                if (folder.GetDirectories().Any())
                {
                    DirectoryInfo[] folders = folder.GetDirectories();
                    return folders.Any(directoryInfo => _seriesSeasonValue.Contains(directoryInfo.Name));
                }
                else
                    return false;
            else
                return false;
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnComplete(EventArgs.Empty);
        }

        private static string[] ParseValue(string strPattern, string strValue)
        {
            string[] objResults = null;
            string[] objPatterns = strPattern.Split("||".ToCharArray());

            foreach (string item in objPatterns)
            {
                objResults = strValue.Split(item.Trim().ToCharArray());

                if ((objResults.Length > 1 && item.Trim() != ".") || (objResults.Length > 2))
                    break;
            }

            return objResults;
        }

        private void OnProgressTotalChanged(EventArgs e)
        {
            if (ProgressTotalChanged != null)
                ProgressTotalChanged(this, e);
        }
        private void OnProgressChanged(EventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }
        private void OnComplete(EventArgs e)
        {
            if (Complete != null)
                Complete(this, e);
        }
        private void OnMessageChanged(EventArgs e)
        {
            if (MessageChanged != null)
                MessageChanged(this, e);
        }
        #region Books

        private void AddBooks(string[] strTemp, string strTitle, string strFileName, string strFilePath)
        {
            Media objMedia = MediaServices.Get(_strMediaName.Trim(), _mediaType, _path,
                                                _cleanTitle, _entityType, _patternType, _useSubFolder, _bGetImage, _bParseNfo, true);

            const string strBookFormat = "Unknown";
            FileFormat objFormat = BookServices.GetFormat(strBookFormat, true);

            //Fix since 2.7.12.0
            strTitle = strTitle.Replace("EPub", "");
            strTitle = strTitle.Replace("Epub", "");
            strTitle = strTitle.Replace("Mobi", "");
            strTitle = strTitle.Replace("epub", "");
            strTitle = strTitle.Replace("mobi", "");
            strTitle = strTitle.Replace(".", " ");
            strTitle = strTitle.Trim();


            Books objBooks = new Books();
            objBooks.Ressources = new List<Ressource>();
            objBooks.Title = strTitle;
            objBooks.AddedDate = DateTime.Now;
            #region File

            objBooks.FileName = strFileName;
            objBooks.FilePath = strFilePath;

            #endregion
            if (_mapping != null)
            {
                string strLanguage = Util.ConstructString(strTemp, (string[])_mapping["Language"], _nbrBaseParsing);
                objBooks.Language = LanguageServices.GetLanguage(strLanguage, true);
            }

            objBooks.FileFormat = objFormat;
            objBooks.Media = objMedia;
            #region Cover

            if (_bGetImage == true)
                RessourcesServices.AddImage(Util.GetLocalImage(objBooks.FilePath, objBooks.FileName, _bFile), objBooks, true);
            int index;
            if (RessourcesServices.GetDefaultCover(objBooks, out index) == null)
            {
                DirectoryInfo objFolder;

                if (_bFile == false)
                    objFolder = new DirectoryInfo(objBooks.FilePath + @"\" + objBooks.FileName);
                else
                    objFolder = new DirectoryInfo(objBooks.FilePath);

                FileInfo[] lstPdf = objFolder.GetFiles("*.pdf", SearchOption.AllDirectories);

                if (lstPdf.Any())
                    RessourcesServices.AddImage(Util.GetImageFromPdf(lstPdf[0].FullName), objBooks, true);
            }

            #endregion
            if (string.IsNullOrEmpty(objBooks.Title) == false)
            {
                bool bExist = false;
                if (Dal.GetInstance.GetBooks(objBooks.Media.Name, objBooks.FilePath, objBooks.FileName) != null)
                    bExist = true;

                if (bExist == false)
                {
                    #region ParseNfo

                    if (_bParseNfo == true)
                    {
                        string errorMessage;
                        BookServices.ParseNfo(objBooks, out  errorMessage);
                    }
                    #endregion
                    Dal.GetInstance.AddBook(objBooks);
                    _intAddedItem++;
                }
                else
                    _intNotAddedItem++;
            }
        }

        #endregion
        #region Gamez

        private void AddGamez(string[] strTemp, string strTitle, string strFileName, string strFilePath)
        {

            Gamez objGames = new Gamez();
            objGames.Title = strTitle;
            objGames.Ressources = new List<Ressource>();
            objGames.AddedDate = DateTime.Now;
            #region File

            objGames.FileName = strFileName;
            objGames.FilePath = strFilePath;

            #endregion
            if (_mapping != null)
            {
                string strLanguage = Util.ConstructString(strTemp, (string[])_mapping["Language"], _nbrBaseParsing);
                objGames.Language = LanguageServices.GetLanguage(strLanguage, true);
            }

            objGames.Media = MediaServices.Get(_strMediaName.Trim(), _mediaType, _path,
                                                _cleanTitle, _entityType, _patternType, _useSubFolder, _bGetImage, _bParseNfo, true);
            #region Cover
            if (_bGetImage == true)
                RessourcesServices.AddImage(Util.GetLocalImage(objGames.FilePath, objGames.FileName, _bFile), objGames, true);
            #endregion
            if (string.IsNullOrEmpty(objGames.Title) == false)
            {

                bool bExist = false;
                if (Dal.GetInstance.GetGames(objGames.Media.Name, objGames.FilePath, objGames.FileName) != null)
                    bExist = true;


                if (bExist == false)
                {
                    #region ParseNfo

                    if (_bParseNfo == true)
                    {
                        string errorMessage;
                        GameServices.ParseNfo(objGames, out errorMessage);
                    }
                    #endregion
                    Dal.GetInstance.AddGame(objGames);

                    _intAddedItem++;
                }
                else
                    _intNotAddedItem++;
            }
        }

        #endregion
        #region Movies

        private void AddMovies(string strTitle, string strFileName, string strFilePath)
        {
            try
            {

                Movie objMovie = new Movie();
                objMovie.Title = strTitle;
                objMovie.Ressources = new List<Ressource>();
                objMovie.Artists = new List<Artist>();
                objMovie.Audios = new List<Audio>();
                objMovie.Subtitles = new List<Language>();
                objMovie.Genres = new List<Genre>();
                objMovie.Links = new List<Links>();
                objMovie.AddedDate = DateTime.Now;
                #region File

                objMovie.FileName = strFileName;
                objMovie.FilePath = strFilePath;

                #endregion
                objMovie.Media = MediaServices.Get(_strMediaName.Trim(), _mediaType, _path,
                                                    _cleanTitle, _entityType, _patternType, _useSubFolder, _bGetImage, _bParseNfo, true);
                #region Cover
                if (_bGetImage == true)
                    RessourcesServices.AddImage(Util.GetLocalImage(objMovie.FilePath, objMovie.FileName, _bFile), objMovie, true);
                #endregion
                if (string.IsNullOrEmpty(objMovie.Title) == false)
                {
                    bool bExist = false;
                    if (Dal.GetInstance.GetMovies(objMovie.Media.Name, objMovie.FilePath, objMovie.FileName) != null)
                        bExist = true;

                    if (bExist == false)
                    {
                        #region MediaInfo
                        FileInfo file = new FileInfo(Path.Combine(strFilePath, strFileName));
                        if (file.Attributes == FileAttributes.Directory)
                        {
                            DirectoryInfo objFolder = new DirectoryInfo(file.FullName);
                            if (objFolder.Exists == true)
                            {
                                FileInfo[] lstFile = objFolder.GetFiles("*.mkv", SearchOption.TopDirectoryOnly);

                                if (!lstFile.Any())
                                    lstFile = objFolder.GetFiles("*.avi", SearchOption.TopDirectoryOnly);

                                if (!lstFile.Any())
                                    lstFile = objFolder.GetFiles("*.divx ", SearchOption.TopDirectoryOnly);

                                if (lstFile.Any())
                                    MovieServices.Fill(MediaInfoService.getInfo(lstFile[0].FullName, objMovie.Id), objMovie);
                            }
                        }
                        else if (file.Attributes == FileAttributes.Normal)
                            MovieServices.Fill(MediaInfoService.getInfo(file.FullName, objMovie.Id), objMovie);

                        #endregion
                        #region ParseNfo

                        if (_bParseNfo == true)
                        {
                            string errorMessage;
                            MovieServices.ParseNfo(objMovie, out errorMessage);
                        }
                        #endregion
                        Dal.GetInstance.AddMovie(objMovie);
                        _intAddedItem++;
                    }
                    else
                        _intNotAddedItem++;
                }
            }
            catch (Exception exception)
            {
                Util.LogException(exception, strTitle);
            }
        }

        #endregion
        #region Music

        private void AddMusic(string[] strTemp, string strTitle, string strFileName, string strFilePath)
        {
            bool isFolder = false;
            bool isEmptyFolder = false;

            FileInfo file = new FileInfo(Path.Combine(strFilePath, strFileName));
            Hashtable tags = new Hashtable();
            Media objMedia = MediaServices.Get(_strMediaName.Trim(), _mediaType, _path, _cleanTitle, _entityType, _patternType, _useSubFolder, _bGetImage, _bParseNfo, true);

            if (file.Exists == false && string.IsNullOrWhiteSpace(file.Extension))
            {
                if (Directory.Exists(file.FullName))
                {
                    DirectoryInfo folder = new DirectoryInfo(file.FullName);

                    FileInfo[] files = folder.GetFiles("*.mp3", SearchOption.TopDirectoryOnly);
                    files = files.Concat(folder.GetFiles("*.flc", SearchOption.TopDirectoryOnly)).ToArray();
                    files = files.Concat(folder.GetFiles("*.flac", SearchOption.TopDirectoryOnly)).ToArray();

                    if (files.Any())
                    {
                        file = files[0];
                        isFolder = true;
                    }
                    else
                        isEmptyFolder = true;
                }
            }

            if (isEmptyFolder == false)
            {
                if (Dal.GetInstance.GetMusics(objMedia.Name, strFilePath, strFileName) == null)
                {
                    switch (file.Extension)
                    {
                        case ".mp3":
                            IID3v2 objMp3Tag = ID3v2Helper.CreateID3v2(file.FullName);

                            tags.Add("Title", objMp3Tag.Title);


                            if (string.IsNullOrWhiteSpace(objMp3Tag.Album) == false)
                                tags.Add("Album", objMp3Tag.Album);

                            if (isFolder == false && objMp3Tag.LengthMilliseconds != null)
                                tags.Add("Length", objMp3Tag.LengthMilliseconds);

                            if (objMp3Tag.PictureList.Count > 0)
                                tags.Add("Cover", objMp3Tag.PictureList[0].PictureData);

                            tags.Add("Genre", objMp3Tag.Genre);
                            tags.Add("Artist", objMp3Tag.Artist);
                            break;

                        case ".flac":
                        case ".flc":
                            try
                            {

                                FlacTagger objFlacTag = new FlacTagger(file.FullName);

                                tags.Add("Title", objFlacTag.Title);

                                if (string.IsNullOrWhiteSpace(objFlacTag.Album) == false)
                                    tags.Add("Album", objFlacTag.Album);

                                if (isFolder == false)
                                    tags.Add("Length", objFlacTag.Length);

                                if (objFlacTag.Arts.Count > 0)
                                    tags.Add("Cover", objFlacTag.Arts[0].PictureData);

                                tags.Add("Genre", objFlacTag.Genre);
                                tags.Add("Artist", objFlacTag.Artist);
                                break;
                            }
                            //FIX 2.8.9.0
                            catch (FileFormatException)
                            {
                                break;
                            }
                    }

                    #region Title

                    if (tags.ContainsKey("Title") == false)
                    {
                        if (string.IsNullOrEmpty(strTitle) == false)
                        {
                            strTitle = strTitle.Replace('_', ' ');
                            strTitle = strTitle.Replace(".MP3", "");
                            strTitle = strTitle.Replace(".Mp3", "");
                            strTitle = strTitle.Replace(".flac", "");
                            strTitle = strTitle.Trim();
                            strTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strTitle);
                            tags.Add("Title", strTitle);

                        }
                    }

                    #endregion

                    Music objMusic = new Music();
                    objMusic.Ressources = new List<Ressource>();
                    objMusic.Genres = new List<Genre>();
                    objMusic.Title = strTitle;
                    objMusic.AddedDate = DateTime.Now;
                    objMusic.FileName = strFileName;
                    objMusic.FilePath = strFilePath;

                    if (tags.ContainsKey("Album"))
                    {
                        objMusic.Album = tags["Album"].ToString();
                        if (isFolder == true && string.IsNullOrWhiteSpace(tags["Album"].ToString()) == false)
                            objMusic.Title = tags["Album"].ToString();
                    }

                    if (tags.ContainsKey("Length"))
                        objMusic.Runtime = tags["Length"] as int?;

                    objMusic.Media = objMedia;

                    #region Cover

                    if (_bGetImage == true)
                        RessourcesServices.AddImage(Util.GetLocalImage(objMusic.FilePath, objMusic.FileName, _bFile), objMusic,
                                               true);
                    else
                        RessourcesServices.AddImage(tags["Cover"] as byte[], objMusic, true);

                    #endregion

                    bool bExist = false;
                    if (Dal.GetInstance.GetMusics(objMusic.Media.Name, objMusic.FilePath, objMusic.FileName) != null)
                        bExist = true;

                    if (bExist == false)
                    {
                        #region ParseNfo

                        if (_bParseNfo == true)
                        {
                            string errorMessage;
                            MusicServices.ParseNfo(objMusic, out errorMessage);
                        }
                        #endregion
                        #region Artist
                        string strArtistFullName = string.Empty;
                        if (tags.ContainsKey("Artist") == true)
                        {
                            if (tags["Artist"] == null)
                            {
                                if (_mapping != null)
                                    strArtistFullName = Util.ConstructString(strTemp, (string[])_mapping["Artist"], _nbrBaseParsing);
                            }
                            else
                                strArtistFullName = tags["Artist"] as string;
                        }
                        #endregion
                        Dal.GetInstance.AddMusic(objMusic);
                        if (tags.ContainsKey("Genre"))
                            GenreServices.AddGenres(new[] { tags["Genre"] as string }, objMusic, true);
                        if (strArtistFullName != null && string.IsNullOrWhiteSpace(strArtistFullName.Trim()) == false)
                            ArtistServices.AddArtist(strArtistFullName, objMusic);

                        _intAddedItem++;
                    }
                    else
                        _intNotAddedItem++;
                }
            }
        }

        #endregion
        #region Nds

        private void AddNds(string[] strTemp, string strTitle, string strFileName, string strFilePath)
        {
            Media objMedia = MediaServices.Get(_strMediaName.Trim(), _mediaType, _path,
                                                _cleanTitle, _entityType, _patternType, _useSubFolder, _bGetImage, _bParseNfo, true);

            Nds objNds = new Nds();
            objNds.Ressources = new List<Ressource>();
            objNds.Title = strTitle;
            objNds.AddedDate = DateTime.Now;
            #region File

            objNds.FileName = strFileName;
            objNds.FilePath = strFilePath;

            #endregion
            if (_mapping != null)
            {
                string strLanguage = Util.ConstructString(strTemp, (string[])_mapping["Language"], _nbrBaseParsing);
                objNds.Language = LanguageServices.GetLanguage(strLanguage, true);
            }
            objNds.Media = objMedia;
            #region Cover
            if (_bGetImage == true)
                RessourcesServices.AddImage(Util.GetLocalImage(objNds.FilePath, objNds.FileName, _bFile), objNds, true);
            #endregion
            if (string.IsNullOrEmpty(objNds.Title) == false)
            {
                bool bExist = false;
                if (Dal.GetInstance.GetNdss(objNds.Media.Name, objNds.FilePath, objNds.FileName) != null)
                    bExist = true;

                if (bExist == false)
                {
                    #region ParseNfo

                    if (_bParseNfo == true)
                    {
                        string errorMessage;
                        NdsServices.ParseNfo(objNds, out errorMessage);
                    }

                    #endregion
                    Dal.GetInstance.AddNds(objNds);
                    _intAddedItem++;
                }
                else
                    _intNotAddedItem++;
            }
        }

        #endregion
        #region XXX

        private void AddXxx(string[] strTemp, string strTitle,
                            string strFileName,
                            string strFilePath)
        {
            Media objMedia = MediaServices.Get(_strMediaName.Trim(), _mediaType, _path,
                                                _cleanTitle, _entityType, _patternType, _useSubFolder, _bGetImage, _bParseNfo, true);

            XXX objEntity = new XXX();
            objEntity.Ressources = new List<Ressource>();
            objEntity.Media = objMedia;
            objEntity.Title = strTitle;
            objEntity.FilePath = strFilePath;
            objEntity.AddedDate = DateTime.Now;
            objEntity.Ressources = new List<Ressource>();
            objEntity.Genres = new List<Genre>();
            objEntity.Audios = new List<Audio>();
            objEntity.Subtitles = new List<Language>();
            objEntity.Artists = new List<Artist>();


            #region File

            objEntity.FileName = strFileName;

            #endregion
            #region Language

            if (_mapping != null)
            {
                string strLanguage = Util.ConstructString(strTemp, (string[])_mapping["Language"], _nbrBaseParsing);
                objEntity.Language = LanguageServices.GetLanguage(strLanguage, true);
            }

            #endregion

            if (_bGetImage == true)
                RessourcesServices.AddImage(Util.GetLocalImage(objEntity.FilePath, objEntity.FileName, _bFile), objEntity, true);

            if (string.IsNullOrEmpty(objEntity.Title) == false)
            {
                bool bExist = false;
                if (Dal.GetInstance.GetXxXs(objEntity.Media.Name, objEntity.FilePath, objEntity.FileName) != null)
                    bExist = true;

                if (bExist == false)
                {
                    #region MediaInfo
                    FileInfo file = new FileInfo(Path.Combine(strFilePath, strFileName));
                    if (file.Attributes == FileAttributes.Directory)
                    {
                        if (Directory.Exists(file.FullName) == true)
                        {
                            string[] lstFile = Util.GetFiles(file.FullName, new []{"*.mkv","*.mp4","*.avi","*.divx"});

                            if (lstFile.Any())
                                XxxServices.Fill(MediaInfoService.getInfo(lstFile[0], objEntity.Id), objEntity);
                        }
                    }
                    else if (file.Attributes == FileAttributes.Normal)
                        XxxServices.Fill(MediaInfoService.getInfo(file.FullName, objEntity.Id), objEntity);

                    #endregion

                    #region ParseNfo

                    if (_bParseNfo == true)
                    {
                        string errorMessage;
                        XxxServices.ParseNfo(objEntity, out errorMessage);
                    }
                    #endregion
                    Dal.GetInstance.AddXxx(objEntity);

                    _intAddedItem++;
                }
                else
                    _intNotAddedItem++;
            }
        }

        #endregion

    }
}