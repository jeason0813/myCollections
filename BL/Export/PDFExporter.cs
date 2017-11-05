using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Export
{
    class PdfExporter : IProgressOperation, IDisposable
    {
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private readonly string _folder;
        private string _what;
        private string _message;

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

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnComplete(EventArgs.Empty);
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

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
        public PdfExporter(string folderPath, string what)
        {
            _what = what;
            _folder = Path.Combine(folderPath, "myCollections_" + _what + ".pdf");

            if (what == "Apps" || what == "All")
                _total += AppServices.Gets().Count;

            if (what == "Books" || what == "All")
                _total += BookServices.Gets().Count;

            if (what == "Games" || what == "All")
                _total += GameServices.Gets().Count;

            if (what == "Movies" || what == "All")
                _total += MovieServices.Gets().Count;

            if (what == "Music" || what == "All")
                _total += MusicServices.Gets().Count;

            if (what == "Nds" || what == "All")
                _total += NdsServices.Gets().Count;

            if (what == "Series" || what == "All")
                _total += SerieServices.Gets().Count;

            if (what == "XXX" || what == "All")
                _total += XxxServices.Gets().Count;

        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = _folder;

            using (Document doc = new Document())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));

                doc.Open();
                doc.AddTitle("myCollections Pdf Export");
                doc.AddCreator("myCollections");
                doc.AddSubject("myCollections");
                doc.AddHeader("myCollections v2.2", @"http://mycollections.codeplex.com/");

                Rectangle page = doc.PageSize;
                PdfPTable head = new PdfPTable(1);
                head.TotalWidth = page.Width;
                Phrase phrase = new Phrase(@"http://mycollections.codeplex.com/" + "            Exported : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            new Font(Font.FontFamily.COURIER, 8));

                PdfPCell c = new PdfPCell(phrase);
                c.BorderColorBottom = BaseColor.BLACK;
                c.Border = Rectangle.BOTTOM_BORDER;
                c.VerticalAlignment = Element.ALIGN_CENTER;
                c.HorizontalAlignment = Element.ALIGN_LEFT;
                head.AddCell(c);

                head.WriteSelectedRows(0, -1, 0, page.Height - doc.TopMargin + head.TotalHeight + 20,
                                        writer.DirectContent);

                Paragraph p = new Paragraph("myCollections", new Font(Font.FontFamily.COURIER, 32, 1));
                p.Alignment = 2;
                doc.Add(p);
                doc.NewPage();

                if ((_what == "Apps" || _what == "All") && _isCancelationPending == false)
                    ExportApps(doc);

                if ((_what == "Books" || _what == "All") && _isCancelationPending == false)
                    ExportBooks(doc);

                if ((_what == "Games" || _what == "All") && _isCancelationPending == false)
                    ExportGames(doc);

                if ((_what == "Movies" || _what == "All") && _isCancelationPending == false)
                    ExportMovies(doc);

                if ((_what == "Music" || _what == "All") && _isCancelationPending == false)
                    ExportMusics(doc);

                if ((_what == "Nds" || _what == "All") && _isCancelationPending == false)
                    ExportNds(doc);

                if ((_what == "Series" || _what == "All") && _isCancelationPending == false)
                    ExportSeries(doc);

                if ((_what == "XXX" || _what == "All") && _isCancelationPending == false)
                    ExportXxXs(doc);
            }
        }
        private void ExportApps(Document doc)
        {
            try
            {
                WriteChapter(doc, "Applications");
                IList items = AppServices.Gets();

                PdfPTable mainTable = new PdfPTable(1);

                for (int i = 0; i < items.Count; i++)
                {
                    Apps entity = items[i] as Apps;

                    CommonServices.GetChild(entity);

                    if (_isCancelationPending == true)
                        break;

                    if (entity != null)
                    {
                        Ressource cover = entity.Ressources.FirstOrDefault(x => x.IsDefault == true);
                        byte[] image = null;

                        if (cover != null)
                            image = cover.Value;

                        PdfPTable table = WriteCover(image, entity.Title);

                        string editorName = string.Empty;
                        if (entity.Publisher != null)
                            editorName = entity.Publisher.Name;

                        PdfPTable text = WriteFirstRow(entity.Title, entity.Version, editorName);
                        DateTime releasedate = DateTime.MinValue;
                        if (entity.ReleaseDate != null)
                            releasedate = (DateTime)entity.ReleaseDate;

                        long rating = 0;
                        if (entity.MyRating != null)
                            rating = (long)entity.MyRating;
                        WriteSecondRow(releasedate, rating, text);

                        WriteDescription(entity.Description, text);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }

                        WriteTypeRow(types.ToString(), entity.AddedDate.ToShortDateString(), text);

                        string media = string.Empty;
                        if (entity.Media != null)
                            media = entity.Media.Name;
                        WriteMediaInfo(media, entity.FilePath, entity.FileName, text);

                        table.AddCell(text);
                        table.SplitRows = false;

                        PdfPCell cell = new PdfPCell(table);
                        cell.Border = 0;
                        cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                        mainTable.AddCell(cell);
                    }

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
                doc.Add(mainTable);

                doc.NewPage();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }
        private void ExportBooks(Document doc)
        {
            try
            {


                WriteChapter(doc, "Books");
                IList items = BookServices.Gets();

                PdfPTable mainTable = new PdfPTable(1);

                for (int i = 0; i < items.Count; i++)
                {
                    Books entity = items[i] as Books;

                    CommonServices.GetChild(entity);

                    if (_isCancelationPending == true)
                        break;

                    if (entity != null)
                    {
                        Ressource cover = entity.Ressources.FirstOrDefault(x => x.IsDefault == true);
                        byte[] image = null;

                        if (cover != null)
                            image = cover.Value;

                        PdfPTable table = WriteCover(image, entity.Title);

                        StringBuilder types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }

                        string editorName = string.Empty;
                        if (entity.Publisher != null)
                            editorName = entity.Publisher.Name;

                        PdfPTable text = WriteFirstRow(entity.Title, types.ToString(), editorName);

                        DateTime releasedate = DateTime.MinValue;
                        if (entity.ReleaseDate != null)
                            releasedate = (DateTime)entity.ReleaseDate;

                        int rating = 0;
                        if (entity.MyRating != null)
                            rating = (int)entity.MyRating;

                        WriteSecondRow(releasedate, rating, text);
                        WriteDescription(entity.Description, text);

                        types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }

                        WriteTypeRow(types.ToString(), entity.AddedDate.ToShortDateString(), text);

                        string media = string.Empty;
                        if (entity.Media != null)
                            media = entity.Media.Name;
                        WriteMediaInfo(media, entity.FilePath, entity.FileName, text);

                        table.AddCell(text);
                        table.SplitRows = false;

                        PdfPCell cell = new PdfPCell(table);
                        cell.Border = 0;
                        cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                        mainTable.AddCell(cell);
                    }

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
                doc.Add(mainTable);

                doc.NewPage();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }
        }
        private void ExportGames(Document doc)
        {
            try
            {

                WriteChapter(doc, "Games");
                IList items = GameServices.Gets();

                PdfPTable mainTable = new PdfPTable(1);

                for (int i = 0; i < items.Count; i++)
                {
                    Gamez entity = items[i] as Gamez;

                    CommonServices.GetChild(entity);

                    if (_isCancelationPending == true)
                        break;

                    if (entity != null)
                    {
                        Ressource cover = entity.Ressources.FirstOrDefault(x => x.IsDefault == true);
                        byte[] image = null;

                        if (cover != null)
                            image = cover.Value;

                        PdfPTable table = WriteCover(image, entity.Title);

                        string editorName = string.Empty;
                        if (entity.Publisher != null)
                            editorName = entity.Publisher.Name;

                        string plateform = string.Empty;
                        if (entity.Platform != null)
                            plateform = entity.Platform.Name;

                        PdfPTable text = WriteFirstRow(entity.Title, plateform, editorName);

                        DateTime releasedate = DateTime.MinValue;
                        if (entity.ReleaseDate != null)
                            releasedate = (DateTime)entity.ReleaseDate;

                        int rating = 0;
                        if (entity.MyRating != null)
                            rating = (int)entity.MyRating;

                        WriteSecondRow(releasedate, rating, text);
                        WriteDescription(entity.Description, text);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (item != null)
                                if (types.Length == 0)
                                    types.Append(item.DisplayName);
                                else
                                    types.Append("," + item.DisplayName);
                        }

                        WriteTypeRow(types.ToString(), entity.AddedDate.ToShortDateString(), text);
                        WriteMediaInfo(entity.Media.Name, entity.FilePath, entity.FileName, text);
                        text.SplitRows = false;
                        table.AddCell(text);
                        table.SplitRows = false;

                        PdfPCell cell = new PdfPCell(table);
                        cell.Border = 0;
                        cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                        mainTable.AddCell(cell);
                    }

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
                doc.Add(mainTable);

                doc.NewPage();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }
        private void ExportMovies(Document doc)
        {
            try
            {
                WriteChapter(doc, "Movies");
                IList items = MovieServices.Gets();

                PdfPTable mainTable = new PdfPTable(1);

                for (int i = 0; i < items.Count; i++)
                {
                    Movie entity = items[i] as Movie;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    if (entity != null)
                    {
                        Ressource cover = entity.Ressources.FirstOrDefault(x => x.IsDefault == true);
                        byte[] image = null;

                        if (cover != null)
                            image = cover.Value;

                        PdfPTable table = WriteCover(image, entity.Title);

                        string editorName = string.Empty;
                        if (entity.Publisher != null)
                            editorName = entity.Publisher.Name;

                        PdfPTable text = WriteFirstRow(entity.Title, entity.OriginalTitle, editorName);

                        DateTime releasedate = DateTime.MinValue;
                        if (entity.ReleaseDate != null)
                            releasedate = (DateTime)entity.ReleaseDate;

                        double? rating = MovieServices.CalculateMovieRating(entity);

                        WriteSecondRow(releasedate, rating, text);
                        WriteDescription(entity.Description, text);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                            {
                                if (item != null)
                                    types.Append(item.DisplayName);
                            }
                            else if (item != null)
                                types.Append("," + item.DisplayName);
                        }

                        WriteTypeRow(types.ToString(), entity.AddedDate.ToShortDateString(), text);

                        types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }
                        WriteCast(types.ToString(), text);

                        string media = string.Empty;
                        if (entity.Media != null)
                            media = entity.Media.Name;
                        WriteMediaInfo(media, entity.FilePath, entity.FileName, text);

                        table.AddCell(text);

                        PdfPCell cell = new PdfPCell(table);
                        cell.Border = 0;
                        cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                        mainTable.AddCell(cell);
                    }

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
                doc.Add(mainTable);

                doc.NewPage();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }
        private void ExportMusics(Document doc)
        {
            try
            {
                WriteChapter(doc, "Musics");
                IList items = MusicServices.Gets();

                PdfPTable mainTable = new PdfPTable(1);

                for (int i = 0; i < items.Count; i++)
                {
                    Music entity = items[i] as Music;

                    CommonServices.GetChild(entity);

                    if (_isCancelationPending == true)
                        break;

                    if (entity != null)
                    {
                        Ressource cover = entity.Ressources.FirstOrDefault(x => x.IsDefault == true);
                        byte[] image = null;

                        if (cover != null)
                            image = cover.Value;

                        PdfPTable table = WriteCover(image, entity.Title);

                        StringBuilder types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }

                        string editorName = string.Empty;
                        if (entity.Publisher != null)
                            editorName = entity.Publisher.Name;

                        PdfPTable text = WriteFirstRow(entity.Title, types.ToString(), editorName);

                        DateTime releasedate = DateTime.MinValue;
                        if (entity.ReleaseDate != null)
                            releasedate = (DateTime)entity.ReleaseDate;

                        int rating = 0;
                        if (entity.MyRating != null)
                            rating = (int)entity.MyRating;

                        WriteSecondRow(releasedate, rating, text);
                        WriteDescription(entity.Comments, text);

                        types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }

                        WriteTypeRow(types.ToString(), entity.AddedDate.ToShortDateString(), text);

                        string media = string.Empty;
                        if (entity.Media != null)
                            media = entity.Media.Name;
                        WriteMediaInfo(media, entity.FilePath, entity.FileName, text);

                        table.AddCell(text);
                        //table.SplitRows = false;

                        PdfPCell cell = new PdfPCell(table);
                        cell.Border = 0;
                        cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                        mainTable.AddCell(cell);
                    }

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
                doc.Add(mainTable);

                doc.NewPage();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }
        private void ExportNds(Document doc)
        {
            try
            {

                WriteChapter(doc, "Nds");
                IList items = NdsServices.Gets();

                PdfPTable mainTable = new PdfPTable(1);

                for (int i = 0; i < items.Count; i++)
                {
                    Nds entity = items[i] as Nds;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    if (entity != null)
                    {
                        Ressource cover = entity.Ressources.FirstOrDefault(x => x.IsDefault == true);
                        byte[] image = null;

                        if (cover != null)
                            image = cover.Value;

                        PdfPTable table = WriteCover(image, entity.Title, true);

                        string editorName = string.Empty;
                        if (entity.Publisher != null)
                            editorName = entity.Publisher.Name;

                        PdfPTable text = WriteFirstRow(entity.Title, string.Empty, editorName);

                        DateTime releasedate = DateTime.MinValue;
                        if (entity.ReleaseDate != null)
                            releasedate = (DateTime)entity.ReleaseDate;

                        int rating = 0;
                        if (entity.MyRating != null)
                            rating = (int)entity.MyRating;

                        WriteSecondRow(releasedate, rating, text);
                        WriteDescription(entity.Description, text);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                                types.Append(item.DisplayName);
                            else
                                types.Append("," + item.DisplayName);
                        }

                        WriteTypeRow(types.ToString(), entity.AddedDate.ToShortDateString(), text);

                        string media = string.Empty;
                        if (entity.Media != null)
                            media = entity.Media.Name;

                        WriteMediaInfo(media, entity.FilePath, entity.FileName, text);

                        table.AddCell(text);
                        table.SplitRows = false;

                        PdfPCell cell = new PdfPCell(table);
                        cell.Border = 0;
                        cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                        mainTable.AddCell(cell);
                    }

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
                doc.Add(mainTable);

                doc.NewPage();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }
        private void ExportSeries(Document doc)
        {
            try
            {
                WriteChapter(doc, "Series");
                IList items = SerieServices.Gets();

                PdfPTable mainTable = new PdfPTable(1);

                for (int i = 0; i < items.Count; i++)
                {
                    SeriesSeason entity = items[i] as SeriesSeason;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    if (entity != null)
                    {
                        Ressource cover = entity.Ressources.FirstOrDefault(x => x.IsDefault == true);
                        byte[] image = null;

                        if (cover != null)
                            image = cover.Value;

                        PdfPTable table = WriteCover(image, entity.Title);

                        string editorName = string.Empty;
                        if (entity.Publisher != null)
                            editorName = entity.Publisher.Name;

                        PdfPTable text = WriteFirstRow(entity.Title, entity.Season.ToString(CultureInfo.InvariantCulture), editorName);

                        DateTime releasedate = DateTime.MinValue;
                        if (entity.ReleaseDate != null)
                            releasedate = (DateTime)entity.ReleaseDate;

                        int rating = 0;
                        if (entity.MyRating != null)
                            rating = (int)entity.MyRating;

                        WriteSecondRow(releasedate, rating, text);
                        WriteDescription(entity.Description, text);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                            {
                                if (item != null)
                                    types.Append(item.DisplayName);
                            }
                            else if (item != null)
                                types.Append("," + item.DisplayName);
                        }

                        WriteTypeRow(types.ToString(), entity.AddedDate.ToShortDateString(), text);

                        types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }
                        WriteCast(types.ToString(), text);

                        string media = string.Empty;
                        if (entity.Media != null)
                            media = entity.Media.Name;
                        WriteMediaInfo(media, entity.FilePath, string.Empty, text);

                        table.AddCell(text);
                        table.SplitRows = false;

                        PdfPCell cell = new PdfPCell(table);
                        cell.Border = 0;
                        cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                        mainTable.AddCell(cell);
                    }

                    _intAddedItem++;
                    Current++;
                    items[i] = null;

                }
                doc.Add(mainTable);

                doc.NewPage();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }
        private void ExportXxXs(Document doc)
        {
            try
            {

                WriteChapter(doc, "XXX");
                IList items = XxxServices.Gets();

                PdfPTable mainTable = new PdfPTable(1);

                for (int i = 0; i < items.Count; i++)
                {
                    XXX entity = items[i] as XXX;

                    CommonServices.GetChild(entity);
                    if (_isCancelationPending == true)
                        break;

                    if (entity != null)
                    {
                        Ressource cover = entity.Ressources.FirstOrDefault(x => x.IsDefault == true);
                        byte[] image = null;

                        if (cover != null)
                            image = cover.Value;

                        PdfPTable table = WriteCover(image, entity.Title);

                        string editorName = string.Empty;
                        if (entity.Publisher != null)
                            editorName = entity.Publisher.Name;

                        string language = string.Empty;
                        if (entity.Language != null)
                            language = entity.Language.LongName;

                        PdfPTable text = WriteFirstRow(entity.Title, language, editorName);
                        DateTime releasedate = DateTime.MinValue;
                        if (entity.ReleaseDate != null)
                            releasedate = (DateTime)entity.ReleaseDate;

                        int rating = 0;
                        if (entity.MyRating != null)
                            rating = (int)entity.MyRating;

                        WriteSecondRow(releasedate, rating, text);
                        WriteDescription(entity.Description, text);

                        StringBuilder types = new StringBuilder();
                        foreach (Genre item in entity.Genres)
                        {
                            if (types.Length == 0)
                            {
                                if (item != null)
                                    types.Append(item.DisplayName);
                            }
                            else if (item != null)
                                types.Append("," + item.DisplayName);
                        }

                        WriteTypeRow(types.ToString(), entity.AddedDate.ToShortDateString(), text);

                        types = new StringBuilder();
                        foreach (Artist item in entity.Artists)
                        {
                            if (types.Length == 0)
                                types.Append(item.FulleName);
                            else
                                types.Append("," + item.FulleName);
                        }
                        WriteCast(types.ToString(), text);

                        string media = string.Empty;
                        if (entity.Media != null)
                            media = entity.Media.Name;
                        WriteMediaInfo(media, entity.FilePath, entity.FileName, text);

                        table.AddCell(text);
                        //table.SplitRows = false;

                        PdfPCell cell = new PdfPCell(table);
                        cell.Border = 0;
                        cell.BorderColor = iTextSharp.text.BaseColor.WHITE;

                        mainTable.AddCell(cell);
                    }

                    _intAddedItem++;
                    Current++;
                    items[i] = null;
                }
                doc.Add(mainTable);

                doc.NewPage();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw;
            }

        }

        private static void WriteCast(string types, PdfPTable text)
        {
            PdfPCell type = new PdfPCell();
            type.Border = 0;
            type.Colspan = 3;
            type.VerticalAlignment = Element.ALIGN_CENTER;
            type.Phrase = new Phrase(types.ToString(CultureInfo.InvariantCulture), new Font(Font.FontFamily.COURIER, 6, 0));
            text.AddCell(type);
            text.CompleteRow();
        }
        private static void WriteTypeRow(string types, string addedDate, PdfPTable text)
        {
            PdfPCell addeddate = new PdfPCell();
            addeddate.Border = 0;
            addeddate.VerticalAlignment = Element.ALIGN_CENTER;
            addeddate.Phrase = new Phrase(addedDate, new Font(Font.FontFamily.COURIER, 6, 0));
            text.AddCell(addeddate);

            #region Type

            PdfPCell type = new PdfPCell();
            type.Border = 0;
            type.Colspan = 2;
            type.VerticalAlignment = Element.ALIGN_CENTER;
            type.Phrase = new Phrase(types, new Font(Font.FontFamily.COURIER, 6, 0));
            text.AddCell(type);

            #endregion

            text.CompleteRow();
        }
        private static void WriteDescription(string strDescription, PdfPTable text)
        {
            PdfPCell description = new PdfPCell();
            description.Border = 0;
            description.Colspan = 3;
            description.NoWrap = false;
            //FIX Since 2.7.12.0
            if (string.IsNullOrWhiteSpace(strDescription) || strDescription.Length > 3000)
                description.Phrase = new Phrase(strDescription, new Font(Font.FontFamily.COURIER, 8, 0));
            else
                description.Phrase = new Phrase(strDescription, new Font(Font.FontFamily.COURIER, 10, 0));
            text.AddCell(description);
            text.CompleteRow();
        }
        private static void WriteSecondRow(DateTime releaseDate, double? rating, PdfPTable text)
        {
            PdfPCell relaeseDate = new PdfPCell();
            relaeseDate.Border = 0;
            relaeseDate.VerticalAlignment = Element.ALIGN_CENTER;
            relaeseDate.Phrase = new Phrase(releaseDate.ToShortDateString(), new Font(Font.FontFamily.COURIER, 6, 0));
            text.AddCell(relaeseDate);

            #region Rating

            if (rating != null)
            {
                PdfPTable ratingTable = new PdfPTable(5);
                ratingTable.SetWidths(new Single[] {1, 1, 1, 1, 5});
                PdfPCell ratingCell = new PdfPCell();
                ratingCell.Border = 0;

                PdfPCell star = new PdfPCell();
                star.Border = 0;
                star.FixedHeight = 9f;
                star.VerticalAlignment = Element.ALIGN_TOP;
                star.HorizontalAlignment = Element.ALIGN_MIDDLE;
                star.AddElement(Image.GetInstance(@".\Images\Rating\star_gold_full.png"));

                PdfPCell emptystar = new PdfPCell();
                emptystar.Border = 0;
                emptystar.FixedHeight = 9f;
                emptystar.AddElement(Image.GetInstance(@".\Images\Rating\star_empty.png"));
                emptystar.VerticalAlignment = Element.ALIGN_TOP;
                emptystar.HorizontalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell halfstar = new PdfPCell();
                halfstar.Border = 0;
                halfstar.FixedHeight = 9f;
                halfstar.AddElement(Image.GetInstance(@".\Images\Rating\star_gold_half.png"));
                halfstar.VerticalAlignment = Element.ALIGN_TOP;
                halfstar.HorizontalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell emptyCell = new PdfPCell();
                emptyCell.Border = 0;

                long starrating =(long)rating/4;
                if (starrating == 0)
                {
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 0.5 && starrating < 1)
                {
                    ratingTable.AddCell(halfstar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 1 && starrating < 1.5)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 1.5 && starrating < 2)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(halfstar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 2 && starrating < 2.5)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 2.5 && starrating < 3)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(halfstar);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 3 && starrating < 3.5)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(emptystar);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 3.5 && starrating < 4)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(halfstar);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 4 && starrating < 4.5)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(emptystar);
                }
                else if (starrating >= 4.5 && starrating < 5)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(halfstar);
                }
                else if (starrating >= 5)
                {
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                    ratingTable.AddCell(star);
                }

                ratingTable.CompleteRow();
                ratingCell.AddElement(ratingTable);
                text.AddCell(ratingCell);
                text.AddCell(emptyCell);
            }
            #endregion
            text.CompleteRow();

        }
        private static PdfPTable WriteFirstRow(string firstCell, string secondCell, string thirdCell)
        {
            PdfPTable text = new PdfPTable(3);
            PdfPCell title = new PdfPCell();
            title.Border = 0;
            title.Colspan = 1;
            if (string.IsNullOrEmpty(firstCell) == false)
                title.Phrase = new Phrase(firstCell, new Font(Font.FontFamily.COURIER, 10, 1));
            text.AddCell(title);

            PdfPCell version = new PdfPCell();
            version.Border = 0;
            if (string.IsNullOrEmpty(secondCell) == false)
                version.Phrase = new Phrase(secondCell, new Font(Font.FontFamily.COURIER, 8, 0));
            text.AddCell(version);

            PdfPCell editor = new PdfPCell();
            editor.Border = 0;
            editor.VerticalAlignment = Element.ALIGN_CENTER;
            editor.HorizontalAlignment = Element.ALIGN_RIGHT;
            if (string.IsNullOrEmpty(thirdCell) == false)
                editor.Phrase = new Phrase(thirdCell, new Font(Font.FontFamily.COURIER, 8, 0));
            text.AddCell(editor);
            text.CompleteRow();
            return text;
        }
        private static PdfPTable WriteCover(Byte[] cover, string title, bool isCube = false)
        {
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new Single[] { 3, 7 });
            table.SpacingBefore = 10;
            if (cover != null)
            {
                Image image = Image.GetInstance(cover);
                if (isCube == true)
                {
                    image.ScaleAbsolute(125, 125);
                    table.AddCell(new PdfPCell(image));
                }

                else
                    table.AddCell(image);

            }
            else
                table.AddCell(new Phrase(title, new Font(Font.FontFamily.COURIER, 13, 1)));
            return table;
        }
        private static void WriteChapter(Document doc, string strTitle)
        {
            Rectangle page = doc.PageSize;
            PdfPTable title = new PdfPTable(1);
            title.TotalWidth = page.Width;
            Phrase phrase = new Phrase(strTitle, new Font(Font.FontFamily.TIMES_ROMAN, 16, 1, BaseColor.ORANGE));

            PdfPCell c = new PdfPCell(phrase);
            c.Border = Rectangle.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_CENTER;
            c.HorizontalAlignment = Element.ALIGN_LEFT;
            c.Phrase = phrase;
            title.AddCell(c);
            doc.Add(title);
        }
        private static void WriteMediaInfo(string mediaName, string filePath, string fileName, PdfPTable text)
        {
            PdfPCell media = new PdfPCell();
            media.Border = 0;
            media.Phrase = new Phrase(mediaName, new Font(Font.FontFamily.COURIER, 8, 0));
            text.AddCell(media);

            PdfPCell filepath = new PdfPCell();
            filepath.Border = 0;
            filepath.Phrase = new Phrase(filePath, new Font(Font.FontFamily.COURIER, 8, 0));
            text.AddCell(filepath);

            PdfPCell filename = new PdfPCell();
            filename.Border = 0;
            filename.Phrase = new Phrase(fileName, new Font(Font.FontFamily.COURIER, 8, 0));
            text.AddCell(filename);
            text.CompleteRow();
        }
    }
}
