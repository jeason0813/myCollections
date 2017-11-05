using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using System.Collections.Specialized;
using System.Net;
using System.IO.Compression;
using myCollections.BL.Services;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using myCollections.Data;
using myCollections.Data.SqlLite;

namespace myCollections.Utils
{
    internal static class Util
    {
        public const int ThumbHeight = 200;
        public const int ThumbWidth = 150;

        const string ConnectionString = "K2oXHV01QxUmoZhYT+VJ+5Ponq+s5YElGKKVvaAFa9kc4Wf3Z/JLyUCszcjoCTBxW4OwOfLHTllLa28kIWnxB5KGwFKn9ouW5uFn8yAZIc2GNm3qXkWjtHHagZbVStsg0PXjQw/VX6B7P875Rr7yucawFjTISERunF0ZGzv80iQbA6z66k7ML+S1BvlDcHOy4c23TFL96LIgrx9nfKVO/NH6F4/tZATJuf4k5qlYhnVkvSBbhWJSlAxW/mcpVRex";
        const string ConnectionStringNews = "K2oXHV01QxUmoZhYT+VJ+5Ponq+s5YElGKKVvaAFa9kc4Wf3Z/JLyUCszcjoCTBxW4OwOfLHTllLa28kIWnxB1PARFfZfqMHxCUaKaTOSzG7JfNX0McVigA4OudJb5cndZVmsOwBpgArP9MJ/G1qXVHDj5ZriJ5b/NHrsF2C0w/8tPFFss2I5OMdNDtajOXOUjxtBoAY05E4dMH1LlbB9HDPJDMBNdL050Z3UhMQX+li2KhlWdN8YScc0p39cP51";

        public static string BraConverter(string usBra)
        {
            switch (usBra)
            {
                case "30AA":
                    return usBra + "-80AA";
                case "30A":
                    return usBra + "-80A";
                case "30B":
                    return usBra + "-80B";
                case "30C":
                    return usBra + "-80C";
                case "30F":
                case "30G":
                case "30GG":
                case "30DDD":
                    return usBra + "-80F";
                case "30DD":
                    return usBra + "-80E";
                case "30D":
                    return usBra + "-80D";
                case "32AA":
                    return usBra + "-85AA";
                case "32A":
                    return usBra + "-85A";
                case "32B":
                    return usBra + "-85B";
                case "32C":
                    return usBra + "-85C";
                case "32F":
                case "32G":
                case "32GG":
                case "32DDD":
                case "32DD+":
                    return usBra + "-85F";
                case "32DD":
                    return usBra + "-85E";
                case "32D":
                    return usBra + "-85D";
                case "34":
                    return usBra + "-90";
                case "34AA":
                    return usBra + "-90AA";
                case "34A":
                case "35A":
                    return usBra + "-90A";
                case "34B":
                    return usBra + "-90B";
                case "34C":
                    return usBra + "-90C";
                case "34F":
                case "34G":
                case "34GG":
                case "34DDD":
                case "34DD+":
                    return usBra + "-90F";
                case "34DD":
                    return usBra + "-90E";
                case "34D":
                    return usBra + "-90D";
                case "36":
                    return usBra + "-95";
                case "36AA":
                    return usBra + "-95AA";
                case "36A":
                    return usBra + "-95A";
                case "36B":
                    return usBra + "-95B";
                case "36C":
                    return usBra + "-95C";
                case "36F":
                case "36G":
                case "36GG":
                case "36DDD":
                case "36DD+":
                    return usBra + "-95F";
                case "36DD":
                    return usBra + "-95E";
                case "36D":
                    return usBra + "-95D";
                case "38AA":
                    return usBra + "-100AA";
                case "38A":
                    return usBra + "-100A";
                case "38B":
                    return usBra + "-100B";
                case "38C":
                    return usBra + "-100C";
                case "38F":
                case "38G":
                case "38GG":
                case "38DDD":
                case "38DD+":
                    return usBra + "-100F";
                case "38DD":
                    return usBra + "-100E";
                case "38D":
                    return usBra + "-100D";
                case "40AA":
                    return usBra + "-105AA";
                case "40A":
                    return usBra + "-150A";
                case "40B":
                    return usBra + "-105B";
                case "40C":
                    return usBra + "-105C";
                case "40F":
                case "40G":
                case "40GG":
                case "40DDD":
                case "40DD+":
                    return usBra + "-105F";
                case "40DD":
                    return usBra + "-105E";
                case "40D":
                    return usBra + "-105D";
                default:
                    return usBra;
            }
        }

        public static string CleanExtensions(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return title;

            string strResults = title.Trim();

            if (strResults.LastIndexOf(".", StringComparison.Ordinal) == strResults.Length - 4 &&
                strResults.LastIndexOf(".", StringComparison.Ordinal) > -1)
                strResults = strResults.Substring(0, strResults.Length - 4);

            if (strResults.ToUpper().EndsWith(".DIVX"))
                strResults = strResults.Substring(0, strResults.Length - 5);

            if (strResults.ToUpper().EndsWith(".EPUB"))
                strResults = strResults.Substring(0, strResults.Length - 5);

            return strResults.Trim();
        }

        public static string ConstructString(string[] values, string[] mapping, int nbrBaseParsing)
        {
            if (mapping == null || mapping.Length == 0)
                return string.Empty;

            StringBuilder strResults = new StringBuilder();

            if (nbrBaseParsing == values.Length)
            {
                foreach (string item in mapping)
                {
                    string[] strTemp = item.Split("-".ToCharArray());
                    int intIndex = Convert.ToInt32(strTemp[0], CultureInfo.InvariantCulture);
                    strResults.Append(values[intIndex - 1]);
                    strResults.Append(" ");
                }
            }

            else if (nbrBaseParsing > values.Length)
            {
                for (int i = 0; i < mapping.Length - (nbrBaseParsing - values.Length); i++)
                {
                    string[] strTemp = mapping[i].Split("-".ToCharArray());
                    int intIndex = Convert.ToInt32(strTemp[0], CultureInfo.InvariantCulture);
                    if (intIndex > values.Length)
                        return string.Empty;
                    else
                    {
                        strResults.Append(values[intIndex - 1]);
                        strResults.Append(" ");
                    }
                }
            }

            else if (nbrBaseParsing < values.Length)
            {
                int intIndex = 0;

                for (int i = 0; i < (mapping.Length + (values.Length - nbrBaseParsing)); i++)
                {
                    if (i > values.Length - 1) break;

                    if (i < mapping.Length)
                    {
                        string[] strTemp = mapping[i].Split("-".ToCharArray());
                        intIndex = Convert.ToInt32(strTemp[0], CultureInfo.InvariantCulture);
                        strResults.Append(values[intIndex - 1]);
                        strResults.Append(" ");
                    }
                    else
                    {
                        intIndex++;
                        if (values.Length > intIndex - 1)
                        {
                            strResults.Append(values[intIndex - 1]);
                            strResults.Append(" ");
                        }
                    }
                }
            }

            if (strResults.Length > 0)
            {
                strResults = strResults.Remove(strResults.Length - 1, 1);
            }
            return strResults.ToString().Trim();
        }
        public static Image ConvertBitmapSourceToImage(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                bitmapEncoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return Image.FromStream(memoryStream);
            }
            return null;
        }
        private static byte[] ConvertImageToByteArray(Image imageToConvert, ImageFormat formatOfImage)
        {
            byte[] ret;
            using (MemoryStream ms = new MemoryStream())
            {
                imageToConvert.Save(ms, formatOfImage);
                ret = ms.ToArray();
                ms.Dispose();
                imageToConvert.Dispose();
            }
            return ret;
        }
        public static Byte[] CreateCover(string name)
        {
            Font font = new Font("Arial", 10.0f, System.Drawing.FontStyle.Regular);
            SolidBrush brush = new SolidBrush(System.Drawing.Color.White);
            RectangleF rectangle = new RectangleF(new PointF(5, 5), new SizeF(65, 90));

            Bitmap objOutput = new Bitmap(75, 100);
            Graphics objCanvas = Graphics.FromImage(objOutput);

            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            objCanvas.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            objCanvas.TextContrast = 0;
            objCanvas.SmoothingMode = SmoothingMode.AntiAlias;

            objCanvas.DrawString(name, font, brush, rectangle, stringFormat);
            objCanvas.Save();

            font.Dispose();
            brush.Dispose();

            return SaveImageData(objOutput);
        }
        public static BitmapImage CreateImage(byte[] imageData)
        {
            try
            {
                if (imageData == null || imageData.Length == 0)
                {
                    return null;
                }

                BitmapImage result = new BitmapImage();
                MemoryStream mem = new MemoryStream(imageData);

                result.BeginInit();
                result.StreamSource = mem;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                result.EndInit();
                result.Freeze();
                mem.Dispose();

                return result;
            }
            catch (NotSupportedException ex)
            {
                LogException(ex);
                return null;
            }
            catch (ArgumentException ex2)
            {
                LogException(ex2);
                return null;
            }
        }
        public static string CreateM3U(IEnumerable<string> tracks)
        {
            const string strName = "myColections.m3u";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), strName);

            StreamWriter sw = new StreamWriter(path, false);

            foreach (string item in tracks)
            {
                sw.Write(item);
                sw.Write(Environment.NewLine);
            }

            sw.Flush();
            sw.Close();

            return path;

        }
        public static BitmapSource CreateSmallImage(byte[] imageData)
        {
            try
            {
                if (imageData == null || imageData.Length == 0)
                    return null;

                BitmapImage result = new BitmapImage();
                MemoryStream mem = new MemoryStream(imageData);
                result.BeginInit();
                result.StreamSource = mem;
                result.DecodePixelWidth = 100;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                result.EndInit();
                result.Freeze();
                mem.Dispose();

                return result;
            }
            catch (NotSupportedException ex)
            {
                LogException(ex);
                return null;
            }
            catch (ArgumentException ex)
            {
                LogException(ex);
                return null;
            }
            catch (InvalidOperationException ex2)
            {
                LogException(ex2);
                return null;
            }
        }
        public static byte[] CreateSmallCover(byte[] imageData, int height, int width)
        {
            try
            {
                if (imageData == null || imageData.Length == 0)
                    return null;

                Image result;
                using (MemoryStream mem = new MemoryStream(imageData))
                {
                    result = Image.FromStream(mem, false, true);
                    result = result.GetThumbnailImage(width, height, null, new IntPtr());
                    mem.Dispose();
                }

                return ConvertImageToByteArray(result, ImageFormat.Jpeg);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string EncodeSearch(string strParams)
        {
            if (string.IsNullOrEmpty(strParams) == false)
            {
                strParams = strParams.Replace("'", "%27");
                strParams = strParams.Replace(" ", "+");
                strParams = strParams.Replace("é", "%E9");
                strParams = strParams.Replace("ç", "%E7");
                strParams = strParams.Replace("è", "%E8");
                strParams = strParams.Replace("ô", "%F4");
                return strParams.Trim();
            }
            else
                return strParams;
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrWhiteSpace(input) == false)
                return input.First().ToString(CultureInfo.InvariantCulture).ToUpper() + String.Join("", input.Skip(1));
            else
                return input;
        }

        public static string GetExternalIp()
        {
            const string whatIsMyIp = "http://automation.whatismyip.com/n09230945.asp";
            WebClient wc = new WebClient();
            UTF8Encoding utf8 = new UTF8Encoding();
            try
            {
                string ipaddr = null;
                bool done = false;

                WebProxy proxy = GetProxy();
                if (proxy != null)
                    wc.Proxy = proxy;

                wc.DownloadDataCompleted += (sender, e) =>
                {
                    try
                    {
                        ipaddr = utf8.GetString(e.Result);
                        done = true;
                    }
                    catch (Exception)
                    {
                        done = true;
                    }
                };

                wc.DownloadDataAsync(new Uri(whatIsMyIp));
                DateTime startTime = DateTime.Now;
                while (!done)
                {
                    TimeSpan sp = DateTime.Now - startTime;

                    // We should get a response in less than timeout.
                    // If not, we cancel all and return the internal IP Address
                    if (sp.TotalMilliseconds > 2000)
                    {
                        done = true;
                        wc.CancelAsync();
                    }
                }
                return ipaddr;
            }
            catch
            {
                return null;
            }
            finally
            {
                wc.Dispose();
            }
        }
        public static string GetExternalIp2()
        {
            const string whatIsMyIp = "http://whatismyipaddress.com/";
            try
            {
                string ipaddr = string.Empty;
                string results = GetHtmlPage(whatIsMyIp, null, BrowserType.Firefox10);
                if (string.IsNullOrWhiteSpace(results) == false)
                {
                    const string strParsing = @"<!-- do not script -->";
                    int intTemp = results.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTemp > -1)
                    {
                        results = results.Substring(intTemp + strParsing.Length);
                        ipaddr = results.Substring(0, results.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                        ipaddr = PurgeHtml(ipaddr);
                    }
                }
                return ipaddr;
            }
            catch
            {
                return null;
            }
        }
        private static byte[] GetEncodedImageData(ImageSource image, string preferredFormat)
        {
            byte[] result = null;
            BitmapEncoder encoder = null;

            switch (preferredFormat.ToLower(CultureInfo.InvariantCulture))
            {
                case ".jpg":
                case ".jpeg":

                    encoder = new JpegBitmapEncoder();
                    break;

                case ".bmp":

                    encoder = new BmpBitmapEncoder();
                    break;

                case ".png":

                    encoder = new PngBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":

                    encoder = new TiffBitmapEncoder();
                    break;

                case ".gif":

                    encoder = new GifBitmapEncoder();
                    break;

                case ".wmp":

                    encoder = new WmpBitmapEncoder();
                    break;
            }

            if (image is BitmapSource)
            {
                MemoryStream stream = new MemoryStream();
                if (encoder != null)
                {
                    encoder.Frames.Add(BitmapFrame.Create(image as BitmapSource));
                    encoder.Save(stream);
                }
                stream.Seek(0, SeekOrigin.Begin);

                result = new byte[stream.Length];
                BinaryReader br = new BinaryReader(stream);

                br.Read(result, 0, (int)stream.Length);
                br.Close();
                stream.Close();
            }

            return result;
        }
        public static Encoding GetFileEncoding(string srcFile)
        {

            // *** Use Default of Encoding.Default (Ansi CodePage)
            Encoding enc = Encoding.Default;

            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];
            FileStream file = new FileStream(srcFile, FileMode.Open, FileAccess.Read);
            file.Read(buffer, 0, 5);
            file.Close();

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;

            return enc;

        }
        public static byte[] GetLocalImage(string strFilePath, string strFileName, bool isFile)
        {
            string strImagePath = string.Empty;

            if (isFile == false && string.IsNullOrEmpty(strFileName) == false)
            {
                if (Directory.Exists(Path.Combine(strFilePath.Trim(), strFileName.Trim())) == false)
                    return null;

                DirectoryInfo objFolder = new DirectoryInfo(Path.Combine(strFilePath.Trim(), strFileName.Trim()));

                FileInfo[] lstImages = objFolder.GetFiles("*.jpg", SearchOption.AllDirectories);

                if (lstImages.Any())
                {
                    if (lstImages.Count() == 1)
                        return LoadImageData(lstImages[0].FullName);
                    else
                    {
                        long lngSize = 0;

                        foreach (FileInfo item in lstImages)
                        {
                            if (item.Length > lngSize)
                            {
                                strImagePath = item.FullName;
                                lngSize = item.Length;
                            }
                        }
                        return LoadImageData(strImagePath);
                    }

                }
            }
            else
            {
                if (Directory.Exists(strFilePath.Trim()) == false)
                    return null;

                if (string.IsNullOrWhiteSpace(strFileName) == false)
                {
                    string shortName = strFileName.Substring(0, strFileName.Length - 4);
                    string fullname = Path.Combine(strFilePath, shortName + ".jpg");

                    if (File.Exists(fullname))
                        return LoadImageData(fullname);
                    else
                    {
                        fullname = Path.Combine(strFilePath, shortName + ".png");
                        if (File.Exists(fullname))
                            return LoadImageData(fullname);
                    }
                }
            }

            return null;
        }
        public static string[] GetFiles(string path, IEnumerable<string> searchPatterns)
        {
            List<string> matchingFiles = new List<string>();

            foreach (string s in searchPatterns)
            {
                var files = from f in Directory.GetFiles(path, s, SearchOption.TopDirectoryOnly)
                            select f;

                matchingFiles.AddRange(files);
            }

            return matchingFiles.ToArray<string>();

        }
        public static Filter GetFilter(EntityType etype)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.AppsSelection, true);
                case EntityType.Books:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.BooksSelection, true);
                case EntityType.Games:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.GamesSelection, true);
                case EntityType.Movie:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.MoviesSelection, true);
                case EntityType.Music:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.MusicsSelection, true);
                case EntityType.Nds:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.NdsSelection, true);
                case EntityType.Series:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.SeriesSelection, true);
                case EntityType.XXX:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.XXXSelection, true);
                default:
                    return (Filter)Enum.Parse(typeof(Filter), MySettings.AppsSelection, true);
            }
        }
        public static GroupBy GetGroupBy(EntityType etype)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.AppsGroupBy, true);
                case EntityType.Books:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.BooksGroupBy, true);
                case EntityType.Games:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.GamesGroupBy, true);
                case EntityType.Movie:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.MoviesGroupBy, true);
                case EntityType.Music:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.MusicsGroupBy, true);
                case EntityType.Nds:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.NdsGroupBy, true);
                case EntityType.Series:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.SeriesGroupBy, true);
                case EntityType.XXX:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.XXXGroupBy, true);
                default:
                    return (GroupBy)Enum.Parse(typeof(GroupBy), MySettings.AppsGroupBy, true);
            }
        }
        public static string GetHtmlPage(string strUrl, Encoding objEncoding, BrowserType browserType = BrowserType.None, bool useCookie = false)
        {
            WebClient objService = null;

            try
            {
                string strResults = string.Empty;

                if (string.IsNullOrEmpty(strUrl) == false)
                {
                    strUrl = strUrl.Replace(" ", "%20");

                    if (useCookie == false)
                        objService = new WebClient();
                    else
                        objService = new CookieAwareWebClient();

                    WebProxy proxy = GetProxy();
                    if (proxy != null)
                        objService.Proxy = proxy;

                    objService.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                    switch (browserType)
                    {
                        case BrowserType.Firefox4:
                            objService.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.0.1) Gecko/20100101 Firefox/4.0.1");
                            break;
                        case BrowserType.Firefox10:
                            objService.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:10.0.2) Gecko/20100101 Firefox/10.0.2");
                            break;
                    }


                    if (objEncoding == null)
                        objEncoding = Encoding.Default;

                    objService.Encoding = objEncoding;

                    using (MemoryStream memoryStream = new MemoryStream(objService.DownloadData(strUrl)))
                    {

                        switch (objService.ResponseHeaders[HttpResponseHeader.ContentEncoding])
                        {
                            case "gzip":
                                strResults = new StreamReader(new GZipStream(memoryStream, CompressionMode.Decompress), objEncoding).ReadToEnd();
                                break;
                            case "deflate":
                                strResults = new StreamReader(new DeflateStream(memoryStream, CompressionMode.Decompress), objEncoding).ReadToEnd();
                                break;
                            default:
                                strResults = new StreamReader(memoryStream, objEncoding).ReadToEnd();
                                break;
                        }
                    }


                    //Replace HTML entity references so that we can load into XElement
                    strResults = strResults.Replace("&nbsp;", "");
                    strResults = strResults.Replace("&", "&amp;");
                }

                return strResults;

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (objService != null)
                    objService.Dispose();
            }

        }
        public static string GetHtmlPage(string strUrl, NameValueCollection objValues)
        {
            string strResults = string.Empty;

            if (string.IsNullOrEmpty(strUrl) == false)
            {

                strUrl = strUrl.Replace(" ", "%20");
                WebClient objService = new WebClient();

                WebProxy proxy = GetProxy();
                if (proxy != null)
                    objService.Proxy = proxy;

                byte[] objResults = objService.UploadValues(strUrl, objValues);

                strResults = Encoding.ASCII.GetString(objResults);
            }

            return strResults;
        }
        public static byte[] GetImage(string strUrl)
        {
            return GetImage(strUrl, string.Empty);
        }
        public static byte[] GetImage(string strUrl, string referer, BrowserType browserType = BrowserType.None, bool useCookie = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strUrl)) return null;

                strUrl = strUrl.Replace(" ", "%20");
                if (strUrl.EndsWith(".js", true, CultureInfo.InvariantCulture) == false)
                {
                    WebClient objService;

                    if (useCookie == false)
                        objService = new WebClient();
                    else
                        objService = new CookieAwareWebClient();

                    WebProxy proxy = GetProxy();
                    if (proxy != null)
                        objService.Proxy = proxy;

                    objService.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

                    switch (browserType)
                    {
                        case BrowserType.Firefox4:
                            objService.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:2.0.1) Gecko/20100101 Firefox/4.0.1");
                            break;
                        case BrowserType.Firefox8:
                            objService.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; rv:8.0.1) Gecko/20100101 Firefox/8.0.1");
                            break;
                        case BrowserType.Firefox10:
                            objService.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:10.0.2) Gecko/20100101 Firefox/10.0.2");
                            break;
                        default:
                            objService.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:10.0.2) Gecko/20100101 Firefox/10.0.2");
                            objService.Headers.Add(HttpRequestHeader.Pragma, "no-cache");
                            objService.Headers.Add(HttpRequestHeader.KeepAlive, "true");
                            break;
                    }

                    if (string.IsNullOrWhiteSpace(referer) == false)
                        objService.Headers.Add("Referer", referer);

                    using (MemoryStream memoryStream = new MemoryStream(objService.DownloadData(strUrl)))
                    {
                        switch (objService.ResponseHeaders[HttpResponseHeader.ContentEncoding])
                        {
                            case "gzip":
                                using (GZipStream stream = new GZipStream(memoryStream, CompressionMode.Decompress))
                                {
                                    const int size = 4096;
                                    byte[] buffer = new byte[size];
                                    using (MemoryStream memory = new MemoryStream())
                                    {
                                        int count;
                                        do
                                        {
                                            count = stream.Read(buffer, 0, size);
                                            if (count > 0)
                                            {
                                                memory.Write(buffer, 0, count);
                                            }
                                        }
                                        while (count > 0);
                                        return memory.ToArray();
                                    }
                                }
                            case "deflate":
                                using (DeflateStream stream = new DeflateStream(memoryStream, CompressionMode.Decompress))
                                {
                                    const int size = 4096;
                                    byte[] buffer = new byte[size];
                                    using (MemoryStream memory = new MemoryStream())
                                    {
                                        int count;
                                        do
                                        {
                                            count = stream.Read(buffer, 0, size);
                                            if (count > 0)
                                            {
                                                memory.Write(buffer, 0, count);
                                            }
                                        }
                                        while (count > 0);
                                        return memory.ToArray();
                                    }
                                }
                            default:
                                return memoryStream.ToArray();
                        }
                    }
                }

                return null;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public static News GetLatestNews(int id)
        {
            SqlConnection connection = null;
            try
            {
                News newsResults = null;
                connection = new SqlConnection(Uncrypte(ConnectionStringNews));
                string query = string.Format(@"SELECT TOP 1 Id,AppName,Title,Body,Link,Ip,BinaryVersion,NewsUrl 
                                             FROM News where id > {0} AND AppName='{1}'", id.ToString(CultureInfo.InvariantCulture), "myCollections");
                connection.Open();
                SqlDataReader newsReader = new SqlCommand(query, connection).ExecuteReader();


                if (newsReader.HasRows == true)
                {
                    newsReader.Read();
                    newsResults = new News();
                    newsResults.Id = newsReader.GetInt32(0);
                    newsResults.AppName = newsReader.GetString(1);
                    newsResults.Title = newsReader.GetString(2);
                    newsResults.Body = newsReader.GetString(3);

                    if (newsReader[4] != DBNull.Value)
                        newsResults.Link = newsReader.GetString(4);

                    if (newsReader[5] != DBNull.Value)
                        newsResults.Ip = newsReader.GetString(5);

                    if (newsReader[6] != DBNull.Value)
                        newsResults.BinaryVersion = newsReader.GetString(6);

                    if (newsReader[7] != DBNull.Value)
                        newsResults.NewsUrl = newsReader.GetString(7);
                }

                connection.Close();
                return newsResults;
            }
            catch (Exception)
            {
                if (connection != null)
                    connection.Close();

                return null;

            }
        }
        public static Order GetOrder(EntityType etype)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    return (Order)Enum.Parse(typeof(Order), MySettings.AppsOrder, true);
                case EntityType.Books:
                    return (Order)Enum.Parse(typeof(Order), MySettings.BooksOrder, true);
                case EntityType.Games:
                    return (Order)Enum.Parse(typeof(Order), MySettings.GamesOrder, true);
                case EntityType.Movie:
                    return (Order)Enum.Parse(typeof(Order), MySettings.MoviesOrder, true);
                case EntityType.Music:
                    return (Order)Enum.Parse(typeof(Order), MySettings.MusicsOrder, true);
                case EntityType.Nds:
                    return (Order)Enum.Parse(typeof(Order), MySettings.NdsOrder, true);
                case EntityType.Series:
                    return (Order)Enum.Parse(typeof(Order), MySettings.SeriesOrder, true);
                case EntityType.XXX:
                    return (Order)Enum.Parse(typeof(Order), MySettings.XXXOrder, true);
                default:
                    return (Order)Enum.Parse(typeof(Order), MySettings.AppsOrder, true);
            }
        }
        private static WebProxy GetProxy()
        {
            if (Convert.ToBoolean(MySettings.UseProxy) == true)
            {
                WebProxy proxy = new WebProxy(MySettings.ProxyIp + ":" + MySettings.ProxyPort, false, null);
                proxy.Credentials = new NetworkCredential(MySettings.ProxyLogin, MySettings.ProxyPwd);
                return proxy;
            }
            else
                return null;
        }
        public static string GetRest(Uri strUrl, bool usecache = true, bool useJson = false, string userAgent = "")
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(strUrl) as HttpWebRequest;
                if (request != null)
                {
                    if (string.IsNullOrWhiteSpace(userAgent))
                        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:10.0.2) Gecko/20100101 Firefox/10.0.2";
                    else
                        request.UserAgent = userAgent;

                    request.KeepAlive = false;
                    request.Timeout = 70 * 10000;
                    request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

                    if (useJson == true)
                        request.Accept = "application/json";

                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                    if (usecache == false)
                        ServicePointManager.DefaultConnectionLimit = 200;

                    WebProxy proxy = GetProxy();
                    if (proxy != null)
                        request.Proxy = proxy;

                    // Get response  
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    if (response != null)
                    {
                        // Get the response stream  
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        // Read it into a StringBuilder  
                        StringBuilder sbSource = new StringBuilder(reader.ReadToEnd());
                        if (sbSource.Length > 0)
                        {

                            reader.Close();
                            response.Close();
                            return sbSource.ToString();
                        }
                        else
                        {
                            reader.Close();
                            response.Close();
                            return null;
                        }
                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                //Fix 2.8.8.0
                LogException(ex, strUrl.AbsoluteUri);
                return null;
            }
        }
        public static string PostRequest(Uri strUrl, string postData)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(strUrl) as HttpWebRequest;
                if (request != null)
                {
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:10.0.2) Gecko/20100101 Firefox/10.0.2";
                    request.KeepAlive = false;
                    request.Timeout = 70 * 10000;
                    request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                    request.Method = "POST";

                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] data = encoding.GetBytes(postData);

                    request.ContentType = "text/plain";
                    request.ContentLength = data.Length;

                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                    WebProxy proxy = GetProxy();
                    if (proxy != null)
                        request.Proxy = proxy;

                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    // Get response  
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    if (response != null)
                    {
                        // Get the response stream  
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        // Read it into a StringBuilder  
                        StringBuilder sbSource = new StringBuilder(reader.ReadToEnd());
                        if (sbSource.Length > 0)
                        {

                            reader.Close();
                            response.Close();
                            return sbSource.ToString();
                        }
                        else
                        {
                            reader.Close();
                            response.Close();
                            return null;
                        }
                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                LogInfo(ex.Message);
                return null;
            }
        }
        public static IServices GetService(EntityType etype)
        {
            Type type = null;
            switch (etype)
            {
                case EntityType.Apps:
                    type = Type.GetType("myCollections.BL.Services." + EntityService.AppServices.ToString());
                    break;
                case EntityType.Books:
                    type = Type.GetType("myCollections.BL.Services." + EntityService.BookServices.ToString());
                    break;
                case EntityType.Games:
                    type = Type.GetType("myCollections.BL.Services." + EntityService.GameServices.ToString());
                    break;
                case EntityType.Movie:
                    type = Type.GetType("myCollections.BL.Services." + EntityService.MovieServices.ToString());
                    break;
                case EntityType.Music:
                    type = Type.GetType("myCollections.BL.Services." + EntityService.MusicServices.ToString());
                    break;
                case EntityType.Nds:
                    type = Type.GetType("myCollections.BL.Services." + EntityService.NdsServices.ToString());
                    break;
                case EntityType.Series:
                    type = Type.GetType("myCollections.BL.Services." + EntityService.SerieServices.ToString());
                    break;
                case EntityType.XXX:
                    type = Type.GetType("myCollections.BL.Services." + EntityService.XxxServices.ToString());
                    break;
            }
            if (type != null)
                return (IServices)Activator.CreateInstance(type);
            else
                return null;
        }
        public static View GetView(EntityType etype)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    return (View)Enum.Parse(typeof(View), MySettings.AppsView, true);
                case EntityType.Books:
                    return (View)Enum.Parse(typeof(View), MySettings.BooksView, true);
                case EntityType.Games:
                    return (View)Enum.Parse(typeof(View), MySettings.GamesView, true);
                case EntityType.Movie:
                    return (View)Enum.Parse(typeof(View), MySettings.MoviesView, true);
                case EntityType.Music:
                    return (View)Enum.Parse(typeof(View), MySettings.MusicsView, true);
                case EntityType.Nds:
                    return (View)Enum.Parse(typeof(View), MySettings.NdsView, true);
                case EntityType.Series:
                    return (View)Enum.Parse(typeof(View), MySettings.SeriesView, true);
                case EntityType.XXX:
                    return (View)Enum.Parse(typeof(View), MySettings.XXXView, true);
                default:
                    return (View)Enum.Parse(typeof(View), MySettings.AppsView, true);
            }
        }
        public static double GetZoom(EntityType etype)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    return Convert.ToDouble(MySettings.AppsZoom);
                case EntityType.Books:
                    return Convert.ToDouble(MySettings.BooksZoom);
                case EntityType.Games:
                    return Convert.ToDouble(MySettings.GamesZoom);
                case EntityType.Movie:
                    return Convert.ToDouble(MySettings.MoviesZoom);
                case EntityType.Music:
                    return Convert.ToDouble(MySettings.MusicsZoom);
                case EntityType.Nds:
                    return Convert.ToDouble(MySettings.NdsZoom);
                case EntityType.Series:
                    return Convert.ToDouble(MySettings.SeriesZoom);
                case EntityType.XXX:
                    return Convert.ToDouble(MySettings.XXXZoom);
            }
            return Convert.ToDouble(MySettings.AppsZoom);
        }

        public static bool IsNumeric(string anyString)
        {
            if (anyString == null)
                anyString = "";

            if (anyString.Length > 0)
            {
                double dummyOut;
                CultureInfo cultureInfo = new CultureInfo("en-US", true);
                return Double.TryParse(anyString, NumberStyles.Any, cultureInfo.NumberFormat, out dummyOut);
            }
            else
                return false;
        }

        public static void LogException(Exception ex)
        {
            Task.Factory.StartNew(() => LogExceptionAsync(ex, string.Empty));

            if (MySettings.LogToFile == true)
            {
                string strName = "Errors" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".log";

                //FIX 2.8.9.0
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "myCollections");

                if (Directory.Exists(folderPath) == false)
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, strName);
                if (File.Exists(filePath) == false)
                {
                    StreamWriter objTemp = File.CreateText(filePath);
                    objTemp.Close();
                }

                using (
                    StreamWriter sw =
                        File.AppendText(filePath))
                {
                    sw.WriteLine("Message :" + ex.Message);
                    sw.WriteLine("Source :" + ex.Source);
                    if (ex.TargetSite != null) sw.WriteLine("TargetSite :" + ex.TargetSite);
                    if (ex.InnerException != null)
                        sw.WriteLine("InnerException :" + ex.InnerException);

                    if (ex is ReflectionTypeLoadException)
                    {
                        var typeLoadException = ex as ReflectionTypeLoadException;
                        Exception[] loaderExceptions = typeLoadException.LoaderExceptions;

                        foreach (Exception loaderException in loaderExceptions)
                        {
                            sw.WriteLine("loader :" + loaderException.Message);
                            sw.WriteLine("StackTrace :" + loaderException.StackTrace);
                        }

                    }

                    sw.WriteLine("StackTrace :" + ex.StackTrace);
                    sw.WriteLine("------------------------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        private static void LogExceptionAsync(Exception ex, string param)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(((App)Application.Current).Ip) == false && MySettings.SendError == true)
                {
                    SqlConnection connection = new SqlConnection(Uncrypte(ConnectionString));
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = string.Format(@"INSERT INTO Exception (AppName,Message,Source,TargetSite,InnerException,StackTrace,Ip,BinaryVersion) 
                                                                VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8)");


                    SqlParameter param1 = new SqlParameter("@param1", SqlDbType.NVarChar);
                    param1.Value = "myCollections";

                    SqlParameter param2 = new SqlParameter("@param2", SqlDbType.Text);
                    param2.Value = ex.Message + Environment.NewLine + "Param :" + param + Environment.NewLine + "Exception type :  " + ex.GetType();

                    SqlParameter param3 = new SqlParameter("@param3", SqlDbType.Text);
                    param3.Value = ex.Source;

                    SqlParameter param4 = new SqlParameter("@param4", SqlDbType.Text);
                    param4.Value = ex.TargetSite.Name;

                    SqlParameter param5 = new SqlParameter("@param5", SqlDbType.Text);
                    if (ex.InnerException != null)
                        param5.Value = ex.InnerException.Message;
                    else
                        param5.Value = DBNull.Value;

                    SqlParameter param6 = new SqlParameter("@param6", SqlDbType.Text);
                    param6.Value = ex.StackTrace;

                    SqlParameter param7 = new SqlParameter("@param7", SqlDbType.NVarChar);
                    param7.Value = ((App)Application.Current).Ip;

                    SqlParameter param8 = new SqlParameter("@param8", SqlDbType.NVarChar);
                    param8.Value = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    command.Parameters.Add(param1);
                    command.Parameters.Add(param2);
                    command.Parameters.Add(param3);
                    command.Parameters.Add(param4);
                    command.Parameters.Add(param5);
                    command.Parameters.Add(param6);
                    command.Parameters.Add(param7);
                    command.Parameters.Add(param8);

                    connection.Open();
                    command.ExecuteNonQuery();

                    command.Dispose();
                    connection.Close();
                    connection.Dispose();
                }

            }
            catch (Exception)
            {

            }

        }
        public static void LogException(Exception ex, string strParams)
        {
            Task.Factory.StartNew(() => LogExceptionAsync(ex, strParams));

            if (MySettings.LogToFile == true)
            {
                string strName = "Errors" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".log";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "myCollections", strName);
                if (File.Exists(filePath) == false)
                {
                    StreamWriter objTemp = File.CreateText(filePath);
                    objTemp.Close();
                }

                using (
                    StreamWriter sw =
                        File.AppendText(filePath))
                {
                    sw.WriteLine("Version : " + GetAppVersion());
                    sw.WriteLine("Params :" + strParams);

                    sw.Flush();
                    sw.Close();
                }

                LogException(ex);
            }
        }
        private static void LogInfo(string strInfo)
        {
            if (MySettings.LogToFile == true)
            {
                string strName = "Log" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".log";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "myCollections", strName);
                if (File.Exists(filePath) == false)
                {
                    StreamWriter objTemp = File.CreateText(filePath);
                    objTemp.Close();
                }

                using (
                    StreamWriter sw =
                        File.AppendText(filePath))
                {
                    sw.WriteLine(strInfo);
                    sw.WriteLine("------------------------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        public static byte[] LoadImageData(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return null;

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            byte[] imageBytes = br.ReadBytes((int)fs.Length);
            br.Close();

            fs.Close();
            return imageBytes;
        }

        public static void NotifyEvent(string strUsage)
        {
            SqlConnection connection = null;
            try
            {
                if (string.IsNullOrWhiteSpace(((App)Application.Current).Ip) == false)
                {
                    strUsage = strUsage.Replace(@"""", "");
                    strUsage = strUsage.Replace(@"'", "");
                    connection = new SqlConnection(Uncrypte(ConnectionString));
                    string query = string.Format(@"INSERT INTO Usages (AppName,BinaryVersion,Event,OSName,OSLanguage,Ip) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
                                                                       "myCollections", Assembly.GetExecutingAssembly().GetName().Version, strUsage, GetOsName(), CultureInfo.CurrentCulture.DisplayName,
                                                                        ((App)Application.Current).Ip);
                    connection.Open();
                    new SqlCommand(query, connection).ExecuteNonQuery();
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch (Exception)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public static string PurgeHtml(string initialValue)
        {
            StringBuilder strParams = new StringBuilder(initialValue);
            strParams = strParams.Replace(@"<P style=""MARGIN: 0in 0in 0pt"">", "");
            strParams = strParams.Replace(@"<table border=""0"" width=""100%"" cellspacing=""0"" cellpadding=""0"">", "");
            strParams = strParams.Replace(@"<table border=""0"" width=""95%"" cellspacing=""0"" cellpadding=""0"">", "");
            strParams = strParams.Replace(@"<p style=""text-align: justify"">", "");
            strParams = strParams.Replace(@"<td align=""center"">", "");
            strParams = strParams.Replace(@"<td align=""left"">", "");
            strParams = strParams.Replace(@"<td width=""25%"">", "");
            strParams = strParams.Replace(@"<span property=""v:summary"">", "");
            strParams = strParams.Replace("<!--[lead]-->", "");
            strParams = strParams.Replace("<!--[/lead]-->", "");
            strParams = strParams.Replace("<!--[features]-->", "");
            strParams = strParams.Replace("<!--[subfeatures]-->", "");
            strParams = strParams.Replace("<!--[/subfeatures]-->", "");
            strParams = strParams.Replace("<!--[/features]-->", "");
            strParams = strParams.Replace("<!--[quality]-->", "");
            strParams = strParams.Replace("<!--[subquality]-->", "");
            strParams = strParams.Replace("<!--[/subquality]-->", "");
            strParams = strParams.Replace("<!--[/quality]-->", "");
            strParams = strParams.Replace("<!--[conclusion]-->", "");
            strParams = strParams.Replace("<!--[subconclusion]-->", "");
            strParams = strParams.Replace("<!--[/subconclusion]-->", "");
            strParams = strParams.Replace("<!--[/conclusion]-->", "");
            strParams = strParams.Replace("<!--[whatsnew]-->", "");
            strParams = strParams.Replace("<!--[subwhatsnew]-->", "");
            strParams = strParams.Replace("<!--[/whatsnew]-->", "");
            strParams = strParams.Replace("<!--[/subwhatsnew]-->", "");
            strParams = strParams.Replace("</a>", " ");
            strParams = strParams.Replace("<div>", " ");
            strParams = strParams.Replace("</div>", " ");
            strParams = strParams.Replace("</div", " ");
            strParams = strParams.Replace("<div", " ");
            strParams = strParams.Replace("<DIV>", " ");
            strParams = strParams.Replace("</DIV>", " ");
            strParams = strParams.Replace("<h2>", " ");
            strParams = strParams.Replace("</h2>", " ");
            strParams = strParams.Replace("<h3>", " ");
            strParams = strParams.Replace("</h3>", " ");
            strParams = strParams.Replace("<em>", " ");
            strParams = strParams.Replace("<em>", " ");
            strParams = strParams.Replace("</span>", " ");
            strParams = strParams.Replace("<nobr>", " ");
            strParams = strParams.Replace(@"<p>", "");
            strParams = strParams.Replace(@"</p>", "");
            strParams = strParams.Replace(@"<P>", "");
            strParams = strParams.Replace(@"</P>", "");
            strParams = strParams.Replace(@"</strong>", " ");
            strParams = strParams.Replace(@"<strong>", " ");
            strParams = strParams.Replace(@"</small>", " ");
            strParams = strParams.Replace(@"<small>", " ");
            strParams = strParams.Replace("<br />", " ");
            strParams = strParams.Replace("<br/>", " ");
            strParams = strParams.Replace("<br>", " ");
            strParams = strParams.Replace("<br >", " ");
            strParams = strParams.Replace("<BR>", " ");
            strParams = strParams.Replace("<i>", " ");
            strParams = strParams.Replace("</i>", " ");
            strParams = strParams.Replace("<b>", " ");
            strParams = strParams.Replace("</b>", " ");
            strParams = strParams.Replace("&amp;#x22;", "");
            strParams = strParams.Replace("&amp;#x26;", "&");
            strParams = strParams.Replace("&amp;#x27;", "'");
            strParams = strParams.Replace("&amp;#32;", " ");
            strParams = strParams.Replace("&amp;#34;", " ");
            strParams = strParams.Replace("&amp;#39;", "'");
            strParams = strParams.Replace("&amp;#46;", ".");
            strParams = strParams.Replace("&amp;#201;", "È");
            strParams = strParams.Replace("&amp;#226;", "à");
            strParams = strParams.Replace("&amp;#231;", "ç");
            strParams = strParams.Replace("&amp;#232;", "è");
            strParams = strParams.Replace("&amp;#233;", "é");
            strParams = strParams.Replace("&amp;#234;", "ê");
            strParams = strParams.Replace("&amp;eacute;", "é");
            strParams = strParams.Replace("&amp;rsquo;", "'");
            strParams = strParams.Replace("&amp;egrave;", "è");
            strParams = strParams.Replace("&amp;circ;", "ê");
            strParams = strParams.Replace("&amp;quot;", @"""");
            strParams = strParams.Replace("&amp;amp;", "&");
            strParams = strParams.Replace("agrave;", "à");
            strParams = strParams.Replace("&amp;#xE0;", "à");
            strParams = strParams.Replace("&amp;#xE1;", "à");
            strParams = strParams.Replace("&amp;#xE2;", "â");
            strParams = strParams.Replace("&amp;#xEF;", "i");
            strParams = strParams.Replace("&amp;#xE7;", "ç");
            strParams = strParams.Replace("&amp;#xE8;", "è");
            strParams = strParams.Replace("&amp;#xE9;", "é");
            strParams = strParams.Replace("&amp;#xEA;", "ê");
            strParams = strParams.Replace("&amp;#xEE;", "î");
            strParams = strParams.Replace("&amp;#xF3;", "ó");
            strParams = strParams.Replace("&amp;#xF4;", "ô");
            strParams = strParams.Replace("&amp;#xF9;", "ù");
            strParams = strParams.Replace("&amp;egrave;", "è");
            strParams = strParams.Replace("&amp;eacute;", "é");
            strParams = strParams.Replace("&amp;ccedil;", "ç");
            strParams = strParams.Replace("#224;", "à");
            strParams = strParams.Replace("#244;", "ô");
            strParams = strParams.Replace("#238;", "î");
            strParams = strParams.Replace("#239;", "ï");
            strParams = strParams.Replace("&amp;", "");
            strParams = strParams.Replace("amp;", "");
            strParams = strParams.Replace("Ã©", "é");
            strParams = strParams.Replace("Ã§", "c");
            strParams = strParams.Replace("\n", "");
            strParams = strParams.Replace("\r", "");
            strParams = strParams.Replace("<p>", " ");
            strParams = strParams.Replace("<u>", " ");
            strParams = strParams.Replace("/b", " ");
            strParams = strParams.Replace("&quot;", "");
            strParams = strParams.Replace("/>", "");
            strParams = strParams.Replace("<ul>", "");
            strParams = strParams.Replace("</ul>", "");
            strParams = strParams.Replace("<li>", "");
            strParams = strParams.Replace("</li>", "");
            strParams = strParams.Replace("<td>", "");
            strParams = strParams.Replace("</td>", "");
            strParams = strParams.Replace("<tr>", "");
            strParams = strParams.Replace("</tr>", "");
            strParams = strParams.Replace("</I>", "");
            strParams = strParams.Replace("<I>", "");
            strParams = strParams.Replace("</B>", "");
            strParams = strParams.Replace("<B>", "");
            strParams = strParams.Replace("<B >", "");
            strParams = strParams.Replace("<sup>", "");
            strParams = strParams.Replace("</sup>", "");
            return strParams.ToString().Trim();
        }
        public static string PurgNfo(string strParamValue, string strNfoLine, string strRegEx)
        {
            string strResults = strNfoLine;
            strResults = strResults.Substring(strResults.IndexOf(strParamValue, StringComparison.Ordinal));
            strResults = strResults.Replace(strParamValue, "");
            strResults = Regex.Replace(strResults, strRegEx, "");
            return strResults.Trim();
        }
        public static string PurgNfoForUrl(string strParamValue, string strNfoLine)
        {
            string strResults = strNfoLine;
            strResults = strResults.Replace(strParamValue, "");
            strResults = Regex.Replace(strResults, @"[^a-zA-Z0-9 /.]", "");

            if (strResults.Contains("Links"))
                strResults = strResults.Replace("Links", "").Trim();

            if (strResults.Contains("URL"))
                strResults = strResults.Replace("URL", "").Trim();

            if (
                strResults.ToUpper(CultureInfo.InvariantCulture).Contains("http://".ToUpper(CultureInfo.InvariantCulture)) ==
                false)
                strResults = "http://" + strResults.Trim();

            return strResults.Trim();
        }

        public static bool ReplaceDb(string strConnectionString, string newDb)
        {
            try
            {
                if (string.IsNullOrEmpty(strConnectionString))
                    return false;

                //FIX 2.8.10.0
                string strFilePath = ParseConnectionString(strConnectionString);
                strFilePath = strFilePath.Replace('"', ' ').Trim();

                File.Copy(newDb, strFilePath, true);

                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, strConnectionString);
                return false;
            }
        }
        public static string RenameFile(string title, string filename, string filepath)
        {
            try
            {
                string path = Path.Combine(filepath, filename);
                string newname = filename;
                if (File.Exists(path))
                {
                    string extension = Path.GetExtension(path);
                    newname = title + extension;
                    File.Move(path, Path.Combine(filepath, newname));
                }
                else if (Directory.Exists(path))
                {
                    newname = title;
                    Directory.Move(path, Path.Combine(filepath, newname));
                }
                return newname;
            }
             //FIX 2.8.9.0
            catch (Exception ex)
            {
                LogException(ex, filename + " " + filepath);
                return string.Empty;
            }
        }
        public static string RemoveDate(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return title;

            string strResults = title.Trim();
            int indexStart = strResults.IndexOf('(');
            int indexEnd = strResults.IndexOf(')');

            if (indexStart > -1 && indexEnd > -1)
            {
                strResults = strResults.Substring(0, indexStart) + strResults.Substring(indexEnd + 1);
            }

            return strResults.Trim();
        }

        private static void SaveAsAai(Bitmap image, string path)
        {
            if (image == null || string.IsNullOrWhiteSpace(path)) return;

            byte[] totalbytes = new byte[image.Width * image.Height * 4 + 8];
            byte[] withbytes = BitConverter.GetBytes(image.Width);
            byte[] heightbytes = BitConverter.GetBytes(image.Height);

            totalbytes[0] = withbytes[0];
            totalbytes[1] = withbytes[1];
            totalbytes[2] = withbytes[2];
            totalbytes[3] = withbytes[3];
            totalbytes[4] = heightbytes[0];
            totalbytes[5] = heightbytes[1];
            totalbytes[6] = heightbytes[2];
            totalbytes[7] = heightbytes[3];

            int i = 8;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (i + 4 > totalbytes.Length)
                        break;
                    totalbytes[i] = image.GetPixel(x, y).B;
                    totalbytes[i + 1] = image.GetPixel(x, y).G;
                    totalbytes[i + 2] = image.GetPixel(x, y).R;
                    totalbytes[i + 3] = (image.GetPixel(x, y).A == 255) ? (byte)254 : image.GetPixel(x, y).A;

                    i = i + 4;
                }
            }

            FileStream filestream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            filestream.Write(totalbytes, 0, totalbytes.Length);
            filestream.Close();
        }

        public static void SaveAsAai(Image image, string path)
        {
            if (image == null || string.IsNullOrWhiteSpace(path)) return;

            Bitmap bitmap = image as Bitmap;

            SaveAsAai(bitmap, path);
        }
        public static byte[] SaveImageData(ImageSource image)
        {
            if (image == null) return null;

            byte[] imageData = GetEncodedImageData(image, ".png");

            return imageData;
        }
        private static byte[] SaveImageData(Image image)
        {

            byte[] ret;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                ret = ms.ToArray();
            }

            return ret;
        }

        public static string ToBase64(byte[] data)
        {
            if (data == null)
                return string.Empty;

            var builder = new StringBuilder();

            using (var writer = new StringWriter(builder))
            {
                using (var transformation = new ToBase64Transform())
                {
                    // Transform the data in chunks the size of InputBlockSize.
                    var bufferedOutputBytes = new byte[transformation.OutputBlockSize];
                    var i = 0;
                    var inputBlockSize = transformation.InputBlockSize;

                    while (data.Length - i > inputBlockSize)
                    {
                        transformation.TransformBlock(data, i, data.Length - i, bufferedOutputBytes, 0);
                        i += inputBlockSize;
                        writer.Write(Encoding.UTF8.GetString(bufferedOutputBytes));
                    }

                    // Transform the final block of data.
                    bufferedOutputBytes = transformation.TransformFinalBlock(data, i, data.Length - i);
                    writer.Write(Encoding.UTF8.GetString(bufferedOutputBytes));

                    // Free up any used resources.
                    transformation.Clear();
                }

                writer.Close();
            }

            return builder.ToString();
        }
        public static T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we’ve reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                //use recursion to proceed with next level
                return TryFindParent<T>(parentObject);
            }
        }
        private static string Uncrypte(string tobeDecrypted)
        {
            byte[] encryptedTextBytes = Convert.FromBase64String(tobeDecrypted);
            MemoryStream ms = new MemoryStream();
            SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
            byte[] rgbIv = Encoding.ASCII.GetBytes("ryojvlzmdalyglrj");
            byte[] key = Encoding.ASCII.GetBytes("hcxilkqbbhczfeultgbskdmaunivmfuo");

            CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key, rgbIv),
            CryptoStreamMode.Write);

            cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
            cs.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string GetValueBetween(string strItem, string strStart, string strEnd)
        {
            string strTemp = strItem.Substring(strItem.IndexOf(strStart, StringComparison.Ordinal) + strStart.Length);

            int intPosition = strTemp.IndexOf(strEnd, StringComparison.Ordinal);
            if (intPosition >= 0)
                strTemp = strTemp.Substring(0, intPosition);

            return strTemp.Trim();
        }
        public static List<DirectoryInfo> GetFolders(string strPath, SearchOption objOption, bool listHidden)
        {
            Stack<string> objFolders = new Stack<string>();
            List<DirectoryInfo> objResults = new List<DirectoryInfo>();

            if (Directory.Exists(strPath) == false)
                throw new ArgumentException("Path not exist");

            objFolders.Push(strPath);

            while (objFolders.Count > 0)
            {
                string strCurrentDir = objFolders.Pop();
                DirectoryInfo[] objSubFolder;
                try
                {
                    DirectoryInfo objFolder = new DirectoryInfo(strCurrentDir);
                    objSubFolder = objFolder.GetDirectories();
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (DirectoryInfo item in objSubFolder)
                {
                    //Fixed since 2.6.0.0
                    if (listHidden == false)
                    {
                        if ((item.Attributes & FileAttributes.Hidden) == 0)
                        {
                            objResults.Add(item);
                            objFolders.Push(item.FullName);
                        }
                    }
                    else
                    {
                        objResults.Add(item);
                        objFolders.Push(item.FullName);
                    }
                }

                if (objOption == SearchOption.TopDirectoryOnly)
                    break;
            }

            return objResults;
        }
        public static int? ParseRunTime(string strRunTime)
        {
            int intTmp;
            if (strRunTime.IndexOf(":", StringComparison.Ordinal) > 0)
            {
                string[] objtemp = strRunTime.Split(":".ToCharArray());

                int intRunTime;
                if (int.TryParse(objtemp[0], out intRunTime))
                {
                    intRunTime = intRunTime * 60;
                    intRunTime += Convert.ToInt32(objtemp[1], CultureInfo.InvariantCulture);
                    return intRunTime;
                }
                else
                    return null;
            }
            else if (strRunTime.IndexOf("h", StringComparison.Ordinal) > -1)
            {
                string[] strTemp = strRunTime.Split("h".ToCharArray());

                if (int.TryParse(strTemp[0], out intTmp))
                {
                    intTmp = intTmp * 60;
                    strTemp = strTemp[1].Split("min".ToCharArray());

                    if (IsNumeric(strTemp[0]))
                        intTmp += Convert.ToInt32(strTemp[0]);

                    return intTmp;
                }
                else
                    return null;
            }
            else if (
                (strRunTime.ToUpper(CultureInfo.InvariantCulture).IndexOf("min".ToUpper(CultureInfo.InvariantCulture), StringComparison.Ordinal) +
                 strRunTime.ToUpper(CultureInfo.InvariantCulture).IndexOf("mn".ToUpper(CultureInfo.InvariantCulture), StringComparison.Ordinal)) >
                -1)
            {
                string[] strTemp =
                    strRunTime.ToUpper(CultureInfo.InvariantCulture).Split(
                        "min".ToUpper(CultureInfo.InvariantCulture).ToCharArray());
                intTmp = Convert.ToInt32(strTemp[0]);
                return intTmp;
            }
            else
            {
                if (int.TryParse(strRunTime.Trim(), out intTmp))
                {
                    return intTmp;
                }
                else
                    return null;
            }
        }
        public static string PurgeTitle(string strTitle)
        {
            StringBuilder objTitle = new StringBuilder(strTitle);
            objTitle = objTitle.Replace(":", " ");
            return objTitle.ToString();
        }
        public static byte[] GetImageFromPdf(string strFilePath)
        {
            //FIX 2.8.8.0
            try
            {

                PdfReader objPdf = new PdfReader(strFilePath);
                RandomAccessFileOrArray objRaf = new RandomAccessFileOrArray(strFilePath);
                PdfDictionary objPage = objPdf.GetPageN(1);
                PdfDictionary objRessources = (PdfDictionary)PdfReader.GetPdfObject(objPage.Get(PdfName.RESOURCES));
                PdfDictionary objObjects = (PdfDictionary)PdfReader.GetPdfObject(objRessources.Get(PdfName.XOBJECT));

                if (objObjects == null) return null;

                foreach (PdfName item in objObjects.Keys)
                {
                    PdfObject objTemp = objObjects.Get(item);

                    if (objTemp.IsIndirect())
                    {
                        PdfDictionary objImage = (PdfDictionary)PdfReader.GetPdfObject(objTemp);
                        PdfName objType = (PdfName)PdfReader.GetPdfObject(objImage.Get(PdfName.SUBTYPE));

                        if (PdfName.IMAGE.Equals(objType))
                        {
                            int intIndex = Convert.ToInt32(((PRIndirectReference)objTemp).Number.ToString(CultureInfo.InvariantCulture),
                                                           CultureInfo.InvariantCulture);
                            PdfStream objPdfStream = (PdfStream)objPdf.GetPdfObject(intIndex);
                            byte[] objResults = PdfReader.GetStreamBytesRaw((PRStream)objPdfStream);
                            objPdf.Close();
                            objRaf.Close();
                            return objResults;
                        }
                    }
                }
                objPdf.Close();
                objRaf.Close();
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool BackupDb(string strConnectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(strConnectionString))
                    return false;

                //FIX 2.8.10.0
                string strFilePath = ParseConnectionString(strConnectionString);
                strFilePath = strFilePath.Replace('"', ' ').Trim();
                string strPath = strFilePath.Substring(0, strFilePath.LastIndexOf(Path.DirectorySeparatorChar)).Trim();

                strPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "myCollections", strPath);
                strPath = Path.GetFullPath(strPath);

                BackupDb(strConnectionString, strPath);

                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, strConnectionString);
                return false;
            }
        }
        public static void BackupDb(string strConnectionString, string destinationPath)
        {
            try
            {
                if (string.IsNullOrEmpty(strConnectionString))
                    return;

                //FIX 2.8.10.0
                string strFilePath = ParseConnectionString(strConnectionString);
                strFilePath = strFilePath.Replace('"', ' ').Trim();

                if (Directory.Exists(destinationPath) == false)
                    Directory.CreateDirectory(destinationPath);

                File.Copy(strFilePath, Path.Combine(destinationPath, "myCollectionBackup_" + DateTime.Now.ToFileTime() + ".db"), true);

            }
            catch (Exception ex)
            {
                LogException(ex, strConnectionString);
            }
        }
        public static string ParseConnectionString(string strConnectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(strConnectionString))
                    return string.Empty;

                string[] configArray = strConnectionString.Split(';');

                string strFilePath = configArray[0].Replace(@"Data Source=", "");
                strFilePath = strFilePath.Replace('"', ' ').Trim();

                return strFilePath;
            }
            catch (Exception ex)
            {
                LogException(ex, strConnectionString);
                return string.Empty;
            }
        }
        public static void SaveFilter(EntityType etype, Filter filter)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    MySettings.AppsSelection = Enum.GetName(typeof(Filter), filter);
                    break;
                case EntityType.Books:
                    MySettings.BooksSelection = Enum.GetName(typeof(Filter), filter);
                    break;
                case EntityType.Games:
                    MySettings.GamesSelection = Enum.GetName(typeof(Filter), filter);
                    break;
                case EntityType.Movie:
                    MySettings.MoviesSelection = Enum.GetName(typeof(Filter), filter);
                    break;
                case EntityType.Music:
                    MySettings.MusicsSelection = Enum.GetName(typeof(Filter), filter);
                    break;
                case EntityType.Nds:
                    MySettings.NdsSelection = Enum.GetName(typeof(Filter), filter);
                    break;
                case EntityType.Series:
                    MySettings.SeriesSelection = Enum.GetName(typeof(Filter), filter);
                    break;
                case EntityType.XXX:
                    MySettings.XXXSelection = Enum.GetName(typeof(Filter), filter);
                    break;
            }

        }
        public static void SaveGroupBy(EntityType etype, GroupBy groupby)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    MySettings.AppsGroupBy = Enum.GetName(typeof(GroupBy), groupby);
                    break;
                case EntityType.Books:
                    MySettings.BooksGroupBy = Enum.GetName(typeof(GroupBy), groupby);
                    break;
                case EntityType.Games:
                    MySettings.GamesGroupBy = Enum.GetName(typeof(GroupBy), groupby);
                    break;
                case EntityType.Movie:
                    MySettings.MoviesGroupBy = Enum.GetName(typeof(GroupBy), groupby);
                    break;
                case EntityType.Music:
                    MySettings.MusicsGroupBy = Enum.GetName(typeof(GroupBy), groupby);
                    break;
                case EntityType.Nds:
                    MySettings.NdsGroupBy = Enum.GetName(typeof(GroupBy), groupby);
                    break;
                case EntityType.Series:
                    MySettings.SeriesGroupBy = Enum.GetName(typeof(GroupBy), groupby);
                    break;
                case EntityType.XXX:
                    MySettings.XXXGroupBy = Enum.GetName(typeof(GroupBy), groupby);
                    break;
            }

        }
        public static void SaveOrder(EntityType etype, Order order)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    MySettings.AppsOrder = Enum.GetName(typeof(Order), order);
                    break;
                case EntityType.Books:
                    MySettings.BooksOrder = Enum.GetName(typeof(Order), order);
                    break;
                case EntityType.Games:
                    MySettings.GamesOrder = Enum.GetName(typeof(Order), order);
                    break;
                case EntityType.Movie:
                    MySettings.MoviesOrder = Enum.GetName(typeof(Order), order);
                    break;
                case EntityType.Music:
                    MySettings.MusicsOrder = Enum.GetName(typeof(Order), order);
                    break;
                case EntityType.Nds:
                    MySettings.NdsOrder = Enum.GetName(typeof(Order), order);
                    break;
                case EntityType.Series:
                    MySettings.SeriesOrder = Enum.GetName(typeof(Order), order);
                    break;
                case EntityType.XXX:
                    MySettings.XXXOrder = Enum.GetName(typeof(Order), order);
                    break;
            }
        }
        public static void SaveView(EntityType etype, View view)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    MySettings.AppsView = Enum.GetName(typeof(View), view);
                    break;
                case EntityType.Books:
                    MySettings.BooksView = Enum.GetName(typeof(View), view);
                    break;
                case EntityType.Games:
                    MySettings.GamesView = Enum.GetName(typeof(View), view);
                    break;
                case EntityType.Movie:
                    MySettings.MoviesView = Enum.GetName(typeof(View), view);
                    break;
                case EntityType.Music:
                    MySettings.MusicsView = Enum.GetName(typeof(View), view);
                    break;
                case EntityType.Nds:
                    MySettings.NdsView = Enum.GetName(typeof(View), view);
                    break;
                case EntityType.Series:
                    MySettings.SeriesView = Enum.GetName(typeof(View), view);
                    break;
                case EntityType.XXX:
                    MySettings.XXXView = Enum.GetName(typeof(View), view);
                    break;
            }
        }
        public static void SaveZoom(EntityType etype, double zoom)
        {
            switch (etype)
            {
                case EntityType.Apps:
                    MySettings.AppsZoom = zoom;
                    break;
                case EntityType.Books:
                    MySettings.BooksZoom = zoom;
                    break;
                case EntityType.Games:
                    MySettings.GamesZoom = zoom;
                    break;
                case EntityType.Movie:
                    MySettings.MoviesZoom = zoom;
                    break;
                case EntityType.Music:
                    MySettings.MusicsZoom = zoom;
                    break;
                case EntityType.Nds:
                    MySettings.NdsZoom = zoom;
                    break;
                case EntityType.Series:
                    MySettings.SeriesZoom = zoom;
                    break;
                case EntityType.XXX:
                    MySettings.XXXZoom = zoom;
                    break;
            }
        }

        public static void GetDriveInfo(string path, out int freeSpace, out int totalSpace)
        {
            freeSpace = -1;
            totalSpace = -1;

            try
            {
                //FIX 2.8.9.0
                string strDriveLetter = path.Substring(0, 1);
                if (strDriveLetter != @"\")
                {
                    ManagementObject objDrive =
                        new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter + ":\"");
                    objDrive.Get();

                    double dblSize;
                    if (Double.TryParse(objDrive["FreeSpace"].ToString(), out dblSize))
                        freeSpace = Convert.ToInt32(dblSize/1024d/1024d/1024d);

                    if (Double.TryParse(objDrive["Size"].ToString(), out dblSize))
                        totalSpace = Convert.ToInt32(dblSize/1024d/1024d/1024d);
                }
            }
            catch (Exception ex)
            {
                LogExceptionAsync(ex, path);
            }
        }
        public static string GetElementValue(XElement node, string elementName)
        {
            var query = from element in node.Elements()
                        where element.Name == elementName
                        select element.Value;

            List<string> elementeValueList = new List<string>(query);
            return (elementeValueList.Count > 0)
                       ? elementeValueList[0]
                       : string.Empty;
        }
        public static string GetElementValue(XElement node, string elementName,
                                             string attributName, string attributValue)
        {
            var query = from element in node.Elements()
                        let xAttribute = element.Attribute(attributName)
                        where xAttribute != null && (element.Name == elementName &&
                                                     xAttribute.Value == attributValue)
                        select element.Value;
            List<string> elementeValueList = new List<string>(query);
            return (elementeValueList.Count > 0)
                       ? elementeValueList[0]
                       : string.Empty;
        }
        public static string GetElementValue(XElement node, string elementName, string attributName)
        {
            var query = from element in node.Elements()
                        where element.Name == elementName
                        select element;

            List<XElement> elementeValueList = query.ToList();
            if (elementeValueList.Count > 0)
                return GetElementValue(elementeValueList[0], attributName);
            else
                return string.Empty;

        }

        public static string GetAttributValue(XElement node, string attributeName)
        {
            var query = from element in node.Attributes()
                        where element.Name == attributeName
                        select element.Value;

            List<string> elementeValueList = new List<string>(query);
            return (elementeValueList.Count > 0)
                       ? elementeValueList[0]
                       : string.Empty;
        }
        public static string GetAttributValue(XElement node, string elementName,
                                          string attributName)
        {
            var query = from element in node.Elements()
                        where element.Name == elementName
                        select element;

            List<XElement> elementeValueList = query.ToList();
            if (elementeValueList.Count > 0)
                return GetAttributValue(elementeValueList[0], attributName);
            else
                return string.Empty;
        }

        private static string GetOsName()
        {

            const string osQuery = "SELECT * FROM Win32_OperatingSystem";
            string strName = string.Empty;
            ManagementObjectSearcher osSearcher = new ManagementObjectSearcher(osQuery);
            foreach (ManagementObject info in osSearcher.Get())
            {
                strName = info.Properties["Caption"].Value.ToString().Trim();
            }
            return strName;

        }
        public static Version GetAppVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version currentVersion = assembly.GetName().Version;
            return currentVersion;
        }
    }
}