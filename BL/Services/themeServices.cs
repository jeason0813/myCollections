using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using myCollections.Data;
using System.Xml;
using System.Linq;

namespace myCollections.BL.Services
{
    static class ThemeServices
    {
        #region Dune

        private static string CreateDuneFiles(IMyCollectionsData objItem, string outputPath, MediaPlayerDevices mediaPlayerDevice)
        {

            SizeF folderCover = new SizeF(mediaPlayerDevice.FolderWidth, mediaPlayerDevice.FolderHeight);

            string strPath = CalculatePath(objItem, outputPath, objItem.ObjectType);

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                string strTheme = GetTheme(objItem.ObjectType);
                if (string.IsNullOrWhiteSpace(strTheme) == false)
                {
                    string strThemePath = @".\TvixTheme\" + strTheme + @"\" + strTheme + ".xml";
                    CreateBackgroundDune((BitmapSource)CreateImage(objItem, strThemePath, mediaPlayerDevice), strPath, mediaPlayerDevice, string.Empty);

                    int index;
                    byte[] cover = RessourcesServices.GetDefaultCover(objItem, out index);
                    if (cover != null)
                        CreateCoverDune(folderCover, cover, strPath, mediaPlayerDevice.FolderFileName);

                    using (StreamWriter file = new StreamWriter(Path.Combine(strPath, mediaPlayerDevice.XmlFileName)))
                    {
                        file.WriteLine("icon_path=" + mediaPlayerDevice.FolderFileName);
                        file.WriteLine("icon_scale_factor=1");
                        file.WriteLine("icon_sel_scale_factor=1.25");
                        file.WriteLine("caption=" + objItem.Title);
                        file.WriteLine("background_path=" + mediaPlayerDevice.BGFileName);
                        file.WriteLine("icon_valign=center");
                        if (objItem.ObjectType != EntityType.Series)
                        {

                            file.WriteLine("background_order=after_all");

                            string path;
                            if (string.IsNullOrWhiteSpace(objItem.FileName) == false)
                                path = Path.Combine(objItem.FilePath, objItem.FileName);
                            else
                                path = objItem.FilePath;

                            if (File.Exists(path))
                            {
                                FileInfo fileName = new FileInfo(path);
                                file.WriteLine("item.0.media_url=./" + fileName.Name);
                            }
                            else if (Directory.Exists(path))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                                List<FileInfo> filesList = directoryInfo.GetFiles().ToList();

                                FileInfo fileName = filesList.FirstOrDefault(x => x.Name.EndsWith(".mkv", StringComparison.CurrentCultureIgnoreCase));
                                if (fileName == null)
                                    fileName = filesList.FirstOrDefault(x => x.Name.EndsWith(".avi", StringComparison.CurrentCultureIgnoreCase));
                                if (fileName == null)
                                    fileName = filesList.FirstOrDefault(x => x.Name.EndsWith(".divx", StringComparison.CurrentCultureIgnoreCase));
                                if (fileName == null)
                                    fileName = filesList.FirstOrDefault(x => x.Name.EndsWith(".iso", StringComparison.CurrentCultureIgnoreCase));

                                if (fileName != null)
                                    file.WriteLine("item.0.media_url=./" + fileName.Name);
                            }
                        }
                        else
                        {
                            file.WriteLine("use_icon_view = yes");
                            file.WriteLine("paint_captions=yes");
                            file.WriteLine("paint_scrollbar=yes");
                            file.WriteLine("paint_help_line=no");
                            file.WriteLine("paint_path_box=no");
                            file.WriteLine("num_cols=4");
                            file.WriteLine("num_rows=3");
                            file.WriteLine("background_order=first");
                            file.WriteLine("content_box_x=110");
                            file.WriteLine("content_box_y=30");
                            file.WriteLine("content_box_width=1700");
                            file.WriteLine("content_box_height=500");
                            file.WriteLine("content_box_padding_left=10");
                            file.WriteLine("content_box_padding_right=10");
                            file.WriteLine("content_box_padding_top=0");
                            file.WriteLine("content_box_padding_bottom=10");
                            file.WriteLine("text_bottom=0");
                            file.WriteLine("caption_font_size=normal");
                        }

                    }

                }
            }
            return strPath;
        }

        private static void CreateBackgroundDune(BitmapSource objImage, string outputPath, MediaPlayerDevices mediaPlayerDevice, string title)
        {
            if (string.IsNullOrWhiteSpace(outputPath) || objImage == null) return;

            if (Directory.Exists(outputPath) == false)
                Directory.CreateDirectory(outputPath);

            string strPath = Path.Combine(outputPath, title + mediaPlayerDevice.BGFileName);

            Util.SaveAsAai(Util.ConvertBitmapSourceToImage(objImage), strPath);

        }
        private static void CreateCoverDune(SizeF folderCover, byte[] cover, string strPath, string fileName)
        {
            MemoryStream objImageMemory = new MemoryStream(cover, 0, cover.Length);
            objImageMemory.Write(cover, 0, cover.Length);
            Image imgCover = Image.FromStream(objImageMemory, true);
            Util.SaveAsAai(imgCover.GetThumbnailImage((int)folderCover.Width, (int)folderCover.Height, null, IntPtr.Zero), Path.Combine(strPath, fileName));
            imgCover.Dispose();
        }
        #endregion
        #region Mede8er

        private static string CreateMede8ErFiles(IMyCollectionsData objItem, string outputPath, MediaPlayerDevices mediaPlayerDevice, EntityType eType)
        {
            SizeF folderCover = new SizeF(mediaPlayerDevice.FolderWidth, mediaPlayerDevice.FolderHeight);

            string strPath = CalculatePath(objItem, outputPath, eType);

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                string strTheme = GetTheme(eType);
                if (string.IsNullOrWhiteSpace(strTheme) == false)
                {
                    string strThemePath = @".\TvixTheme\" + strTheme + @"\" + strTheme + ".xml";
                    CreatePreviewTvix((BitmapSource)CreateImage(objItem, strThemePath, mediaPlayerDevice), strPath, mediaPlayerDevice);

                    int index;
                    byte[] cover = RessourcesServices.GetDefaultCover(objItem, out index);
                    if (cover != null)
                        CreateCover(folderCover, cover, strPath, mediaPlayerDevice.FolderFileName);
                }
            }
            return strPath;
        }
        #endregion
        #region MyMovies

        private static string CreateMyMoviesFiles(IMyCollectionsData objItem, string outputPath, MediaPlayerDevices mediaPlayerDevice, EntityType eType)
        {
            SizeF folderCover = new SizeF(mediaPlayerDevice.FolderWidth, mediaPlayerDevice.FolderHeight);
            SizeF folder2Cover = new SizeF(mediaPlayerDevice.Folder2Width, mediaPlayerDevice.Folder2Height);

            string strPath = CalculatePath(objItem, outputPath, eType);

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                int index;
                byte[] cover = RessourcesServices.GetDefaultCover(objItem, out index);
                if (cover != null)
                {
                    CreateCover(folderCover, cover, strPath, mediaPlayerDevice.FolderFileName);
                    CreateCover(folder2Cover, cover, strPath, mediaPlayerDevice.Folder2FileName);
                }

                string xmlpath = Path.Combine(strPath, mediaPlayerDevice.XmlFileName);
                using (FileStream fs = File.Create(xmlpath, 655360, FileOptions.WriteThrough))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    settings.NewLineOnAttributes = true;
                    settings.OmitXmlDeclaration = true;

                    XmlWriter writer = XmlWriter.Create(fs, settings);

                    writer.WriteStartElement("Title");
                    writer.WriteElementString("LocalTitle", objItem.Title);
                    writer.WriteElementString("SortTitle", objItem.Title);
                    writer.WriteElementString("OriginalTitle", objItem.OriginalTitle);
                    writer.WriteElementString("TMDbId", "");
                    writer.WriteElementString("IMDB", "");
                    writer.WriteElementString("Description", objItem.Description);
                    writer.WriteElementString("IMDBrating", objItem.PublicRating.ToString());

                    if (objItem.ReleaseDate != null)
                        writer.WriteElementString("ProductionYear", objItem.ReleaseDate.Value.Year.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("ProductionYear", "");

                    if (objItem.Runtime != null)
                        writer.WriteElementString("RunningTime", objItem.Runtime.Value.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("RunningTime", "");

                    writer.WriteStartElement("Genres");
                    foreach (string genre in GenreServices.GetGenres(objItem))
                    {
                        writer.WriteElementString("Genre", genre);
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("Studios");
                    writer.WriteElementString("Studio", objItem.Publisher.Name);
                    writer.WriteEndElement();

                    writer.WriteStartElement("Persons");
                    foreach (string person in ArtistServices.GetCast(objItem))
                    {
                        writer.WriteStartElement("Person");
                        writer.WriteElementString("Name", person);
                        writer.WriteElementString("Type", "Actor");
                        writer.WriteElementString("Role", "");
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("Covers");
                    writer.WriteElementString("Front", mediaPlayerDevice.FolderFileName);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.Flush();

                }
            }
            return strPath;
        }
        #endregion
        #region TVIX
        public static string CreatePreviewTvix(BitmapSource objImage, string outputPath, MediaPlayerDevices mediaPlayerDevice)
        {
            if (string.IsNullOrWhiteSpace(outputPath) || objImage == null) return null;

            if (Directory.Exists(outputPath) == false)
                Directory.CreateDirectory(outputPath);

            //FIX 2.8.3.0
            string path;
            if (string.IsNullOrWhiteSpace(mediaPlayerDevice.BGFileName))
                path = Path.Combine(outputPath, "background.jpg");
            else
                path = Path.Combine(outputPath, mediaPlayerDevice.BGFileName);

            FileStream stream = new FileStream(path, FileMode.Create);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(objImage));
            encoder.Save(stream);
            stream.Close();
            return outputPath;

        }

        private static string CreateTvixFile(IMyCollectionsData objItem, string outputPath, MediaPlayerDevices mediaPlayerDevice, EntityType eType)
        {

            SizeF folderCover = new SizeF(mediaPlayerDevice.FolderWidth, mediaPlayerDevice.FolderHeight);

            string strPath = CalculatePath(objItem, outputPath, eType);

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                string strTheme = GetTheme(eType);
                if (string.IsNullOrWhiteSpace(strTheme) == false)
                {
                    string strThemePath = @".\TvixTheme\" + strTheme + @"\" + strTheme + ".xml";
                    CreatePreviewTvix((BitmapSource)CreateImage(objItem, strThemePath, mediaPlayerDevice), strPath, mediaPlayerDevice);

                    int index;
                    byte[] cover = RessourcesServices.GetDefaultCover(objItem, out index);
                    if (cover != null)
                        CreateCover(folderCover, cover, strPath, mediaPlayerDevice.FolderFileName);
                }
            }
            return strPath;
        }
        #endregion
        #region WDHDTV

        private static string CreateWdhdtvFiles(IMyCollectionsData objItem, string outputPath, MediaPlayerDevices mediaPlayerDevice, EntityType eType)
        {
            SizeF folderCover = new SizeF(mediaPlayerDevice.FolderWidth, mediaPlayerDevice.FolderHeight);

            string strPath = CalculatePath(objItem, outputPath, eType);

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                string strTheme = GetTheme(eType);
                if (string.IsNullOrWhiteSpace(strTheme) == false)
                {
                    string strThemePath = @".\TvixTheme\" + strTheme + @"\" + strTheme + ".xml";
                    CreatePreviewTvix((BitmapSource)CreateImage(objItem, strThemePath, mediaPlayerDevice), strPath, mediaPlayerDevice);

                    int index;
                    byte[] cover = RessourcesServices.GetDefaultCover(objItem, out index);
                    if (cover != null)
                        CreateCover(folderCover, cover, strPath, mediaPlayerDevice.FolderFileName);
                }
            }
            return strPath;
        }
        #endregion
        #region WMC

        private static string CreateWmcFiles(IMyCollectionsData objItem, string outputPath, MediaPlayerDevices mediaPlayerDevice, EntityType eType)
        {
            SizeF folderCover = new SizeF(mediaPlayerDevice.FolderWidth, mediaPlayerDevice.FolderHeight);
            SizeF folder2Cover = new SizeF(mediaPlayerDevice.Folder2Width, mediaPlayerDevice.Folder2Height);

            string strPath = CalculatePath(objItem, outputPath, eType);

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                int index;
                byte[] cover = RessourcesServices.GetDefaultCover(objItem, out index);
                if (cover != null)
                {
                    CreateCover(folderCover, cover, strPath, mediaPlayerDevice.FolderFileName);
                    CreateCover(folder2Cover, cover, strPath, mediaPlayerDevice.Folder2FileName);
                }

                byte[] background = RessourcesServices.GetBackground(objItem);
                if (background != null)
                    CreateCover(new SizeF(mediaPlayerDevice.BGWidth, mediaPlayerDevice.BGHeight), background, strPath, mediaPlayerDevice.BGFileName);

                string xmlpath = Path.Combine(strPath, objItem.Title + mediaPlayerDevice.XmlFileName);
                using (FileStream fs = File.Create(xmlpath, 655360, FileOptions.WriteThrough))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    settings.NewLineOnAttributes = true;

                    XmlWriter writer = XmlWriter.Create(fs, settings);

                    writer.WriteStartDocument();
                    writer.WriteStartElement("Disc");
                    writer.WriteElementString("Name", objItem.Title);
                    writer.WriteElementString("ID", objItem.Id.Substring(0, 8) + "|" + objItem.Id.Substring(objItem.Id.Length - 8, 8));
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();

                }
            }
            return strPath;
        }
        #endregion
        #region XBMC

        private static string CreateXbmcFiles(IMyCollectionsData objItem, string outputPath, MediaPlayerDevices mediaPlayerDevice, EntityType eType)
        {
            string strPath = CalculatePath(objItem, outputPath, eType);

            switch (eType)
            {
                case EntityType.Movie:
                    return CreateXbmcMovies(objItem as Movie, strPath, mediaPlayerDevice);
                case EntityType.Series:
                    return CreateXbmcSeries(objItem as SeriesSeason, strPath, mediaPlayerDevice);
                case EntityType.Music:
                    return CreateXbmcMusic(objItem as Music, strPath, mediaPlayerDevice);
                case EntityType.XXX:
                    return CreateXbmcXxx(objItem as XXX, strPath, mediaPlayerDevice);

            }


            return string.Empty;
        }

        private static string CreateXbmcMovies(Movie objItem, string strPath, MediaPlayerDevices mediaPlayerDevice)
        {
            const string startDocument = "movie";

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                string actorPath = Path.Combine(strPath, ".actors");
                string mainName;

                if (string.IsNullOrWhiteSpace(objItem.FileName))
                    mainName = objItem.Title;
                else
                    mainName = objItem.FileName;

                Directory.CreateDirectory(actorPath);
                string xmlpath = Path.Combine(strPath, mainName + mediaPlayerDevice.XmlFileName);
                using (FileStream fs = File.Create(xmlpath, 655360, FileOptions.WriteThrough))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    settings.NewLineOnAttributes = true;
                    settings.OmitXmlDeclaration = true;

                    XmlWriter writer = XmlWriter.Create(fs, settings);

                    writer.WriteStartElement(startDocument);
                    writer.WriteElementString("title", objItem.Title);
                    writer.WriteElementString("originaltitle", objItem.OriginalTitle);
                    writer.WriteElementString("sorttitle", objItem.Title);
                    writer.WriteElementString("rating", objItem.PublicRating.ToString());

                    if (objItem.ReleaseDate != null)
                        writer.WriteElementString("year", objItem.ReleaseDate.Value.Year.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("year", "");

                    writer.WriteElementString("plot", objItem.Description);

                    if (objItem.Runtime != null)
                        writer.WriteElementString("runtime", objItem.Runtime.Value.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("runtime", "");

                    writer.WriteElementString("watched", objItem.Watched.ToString());
                    writer.WriteElementString("id", objItem.Id);
                    writer.WriteElementString("filenameandpath", Path.Combine(objItem.FilePath, mainName));

                    foreach (string genre in GenreServices.GetGenres(objItem))
                        writer.WriteElementString("genre", genre);

                    WriteXbmcCommonValue(objItem, strPath, mediaPlayerDevice, writer, actorPath, mainName);

                    writer.WriteEndElement();
                    writer.Flush();

                }
            }
            return strPath;
        }

        private static void WriteXbmcCommonValue(IMyCollectionsData objItem, string strPath, MediaPlayerDevices mediaPlayerDevice, XmlWriter writer, string actorPath, string mainName)
        {
            foreach (Artist person in objItem.Artists)
            {
                writer.WriteStartElement("actor");
                writer.WriteElementString("name", person.FulleName.Replace(" ", "_"));
                writer.WriteElementString("role", "");

                if (person.Picture != null)
                    writer.WriteElementString("thumb", string.Format(@".\.actors\{0}.jpg", person.FulleName.Replace(" ", "_")));

                writer.WriteEndElement();

                if (person.Picture != null)
                    CreateCover(new SizeF(400, 600), person.Picture, actorPath, person.FulleName + ".jpg");
            }

            int index;
            byte[] cover = RessourcesServices.GetDefaultCover(objItem, out index);
            if (cover != null)
                CreateCover(new SizeF(mediaPlayerDevice.FolderWidth, mediaPlayerDevice.FolderHeight), cover, strPath,
                    mainName + "-poster.jpg");

            cover = RessourcesServices.GetBackground(objItem);
            if (cover != null)
                CreateCover(new SizeF(mediaPlayerDevice.BGWidth, mediaPlayerDevice.BGHeight), cover, strPath,
                    mainName + "-fanart.jpg");
        }

        private static string CreateXbmcSeries(SeriesSeason objItem, string strPath, MediaPlayerDevices mediaPlayerDevice)
        {
            const string startDocument = "tvshow";

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                string actorPath = Path.Combine(strPath, ".actors");
                string mainName;

                if (string.IsNullOrWhiteSpace(objItem.FileName))
                    mainName = objItem.Title;
                else
                    mainName = objItem.FileName;

                Directory.CreateDirectory(actorPath);
                string xmlpath = Path.Combine(strPath, mainName + mediaPlayerDevice.XmlFileName);
                using (FileStream fs = File.Create(xmlpath, 655360, FileOptions.WriteThrough))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    settings.NewLineOnAttributes = true;
                    settings.OmitXmlDeclaration = true;

                    XmlWriter writer = XmlWriter.Create(fs, settings);

                    writer.WriteStartElement(startDocument);
                    writer.WriteElementString("title", objItem.Title);
                    writer.WriteElementString("rating", objItem.PublicRating.ToString());

                    if (objItem.ReleaseDate != null)
                        writer.WriteElementString("year", objItem.ReleaseDate.Value.Year.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("year", "");

                    writer.WriteElementString("season", "-" + objItem.Season.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("episode", objItem.TotalEpisodes.ToString());
                    writer.WriteElementString("plot", objItem.Description);

                    if (objItem.Runtime != null)
                        writer.WriteElementString("runtime", objItem.Runtime.Value.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("runtime", "");

                    writer.WriteElementString("watched", objItem.Watched.ToString());
                    writer.WriteElementString("id", objItem.Id);
                    writer.WriteElementString("studio", objItem.Publisher.Name);

                    foreach (string genre in GenreServices.GetGenres(objItem))
                        writer.WriteElementString("genre", genre);

                    WriteXbmcCommonValue(objItem, strPath, mediaPlayerDevice, writer, actorPath, mainName);

                    writer.WriteEndElement();
                    writer.Flush();

                }
            }
            return strPath;
        }

        private static string CreateXbmcMusic(Music objItem, string strPath, MediaPlayerDevices mediaPlayerDevice)
        {
            const string startDocument = "album";

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                string actorPath = Path.Combine(strPath, ".actors");
                string mainName;

                if (string.IsNullOrWhiteSpace(objItem.FileName))
                    mainName = objItem.Title;
                else
                    mainName = objItem.FileName;

                Directory.CreateDirectory(actorPath);
                string xmlpath = Path.Combine(strPath, mainName + mediaPlayerDevice.XmlFileName);
                using (FileStream fs = File.Create(xmlpath, 655360, FileOptions.WriteThrough))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    settings.NewLineOnAttributes = true;
                    settings.OmitXmlDeclaration = true;

                    XmlWriter writer = XmlWriter.Create(fs, settings);

                    writer.WriteStartElement(startDocument);
                    writer.WriteElementString("title", objItem.Title);
                    writer.WriteElementString("artist", objItem.Artists.First().FulleName);
                    writer.WriteElementString("album", objItem.Album);
                    writer.WriteElementString("rating", objItem.PublicRating.ToString());

                    if (objItem.ReleaseDate != null)
                        writer.WriteElementString("year", objItem.ReleaseDate.Value.Year.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("year", "");

                    writer.WriteElementString("plot", objItem.Description);

                    if (objItem.Runtime != null)
                        writer.WriteElementString("runtime", objItem.Runtime.Value.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("runtime", "");

                    writer.WriteElementString("id", objItem.Id);

                    foreach (string genre in GenreServices.GetGenres(objItem))
                        writer.WriteElementString("genre", genre);

                    if (objItem.Publisher != null)
                        writer.WriteElementString("studio", objItem.Publisher.Name);

                    WriteXbmcCommonValue(objItem, strPath, mediaPlayerDevice, writer, actorPath, mainName);

                    writer.WriteEndElement();
                    writer.Flush();

                }
            }
            return strPath;
        }

        private static string CreateXbmcXxx(XXX objItem, string strPath, MediaPlayerDevices mediaPlayerDevice)
        {
            const string startDocument = "movie";

            if (!string.IsNullOrWhiteSpace(strPath))
            {
                string actorPath = Path.Combine(strPath, ".actors");
                string mainName;

                if (string.IsNullOrWhiteSpace(objItem.FileName))
                    mainName = objItem.Title;
                else
                    mainName = objItem.FileName;

                Directory.CreateDirectory(actorPath);
                string xmlpath = Path.Combine(strPath, mainName + mediaPlayerDevice.XmlFileName);
                using (FileStream fs = File.Create(xmlpath, 655360, FileOptions.WriteThrough))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    settings.NewLineOnAttributes = true;
                    settings.OmitXmlDeclaration = true;

                    XmlWriter writer = XmlWriter.Create(fs, settings);

                    writer.WriteStartElement(startDocument);
                    writer.WriteElementString("title", objItem.Title);
                    writer.WriteElementString("sorttitle", objItem.Title);
                    writer.WriteElementString("rating", objItem.PublicRating.ToString());

                    if (objItem.ReleaseDate != null)
                        writer.WriteElementString("year", objItem.ReleaseDate.Value.Year.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("year", "");

                    writer.WriteElementString("plot", objItem.Description);

                    if (objItem.Runtime != null)
                        writer.WriteElementString("runtime", objItem.Runtime.Value.ToString(CultureInfo.InvariantCulture));
                    else
                        writer.WriteElementString("runtime", "");

                    writer.WriteElementString("watched", objItem.Watched.ToString());
                    writer.WriteElementString("id", objItem.Id);
                    writer.WriteElementString("filenameandpath", Path.Combine(objItem.FilePath, mainName));

                    foreach (string genre in GenreServices.GetGenres(objItem))
                        writer.WriteElementString("genre", genre);

                    WriteXbmcCommonValue(objItem, strPath, mediaPlayerDevice, writer, actorPath, mainName);

                    writer.WriteEndElement();
                    writer.Flush();

                }
            }
            return strPath;
        }

        #endregion
        #region Common
        private static string CalculatePath(IMyCollectionsData objItem, string outputPath, EntityType eType)
        {
            string strPath;

            if (Convert.ToBoolean(MySettings.TvixInFolder) == false)
            {
                strPath = Path.Combine(outputPath, Util.PurgeTitle(objItem.Title));
                if (Directory.Exists(strPath) == false)
                    Directory.CreateDirectory(strPath);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(objItem.FilePath) && !string.IsNullOrWhiteSpace(objItem.FileName))
                {
                    strPath = Path.Combine(objItem.FilePath, objItem.FileName);
                    //Fix Since 2.6.0.0
                    if (!string.IsNullOrWhiteSpace(strPath))
                        if (Directory.Exists(strPath) == false && File.Exists(strPath) == false)
                            Directory.CreateDirectory(strPath);
                        else if (File.Exists(strPath) == true)
                        {
                            FileInfo file = new FileInfo(strPath);
                            strPath = file.DirectoryName;
                            if (strPath != null && Directory.Exists(strPath))
                                Directory.CreateDirectory(strPath);
                        }
                }
                else if (eType == EntityType.Series && string.IsNullOrWhiteSpace(objItem.FilePath) == false)
                {
                    strPath = objItem.FilePath;
                    if (Directory.Exists(strPath) == false && File.Exists(strPath) == false)
                        Directory.CreateDirectory(strPath);
                    else if (File.Exists(strPath) == true)
                    {
                        FileInfo file = new FileInfo(strPath);
                        strPath = file.DirectoryName;
                        if (strPath != null && Directory.Exists(strPath))
                            Directory.CreateDirectory(strPath);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(outputPath))
                        outputPath = Environment.CurrentDirectory;

                    strPath = Path.Combine(outputPath, Util.PurgeTitle(objItem.Title));
                    if (Directory.Exists(strPath) == false)
                        Directory.CreateDirectory(strPath);
                }
            }

            return strPath;
        }
        private static void CreateCover(SizeF folderCover, byte[] cover, string strPath, string fileName)
        {
            MemoryStream objImageMemory = new MemoryStream(cover, 0, cover.Length);
            objImageMemory.Write(cover, 0, cover.Length);
            Image imgCover = Image.FromStream(objImageMemory, true);

            Bitmap objOutput = new Bitmap((int)folderCover.Width, (int)folderCover.Height);
            Graphics objCanvas = Graphics.FromImage(objOutput);
            objCanvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
            objCanvas.DrawImage(imgCover, new RectangleF(new PointF(0, 0), folderCover));
            objCanvas.Save();
            objOutput.Save(Path.Combine(strPath, fileName), ImageFormat.Jpeg);

            imgCover.Dispose();
        }
        public static string CreateFiles(string id, string outputPath, EntityType eType)
        {
            IMyCollectionsData objItem = null;
            switch (eType)
            {
                case EntityType.Games:
                    objItem = Dal.GetInstance.GetGames(id);
                    break;
                case EntityType.Movie:
                    objItem = Dal.GetInstance.GetMovies(id);
                    break;
                case EntityType.Music:
                    objItem = Dal.GetInstance.GetMusics(id);
                    break;
                case EntityType.Series:
                    objItem = Dal.GetInstance.GetSeries_Seasons(id);
                    break;
                case EntityType.XXX:
                    objItem = Dal.GetInstance.GetXxXs(id);
                    break;
            }
            if (objItem != null)
            {

                MediaPlayerDevices mediaPlayerDevice = DevicesServices.GetDevice();

                string info = string.Format(@"Title : {0} Device : {1} Type : {2}", objItem.Title,
                    mediaPlayerDevice.Name, objItem.ObjectType);

                Task.Factory.StartNew(() => Util.NotifyEvent(@"GenerateCovers: " + info));

                switch (mediaPlayerDevice.Name)
                {
                    case SupportedDevice.Dune:
                        return CreateDuneFiles(objItem, outputPath, mediaPlayerDevice);
                    case SupportedDevice.Mede8er:
                        return CreateMede8ErFiles(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.MyMovies:
                        return CreateMyMoviesFiles(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.Tvix:
                        return CreateTvixFile(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.WindowsMediaCenter:
                        return CreateWmcFiles(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.WDHDTV:
                        return CreateWdhdtvFiles(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.XBMC:
                        return CreateXbmcFiles(objItem, outputPath, mediaPlayerDevice, eType);
                }
            }

            return string.Empty;
        }
        public static string CreateFiles(IMyCollectionsData objItem, string outputPath, EntityType eType)
        {
            if (objItem != null)
            {
                MediaPlayerDevices mediaPlayerDevice = DevicesServices.GetDevice();

                string info = string.Format(@"Title : {0} Device : {1} Type : {2}", objItem.Title,
                    mediaPlayerDevice.Name, objItem.ObjectType);

                Task.Factory.StartNew(() => Util.NotifyEvent(@"GenerateCovers: " + info));

                switch (mediaPlayerDevice.Name)
                {
                    case SupportedDevice.Dune:
                        return CreateDuneFiles(objItem, outputPath, mediaPlayerDevice);
                    case SupportedDevice.Mede8er:
                        return CreateMede8ErFiles(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.MyMovies:
                        return CreateMyMoviesFiles(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.Tvix:
                        return CreateTvixFile(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.WindowsMediaCenter:
                        return CreateWmcFiles(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.WDHDTV:
                        return CreateWdhdtvFiles(objItem, outputPath, mediaPlayerDevice, eType);
                    case SupportedDevice.XBMC:
                        return CreateXbmcFiles(objItem, outputPath, mediaPlayerDevice, eType);
                }
            }

            return string.Empty;
        }


        public static ImageSource CreateImage(IMyCollectionsData objItem, string strThemePath, MediaPlayerDevices mediaPlayerDevice)
        {
            string releasedDate = string.Empty;

            if (objItem != null)
            {
                int index;
                IList<string> cast = null;
                byte[] cover = RessourcesServices.GetDefaultCover(objItem, out index);

                string title = objItem.Title;
                string description = objItem.Description;
                string originalTitle = objItem.OriginalTitle;

                byte[] background = RessourcesServices.GetBackground(objItem);
                IList<string> genres = GenreServices.GetGenres(objItem);

                if (objItem.ObjectType != EntityType.Games && objItem.ObjectType != EntityType.Nds)
                    cast = ArtistServices.GetCast(objItem);

                string rating = string.Empty;
                if (objItem.PublicRating.HasValue == true)
                    rating = objItem.PublicRating.Value.ToString(CultureInfo.InvariantCulture);
                else if (objItem.MyRating.HasValue == true)
                    rating = objItem.MyRating.Value.ToString(CultureInfo.InvariantCulture);

                if (objItem.ReleaseDate != null)
                    releasedDate = objItem.ReleaseDate.Value.ToShortDateString();

                string runTime = string.Empty;
                if (objItem.Runtime != null && objItem.Runtime != 0)
                    runTime = objItem.Runtime.ToString();

                return CreateBackgroundImage(cover, title, originalTitle, description, genres,
                                    runTime, rating, background, strThemePath, releasedDate, cast, mediaPlayerDevice, objItem.ObjectType);
            }

            return null;
        }
        private static ImageSource CreateBackgroundImage(byte[] cover, string title, string originalTitle,
                                           string description, IList<string> genres, string runTime, string rating,
                                           byte[] background, string strThemePath, string releaseDate, IList<string> cast,
                                          MediaPlayerDevices mediaPlayerDevice, EntityType entityType)
        {
            Image imgCover = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Theme));
            using (TextReader reader = new StreamReader(strThemePath))
            {
                Theme objTheme = (Theme)serializer.Deserialize(reader);

                //Total Size
                int totalHeight = mediaPlayerDevice.BGHeight;
                int totalWidth = mediaPlayerDevice.BGWidth;

                #region BackGround
                Image imgBackground;
                MemoryStream objImageMemory;
                if (background == null || objTheme.Elements.Background.visible == false)
                    imgBackground = Image.FromFile(@".\Images\BlackBackground_1700_955.jpg");
                else if (objTheme.Elements.Background.useItemBackground == true)
                {
                    objImageMemory = new MemoryStream(background, 0, background.Length);
                    objImageMemory.Write(background, 0, background.Length);
                    imgBackground = Image.FromStream(objImageMemory, true);
                }
                else if (string.IsNullOrWhiteSpace(objTheme.Elements.Background.defaultBackground) == false)
                    imgBackground = Image.FromFile(objTheme.Elements.Background.defaultBackground);
                else
                    imgBackground = Image.FromFile(@".\Images\BlackBackground_1700_955.jpg");

                Bitmap objOutput;
                if (totalWidth == 0 && totalHeight == 0)
                    objOutput = new Bitmap(1280, 720);
                else
                    objOutput = new Bitmap(totalWidth, totalHeight);

                Graphics objCanvas = Graphics.FromImage(objOutput);
                objCanvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                objCanvas.DrawImage(imgBackground, new Rectangle(0, 0, totalWidth, totalHeight),
                                    new Rectangle(0, 0, imgBackground.Width, imgBackground.Height), GraphicsUnit.Pixel);
                #endregion
                #region SeparatorImage
                if (objTheme.Separators.Image != null)
                {
                    foreach (ThemeSeparatorsImage item in objTheme.Separators.Image)
                    {
                        Image imgSeparator = Image.FromFile(item.path);

                        float objFloat;
                        ImageAttributes imgAttributes = null;
                        if (float.TryParse(item.Opacity, out objFloat))
                        {
                            ColorMatrix objMatrix = new ColorMatrix();
                            objMatrix.Matrix33 = (objFloat) / 100;

                            imgAttributes = new ImageAttributes();
                            imgAttributes.SetColorMatrix(objMatrix);
                        }

                        objCanvas.DrawImage(imgSeparator, new Rectangle((int)(item.horisontalPos * mediaPlayerDevice.BGRatio), (int)(item.verticalPos * mediaPlayerDevice.BGRatio),
                                                                        (int)(item.width * mediaPlayerDevice.BGRatio), (int)(item.height * mediaPlayerDevice.BGRatio)),
                                                                        0, 0, item.width, item.height,
                                                                        GraphicsUnit.Pixel, imgAttributes);

                    }
                }
                #endregion
                #region SeparatorRectangle
                if (objTheme.Separators.Rectangle != null)
                {
                    foreach (ThemeSeparatorsRectangle item in objTheme.Separators.Rectangle)
                    {
                        Rectangle objRectangle = new Rectangle((int)(item.horisontalPos * mediaPlayerDevice.BGRatio), (int)(item.verticalPos * mediaPlayerDevice.BGRatio), totalWidth, totalHeight);

                        if (item.SolidBrushColor != 0)
                        {
                            SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(item.SolidBrushColor));
                            objCanvas.FillRectangle(drawBrush, objRectangle);
                        }
                        else if (item.LinearGradientBrushColor1 != 0 && item.LinearGradientBrushColor2 != 0 &&
                             string.IsNullOrWhiteSpace(item.LinearGradientBrushDirection) == false)
                        {
                            System.Drawing.Drawing2D.LinearGradientBrush gradientBrush;
                            if (item.LinearGradientBrushDirection == "Vertical")
                            {
                                gradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(objRectangle,
                                   System.Drawing.Color.FromArgb(item.LinearGradientBrushColor1),
                                   System.Drawing.Color.FromArgb(item.LinearGradientBrushColor2),
                                   LinearGradientMode.Vertical);
                            }
                            else
                            {
                                gradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(objRectangle,
                                  System.Drawing.Color.FromArgb(item.LinearGradientBrushColor1),
                                  System.Drawing.Color.FromArgb(item.LinearGradientBrushColor2),
                                  LinearGradientMode.Horizontal);
                            }
                            objCanvas.FillRectangle(gradientBrush, objRectangle);
                        }
                    }
                }
                #endregion
                #region Title
                if (objTheme.Elements.Title.visible == true)
                {

                    PointF titlePosition = new PointF((float)(objTheme.Elements.Title.horisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Title.verticalPos * mediaPlayerDevice.BGRatio));

                    Font drawFont;
                    if (objTheme.Elements.Title.fontBold == false)
                        drawFont = new Font(objTheme.Elements.Title.fontName, (float)(objTheme.Elements.Title.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.Title.fontName, (float)(objTheme.Elements.Title.fontSize * mediaPlayerDevice.BGRatio));

                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Title.foreColor));
                    objCanvas.DrawString(title, drawFont, drawBrush, titlePosition);
                }
                #endregion
                #region OriginalTitle
                if (objTheme.Elements.OriginalTitle.visible == true && string.IsNullOrEmpty(originalTitle) == false)
                {
                    PointF originalTitlePosition = new PointF((float)(objTheme.Elements.OriginalTitle.horisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.OriginalTitle.verticalPos * mediaPlayerDevice.BGRatio));
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.OriginalTitle.foreColor));

                    Font drawFont;
                    if (objTheme.Elements.OriginalTitle.fontBold == false)
                        drawFont = new Font(objTheme.Elements.OriginalTitle.fontName, (float)(objTheme.Elements.OriginalTitle.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.OriginalTitle.fontName, (float)(objTheme.Elements.OriginalTitle.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(originalTitle, drawFont, drawBrush, originalTitlePosition);
                }
                #endregion
                #region Cover
                if (cover != null && objTheme.Elements.Cover.visible == true)
                {
                    objImageMemory = new MemoryStream(cover, 0, cover.Length);
                    objImageMemory.Write(cover, 0, cover.Length);
                    imgCover = Image.FromStream(objImageMemory, true);

                    PointF coverPosition = new PointF((float)(objTheme.Elements.Cover.horisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Cover.verticalPos * mediaPlayerDevice.BGRatio));
                    SizeF coverSize = new SizeF((float)(objTheme.Elements.Cover.width * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Cover.height * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawImage(imgCover, new RectangleF(coverPosition, coverSize));
                }
                #endregion
                #region Description
                if (string.IsNullOrEmpty(description) == false)
                {
                    PointF descriptionPosition = new PointF((float)(objTheme.Elements.Description.horisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Description.verticalPos * mediaPlayerDevice.BGRatio));
                    SizeF descriptionSize = new SizeF((float)(objTheme.Elements.Description.width * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Description.height * mediaPlayerDevice.BGRatio));
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Description.foreColor));
                    Font drawFont;
                    if (objTheme.Elements.Description.fontBold == false)
                        drawFont = new Font(objTheme.Elements.Description.fontName, (float)(objTheme.Elements.Description.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.Description.fontName, (float)(objTheme.Elements.Description.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(description, drawFont, drawBrush, new RectangleF(descriptionPosition, descriptionSize));
                }
                #endregion
                #region Genres
                if (string.IsNullOrEmpty(objTheme.Elements.Genres.title) == false)
                {
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Genres.titleForeColor));
                    PointF position = new PointF((float)(objTheme.Elements.Genres.titleHorisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Genres.titleVerticalPos * mediaPlayerDevice.BGRatio));

                    Font drawFont;

                    if (objTheme.Elements.Genres.fontBold == false)
                        drawFont = new Font(objTheme.Elements.Genres.fontName, (float)(objTheme.Elements.Genres.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.Genres.fontName, (float)(objTheme.Elements.Genres.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(objTheme.Elements.Genres.title, drawFont, drawBrush, position);
                }

                if (genres != null)
                {
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Genres.foreColor));
                    PointF position = new PointF((float)(objTheme.Elements.Genres.horisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Genres.verticalPos * mediaPlayerDevice.BGRatio));
                    Font drawFont;

                    if (objTheme.Elements.Genres.fontBold == false)
                        drawFont = new Font(objTheme.Elements.Genres.fontName, (float)(objTheme.Elements.Genres.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.Genres.fontName, (float)(objTheme.Elements.Genres.fontSize * mediaPlayerDevice.BGRatio));

                    if (objTheme.Elements.Genres.Orientation == "H")
                    {
                        StringBuilder strGenres = new StringBuilder();
                        for (int i = 0; i < objTheme.Elements.Genres.MaximumValues; i++)
                        {
                            if (i < genres.Count)
                            {
                                strGenres.Append(genres[i]);

                                if (i < objTheme.Elements.Genres.MaximumValues - 1 && i < genres.Count - 1)
                                    strGenres.Append(objTheme.Elements.Genres.Separator);
                            }
                        }
                        objCanvas.DrawString(strGenres.ToString(), drawFont, drawBrush, position);
                    }
                    else
                    {
                        for (int i = 0; i < objTheme.Elements.Genres.MaximumValues; i++)
                        {
                            position = new PointF((float)(objTheme.Elements.Genres.horisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Genres.verticalPos * mediaPlayerDevice.BGRatio) + (i * 20));

                            if (i < genres.Count)
                                objCanvas.DrawString(genres[i], drawFont, drawBrush, position);
                        }
                    }
                }
                #endregion
                #region RunTime
                if (string.IsNullOrWhiteSpace(objTheme.Elements.Runtime.title) == false && string.IsNullOrEmpty(runTime) == false)
                {
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Runtime.titleForeColor));
                    PointF position = new PointF((float)(objTheme.Elements.Runtime.titleHorisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Runtime.titleVerticalPos * mediaPlayerDevice.BGRatio));

                    Font drawFont;

                    if (objTheme.Elements.RelaeseDate.fontBold == false)
                        drawFont = new Font(objTheme.Elements.Runtime.fontName, (float)(objTheme.Elements.Runtime.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.Runtime.fontName, (float)(objTheme.Elements.Runtime.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(objTheme.Elements.Runtime.title, drawFont, drawBrush, position);
                }

                if (string.IsNullOrWhiteSpace(runTime) == false)
                {
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Runtime.foreColor));
                    PointF runTimePosition = new PointF((float)(objTheme.Elements.Runtime.horisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Runtime.verticalPos * mediaPlayerDevice.BGRatio));

                    Font drawFont;
                    if (objTheme.Elements.Runtime.fontBold == false)
                        drawFont = new Font(objTheme.Elements.Runtime.fontName, (float)(objTheme.Elements.Runtime.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.Runtime.fontName, (float)(objTheme.Elements.Runtime.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(runTime, drawFont, drawBrush, runTimePosition);
                }
                #endregion
                #region Rating

                if (string.IsNullOrEmpty(objTheme.Elements.Rating.title) == false)
                {
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Rating.titleForeColor));
                    PointF ratingPosition = new PointF((float)(objTheme.Elements.Rating.titleHorisontalPos * mediaPlayerDevice.BGRatio),
                                                        (float)(objTheme.Elements.Rating.titleVerticalPos * mediaPlayerDevice.BGRatio));

                    Font drawFont;

                    if (objTheme.Elements.Rating.fontBold == false)
                        drawFont = new Font(objTheme.Elements.Rating.fontName, (float)(objTheme.Elements.Rating.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.Rating.fontName, (float)(objTheme.Elements.Rating.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(objTheme.Elements.Rating.title, drawFont, drawBrush, ratingPosition);
                }

                if (string.IsNullOrEmpty(rating) == false)
                {
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Rating.foreColor));
                    PointF ratingPosition = new PointF((float)(objTheme.Elements.Rating.horisontalPos * mediaPlayerDevice.BGRatio),
                                                        (float)(objTheme.Elements.Rating.verticalPos * mediaPlayerDevice.BGRatio));

                    Font drawFont;

                    if (objTheme.Elements.Rating.fontBold == false)
                        drawFont = new Font(objTheme.Elements.Rating.fontName, (float)(objTheme.Elements.Rating.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.Rating.fontName, (float)(objTheme.Elements.Rating.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(rating, drawFont, drawBrush, ratingPosition);
                }
                #endregion
                #region RealaseDate

                if (string.IsNullOrEmpty(objTheme.Elements.RelaeseDate.title) == false)
                {
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.RelaeseDate.titleForeColor));
                    PointF position = new PointF((float)(objTheme.Elements.RelaeseDate.titleHorisontalPos * mediaPlayerDevice.BGRatio),
                                                    (float)(objTheme.Elements.RelaeseDate.titleVerticalPos * mediaPlayerDevice.BGRatio));

                    Font drawFont;

                    if (objTheme.Elements.RelaeseDate.fontBold == false)
                        drawFont = new Font(objTheme.Elements.RelaeseDate.fontName, (float)(objTheme.Elements.RelaeseDate.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.RelaeseDate.fontName, (float)(objTheme.Elements.RelaeseDate.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(objTheme.Elements.RelaeseDate.title, drawFont, drawBrush, position);
                }

                if (string.IsNullOrEmpty(releaseDate) == false)
                {
                    SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.RelaeseDate.foreColor));
                    PointF position = new PointF((float)(objTheme.Elements.RelaeseDate.horisontalPos * mediaPlayerDevice.BGRatio),
                                                (float)(objTheme.Elements.RelaeseDate.verticalPos * mediaPlayerDevice.BGRatio));

                    Font drawFont;

                    if (objTheme.Elements.Rating.fontBold == false)
                        drawFont = new Font(objTheme.Elements.RelaeseDate.fontName, (float)(objTheme.Elements.RelaeseDate.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                    else
                        drawFont = new Font(objTheme.Elements.RelaeseDate.fontName, (float)(objTheme.Elements.RelaeseDate.fontSize * mediaPlayerDevice.BGRatio));

                    objCanvas.DrawString(releaseDate, drawFont, drawBrush, position);
                }
                #endregion
                #region Actors

                if (entityType != EntityType.Music && entityType != EntityType.Games)
                {
                    if (string.IsNullOrEmpty(objTheme.Elements.Actors.title) == false)
                    {
                        SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Actors.titleForeColor));
                        PointF position = new PointF((float)(objTheme.Elements.Actors.titleHorisontalPos * mediaPlayerDevice.BGRatio),
                                                (float)(objTheme.Elements.Actors.titleVerticalPos * mediaPlayerDevice.BGRatio));

                        Font drawFont;

                        if (objTheme.Elements.Actors.fontBold == false)
                            drawFont = new Font(objTheme.Elements.Actors.fontName, (float)(objTheme.Elements.Actors.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                        else
                            drawFont = new Font(objTheme.Elements.Actors.fontName, (float)(objTheme.Elements.Actors.fontSize * mediaPlayerDevice.BGRatio));

                        objCanvas.DrawString(objTheme.Elements.Actors.title, drawFont, drawBrush, position);
                    }

                    if (cast != null)
                    {
                        SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.FromArgb(objTheme.Elements.Actors.foreColor));
                        PointF position = new PointF((float)(objTheme.Elements.Actors.horisontalPos * mediaPlayerDevice.BGRatio), (float)(objTheme.Elements.Actors.verticalPos * mediaPlayerDevice.BGRatio));
                        Font drawFont;

                        if (objTheme.Elements.Rating.fontBold == false)
                            drawFont = new Font(objTheme.Elements.Actors.fontName, (float)(objTheme.Elements.Actors.fontSize * mediaPlayerDevice.BGRatio), System.Drawing.FontStyle.Bold);
                        else
                            drawFont = new Font(objTheme.Elements.Actors.fontName, (float)(objTheme.Elements.Actors.fontSize * mediaPlayerDevice.BGRatio));

                        if (objTheme.Elements.Actors.Orientation == "H")
                        {
                            StringBuilder strCast = new StringBuilder();
                            for (int i = 0; i < objTheme.Elements.Actors.MaximumValues; i++)
                            {
                                if (i < cast.Count)
                                {
                                    strCast.Append(cast[i]);
                                    if (i < objTheme.Elements.Actors.MaximumValues - 1 && i < cast.Count - 1)
                                    {
                                        strCast.Append(objTheme.Elements.Actors.Separator);
                                    }
                                }
                            }
                            objCanvas.DrawString(strCast.ToString(), drawFont, drawBrush, position);
                        }
                        else
                        {
                            for (int i = 0; i < objTheme.Elements.Actors.MaximumValues; i++)
                            {
                                position = new PointF((float)(objTheme.Elements.Actors.horisontalPos * mediaPlayerDevice.BGRatio),
                                                        (float)(objTheme.Elements.Actors.verticalPos * mediaPlayerDevice.BGRatio) + (i * 20));
                                if (i < cast.Count)
                                    objCanvas.DrawString(cast[i], drawFont, drawBrush, position);
                            }
                        }

                    }
                }
                #endregion
                objCanvas.Save();

                if (imgCover != null)
                    imgCover.Dispose();

                imgBackground.Dispose();

                return GetBitmapSource(objOutput);
            }

        }


        private static BitmapSource GetBitmapSource(Bitmap image)
        {
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    image.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }
        private static string GetTheme(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Apps:
                    return MySettings.TvixThemeMovie;
                case EntityType.Artist:
                    return string.Empty;
                case EntityType.Books:
                    return MySettings.TvixThemeMovie;
                case EntityType.Games:
                    return MySettings.TvixThemeGames;
                case EntityType.LateLoan:
                    return string.Empty;
                case EntityType.Loan:
                    return string.Empty;
                case EntityType.Movie:
                    return MySettings.TvixThemeMovie;
                case EntityType.Music:
                    return MySettings.TvixThemeMusic;
                case EntityType.Nds:
                    return MySettings.TvixThemeGames;
                case EntityType.Series:
                    return MySettings.TvixThemeSerie;
                case EntityType.XXX:
                    return MySettings.TvixThemeXXX;
            }

            return string.Empty;
        }
        #endregion
    }
}
