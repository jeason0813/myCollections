using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using myCollections.Utils;
using EntityType = myCollections.Utils.EntityType;

namespace myCollections.Data.SqlLite
{
    internal class Dal : IDisposable
    {
        private static readonly Dal DalInstance = new Dal();
        private static string _connectionString;

        static Dal()
        {
        }

        private Dal()
        {

            _connectionString = ConfigurationManager.ConnectionStrings["myCollectionsSQLiteEntities"].ConnectionString;

            string strPath = Path.GetFullPath(Util.ParseConnectionString(_connectionString));
            if (File.Exists(strPath) == false)
                throw new FileNotFoundException("Can't find Database : " + strPath + ". Please go to options to change it");

        }
        #region Const
        #region Apps
        private const string ConstAppsThumb = @" Apps.Id, Apps.Title, smallCover, AddedDate, IsDeleted, Rating, Description, TobeDeleted, Media.Name, IsWhish, IsComplete, 
                                                IsTested, ToTest, FileName, FilePath, 'None', PublicRating, Apps.NumID ";

        private const string ConstApps = @" Apps.Id, Apps.Title, Apps.BarCode, Apps.Language_Id, Apps.Editor_Id, Apps.Media_Id, Apps.Version, 
                                            Apps.Description, Apps.IsDeleted, Apps.IsWhish,  Apps.IsComplete, Apps.AddedDate, Apps.ReleaseDate, Apps.FilePath,
                                            Apps.FileName, Apps.Rating, Apps.IsTested, Apps.Comments, Apps.ToBeDeleted, Apps.smallCover, Apps.ToTest,
                                            Media.Name,Apps.PublicRating, Apps.NumID ";
        #endregion
        #region Books
        private const string ConstBooksThumb = @"  Books.Id, Books.Title, smallCover, AddedDate, IsDeleted, Rating, Description, ToBeDeleted, Media.Name,
                                                   IsWhish, IsComplete, IsRead, ToRead, FileName, FilePath, 'None', PublicRating, Books.NumID ";
        private const string ConstBookThumbArtistPart = @", Artist.FirstName, Artist.LastName, Job.Name ";
        private const string ConstBookThumbTypePart = @", BookType.DisplayName ";
        private const string ConstBooks = @" Books.Id, Books.Title, Books.BarCode, Books.Editor_Id, Books.Format_Id, Books.Media_Id, Books.Language_Id, Books.ISBN,
                                             Books.NbrPages, Books.IsRead, Books.IsDeleted, Books.IsWhish, Books.IsComplete, Books.Description, Books.FileName,
                                             Books.FilePath, Books.AddedDate, Books.ReleaseDate, Books.ToBeDeleted, Books.Rating, Books.Comments, Books.Rated,
                                             Books.smallCover, Books.ToRead, Media.Name,Books.PublicRating, Books.NumID ";
        #endregion
        #region Games
        private const string ConstGamesThumb = @"  Gamez.Id, Gamez.Title, smallCover, AddedDate, IsDeleted, Rating, Descriptions, ToBeDeleted, Media.Name,
                                                            IsWhish, IsComplete, IsTested, ToTest, FileName, FilePath, 'None', PublicRating, Gamez.NumID ";
        private const string ConstGames = @" Gamez.Id, Gamez.Language_Id, Gamez.Media_Id, Gamez.Editor_Id, Gamez.Title, Gamez.BarCode, Gamez.Rating,
                                            Gamez.Descriptions, Gamez.ReleaseDate, Gamez.Comments, Gamez.FileName, Gamez.AddedDate, Gamez.IsDeleted,
                                            Gamez.IsWhish, Gamez.IsComplete, Gamez.FilePath, Gamez.IsTested, Gamez.ToBeDeleted, Gamez.Rated,
                                            Gamez.smallCover, Gamez.Platform_Id, Gamez.Price, Gamez.ToTest, Media.Name,Gamez.PublicRating, Gamez.NumID ";
        #endregion
        #region Movie
        private const string ConstMovieThumb = @"  Movie.Id, Movie.Title, smallCover, AddedDate, Runtime, OriginalTitle, IsDeleted, Imdb, AlloCine, Rating, Description, TobeDeleted, Media.Name, 
                                                   IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, 'None', PublicRating, NumID  ";
        private const string ConstMovieThumbArtistPart = @", Artist.FirstName, Artist.LastName, Job.Name ";

        private const string ConstMovie = @" Movie.Id, Movie.Studio_Id, Movie.Media_Id, Movie.FileFormat_Id, Movie.AspectRatio_Id, Movie.Title, Movie.BarCode, Movie.Imdb,
                                            Movie.AlloCine, Movie.Rating, Movie.Description, Movie.Runtime, Movie.Tagline, Movie.Rated, Movie.ReleaseDate, Movie.Seen,
                                            Movie.IsDeleted, Movie.IsWhish, Movie.IsComplete, Movie.FileName, Movie.AddedDate, Movie.FilePath, Movie.OriginalTitle,
                                            Movie.TobeDeleted, Movie.Comments, Movie.Country, Movie.smallCover, Movie.ToWatch, Movie.Goofs, Media.Name,Movie.PublicRating, Movie.NumID";
        #endregion
        #region Music
        private const string ConstMusicThumb = @" Music.Id, Music.Title, smallCover, AddedDate, IsDeleted, Rating, Comments, ToBeDeleted, Media.Name,
                                                IsWhish, IsComplete, IsHear, ToHear, FileName, FilePath, 'None', PublicRating, Music.NumID ";
        private const string ConstMusicThumbArtistPart = @", Artist.FirstName, Artist.LastName, Job.Name ";
        private const string ConstMusicThumbTypePart = @", Music_Genre.DisplayName ";
        private const string ConstMusic = @" Music.Id, Music.Title, Music.BarCode, Music.Media_Id, Music.Studio_Id, Music.Genre_Id, Music.FileFormat_Id, Music.Rating,
                                            Music.Album, Music.Length,Music.BitRate, Music.AddedDate, Music.IsDeleted, Music.IsWhish, Music.IsComplete, Music.IsHear,
                                            Music.ReleaseDate, Music.FileName, Music.FilePath, Music.ToBeDeleted, Music.Comments, Music.smallCover, Music.ToHear,
                                            Media.Name,Music.PublicRating, Music.NumID";
        #endregion
        #region Nds
        private const string ConstNdsThumb = @"  Nds.Id, Nds.Title, smallCover, AddedDate, IsDeleted, Rating, Description, ToBeDeleted, Media.Name,
                                              IsWhish, IsComplete, IsTested, ToTest, FileName, FilePath, 'None', PublicRating, Nds.NumID ";
        private const string ConstNds = @" Nds.Id, Nds.Title, Nds.BarCode, Nds.Editor_Id, Nds.Language_Id, Nds.Media_Id, Nds.Rating, Nds.Description, Nds.ReleaseDate, Nds.Comments,
                                           Nds.FileName, Nds.FilePath, Nds.AddedDate, Nds.IsDeleted, Nds.IsWhish, Nds.IsComplete, Nds.ToBeDeleted, Nds.IsTested, Nds.Rated,  Nds.smallCover,
                                           Nds.ToTest, Media.Name,Nds.PublicRating, Nds.NumID ";
        #endregion
        #region Series
        private const string ConstSerieThumb = @" Series_Season.Id, Series.Title, smallCover, AddedDate, Series.Runtime, IsDeleted, Rating, Series.Description, TobeDeleted, Media.Name, 
                                                 IsWhish, IsComplete,Seen, ToWatch, FilesPath, 'None',Series_Season.Season, Series_Season.PublicRating, Series_Season.NumID  ";
        private const string ConstSerie = @" Series.Id, Series.Studio_Id, Series.Title, Series.IsInProduction,  Series.Country, Series.Description, Series.OfficialWebSite, Series.RunTime,
                                             Series.Rated, Series_Season.Id, Series_Season.Series_Id, Series_Season.Media_Id, Series_Season.Season, Series_Season.BarCode, Series_Season.TotalEpisodes,
                                             Series_Season.AvailableEpisodes, Series_Season.MissingEpisodes, Series_Season.Seen, Series_Season.AddedDate, Series_Season.IsComplete, Series_Season.FilesPath,
                                             Series_Season.IsDeleted, Series_Season.IsWhish, Series_Season.ToBeDeleted, Series_Season.Rating, Series_Season.ReleaseDate, Series_Season.Comments, Series_Season.smallCover,
                                             Series_Season.ToWatch, Media.Name,Series_Season.PublicRating, Series_Season.NumID ";
        #endregion
        #region XXX
        private const string ConstXxxThumb = @"SELECT XXX.Id, XXX.Title, XXX.smallCover, XXX.AddedDate, XXX.Runtime, XXX.IsDeleted, XXX.Rating, XXX.Description, 
                                               XXX.ToBeDeleted, Media.Name, XXX.IsWhish, XXX.IsComplete,XXX.Seen, XXX.ToWatch, XXX.FileName, XXX.FilePath, 
                                               'None', PublicRating, XXX.NumID ";
        private const string ConstXxxThumbArtistPart = @", Artist.FirstName, Artist.LastName, Job.Name ";
        private const string ConstXxx = @" XXX.Title, XXX.Id, XXX.Media_Id, XXX.Language_Id, XXX.FileFormat_Id, XXX.BarCode, XXX.Rating, XXX.Description,
                                           XXX.Runtime, XXX.Seen, XXX.ReleaseDate, XXX.Comments, XXX.IsDeleted, XXX.IsComplete, XXX.IsWhish, XXX.AddedDate,
                                           XXX.FileName, XXX.FilePath, XXX.ToBeDeleted, XXX.Studio_Id, XXX.Country, XXX.smallCover, XXX.ToWatch, Media.Name,
                                           XXX.PublicRating, XXX.NumID ";
        #endregion
        #endregion
        #region Reference Table

        #region Artist
        public void AddArtist(Artist objItem, IMyCollectionsData entity)
        {
            //FIX 2.7.12.0
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(objItem.Id))
            {
                objItem.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = @"INSERT INTO Artist (Id,FirstName,LastName,BirthDay,Picture,Bio,PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,
                                                            FulleName,Aka,Sex)
                                                    VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,
                                                           @param12,@param13,@param14)";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Artist 
                                                        SET FirstName=@param2,LastName=@param3,BirthDay=@param4,Picture=@param5,Bio=@param6,PlaceBirth=@param7,
                                                            WebSite=@param8, YearsActive=@param9, Ethnicity=@param10, Breast=@param11, FulleName=@param12, 
                                                            Aka=@param13, Sex=@param14
                                                         WHERE id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = objItem.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = objItem.FirstName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = objItem.LastName;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.DateTime);
            param4.Value = objItem.BirthDay;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.Binary);
            param5.Value = objItem.Picture;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = objItem.Bio;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
            param7.Value = objItem.PlaceBirth;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
            param8.Value = objItem.WebSite;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.String);
            param9.Value = objItem.YearsActive;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.String);
            param10.Value = objItem.Ethnicity;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.String);
            param11.Value = objItem.Breast;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.String);
            param12.Value = objItem.FulleName;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.String);
            param13.Value = objItem.Aka;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Boolean);
            param14.Value = objItem.Sex;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            LinkArtist(entity, objItem.Job.Id, objItem.Id);
            AddArtistCredits(objItem);

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        public void AddArtist(Artist objItem)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(objItem.Id))
            {
                objItem.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = @"INSERT INTO Artist (Id,FirstName,LastName,BirthDay,Picture,Bio,PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,
                                                            FulleName,Aka,Sex)
                                                    VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,
                                                           @param12,@param13,@param14)";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Artist 
                                                        SET FirstName=@param2,LastName=@param3,BirthDay=@param4,Picture=@param5,Bio=@param6,PlaceBirth=@param7,
                                                            WebSite=@param8, YearsActive=@param9, Ethnicity=@param10, Breast=@param11, FulleName=@param12, 
                                                            Aka=@param13, Sex=@param14
                                                         WHERE id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = objItem.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = objItem.FirstName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = objItem.LastName;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.DateTime);
            param4.Value = objItem.BirthDay;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.Binary);
            param5.Value = objItem.Picture;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = objItem.Bio;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
            param7.Value = objItem.PlaceBirth;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
            param8.Value = objItem.WebSite;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.String);
            param9.Value = objItem.YearsActive;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.String);
            param10.Value = objItem.Ethnicity;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.String);
            param11.Value = objItem.Breast;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.String);
            param12.Value = objItem.FulleName;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.String);
            param13.Value = objItem.Aka;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Boolean);
            param14.Value = objItem.Sex;


            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            AddArtistCredits(objItem);

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        private ThumbItem ArtistToThumb(SQLiteDataReader reader)
        {
            ThumbItem item = new ThumbItem();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(3) == false)
                item.ReleaseDate = reader.GetDateTime(3);

            if (reader.IsDBNull(4) == false)
                item.Cover = (byte[])reader.GetValue(4);

            if (reader.IsDBNull(5) == false)
                item.Description = reader.GetString(5);

            item.EType = EntityType.Artist;

            if (reader.IsDBNull(11) == false)
                item.Name = reader.GetString(11);

            return item;
        }

        public void DeleteArtist(string id)
        {

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            UnlinkArtist(id, EntityType.Books);
            UnlinkArtist(id, EntityType.Movie);
            UnlinkArtist(id, EntityType.Music);
            UnlinkArtist(id, EntityType.Series);
            UnlinkArtist(id, EntityType.XXX);

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM Artist_Credits  
                                                      WHERE Artist_Id='{0}' 
                                                      ", id);
            objCommand.ExecuteScalar();

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM Artist  
                                                      WHERE Id='{0}' 
                                                      ", id);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public IList FindArtist(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            #region Search
            objCommand.CommandText = string.Format(@"SELECT Id, FirstName, LastName, BirthDay, Picture, Bio,
                                                            PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                     FROM Artist
                                                     WHERE Artist.FulleName LIKE ""{0}"";", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<Artist> artists = new List<Artist>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    artists.Add(ResultsToArtist(objResults));
            }

            objResults.Close();
            objResults.Dispose();

            if (artists.Count == 0)
            {
                objCommand.CommandText = string.Format(@"SELECT Id, FirstName, LastName, BirthDay, Picture, Bio,
                                                            PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                     FROM Artist
                                                     WHERE Artist.FulleName LIKE ""{0}%"";", name);

                objResults = objCommand.ExecuteReader();

                if (objResults.HasRows == true)
                {
                    while (objResults.Read())
                        artists.Add(ResultsToArtist(objResults));
                }

                objResults.Close();
                objResults.Dispose();
            }

            if (artists.Count == 0)
            {
                objCommand.CommandText = string.Format(@"SELECT Id, FirstName, LastName, BirthDay, Picture, Bio,
                                                            PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                     FROM Artist
                                                     WHERE Artist.FulleName LIKE ""%{0}"";", name);

                objResults = objCommand.ExecuteReader();

                if (objResults.HasRows == true)
                {
                    while (objResults.Read())
                        artists.Add(ResultsToArtist(objResults));
                }

                objResults.Close();
                objResults.Dispose();
            }

            if (artists.Count == 0)
            {
                objCommand.CommandText = string.Format(@"SELECT Id, FirstName, LastName, BirthDay, Picture, Bio,
                                                            PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                     FROM Artist
                                                     WHERE Artist.FulleName LIKE ""%{0}%"";", name);

                objResults = objCommand.ExecuteReader();

                if (objResults.HasRows == true)
                {
                    while (objResults.Read())
                        artists.Add(ResultsToArtist(objResults));
                }

                objResults.Close();
                objResults.Dispose();
            }

            if (artists.Count == 0)
            {
                objCommand.CommandText = string.Format(@"SELECT Id, FirstName, LastName, BirthDay, Picture, Bio,
                                                            PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                     FROM Artist
                                                     WHERE Artist.FirstName LIKE ""{0}"" 
                                                     OR Artist.LastName LIKE ""{0}""", name);

                objResults = objCommand.ExecuteReader();

                if (objResults.HasRows == true)
                {
                    while (objResults.Read())
                        artists.Add(ResultsToArtist(objResults));
                }

                objResults.Close();
                objResults.Dispose();
            }
            #endregion
            List<ThumbItem> thumbItems = new List<ThumbItem>();

            foreach (Artist artist in artists)
            {
                #region Books

                objCommand.CommandText = string.Format(@"SELECT 
                                                          Books.Id, Books.Title,Books.Cover, Books.AddedDate,Books.IsDeleted,Books.Rating, Books.Description, Books.ToBeDeleted, Media.Name,
                                                          Books.IsWhish,  Books.IsComplete, Books.IsRead, Books.ToRead,  Books.FileName, Books.FilePath, 'None',
                                                          Artist.FirstName, Artist.LastName, Job.Name                                                    
                                                        FROM Books
                                                        LEFT OUTER JOIN Book_Artist_Job ON (Books.Id=Book_Artist_Job.Book_Id)
                                                        LEFT OUTER JOIN Artist ON (Book_Artist_Job.Artist_Id = Artist.Id)
                                                        LEFT OUTER JOIN Job ON (Book_Artist_Job.Job_Id = Job.Id)
                                                        INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                        WHERE Book_Artist_Job.Artist_Id='{0}';", artist.Id);

                objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                    while (objResults.Read())
                        thumbItems.Add(BooksToThumb(objResults));

                objResults.Close();
                objResults.Dispose();

                #endregion
                #region Movies
                objCommand.CommandText = string.Format(@"SELECT Movie.Id, Title, smallCover, AddedDate, Runtime, OriginalTitle, IsDeleted, Imdb, AlloCine, Rating, Description, TobeDeleted, Media.Name, 
                                                                IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, 'None', Artist.FirstName, Artist.LastName, Job.Name 
                                                          FROM Movie
                                                          LEFT OUTER JOIN Movie_Artist_Job ON (Movie.Id=Movie_Artist_Job.Movie_Id)
                                                          LEFT OUTER JOIN Artist ON (Movie_Artist_Job.Artist_Id = Artist.Id)
                                                          LEFT OUTER JOIN Job ON (Movie_Artist_Job.Job_Id = Job.Id)
                                                          INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                          WHERE Movie_Artist_Job.Artist_Id='{0}';", artist.Id);

                objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                    while (objResults.Read())
                        thumbItems.Add(MovieToThumb(objResults));

                objResults.Close();
                objResults.Dispose();

                #endregion
                #region Music

                objCommand.CommandText = string.Format(@"SELECT Music.Id, Title, smallCover, AddedDate, IsDeleted, Rating, Comments, ToBeDeleted, Media.Name,
                                                            IsWhish, IsComplete, IsHear, ToHear, FileName, FilePath, 'None',
                                                            Artist.FirstName, Artist.LastName, Job.Name,Album
                                                         FROM Music
                                                         LEFT OUTER JOIN Music_Artist_Job ON (Music.Id=Music_Artist_Job.Music_Id)
                                                         LEFT OUTER JOIN Artist ON (Music_Artist_Job.Artist_Id = Artist.Id)
                                                         LEFT OUTER JOIN Job ON (Music_Artist_Job.Job_Id = Job.Id)
                                                         INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                         WHERE Music_Artist_Job.Artist_Id='{0}';", artist.Id);

                objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                    while (objResults.Read())
                        thumbItems.Add(MusicToThumb(objResults));

                objResults.Close();
                objResults.Dispose();

                #endregion
                #region Series
                objCommand.CommandText = string.Format(@"SELECT Series_Season.Id, Series.Title, Series_Season.smallCover, Series_Season.AddedDate, Series.Runtime, Series_Season.IsDeleted, 
                                                            Series_Season.Rating, Series.Description, Series_Season.TobeDeleted, Media.Name, 
                                                            Series_Season.IsWhish, Series_Season.IsComplete,Series_Season.Seen, Series_Season.ToWatch, Series_Season.FilesPath, 'None',Series_Season.Season, Artist.FirstName, Artist.LastName, Job.Name 
                                                          FROM Series_Season
                                                          LEFT OUTER JOIN Series_Artist_Job ON (Series.Id=Series_Artist_Job.Series_Id)
                                                          LEFT OUTER JOIN Artist ON (Series_Artist_Job.Artist_Id = Artist.Id)
                                                          LEFT OUTER JOIN Job ON (Series_Artist_Job.Job_Id = Job.Id)
                                                          INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                          INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                          WHERE Series_Artist_Job.Artist_Id='{0}';", artist.Id);

                objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                    while (objResults.Read())
                        thumbItems.Add(SeriesToThumb(objResults));

                objResults.Close();
                objResults.Dispose();
                #endregion
                #region XXX
                objCommand.CommandText = string.Format(@"SELECT XXX.Id,  XXX.Title, smallCover, AddedDate, Runtime, IsDeleted, Rating, Description, ToBeDeleted, Media.Name, 
                                                                 IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, 'None', Artist.FirstName, Artist.LastName, Job.Name 
                                                          FROM XXX
                                                          LEFT OUTER JOIN XXX_Artist_Job ON (XXX.Id=XXX_Artist_Job.XXX_Id)
                                                          LEFT OUTER JOIN Artist ON (XXX_Artist_Job.Artist_Id = Artist.Id)
                                                          LEFT OUTER JOIN Job ON (XXX_Artist_Job.Job_Id = Job.Id)
                                                          INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                          WHERE XXX_Artist_Job.Artist_Id='{0}';", artist.Id);

                objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                    while (objResults.Read())
                        thumbItems.Add(XxxToThumb(objResults));

                objResults.Close();
                objResults.Dispose();
                #endregion
            }


            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return thumbItems;

        }

        public Artist GetArtist(string firstName, string lastname)
        {
            try
            {
                //Fix since 2.7.12.0
                firstName = firstName.Replace(@"""", "");
                lastname = lastname.Replace(@"""", "");


                SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
                SQLiteCommand objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();

                //Fix since 2.7.12.0
                objCommand.CommandText = string.Format(@"SELECT Id, FirstName, LastName, BirthDay, Picture, Bio,
                                                            PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                     FROM Artist
                                                     WHERE Artist.FirstName = ""{0}"" 
                                                     AND Artist.LastName = ""{1}""
                                                     LIMIT 1", firstName, lastname);

                SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

                Artist artist = null;

                if (objResults.HasRows == true)
                {
                    objResults.Read();
                    artist = ResultsToArtist(objResults);

                    objResults.Close();
                    objResults.Dispose();

                    artist.ArtistCredits = GetArtistCreditbyId(artist.Id);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
                return artist;

            }
            catch (Exception exception)
            {
                Util.LogException(exception, firstName + " " + lastname);
                throw;
            }
        }
        public Artist GetArtistById(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Id, FirstName, LastName, BirthDay, Picture, Bio,
                                                            PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                     FROM Artist
                                                     WHERE Artist.Id = '{0}' 
                                                     LIMIT 1", id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            Artist artist = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                artist = ResultsToArtist(objResults);

                objResults.Close();
                objResults.Dispose();

                artist.ArtistCredits = GetArtistCreditbyId(artist.Id);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return artist;
        }
        private IList<Artist> GetArtists(string entityId, EntityType entityType, bool getCredits)
        {
            string tableName = string.Empty;
            string entityName = string.Empty;
            string orderBy = string.Empty;
            switch (entityType)
            {
                case EntityType.Books:
                    tableName = "Book_Artist_Job";
                    entityName = "Book_Id";
                    break;
                case EntityType.Movie:
                    tableName = "Movie_Artist_Job";
                    entityName = "Movie_Id";
                    orderBy = string.Format("ORDER BY {0}.Added", tableName);
                    break;
                case EntityType.Music:
                    tableName = "Music_Artist_Job";
                    entityName = "Music_Id";
                    break;
                case EntityType.Series:
                    tableName = "Series_Artist_Job";
                    entityName = "Series_Id";
                    orderBy = string.Format("ORDER BY {0}.Added", tableName);
                    break;
                case EntityType.XXX:
                    tableName = "XXX_Artist_Job";
                    entityName = "XXX_Id";
                    orderBy = string.Format("ORDER BY {0}.Added", tableName);
                    break;
            }

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT {0}.Artist_Id, Artist.FirstName, Artist.LastName, Artist.BirthDay,Artist.Picture, Artist.Bio,
                                                          Artist.PlaceBirth, Artist.WebSite, Artist.YearsActive, Artist.Ethnicity, Artist.Breast,
                                                          Artist.FulleName, Artist.Aka, Artist.Sex, Job.Id, Job.Name
                                                      FROM {0}
                                                      INNER JOIN Artist ON ({0}.Artist_Id = Artist.Id)
                                                      INNER JOIN Job ON ({0}.Job_Id = Job.Id)
                                                      WHERE {0}.{1}='{2}' {3};", tableName, entityName, entityId, orderBy);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<Artist> artists = new List<Artist>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                {
                    Artist artist = ResultsToArtist(objResults);
                    artists.Add(artist);
                }
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (getCredits == true)
                foreach (Artist artist in artists)
                    artist.ArtistCredits = GetArtistCreditbyId(artist.Id);


            return artists;
        }
        public IList GetArtistFullNames()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT FulleName FROM Artist;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<string> artist = new List<string>();

            if (objResults.HasRows == true)
                while (objResults.Read())
                    if (objResults.IsDBNull(0) == false)
                        artist.Add(objResults.GetString(0));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return artist;
        }
        public IList GetArtists(string childTable)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Artist> artists = new List<Artist>();
            objCommand.CommandText = string.Format(@"SELECT Artist.Id, FirstName, LastName, BirthDay, Picture, Bio, PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                    FROM {0}
                                                    INNER JOIN Artist ON ({0}.Artist_Id = Artist.Id)", childTable);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            if (objResults.HasRows)
                while (objResults.Read())
                    artists.Add(ResultsToArtist(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return artists;
        }
        public IList GetArtistsThumb(string childTable)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<ThumbItem> artists = new List<ThumbItem>();
            objCommand.CommandText = string.Format(@"SELECT DISTINCT Artist.Id, FirstName, LastName, BirthDay, Picture, Bio, PlaceBirth,WebSite,YearsActive,Ethnicity,Breast,FulleName,Aka,Sex
                                                    FROM {0}
                                                    INNER JOIN Artist ON ({0}.Artist_Id = Artist.Id)", childTable);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            if (objResults.HasRows)
                while (objResults.Read())
                    artists.Add(ArtistToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return artists;
        }
        public IList<string> GetArtistsOwndedByName(string fullname, string table, string joinTable, string joinField)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<string> artists = new List<string>();
            objCommand.CommandText = string.Format(@"SELECT Title
                                                      FROM {0}
                                                      INNER JOIN {1} ON ({1}.{2} = {0}.Id)
                                                      INNER JOIN Artist ON ({1}.Artist_Id = Artist.Id)
                                                      WHERE Artist.FulleName=""{3}"";", table, joinTable, joinField, fullname);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            if (objResults.HasRows)
                while (objResults.Read())
                    artists.Add(objResults.GetString(0));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return artists;
        }
        public IList<string> GetArtistsOwndedById(string id, string table, string joinTable, string joinField)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<string> artists = new List<string>();
            objCommand.CommandText = string.Format(@"SELECT {0}.Title
                                                      FROM {1}
                                                      INNER JOIN {0} ON ({0}.Id = {1}.{2})
                                                      WHERE {1}.Artist_Id='{3}';", table, joinTable, joinField, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            if (objResults.HasRows)
                while (objResults.Read())
                    artists.Add(objResults.GetString(0));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return artists;
        }

        private void LinkArtist(IMyCollectionsData entity, string jobId, string artistId)
        {

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            //FIX 2.9.0.0
            try
            {

                string entityId = entity.Id;
                switch (entity.ObjectType)
                {
                    case EntityType.Books:
                        objCommand.CommandText = string.Format(@"INSERT OR IGNORE INTO Book_Artist_Job (Book_Id,Artist_Id,Job_Id)
                                                    VALUES(@param1,@param2,@param3);");

                        break;
                    case EntityType.Movie:
                        objCommand.CommandText =
                            string.Format(@"INSERT OR IGNORE INTO Movie_Artist_Job (Movie_Id,Artist_Id,Job_Id,Added)
                                                    VALUES(@param1,@param2,@param3,@param4);");

                        break;
                    case EntityType.Music:
                        objCommand.CommandText =
                            string.Format(@"INSERT OR IGNORE INTO Music_Artist_Job (Music_Id,Artist_Id,Job_Id)
                                                    VALUES(@param1,@param2,@param3);");

                        break;
                    case EntityType.Series:
                        objCommand.CommandText =
                            string.Format(@"INSERT OR IGNORE INTO Series_Artist_Job (Series_Id,Artist_Id,Job_Id,Added)
                                                    VALUES(@param1,@param2,@param3,@param4);");
                        entityId = entity.SerieId;


                        break;
                    case EntityType.XXX:
                        objCommand.CommandText =
                            string.Format(@"INSERT OR IGNORE INTO XXX_Artist_Job (XXX_Id,Artist_Id,Job_Id,Added)
                                                    VALUES(@param1,@param2,@param3,@param4);");

                        break;
                }

                SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
                param1.Value = entityId;

                SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
                param2.Value = artistId;

                SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
                param3.Value = jobId;

                SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.DateTime);
                param4.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

                objCommand.Parameters.Add(param1);
                objCommand.Parameters.Add(param2);
                objCommand.Parameters.Add(param3);
                objCommand.Parameters.Add(param4);

                objCommand.ExecuteNonQuery();
                objCommand.Parameters.Clear();

            }
            catch (SQLiteException)
            {
            }

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        private Artist ResultsToArtist(SQLiteDataReader reader)
        {
            Artist item = new Artist();

            item.Id = reader.GetString(0);
            item.FirstName = reader.GetString(1);
            item.LastName = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
                item.BirthDay = reader.GetDateTime(3);

            if (reader.IsDBNull(4) == false)
                item.Picture = (byte[])reader.GetValue(4);

            if (reader.IsDBNull(5) == false)
                item.Bio = reader.GetString(5);

            if (reader.IsDBNull(6) == false)
                item.PlaceBirth = reader.GetString(6);

            if (reader.IsDBNull(7) == false)
                item.WebSite = reader.GetString(7);

            if (reader.IsDBNull(8) == false)
                item.YearsActive = reader.GetString(8);

            if (reader.IsDBNull(9) == false)
                item.Ethnicity = reader.GetString(9);

            if (reader.IsDBNull(10) == false)
                item.Breast = reader.GetString(10);

            if (reader.IsDBNull(11) == false)
                item.FulleName = reader.GetString(11);

            if (reader.IsDBNull(12) == false)
                item.Aka = reader.GetString(12);

            if (reader.IsDBNull(13) == false)
                item.Sex = reader.GetBoolean(13);

            if (reader.FieldCount > 14)
            {
                item.Job = new Job();
                item.Job.Id = reader.GetString(14);
                item.Job.Name = reader.GetString(15);
            }

            return item;
        }

        public void UnlinkArtist(Artist artist, IMyCollectionsData entity)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            string tableName = string.Empty;
            string entityName = string.Empty;
            string entityId = entity.Id;
            switch (entity.ObjectType)
            {
                case EntityType.Books:
                    tableName = "Book_Artist_Job";
                    entityName = "Book_Id";
                    break;
                case EntityType.Movie:
                    tableName = "Movie_Artist_Job";
                    entityName = "Movie_Id";
                    break;
                case EntityType.Music:
                    tableName = "Music_Artist_Job";
                    entityName = "Music_Id";
                    break;
                case EntityType.Series:
                    tableName = "Series_Artist_Job";
                    entityName = "Series_Id";
                    entityId = entity.SerieId;
                    break;
                case EntityType.XXX:
                    tableName = "XXX_Artist_Job";
                    entityName = "XXX_Id";
                    break;
            }

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM {0}  
                                                      WHERE {1}='{2}' 
                                                      AND Artist_Id='{3}' 
                                                      AND Job_Id='{4}'", tableName, entityName, entityId, artist.Id, artist.Job.Id);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        private void UnlinkArtist(string artistId, EntityType entityType)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            string tableName = string.Empty;
            switch (entityType)
            {
                case EntityType.Books:
                    tableName = "Book_Artist_Job";
                    break;
                case EntityType.Movie:
                    tableName = "Movie_Artist_Job";
                    break;
                case EntityType.Music:
                    tableName = "Music_Artist_Job";
                    break;
                case EntityType.Series:
                    tableName = "Series_Artist_Job";
                    break;
                case EntityType.XXX:
                    tableName = "XXX_Artist_Job";
                    break;
            }

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM {0}  
                                                      WHERE Artist_Id='{1}' 
                                                      ", tableName, artistId);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public void UpdateFullName()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Id,FirstName,LastName
                                                          FROM Artist
                                                           WHERE Artist.FulleName IsNull);");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            if (objResults.HasRows)
            {
                while (objResults.Read())
                {
                    objCommand.CommandText = string.Format(@"UPDATE Artist 
                                                             SET FulleName=@param2
                                                              WHERE id=@param1);");


                    SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
                    param1.Value = objResults.GetString(0);

                    SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
                    param2.Value = objResults.GetString(1) + " " + objResults.GetString(2);

                    objCommand.ExecuteScalar();
                    objCommand.Parameters.Clear();
                }
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        #endregion
        #region ArtistCredit

        private void AddArtistCredits(Artist artist)
        {
            if (artist.ArtistCredits != null)
            {
                DeleteArtistCredit(artist.Id);

                //FIX 2.8.3.0
                SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
                SQLiteCommand objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();

                foreach (ArtistCredits credit in artist.ArtistCredits)
                {
                    if (credit.IsOld == false)
                    {
                        objCommand.CommandText =
                            string.Format(
                                @"INSERT INTO Artist_Credits (Id,Artist_Id,EntityType,Title,Notes,ReleaseDate,BuyLink)
                                                          VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7);");
                        string id = Guid.NewGuid().ToString();

                        SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
                        param1.Value = id;

                        SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
                        param2.Value = artist.Id;

                        SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
                        param3.Value = credit.EntityType.ToString();

                        SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
                        param4.Value = credit.Title;

                        SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
                        param5.Value = credit.Notes;

                        SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.DateTime);
                        param6.Value = credit.ReleaseDate;

                        SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
                        param7.Value = credit.BuyLink;

                        objCommand.Parameters.Add(param1);
                        objCommand.Parameters.Add(param2);
                        objCommand.Parameters.Add(param3);
                        objCommand.Parameters.Add(param4);
                        objCommand.Parameters.Add(param5);
                        objCommand.Parameters.Add(param6);
                        objCommand.Parameters.Add(param7);

                        objCommand.ExecuteNonQuery();
                        objCommand.Parameters.Clear();
                    }

                }
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
        }

        public void DeleteArtistCredit(string artistId)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE 
                                                    FROM Artist_Credits 
                                                    WHERE Artist_Id ='{0}';", artistId);

            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public IList<ArtistCredits> GetArtistCredit(string fullName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<ArtistCredits> items = new List<ArtistCredits>();

            objCommand.CommandText = string.Format(@"SELECT Artist_Credits.Id,
                                                             Artist_Credits.Artist_Id,
                                                             Artist_Credits.EntityType,
                                                             Artist_Credits.Notes,
                                                             Artist_Credits.Title,
                                                             Artist_Credits.ReleaseDate,
                                                             Artist_Credits.BuyLink
                                                     FROM Artist_Credits
                                                     INNER JOIN Artist ON (Artist_Credits.Artist_Id = Artist.Id)
                                                     WHERE Artist.FulleName LIKE ""{0}""
                                                     ORDER BY Artist_Credits.Title;", fullName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                items = new List<ArtistCredits>();
                while (objResults.Read())
                    items.Add(ResultsToArtistCredit(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public IList<ArtistCredits> GetArtistCreditbyId(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<ArtistCredits> items = new List<ArtistCredits>();

            objCommand.CommandText = string.Format(@"SELECT Artist_Credits.Id,
                                                             Artist_Credits.Artist_Id,
                                                             Artist_Credits.EntityType,
                                                             Artist_Credits.Notes,
                                                             Artist_Credits.Title,
                                                             Artist_Credits.ReleaseDate,
                                                             Artist_Credits.BuyLink
                                                     FROM Artist_Credits
                                                     WHERE Artist_Credits.Artist_Id = '{0}'
                                                     ORDER BY Artist_Credits.Title;", id);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                items = new List<ArtistCredits>();
                while (objResults.Read())
                    items.Add(ResultsToArtistCredit(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public ArtistCredits GetArtistCredit(string title, string fullName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Artist_Credits.Id,
                                                            Artist_Credits.Artist_Id,
                                                            Artist_Credits.EntityType,
                                                            Artist_Credits.Notes,
                                                            Artist_Credits.Title,
                                                            Artist_Credits.ReleaseDate,
                                                            Artist_Credits.BuyLink
                                                     FROM
                                                            Artist_Credits
                                                     INNER JOIN Artist ON (Artist_Credits.Artist_Id = Artist.Id)
                                                     WHERE
                                                            Artist.FulleName LIKE @param1 
                                                     AND 
                                                            Artist_Credits.Title LIKE @param2
                                                     LIMIT 1");

            //FIX 2.8.9.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = fullName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = title;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            ArtistCredits artistCredits = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                artistCredits = ResultsToArtistCredit(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return artistCredits;
        }

        private ArtistCredits ResultsToArtistCredit(SQLiteDataReader reader)
        {
            ArtistCredits item = new ArtistCredits();

            item.Id = reader.GetString(0);
            item.ArtistId = reader.GetString(1);
            item.EntityType = (EntityType)Enum.Parse(typeof(EntityType), reader.GetString(2));

            if (reader.IsDBNull(3) == false)
                item.Notes = reader.GetString(3);

            item.Title = reader.GetString(4);

            if (reader.IsDBNull(5) == false)
                item.ReleaseDate = reader.GetDateTime(5);

            if (reader.IsDBNull(6) == false)
                item.BuyLink = reader.GetString(6);

            return item;
        }

        #endregion
        #region AspectRatio
        public void AddAspectRatio(AspectRatio item)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            item.Id = Guid.NewGuid().ToString();
            objCommand.CommandText = @"INSERT INTO AspectRatio (Id,Name,Image)
                                                   VALUES(@param1,@param2,@param3);";

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = item.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = item.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.Binary);
            param3.Value = item.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public AspectRatio GetAspectRatio(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT AspectRatio.Id, AspectRatio.Name, AspectRatio.Image
                                                      FROM AspectRatio
                                                      WHERE AspectRatio.Name = '{0}';", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            AspectRatio item = new AspectRatio();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToAspectRatio(objResults);
            }
            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public AspectRatio GetAspectRatioById(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT AspectRatio.Id, AspectRatio.Name, AspectRatio.Image
                                                      FROM AspectRatio
                                                      WHERE AspectRatio.Id = '{0}';", id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            AspectRatio item = new AspectRatio();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToAspectRatio(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public IList<AspectRatio> GetAspectRatios()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT AspectRatio.Id, AspectRatio.Name, AspectRatio.Image
                                                      FROM AspectRatio;");

            List<AspectRatio> items = new List<AspectRatio>();

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToAspectRatio(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        private AspectRatio ResultsToAspectRatio(SQLiteDataReader reader)
        {
            AspectRatio item = new AspectRatio();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.Name = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.Image = (byte[])reader.GetValue(2);

            return item;
        }
        #endregion
        #region Audio
        public void AddAudio(Audio audio, string itemId)
        {
            if (string.IsNullOrWhiteSpace(audio.Language.Id))
                AddLanguage(audio.Language);

            if (string.IsNullOrWhiteSpace(audio.AudioType.Id))
                AddAudioType(audio.AudioType);


            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"INSERT INTO Movie_Audio (Id,Movie_Id,Language_Id,Audio_Id)
                                                      VALUES(@param1,@param2,@param3,@param4);");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = Guid.NewGuid().ToString();

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = itemId;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = audio.Language.Id;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            param4.Value = audio.AudioType.Id;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public void AddAudioType(AudioType objItem)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();


            if (string.IsNullOrWhiteSpace(objItem.Id))
            {
                AudioType tmp = GetAudioType(objItem.Name);
                if (string.IsNullOrWhiteSpace(tmp.Id) == false)
                {
                    objItem.Id = tmp.Id;
                    return;
                }

                objItem.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = @"INSERT INTO AudioType (Id,Name,Image)
                                                    VALUES(@param1,@param2,@param3)";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE AudioType 
                                                        SET Name=@param2,Image=@param3
                                                         WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = objItem.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = objItem.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.Binary);
            param3.Value = objItem.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public AudioType GetAudioType(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Id,Name,Image
                                                     FROM AudioType 
                                                     WHERE Name='{0}';", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            AudioType item = new AudioType();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    item = ResultsToAudioType(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        private IList<Audio> GetAudios(string entityId)
        {
            //FIX 2.7.12.0
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Audio> items = new List<Audio>();

            objCommand.CommandText = string.Format(@"SELECT Movie_Audio.Id,Language.DisplayName, Language.LongName, Language.ShortName, AudioType.Name, AudioType.Image,
                                                             Movie_Audio.Movie_Id,  Movie_Audio.Language_Id, Movie_Audio.Audio_Id
                                                      FROM Movie_Audio
                                                      INNER JOIN AudioType ON (Movie_Audio.Audio_Id = AudioType.Id)
                                                      INNER JOIN Language ON (Movie_Audio.Language_Id = Language.Id)
                                                      WHERE Movie_Audio.Movie_Id='{0}' ;", entityId);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                {
                    Audio audio = new Audio();
                    audio.Id = objResults.GetString(0);
                    audio.Language = new Language();
                    audio.Language.Id = objResults.GetString(7);
                    audio.Language.DisplayName = objResults.GetString(1);

                    if (objResults.IsDBNull(2) == false)
                        audio.Language.LongName = objResults.GetString(2);

                    audio.Language.ShortName = objResults.GetString(3);
                    audio.AudioType = new AudioType();
                    audio.AudioType.Id = objResults.GetString(8);
                    audio.AudioType.Name = objResults.GetString(4);

                    if (objResults.IsDBNull(5) == false)
                        audio.AudioType.Image = (byte[])objResults.GetValue(5);

                    items.Add(audio);
                }
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        private AudioType ResultsToAudioType(SQLiteDataReader reader)
        {
            AudioType item = new AudioType();

            item.Id = reader.GetString(0);
            item.Name = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.Image = (byte[])reader.GetValue(2);

            return item;
        }
        #endregion
        #region BookFormat
        public void AddBookFormat(FileFormat item)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = @"INSERT INTO BookFormat (Id,Name,Image)
                                                   VALUES(@param1,@param2,@param3)";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE BookFormat 
                                                        SET Name=@param2,Image=@param3
                                                         WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = item.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = item.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.Binary);
            param3.Value = item.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        public IEnumerable<FileFormat> GetBookFormatList()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT BookFormat.Id, BookFormat.Name, BookFormat.Image
                                                      FROM BookFormat");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<FileFormat> items = new List<FileFormat>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToFileFormat(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public FileFormat GetBookFormatByName(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT BookFormat.Id, BookFormat.Name, BookFormat.Image
                                                      FROM BookFormat
                                                      WHERE BookFormat.Name='{0}';", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            FileFormat item = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToFileFormat(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public FileFormat GetBookFormatById(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT BookFormat.Id, BookFormat.Name, BookFormat.Image
                                                      FROM BookFormat
                                                      WHERE BookFormat.Id='{0}';", id);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            FileFormat item = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToFileFormat(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        #endregion
        #region CleanTitle
        public void AddCleanTitle(CleanTitle objItem)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(objItem.Id))
                objItem.Id = Guid.NewGuid().ToString();

            objCommand.CommandText = @"INSERT OR REPLACE INTO CleanTitle (Id,Value,Category)
                                                   VALUES(@param1,@param2,@param3)";


            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = objItem.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = objItem.Value;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = objItem.Category;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public IList<CleanTitle> GetAllCleanTitle()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Id, Value, Category
                                                     FROM CleanTitle
                                                     ORDER BY Category,Value");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<CleanTitle> items = new List<CleanTitle>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToCleanTitle(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList<string> GetDirtyTags(EntityType type)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            string category = Enum.GetName(typeof(EntityType), type);
            objCommand.CommandText = string.Format(@"SELECT CleanTitle.Value
                                                     FROM CleanTitle 
                                                     WHERE CleanTitle.Category = '{0}' 
                                                     OR CleanTitle.Category = 'All' 
                                                     ORDER BY CleanTitle.Value", category);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<string> items = new List<string>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(objResults.GetString(0));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        private CleanTitle ResultsToCleanTitle(SQLiteDataReader reader)
        {
            CleanTitle item = new CleanTitle();

            item.Id = reader.GetString(0);
            item.Value = reader.GetString(1);
            item.Category = reader.GetString(2);
            return item;
        }
        #endregion
        #region FileFormat
        public void AddFileFormat(FileFormat item)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            item.Id = Guid.NewGuid().ToString();
            objCommand.CommandText = @"INSERT INTO FileFormat (Id,Name,Image)
                                                   VALUES(@param1,@param2,@param3);";

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = item.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = item.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.Binary);
            param3.Value = item.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public IEnumerable<FileFormat> GetFileFormatList()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT FileFormat.Id, FileFormat.Name, FileFormat.Image
                                                      FROM FileFormat");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<FileFormat> items = new List<FileFormat>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToFileFormat(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public FileFormat GetFileFormatByName(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT FileFormat.Id, FileFormat.Name, FileFormat.Image
                                                      FROM FileFormat
                                                      WHERE FileFormat.Name='{0}';", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            FileFormat item = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToFileFormat(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public FileFormat GetFileFormatById(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT FileFormat.Id, FileFormat.Name, FileFormat.Image
                                                      FROM FileFormat
                                                      WHERE FileFormat.Id='{0}';", id);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            FileFormat item = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToFileFormat(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        private FileFormat ResultsToFileFormat(SQLiteDataReader reader)
        {
            FileFormat item = new FileFormat();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.Name = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.Image = (byte[])reader.GetValue(2);

            return item;
        }
        #endregion
        #region Friends
        public void AddFriend(Friends item)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = @"INSERT INTO Friends (Id,Alias,FirstName,LastName,Sex,BirthDate,Adresse,PhoneNumber,eMail,nbrCurrentLoan,nbrMaxLoan,Picture,Comments)
                                                   VALUES(@param1,@param13, @param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,@param12)";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Friends 
                                                        SET Alias=@param13,FirstName=@param2,LastName=@param3,Sex=@param4,BirthDate=@param5,Adresse=@param6,
                                                            PhoneNumber=@param7,eMail=@param8,nbrCurrentLoan=@param9,nbrMaxLoan=@param10,Picture=@param11,Comments=@param12
                                                         WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = item.Id;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.String);
            param13.Value = item.Alias;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = item.FirstName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = item.LastName;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.Boolean);
            param4.Value = item.Sex;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.DateTime);
            param5.Value = item.BirthDate;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = item.Adresse;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
            param7.Value = item.PhoneNumber;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
            param8.Value = item.EMail;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.Int32);
            param9.Value = item.NbrCurrentLoan;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.Int32);
            param10.Value = item.NbrMaxLoan;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.Binary);
            param11.Value = item.Picture;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.String);
            param12.Value = item.Comments;


            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public IList<Friends> GetFriends()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Friends.Id, Friends.Alias, Friends.FirstName, Friends.LastName, Friends.Sex, Friends.Adresse, Friends.BirthDate,
                                                              Friends.PhoneNumber,Friends.eMail, Friends.nbrCurrentLoan, Friends.nbrMaxLoan, Friends.Picture, Friends.Comments
                                                     FROM Friends
                                                     ORDER BY Friends.Alias;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<Friends> items = new List<Friends>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToFriend(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Friends GetFriendByName(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Friends.Id, Friends.Alias, Friends.FirstName, Friends.LastName, Friends.Sex, Friends.Adresse, Friends.BirthDate,
                                                              Friends.PhoneNumber,Friends.eMail, Friends.nbrCurrentLoan, Friends.nbrMaxLoan, Friends.Picture, Friends.Comments
                                                     FROM Friends
                                                      WHERE Friends.Alias = '{0}';", name);



            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            Friends item = new Friends();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToFriend(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public Friends GetFriendById(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Friends.Id, Friends.Alias, Friends.FirstName, Friends.LastName, Friends.Sex, Friends.Adresse, Friends.BirthDate,
                                                              Friends.PhoneNumber,Friends.eMail, Friends.nbrCurrentLoan, Friends.nbrMaxLoan, Friends.Picture, Friends.Comments
                                                     FROM Friends
                                                      WHERE Friends.Id = '{0}';", id);



            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            Friends item = new Friends();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToFriend(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }

        private Friends ResultsToFriend(SQLiteDataReader reader)
        {
            Friends item = new Friends();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.Alias = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.FirstName = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
                item.LastName = reader.GetString(3);

            if (reader.IsDBNull(4) == false)
                item.Sex = reader.GetBoolean(4);

            if (reader.IsDBNull(5) == false)
                item.Adresse = reader.GetString(5);

            if (reader.IsDBNull(6) == false)
                item.BirthDate = reader.GetDateTime(6);

            if (reader.IsDBNull(7) == false)
                item.PhoneNumber = reader.GetString(7);

            if (reader.IsDBNull(8) == false)
                item.EMail = reader.GetString(8);

            if (reader.IsDBNull(9) == false)
                item.NbrCurrentLoan = reader.GetInt32(9);

            if (reader.IsDBNull(10) == false)
                item.NbrMaxLoan = reader.GetInt32(10);

            if (reader.IsDBNull(11) == false)
                item.Picture = (byte[])reader.GetValue(11);

            if (reader.IsDBNull(12) == false)
                item.Comments = reader.GetString(12);

            return item;
        }
        #endregion
        #region Genre
        public void AddGenre(Genre objItem, EntityType entityType)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            string tableName = string.Empty;
            switch (entityType)
            {
                case EntityType.Apps:
                    tableName = "AppType";
                    break;
                case EntityType.Books:
                    tableName = "BookType";
                    break;
                case EntityType.Games:
                case EntityType.Nds:
                    tableName = "GamezType";
                    break;
                case EntityType.Movie:
                case EntityType.Series:
                    tableName = "Movie_Genre";
                    break;
                case EntityType.Music:
                    tableName = "Music_Genre";
                    break;
                case EntityType.XXX:
                    tableName = "XXX_Genre";
                    break;
            }

            if (string.IsNullOrWhiteSpace(objItem.Id))
            {
                objItem.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO {0} (Id,RealName,DisplayName,Image)
                                                   VALUES(@param1,@param2,@param3,@param4)", tableName);
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE {0} 
                                                        SET RealName=@param2,DisplayName=@param3,Image=@param4
                                                         WHERE Id=@param1", tableName);

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = objItem.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = objItem.RealName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = objItem.DisplayName;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.Binary);
            param4.Value = objItem.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public Genre GetGenre(string genreName, string genreTable)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            genreName = genreName.Replace("'", "''");

            objCommand.CommandText = string.Format(@"SELECT {0}.Id, {0}.RealName,  {0}.DisplayName, {0}.Image
                                                      FROM {0}
                                                      WHERE {0}.RealName = '{1}';", genreTable, genreName);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            Genre item = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToGenre(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;

        }
        public IList GetGenres(Genre objGenre, string genreTable)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT {0}.Id, {0}.RealName,  {0}.DisplayName, {0}.Image
                                                      FROM {0}
                                                      WHERE {0}.RealName = '{1}';", genreTable, objGenre);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<Genre> items = new List<Genre>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToGenre(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ObservableCollection<Genre> GetGenres(string genreTable)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ObservableCollection<Genre> items = new ObservableCollection<Genre>();

            objCommand.CommandText = string.Format(@"SELECT {0}.Id, {0}.RealName,  {0}.DisplayName, {0}.Image
                                                      FROM {0} ;", genreTable);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            //FIX since 2.7.12.0
            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToGenre(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();


            return items;
        }
        private IList<Genre> GetGenres(string table, string childTable, string field, string entityId, string genreField)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Genre> items = new List<Genre>();

            objCommand.CommandText = string.Format(@"SELECT {0}.Id, {0}.RealName,  {0}.DisplayName, {0}.Image
                                                    FROM {1}
                                                    INNER JOIN {0} ON ({1}.{4} = {0}.Id)
                                                    WHERE {1}.{2}='{3}' ;", table, childTable, field, entityId, genreField);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(ResultsToGenre(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetGenresDisplayName(string genreTable)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT DISTINCT {0}.DisplayName
                                                      FROM {0}
                                                      ORDER BY {0}.DisplayName;", genreTable);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<string> items = new List<string>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(objResults.GetString(0));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetTypeList(IEnumerable<string> ids, string genreTable, string linkTable, string genreField, string entityField)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            StringBuilder list = new StringBuilder();
            int i = 0;
            foreach (string id in ids)
            {
                list.Append("'");
                list.Append(id);
                list.Append("'");

                if (i < ids.Count() - 1)
                    list.Append(",");
                i++;
            }
            //SAMPLE
            //SELECT Movie_Genre.DisplayName
            //FROM  Movie_MovieGenre
            //INNER JOIN Movie_Genre ON Movie_MovieGenre.MovieGenre = Movie_Genre.Id
            //WHERE Movie_MovieGenre.Movie_Id IN ('4d40bbcb-01a9-4a5d-bf37-e4e67e009348','786f50ce-fdfb-42e1-99b9-58b4a9b04f83')

            objCommand.CommandText = string.Format(@"SELECT {0}.DisplayName
                                                      FROM {1}
                                                      INNER JOIN {0} ON {1}.{2} = {0}.Id                                                        
                                                      WHERE {1}.{3} IN ({4});", genreTable, linkTable, genreField, entityField, list);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<string> items = new List<string>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(objResults.GetString(0));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public void LinkGenre(IMyCollectionsData entity, string genreId)
        {
            string tableName = string.Empty;
            string entityName = string.Empty;
            string genreName = string.Empty;
            string id = entity.Id;
            switch (entity.ObjectType)
            {
                case EntityType.Apps:
                    tableName = "Apps_AppType";
                    entityName = "Apps_Id";
                    genreName = "AppType_Id";
                    break;
                case EntityType.Books:
                    tableName = "Books_BookType";
                    entityName = "Books_Id";
                    genreName = "BookType_Id";
                    break;
                case EntityType.Games:
                    tableName = "Gamez_GamezType";
                    entityName = "Gamez_Id";
                    genreName = "GamezType_Id";
                    break;
                case EntityType.Movie:
                    tableName = "Movie_MovieGenre";
                    entityName = "Movie_Id";
                    genreName = "MovieGenre";
                    break;
                case EntityType.Music:
                    tableName = "Music_MusicGenre";
                    entityName = "Music_Id";
                    genreName = "MusicGenre_Id";
                    break;
                case EntityType.Nds:
                    tableName = "Nds_GamezType";
                    entityName = "Nds_Id";
                    genreName = "GamezType_Id";
                    break;
                case EntityType.Series:
                    tableName = "Series_MovieGenre";
                    entityName = "Series_Id";
                    genreName = "Genre_Id";
                    id = entity.SerieId;
                    break;
                case EntityType.XXX:
                    tableName = "XXX_XXXGenre";
                    entityName = "XXX_Id";
                    genreName = "Genre_Id";
                    break;
            }

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"INSERT INTO {0} (Id,{1},{2})
                                                    VALUES(@param1,@param2,@param3);", tableName, entityName, genreName);

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = Guid.NewGuid().ToString();

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = id;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = genreId;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        private Genre ResultsToGenre(SQLiteDataReader reader)
        {
            Genre item = new Genre();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.RealName = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.DisplayName = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
                item.Image = (byte[])reader.GetValue(3);

            return item;
        }

        public void UnlinkGenre(IMyCollectionsData entity)
        {
            string tableName = string.Empty;
            string entityName = string.Empty;
            string id = entity.Id;
            switch (entity.ObjectType)
            {
                case EntityType.Apps:
                    tableName = "Apps_AppType";
                    entityName = "Apps_Id";
                    break;
                case EntityType.Books:
                    tableName = "Books_BookType";
                    entityName = "Books_Id";
                    break;
                case EntityType.Games:
                    tableName = "Gamez_GamezType";
                    entityName = "Gamez_Id";
                    break;
                case EntityType.Movie:
                    tableName = "Movie_MovieGenre";
                    entityName = "Movie_Id";
                    break;
                case EntityType.Music:
                    tableName = "Music_MusicGenre";
                    entityName = "Music_Id";
                    break;
                case EntityType.Nds:
                    tableName = "Nds_GamezType";
                    entityName = "Nds_Id";
                    break;
                case EntityType.Series:
                    tableName = "Series_MovieGenre";
                    entityName = "Series_Id";
                    id = entity.SerieId;
                    break;
                case EntityType.XXX:
                    tableName = "XXX_XXXGenre";
                    entityName = "XXX_Id";
                    break;
            }

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM {0}  
                                                      WHERE {1}='{2}'", tableName, entityName, id);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        #endregion
        #region Job
        public void AddJob(Job objItem)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(objItem.Id))
            {
                objItem.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO Job (Id,Name,Image)
                                                   VALUES(@param1,@param2,@param3)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Job 
                                                        SET Name=@param2,Image=@param3
                                                         WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = objItem.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = objItem.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.Binary);
            param3.Value = objItem.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public Job GetJob(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Job.Id, Job.Name, Job.Image
                                                      FROM Job
                                                      WHERE Job.Name='{0}';", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            Job item = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToJob(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        private Job ResultsToJob(SQLiteDataReader reader)
        {
            Job item = new Job();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.Name = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.Image = (byte[])reader.GetValue(4);

            return item;
        }

        #endregion
        #region Language
        public void AddLanguage(Language objItem)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(objItem.Id))
            {
                objItem.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = @"INSERT INTO Language (Id,ShortName,LongName,DisplayName,Image)
                                                   VALUES(@param1,@param2,@param3,@param4,@param5)";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Language 
                                                        SET ShortName=@param2,LongName=@param3,DisplayName=@param4,Image=@param5
                                                        WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = objItem.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = objItem.ShortName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = objItem.LongName;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            param4.Value = objItem.DisplayName;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.Binary);
            param5.Value = objItem.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public IList<Language> GetLanguages()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Language.Id, Language.ShortName, Language.LongName, Language.DisplayName, Language.Image
                                                      FROM Language
                                                      WHERE Language.DisplayName Is NOT NULL AND Language.DisplayName <> ''
                                                      ORDER BY Language.DisplayName");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<Language> items = new List<Language>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToLanguage(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList<Language> GetAllLanguages()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Language.Id, Language.ShortName, Language.LongName, Language.DisplayName, Language.Image
                                                      FROM Language");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<Language> items = new List<Language>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToLanguage(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Language GetLanguageById(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Language.Id, Language.ShortName, Language.LongName, Language.DisplayName, Language.Image
                                                      FROM Language
                                                      WHERE Language.Id = '{0}';", id);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            Language item = new Language();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToLanguage(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public Language GetLanguageByShortName(string shortName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            //FIX 2.7.12.0
            shortName = shortName.Replace(@"""", "");
            objCommand.CommandText = string.Format(@"SELECT Language.Id, Language.ShortName, Language.LongName, Language.DisplayName, Language.Image
                                                      FROM Language
                                                      WHERE Language.ShortName = '{0}';", shortName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            Language item = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToLanguage(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        private Language ResultsToLanguage(SQLiteDataReader reader)
        {
            Language item = new Language();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.ShortName = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.LongName = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
                item.DisplayName = reader.GetString(3);

            if (reader.IsDBNull(4) == false)
                item.Image = (byte[])reader.GetValue(4);

            return item;
        }

        #endregion
        #region Links
        public Links CreateLinks(string tableName, string foreignKeyName, string foreignKeyValue, string type, string url)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"INSERT INTO {0} (Id,{1},Type,Path)
                                                            VALUES(@param1,@param2,@param3,@param4);", tableName, foreignKeyName);
            string id = Guid.NewGuid().ToString();

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = foreignKeyValue;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = type;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            param4.Value = url;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return GetLink(id, tableName, foreignKeyName);

        }

        private IList<Links> GetLinks(string table, string field, string entityId)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Links> items = new List<Links>();

            objCommand.CommandText = string.Format(@"SELECT Id, {0},Type, Path 
                                                    FROM {1}
                                                    WHERE {0}='{2}' ;", field, table, entityId);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                {
                    Links links = new Links();
                    links.Id = objResults.GetString(0);
                    links.ItemId = entityId;
                    links.Type = objResults.GetString(2);
                    links.Path = objResults.GetString(3);

                    items.Add(links);
                }
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Links GetLink(string url, string entityId, string table, string field)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Links links = null;

            objCommand.CommandText = string.Format(@"SELECT Id, {0},Type, Path 
                                                    FROM {1}
                                                    WHERE {0}='{2}' AND Path Like ""{3}"";", field, table, entityId, url);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            if (objResults.HasRows)
            {
                links = new Links();

                objResults.Read();
                links.Id = objResults.GetString(0);
                links.ItemId = entityId;
                links.Type = objResults.GetString(2);
                links.Path = objResults.GetString(3);

            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return links;

        }
        private Links GetLink(string id, string table, string field)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Links links = null;

            objCommand.CommandText = string.Format(@"SELECT Id, {0},Type, Path 
                                                    FROM {1}
                                                    WHERE Id='{2}' ;", field, table, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                links = new Links();

                objResults.Read();
                links.Id = objResults.GetString(0);
                links.ItemId = objResults.GetString(1);
                links.Type = objResults.GetString(2);
                links.Path = objResults.GetString(3);

            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return links;

        }
        #endregion
        #region Loan
        public void AddLoan(Loan item)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
                //FIX 2.8.9.0
                objCommand.CommandText = @"INSERT INTO Loan (Id,ItemTypeId,ItemId,FriendId,StartDate,EndDate,IsBack,BackDate)
                                                   VALUES(@param1,@param2, @param3,@param4,@param5,@param6,@param7,@param8);";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Loan 
                                                        SET ItemTypeId=@param2,ItemId=@param3,FriendId=@param4,StartDate=@param5,EndDate=@param6,
                                                            IsBack=@param7,BackDate=@param8
                                                         WHERE Id=@param1;");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = item.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = item.ItemTypeId;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = item.ItemId;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            param4.Value = item.Friend.Id;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.DateTime);
            param5.Value = item.StartDate;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.DateTime);
            param6.Value = item.EndDate;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.Boolean);
            param7.Value = item.IsBack;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.DateTime);
            param8.Value = item.BackDate;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public IList<string> GetLoan(long lngType)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Loan.ItemId
                                                     FROM Loan
                                                     WHERE Loan.BackDate IS NULL AND Loan.ItemTypeId={0}
                                                     ORDER BY Loan.BackDate;", lngType);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<string> items = new List<string>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(objResults.GetString(0));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public IList<string> GetLateLoan(long lngType)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Loan.ItemId
                                                     FROM Loan
                                                     WHERE Loan.BackDate IS NULL AND Loan.ItemTypeId={0}
                                                     AND Loan.BackDate < @param1
                                                     ORDER BY Loan.BackDate;", lngType);

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = DateTime.Now;

            objCommand.Parameters.Add(param1);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            objCommand.Parameters.Clear();

            List<string> items = new List<string>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(objResults.GetString(0));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public Loan GetLoanByItemId(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Loan.Id, Loan.ItemTypeId, Loan.ItemId, Loan.FriendId, Loan.StartDate, Loan.EndDate, Loan.IsBack,Loan.BackDate
                                                     FROM Loan
                                                     WHERE Loan.ItemId = '{0}';", id);



            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            //Fix since 2.7.12.0
            Loan item = new Loan();
            item.StartDate = DateTime.Now;
            item.EndDate = DateTime.Now.AddMonths(1);

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToLoan(objResults);
                string friendId = objResults.GetString(3);

                objResults.Close();
                objResults.Dispose();

                item.Friend = GetFriendByName(friendId);
            }

            if (objResults != null)
            {
                objResults.Close();
                objResults.Dispose();
            }


            return item;
        }
        private Loan ResultsToLoan(SQLiteDataReader reader)
        {
            Loan item = new Loan();

            item.Id = reader.GetString(0);
            item.ItemTypeId = reader.GetInt32(1);
            item.ItemId = reader.GetString(2);

            //Fix since 2.7.12.0
            if (reader.IsDBNull(4) == false)
                item.StartDate = reader.GetDateTime(4);
            else
                item.StartDate = DateTime.Now;

            if (reader.IsDBNull(5) == false)
                item.EndDate = reader.GetDateTime(5);
            else
                item.EndDate = DateTime.Now.AddMonths(1);

            if (reader.IsDBNull(6) == false)
                item.IsBack = reader.GetBoolean(6);

            if (reader.IsDBNull(7) == false)
                item.BackDate = reader.GetDateTime(7);

            return item;
        }

        #endregion
        #region Media

        public void AddMedia(Media item)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = @"INSERT INTO Media( Id, MediaType_Id, Name, Path, Image, FreeSpace, TotalSpace, LastUpdate, EntityType, LastPattern, SearchSub, LocalImage, UseNfo, CleanTitle)
                                                        VALUES(@param1, @param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,@param12,@param13,@param14);";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Media 
                                                        SET MediaType_Id=@param2,Name=@param3,Path=@param4,Image=@param5,FreeSpace=@param6, TotalSpace=@param7,LastUpdate=@param8, EntityType=@param9,
                                                         LastPattern=@param10,SearchSub=@param11,LocalImage=@param12,UseNfo=@param13,CleanTitle=@param14 
                                                         WHERE Id=@param1;");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = item.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            if (item.MediaType != null)
                param2.Value = item.MediaType.Id;
            else
                param2.Value = DBNull.Value;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = item.Name;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            param4.Value = item.Path;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.Binary);
            param5.Value = item.Image;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.Int32);
            param6.Value = item.FreeSpace;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.Int32);
            param7.Value = item.TotalSpace;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.DateTime);
            param8.Value = item.LastUpdate;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.String);
            param9.Value = item.EntityType;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.String);
            param10.Value = item.LastPattern;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.Boolean);
            param11.Value = item.SearchSub;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.Boolean);
            param12.Value = item.LocalImage;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.Boolean);
            param13.Value = item.UseNfo;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Boolean);
            param14.Value = item.CleanTitle;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public bool DeleteMedia(string mediaName, string noneMediaId)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Media.Id
                                                      FROM Media
                                                      WHERE Media.Name = '{0}';", mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                string item = objResults.GetString(0);

                objResults.Close();
                objResults.Dispose();

                #region Apps

                objCommand.CommandText = string.Format(@"UPDATE Apps
                                                       SET Media_Id=@param2
                                                      WHERE Media_Id =@param1;");

                SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
                param1.Value = item;

                SQLiteParameter param2 = new SQLiteParameter("@param2");
                param2.Value = noneMediaId;

                objCommand.Parameters.Add(param1);
                objCommand.Parameters.Add(param2);

                objCommand.ExecuteNonQuery();

                #endregion

                #region Books

                objCommand.CommandText = string.Format(@"UPDATE Books
                                                       SET Media_Id=@param2
                                                      WHERE Media_Id =@param1;");



                objCommand.ExecuteNonQuery();

                #endregion

                #region Gamez

                objCommand.CommandText = string.Format(@"UPDATE Gamez
                                                       SET Media_Id=@param2
                                                      WHERE Media_Id =@param1;");



                objCommand.ExecuteNonQuery();

                #endregion

                #region Movie

                objCommand.CommandText = string.Format(@"UPDATE Movie
                                                       SET Media_Id=@param2
                                                      WHERE Media_Id =@param1;");



                objCommand.ExecuteNonQuery();

                #endregion

                #region Music

                objCommand.CommandText = string.Format(@"UPDATE Music
                                                       SET Media_Id=@param2
                                                      WHERE Media_Id =@param1;");



                objCommand.ExecuteNonQuery();

                #endregion

                #region Nds

                objCommand.CommandText = string.Format(@"UPDATE Nds
                                                       SET Media_Id=@param2
                                                      WHERE Media_Id =@param1;");



                objCommand.ExecuteNonQuery();

                #endregion

                #region Series_Season

                objCommand.CommandText = string.Format(@"UPDATE Series_Season
                                                       SET Media_Id=@param2
                                                      WHERE Media_Id =@param1;");



                objCommand.ExecuteNonQuery();

                #endregion

                #region XXX

                objCommand.CommandText = string.Format(@"UPDATE XXX
                                                       SET Media_Id=@param2
                                                      WHERE Media_Id =@param1;");



                objCommand.ExecuteNonQuery();
                objCommand.Parameters.Clear();

                #endregion

                objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM Media  
                                                      WHERE Id='{0}' 
                                                      ", item);
                objCommand.ExecuteNonQuery();

                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

                return true;
            }
            else
            {
                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

                return false;
            }


        }

        public MediaType GetMediaType(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT MediaType.Id, MediaType.Name, MediaType.Image
                                                      FROM MediaType
                                                      WHERE MediaType.Name = '{0}';", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            MediaType item = new MediaType();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToMediaType(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public IList<string> GetMediaTypeList()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT MediaType.Name
                                                     FROM MediaType
                                                     ORDER BY MediaType.Name;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<string> items = new List<string>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(objResults.GetString(0));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList<string> GetMediaList()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Media.Name
                                                     FROM Media
                                                     ORDER BY Media.Name;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<string> items = new List<string>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    if (objResults.IsDBNull(0) == false)
                        items.Add(objResults.GetString(0));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Media GetMediaByName(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Media.Id, Media.MediaType_Id, Media.Name, Media.Path, Media.Image, Media.FreeSpace, Media.TotalSpace,
                                                             Media.LastUpdate, Media.EntityType, Media.LastPattern, Media.SearchSub, Media.LocalImage,
                                                             Media.UseNfo, Media.CleanTitle, MediaType.Name, MediaType.Id, MediaType.Image
                                                      FROM Media
                                                      LEFT JOIN MediaType ON (Media.MediaType_Id = MediaType.Id)
                                                      WHERE Media.Name = '{0}';", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            Media item = null;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToMedia(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }

        private MediaType ResultsToMediaType(SQLiteDataReader reader)
        {
            MediaType item = new MediaType();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.Name = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.Image = (byte[])reader.GetValue(2);

            return item;
        }
        private Media ResultsToMedia(SQLiteDataReader reader)
        {
            Media item = new Media();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(2) == false)
                item.Name = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
                item.Path = reader.GetString(3);

            if (reader.IsDBNull(4) == false)
                item.Image = (byte[])reader.GetValue(4);

            if (reader.IsDBNull(5) == false)
                item.FreeSpace = reader.GetInt32(5);

            if (reader.IsDBNull(6) == false)
                item.TotalSpace = reader.GetInt32(6);

            if (reader.IsDBNull(7) == false)
                item.LastUpdate = reader.GetDateTime(7);

            if (reader.IsDBNull(8) == false)
                item.EntityType = (EntityType)Enum.Parse(typeof(EntityType), reader.GetString(8));

            if (reader.IsDBNull(9) == false)
                item.LastPattern = reader.GetString(9);

            if (reader.IsDBNull(10) == false)
                item.SearchSub = reader.GetBoolean(10);

            if (reader.IsDBNull(11) == false)
                item.LocalImage = reader.GetBoolean(11);

            if (reader.IsDBNull(12) == false)
                item.UseNfo = reader.GetBoolean(12);

            if (reader.IsDBNull(13) == false)
                item.CleanTitle = reader.GetBoolean(13);

            if (reader.IsDBNull(14) == false)
            {
                item.MediaType = new MediaType();
                item.MediaType.Name = reader.GetString(14);
                item.MediaType.Id = reader.GetString(15);

                if (reader.IsDBNull(16) == false)
                    item.MediaType.Image = (byte[])reader.GetValue(16);

            }

            return item;
        }

        #endregion
        #region MetaData
        public void AddMetaData(MetaData objItem)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(objItem.Id))
                objItem.Id = Guid.NewGuid().ToString();

            objCommand.CommandText = @"INSERT OR REPLACE INTO MetaData (Id,Name,Category)
                                                   VALUES(@param1,@param2,@param3)";

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = objItem.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = objItem.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = objItem.Category;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        public IList<MetaData> GetAllMetaDatas()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<MetaData> items = new List<MetaData>();

            objCommand.CommandText = string.Format(@"SELECT MetaData.Name,NULL, NULL, MetaData.Id, Metadata.Category
                                                    FROM MetaData
                                                    ORDER BY Metadata.Category ;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMetaData(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        private IList<MetaData> GetMetaDatas(string entityId)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<MetaData> items = new List<MetaData>();

            objCommand.CommandText = string.Format(@"SELECT MetaData.Name,Item_Metadata.Id, Item_Metadata.Item_Id, Item_Metadata.MetaData_Id, Item_Metadata.Item_Category
                                                    FROM Item_Metadata
                                                    INNER JOIN MetaData ON (MetaData.Id = Item_Metadata.MetaData_Id)    
                                                    WHERE Item_Metadata.Item_Id='{0}' ;", entityId);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMetaData(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public string GetMetaDataId(string name, EntityType entityType)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Id
                                                      FROM MetaData
                                                      WHERE (Category='{0}' OR Category='All') AND Name='{1}';", entityType, name);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            string item = string.Empty;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = objResults.GetString(0);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public IList GetMetaDataName(EntityType entityType)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT DISTINCT Name
                                                      FROM MetaData
                                                      WHERE Category='{0}' OR Category='All' 
                                                      ORDER BY Name;", entityType);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<string> items = new List<string>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(objResults.GetString(0));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public void LinkMetaData(IMyCollectionsData entity, string metaDataId)
        {
            string id = entity.Id;
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"INSERT INTO Item_Metadata (Id,Item_Id,MetaData_Id,Item_Category)
                                                    VALUES(@param1,@param2,@param3,@param4);");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = Guid.NewGuid().ToString();

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = id;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = metaDataId;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            param4.Value = entity.ObjectType;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        private MetaData ResultsToMetaData(SQLiteDataReader reader)
        {
            MetaData item = new MetaData();

            item.Name = reader.GetString(0);

            if (reader.IsDBNull(3) == false)
                item.Id = reader.GetString(3);

            if (reader.IsDBNull(2) == false)
                item.ItemId = reader.GetString(2);

            if (reader.IsDBNull(4) == false)
                item.Category = reader.GetString(4);

            return item;
        }

        public void UnlinkMetaData(IMyCollectionsData entity)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM Item_Metadata  
                                                      WHERE Item_Id='{0}'", entity.Id);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        #endregion
        #region Platform
        public void AddPlatform(Platform item)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = @"INSERT INTO Platform (Id,Name,DisplayName)
                                                   VALUES(@param1,@param2,@param3)";
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Platform 
                                                        SET Name=@param2,DisplayName=@param3
                                                         WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = item.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = item.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = item.DisplayName;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        public List<Platform> GetDisctinctPlatforms()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Platform.Id, Platform.Name, Platform.DisplayName
                                                     FROM Platform
                                                      GROUP BY Platform.DisplayName
                                                      ORDER BY Platform.DisplayName;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<Platform> items = new List<Platform>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToPlatform(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public List<Platform> GetPlatforms()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Platform.Id, Platform.Name, Platform.DisplayName
                                                      FROM Platform
                                                      ORDER BY Platform.DisplayName;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            List<Platform> items = new List<Platform>();

            if (objResults.HasRows == true)
            {
                while (objResults.Read())
                    items.Add(ResultsToPlatform(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Platform GetPlatformByName(string name)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Platform.Id, Platform.Name, Platform.DisplayName
                                                      FROM Platform
                                                      WHERE Platform.Name = '{0}';", name);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            Platform item = new Platform();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToPlatform(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public Platform GetPlatformById(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Platform.Id, Platform.Name, Platform.DisplayName
                                                      FROM Platform
                                                      WHERE Platform.Id = '{0}';", id);

            SQLiteDataReader objResults = objCommand.ExecuteReader();

            Platform item = new Platform();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToPlatform(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        private Platform ResultsToPlatform(SQLiteDataReader reader)
        {
            Platform item = new Platform();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.Name = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.DisplayName = reader.GetString(2);

            return item;
        }

        #endregion
        #region Publisher
        public void AddPublisher(string tableName, Publisher publisher)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"INSERT INTO {0} (Id,Name,Image)
                                                      VALUES(@param1,@param2,@param3);", tableName);

            if (string.IsNullOrWhiteSpace(publisher.Id))
                publisher.Id = Guid.NewGuid().ToString();

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = publisher.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = publisher.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.Binary);
            param3.Value = publisher.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public Publisher GetPublisher(string value, string table, string field)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Publisher item = null;

            objCommand.CommandText = string.Format(@"SELECT Id, Name,Image
                                                    FROM {0}
                                                    WHERE {1}=""{2}"" ;", table, field, value);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            if (objResults.HasRows)
            {
                item = new Publisher();

                objResults.Read();
                item.Id = objResults.GetString(0);
                item.Name = objResults.GetString(1);

                if (objResults.IsDBNull(2) == false)
                    item.Image = (byte[])objResults.GetValue(2);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;

        }
        public ObservableCollection<Publisher> GetPublishers(string table)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ObservableCollection<Publisher> items = new ObservableCollection<Publisher>();

            objCommand.CommandText = string.Format(@"SELECT Id, Name,Image
                                                    FROM {0}
                                                    GROUP BY Name
                                                    ORDER BY Name;", table);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                {
                    Publisher item = new Publisher();
                    item.Id = objResults.GetString(0);
                    item.Name = objResults.GetString(1);

                    if (objResults.IsDBNull(2) == false)
                        item.Image = (byte[])objResults.GetValue(2);

                    items.Add(item);
                }
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        #endregion
        #region Ressources

        public void AddRessource(string tableName, string foreignKeyName, string foreignKeyValue, string ressourceTypeId, byte[] image, string link, bool isdefault)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"INSERT INTO {0} (Id,{1},RessourceType_ID,Ressource,Link,IsDefault)
                                                            VALUES(@param1,@param2,@param3,@param4,@param5,@param6);", tableName, foreignKeyName);
            string id = Guid.NewGuid().ToString();

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = foreignKeyValue;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = ressourceTypeId;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.Binary);
            param4.Value = image;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
            param5.Value = link;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.Boolean);
            param6.Value = isdefault;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public byte[] GetBigCover(string table, string field, string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            byte[] item = null;
            objCommand.CommandText = string.Format(@"SELECT {0}.Ressource
                                                      FROM {0}
                                                      WHERE {0}.{1} = '{2}'
                                                      AND  {0}.IsDefault= 'true';", table, field, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                item = (byte[])objResults.GetValue(0);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public Ressource GetRessource(string entityId, string table, string field, string ressourceType)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Ressource item = null;

            objCommand.CommandText = string.Format(@"SELECT {1}.Id, {0},RessourceType_ID, Ressource,Link,IsDefault, ResourcesType.Type,ResourcesType.Image
                                                    FROM {1}
                                                    INNER JOIN ResourcesType ON {1}.RessourceType_ID=ResourcesType.Id
                                                    WHERE {0}='{2}' AND  ResourcesType.Type Like ""{3}"";", field, table, entityId, ressourceType);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            if (objResults.HasRows)
            {
                objResults.Read();
                item = ResultsToRessource(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;

        }
        private IList<Ressource> GetRessources(string table, string field, string entityId)
        {
            List<Ressource> items = new List<Ressource>();

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT {1}.Id, {0},RessourceType_ID, Ressource,Link,IsDefault, ResourcesType.Type,ResourcesType.Image
                                                    FROM {1}
                                                    INNER JOIN ResourcesType ON {1}.RessourceType_ID=ResourcesType.Id
                                                    WHERE {0}='{2}';", field, table, entityId);


            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(ResultsToRessource(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public byte[] GetDefaultRessourceValue(string id, string table, string field)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Byte[] item = null;
            //FIX since 2.8.4.0
            objCommand.CommandText = string.Format(@"SELECT Ressource
                                                    FROM {1}
                                                    INNER JOIN ResourcesType ON {1}.RessourceType_ID=ResourcesType.Id
                                                    WHERE {0}='{2}' AND  IsDefault = 1;", field, table, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            if (objResults.HasRows)
            {
                objResults.Read();
                item = (byte[])objResults.GetValue(0);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;
        }
        public ResourcesType GetRessourceType(string strRessourceType)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            //FIX since 2.7.12.0
            objCommand.CommandText = string.Format(@"SELECT ResourcesType.Id, ResourcesType.Type, ResourcesType.Image
                                                      FROM ResourcesType
                                                      WHERE ResourcesType.Type = '{0}';", strRessourceType);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            ResourcesType item = new ResourcesType();

            if (objResults.HasRows == true)
            {
                objResults.Read();
                item = ResultsToResourcesType(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return item;

        }

        private ResourcesType ResultsToResourcesType(SQLiteDataReader reader)
        {
            ResourcesType item = new ResourcesType();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
                item.Name = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.Image = (byte[])reader.GetValue(2);

            return item;
        }
        private Ressource ResultsToRessource(SQLiteDataReader reader)
        {
            Ressource item = new Ressource();

            item.Id = reader.GetString(0);
            item.ItemId = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
            {
                item.ResourcesType = new ResourcesType();
                item.ResourcesType.Id = reader.GetString(2);
                item.ResourcesType.Name = reader.GetString(6);

                if (reader.IsDBNull(7) == false)
                    item.ResourcesType.Image = (byte[])reader.GetValue(7);
            }

            if (reader.IsDBNull(3) == false)
                item.Value = (byte[])reader.GetValue(3);

            if (reader.IsDBNull(4) == false)
                item.Link = reader.GetString(4);

            item.IsDefault = reader.GetBoolean(5);

            return item;
        }

        #endregion
        #region SubTitle
        public void AddSubTitle(Language subtitle, string itemId)
        {
            if (string.IsNullOrWhiteSpace(subtitle.Id))
                AddLanguage(subtitle);

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"INSERT INTO Movie_SubTitle (Id,Movie_Id,Language_Id)
                                                      VALUES(@param1,@param2,@param3);");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = Guid.NewGuid().ToString();

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = itemId;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = subtitle.Id;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        private IList<Language> GetSubTitle(string entityId)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Language> items = new List<Language>();

            objCommand.CommandText = string.Format(@"SELECT Language.Id, Language.ShortName, Language.LongName, Language.DisplayName, Language.Image
                                                    FROM Movie_SubTitle
                                                    INNER JOIN Language ON (Movie_SubTitle.Language_Id = Language.Id)    
                                                    WHERE Movie_SubTitle.Movie_Id='{0}' ;", entityId);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                items = new List<Language>();

                while (objResults.Read())
                    items.Add(ResultsToLanguage(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        #endregion
        #region Tracks
        public void AddTracks(Music music)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (music.Tracks != null)
            {
                DeleteTracks(music.Id);
                foreach (Track track in music.Tracks)
                {
                    if (track.IsOld == false)
                    {
                        objCommand.CommandText =
                            string.Format(
                                @"INSERT INTO Music_Tracks (Id,Music_Id,Title,Path)
                                                          VALUES(@param1,@param2,@param3,@param4);");
                        string id = Guid.NewGuid().ToString();

                        SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
                        param1.Value = id;

                        SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
                        param2.Value = music.Id;

                        SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
                        param3.Value = track.Title;

                        SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
                        param4.Value = track.Path;

                        objCommand.Parameters.Add(param1);
                        objCommand.Parameters.Add(param2);
                        objCommand.Parameters.Add(param3);
                        objCommand.Parameters.Add(param4);

                        objCommand.ExecuteNonQuery();
                        objCommand.Parameters.Clear();

                    }

                }
            }

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public void DeleteTracks(string musicId)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE 
                                                    FROM Music_Tracks 
                                                    WHERE Music_Id ='{0}';", musicId);

            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        private IList<Track> GetTracks(string entityId)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Track> items = new List<Track>();

            objCommand.CommandText = string.Format(@"SELECT Music_Tracks.Id,Music_Tracks.Music_Id, Music_Tracks.Title, Music_Tracks.Path
                                                      FROM Music_Tracks
                                                      WHERE Music_Tracks.Music_Id='{0}';", entityId);


            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                {
                    Track track = new Track();
                    track.Id = objResults.GetString(0);
                    track.Title = objResults.GetString(2);
                    if (objResults.IsDBNull(3) == false)
                        track.Path = objResults.GetString(3);
                    items.Add(track);
                }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        #endregion

        #endregion
        #region Utils
        public void AddConfiguration(string key, string value)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(GetConfiguration(key)))
                objCommand.CommandText = @"INSERT INTO Configuration (Key, Value) 
                                                   VALUES(@param1,@param2)";
            else
                objCommand.CommandText = @"UPDATE Configuration SET Value=@param2 WHERE Key=@param1";

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = key;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = value;


            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public void DeleteById(string tableName, string field, string value)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM {0}  
                                                      WHERE {0}.{1}=""{2}""", tableName, field, value);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        private void DeleteOrphelans(string childTable, string childId, string parentTable, string parentFild)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM {0} 
                                                      WHERE Id='{1}' 
                                                      AND NOT EXISTS (SELECT 1 
                                                                      FROM {2}
                                                                      WHERE {3} = '{1}')", childTable, childId, parentTable, parentFild);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        private void DeleteOrphelanArtist(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE 
                                                      FROM Artist 
                                                      WHERE Id='{0}' 
                                                      AND NOT EXISTS (SELECT 1 
                                                                      FROM Book_Artist_Job
                                                                      WHERE Artist_Id = '{0}')
                                                      AND NOT EXISTS (SELECT 1 
                                                                      FROM Movie_Artist_Job
                                                                      WHERE Artist_Id = '{0}')
                                                      AND NOT EXISTS (SELECT 1 
                                                                      FROM Music_Artist_Job
                                                                      WHERE Artist_Id = '{0}')
                                                      AND NOT EXISTS (SELECT 1 
                                                                      FROM Series_Artist_Job
                                                                      WHERE Artist_Id = '{0}')
                                                      AND NOT EXISTS (SELECT 1 
                                                                      FROM XXX_Artist_Job
                                                                      WHERE Artist_Id = '{0}')", id);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        public string GetConfiguration(string key)
        {
            SQLiteConnection objConnection = null;
            SQLiteCommand objCommand = null;
            SQLiteDataReader objResults = null;
            try
            {
                objConnection = new SQLiteConnection(_connectionString);
                objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();

                objCommand.CommandText = string.Format(@"Select Value from Configuration where Key='{0}'", key);

                objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
                string results = null;

                if (objResults.HasRows == true)
                {
                    objResults.Read();
                    results = objResults[0].ToString();
                }

                return results;
            }
            catch (SQLiteException)
            {
                return null;
            }
            finally
            {
                if (objResults != null)
                {
                    objResults.Close();
                    objResults.Dispose();
                }

                if (objCommand != null)
                    objCommand.Dispose();

                if (objConnection != null)
                {
                    objConnection.Close();
                    objConnection.Dispose();
                }

            }
        }
        public int GetItemType(string typeName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int results = 0;
            if (typeName == "Movie") typeName = "Movies";
            if (typeName == "Games") typeName = "Gamez";

            objCommand.CommandText = string.Format(@"SELECT Type_id 
                                                      FROM Type  
                                                      WHERE Name='{0}'", typeName);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            if (objResults.HasRows)
            {
                objResults.Read();
                results = objResults.GetInt32(0);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return results;
        }

        public int GetLastCollectionNumber(EntityType entityType)
        {
            int results = 0;
            string tableName;
            switch (entityType)
            {
                case EntityType.Apps:
                    tableName = "Apps";
                    break;
                case EntityType.Books:
                    tableName = "Books";
                    break;
                case EntityType.Games:
                    tableName = "Gamez";
                    break;
                case EntityType.Movie:
                    tableName = "Movie";
                    break;
                case EntityType.Music:
                    tableName = "Music";
                    break;
                case EntityType.Nds:
                    tableName = "Nds";
                    break;
                case EntityType.Series:
                    tableName = "Series_Season";
                    break;
                case EntityType.XXX:
                    tableName = "XXX";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("entityType");
            }

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT MAX(NumID) 
                                                      FROM {0} ", tableName);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);

            if (objResults.HasRows)
            {
                objResults.Read();
                if (objResults.IsDBNull(0) == false)
                    results = objResults.GetInt32(0);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return results + 1;
        }
        public void PurgeChildTable(string child, string childField, string parentTable)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE FROM {0} WHERE NOT EXISTS 
                                                        (SELECT 1 FROM {1} WHERE {1}.Id={0}.{2})", child, parentTable, childField);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public void PurgeTable(string parentTable)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"DELETE FROM {0}", parentTable);
            objCommand.ExecuteScalar();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public void UpdateMain(string tableName, string id, string key, string value)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();


            objCommand.CommandText = string.Format(@"UPDATE {0} 
                                                      SET {1}=@param1 
                                                      WHERE {1}=""{2}""", tableName, key, id);

            SQLiteParameter param1;
            if (string.IsNullOrEmpty(value))
                param1 = new SQLiteParameter("@param1", DBNull.Value);
            else
            {
                param1 = new SQLiteParameter("@param1", DbType.String);
                param1.Value = value;
            }

            objCommand.Parameters.Add(param1);
            objCommand.ExecuteScalar();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public void CreateConfigurationTable()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = "CREATE TABLE Configuration (Key VARCHAR(50) NOT NULL, Value VARCHAR(50) NOT NULL)";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public static string ConnectionString
        {
            get { return _connectionString; }
        }

        public string GetDbVersion()
        {
            SQLiteConnection objConnection = null;
            SQLiteCommand objCommand = null;
            SQLiteDataReader objResults = null;
            try
            {
                objConnection = new SQLiteConnection(_connectionString);
                objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();

                objCommand.CommandText = "Select Value from Configuration where Key='Version'";

                objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
                string strResults = string.Empty;

                if (objResults.HasRows == true)
                {
                    objResults.Read();
                    strResults = objResults[0].ToString();
                }

                if (strResults == string.Empty)
                    strResults = "0";

                return strResults;
            }
            catch (SQLiteException)
            {
                return null;
            }
            finally
            {
                if (objResults != null)
                {
                    objResults.Close();
                    objResults.Dispose();
                }

                if (objCommand != null)
                    objCommand.Dispose();

                if (objConnection != null)
                {
                    objConnection.Close();
                    objConnection.Dispose();
                }

            }
        }
        public Boolean GetDonate()
        {
            SQLiteDataReader objResults = null;
            SQLiteCommand objCommand = null;
            SQLiteConnection objConnection = null;

            try
            {
                objConnection = new SQLiteConnection(_connectionString);
                objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();


                objCommand.CommandText = "Select Value from Configuration where Key='Donate'";

                objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
                string strResults = string.Empty;

                if (objResults.HasRows == true)
                {
                    objResults.Read();
                    strResults = objResults[0].ToString();
                }

                if (string.IsNullOrWhiteSpace(strResults))
                {
                    InsertDonate();
                    return false;
                }
                else if (strResults == "False")
                    return false;
                else
                    return true;
            }
            catch (SQLiteException)
            {
                return false;
            }
            finally
            {
                if (objResults != null)
                {
                    objResults.Close();
                    objResults.Dispose();
                }

                if (objCommand != null)
                    objCommand.Dispose();

                if (objConnection != null)
                {
                    objConnection.Close();
                    objConnection.Dispose();
                }
            }
        }
        public int GetLaunch()
        {

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();
            int result;

            objCommand.CommandText = "Select Value from Configuration where Key='Launch'";

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            string strResults = string.Empty;

            if (objResults.HasRows == true)
            {
                objResults.Read();
                strResults = objResults[0].ToString();

            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (string.IsNullOrWhiteSpace(strResults))
            {
                InsertLaunchCount();
                result = 1;
            }
            else
                result = Convert.ToInt32(strResults);

            return result;

        }
        public string GetIp()
        {
            try
            {
                SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
                SQLiteCommand objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();

                objCommand.CommandText = "Select Value from Configuration where Key='IP'";

                SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
                string strResults = string.Empty;

                if (objResults.HasRows == true)
                {
                    objResults.Read();
                    if (objResults.IsDBNull(0) == false)
                        strResults = objResults.GetString(0);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

                Match match = Regex.Match(strResults,
                                          @"^(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})(\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9]{1,2})){3}$");

                if (string.IsNullOrWhiteSpace(strResults) || strResults.StartsWith("78.225.") || match.Success == false)
                {
                    string ip = Util.GetExternalIp2();
                    if (string.IsNullOrWhiteSpace(ip) == false)
                    {
                        InsertIp(ip);
                        return ip;
                    }
                    else
                        return string.Empty;
                }
                else
                    return strResults;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static Dal GetInstance
        {
            get { return DalInstance; }
        }

        private void InsertDonate()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = "INSERT INTO Configuration (Key,Value)VALUES('Donate','False')";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        private void InsertIp(string ip)
        {

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = "Delete FROM Configuration Where Key='IP'";
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "INSERT INTO Configuration (Key,Value)VALUES('IP','" + ip + "')";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        private void InsertLaunchCount()
        {
            SQLiteConnection objConnection = null;
            SQLiteCommand objCommand = null;
            try
            {
                objConnection = new SQLiteConnection(_connectionString);
                objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();

                objCommand.CommandText = "INSERT INTO Configuration (Key,Value)VALUES('Launch','1')";
                objCommand.ExecuteNonQuery();
            }
            catch (SQLiteException)
            {

            }
            finally
            {
                if (objCommand != null)
                    objCommand.Dispose();

                if (objConnection != null)
                {
                    objConnection.Close();
                    objConnection.Dispose();
                }
            }
        }
        #region Upgrade
        public void UpgradeDb0To17()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.CreateCleanTitleTable;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='17' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        public void UpgradeDb0To18()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.CreateAspectRatioTable;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddAspectRatioMovieColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='18' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public void UpgradeDb0To19()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.UpdateMediaSettings;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddBluRayFileFormat;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.CreateArtistCreditTable;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddBioArtist;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddPlaceBirthArtist;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddWebSiteArtist;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddYearsActiveArtist;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddEthnicityArtist;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddBreastArtist;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddFullNameArtist;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='19' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public void UpgradeDb0To20()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.AddReleaseDateArtistCredit;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddBuyLinkArtistCredit;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='20' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public void UpgradeDb0To21()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            try
            {
                objCommand.CommandText = UpgradeScripts.AddAkaArtist;
                objCommand.ExecuteNonQuery();
            }
            catch (SQLiteException) { }

            objCommand.CommandText = "UPDATE Configuration SET Value='22' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();


        }
        public void UpgradeDb0To22()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.AddAkaArtist;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='22' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public void UpgradeDb0To23()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.AddToTestAppsColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddToReadBooksColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddToTestGamesColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddToWatchMoviesColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddToHearMusicColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddToTestNdsColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddToWatchSeriesSeasonColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddToWatchXXXColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddSexArtistColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddGoofsMoviesColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='23' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public void UpgradeDb0To24()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.AddPlateformDisplayNameColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='24' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }

        public void UpgradeDb0To25()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.AddPublicRatingAppsColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddPublicRatingBooksColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddPublicRatingGamesColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddPublicRatingMoviesColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddPublicRatingMusicColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddPublicRatingNdsColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddPublicRatingSeriesSeasonColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddPublicRatingXXXColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddNumIdAppsColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddNumIdBooksColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddNumIdGamesColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddNumIdMoviesColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddNumIdMusicColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddNumIdNdsColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddNumIdSeriesSeasonColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddNumIdXXXColumn;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddMetaDataTable;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddItemMetaDataTable;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='25' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

        }
        public void UpgradeDb0To26()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = UpgradeScripts.Add1080I;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.Add3D;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.Add720p;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddLossless;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddClean1080i;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddClean1080p;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddClean2013;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddClean720p;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddCleanAC3;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddCleanBdRip;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddCleanDvdRip;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddCleanRepack;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = UpgradeScripts.AddCleanXvid;
            objCommand.ExecuteNonQuery();

            objCommand.CommandText = "UPDATE Configuration SET Value='26' WHERE Key='Version';";
            objCommand.ExecuteNonQuery();

        }


        #endregion
        public void UpdateDonate()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = "UPDATE Configuration SET Value='True' WHERE Key='Donate';";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

        }
        public void UpdateCount(int count)
        {
            try
            {
                SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
                SQLiteCommand objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();

                objCommand.CommandText = "UPDATE Configuration SET Value='" + count + "' WHERE Key='Launch';";
                objCommand.ExecuteNonQuery();

                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            catch (Exception ex)
            {
                Util.LogException(ex, _connectionString);
            }
        }
        public void VaccumDb()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = "VACUUM";
            objCommand.ExecuteNonQuery();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public static Hashtable ParseNfo(string strFilePath)
        {
            string nfo = strFilePath;
            Task.Factory.StartNew(() => Util.NotifyEvent("ParseNfo : " + nfo));

            Hashtable objResults = new Hashtable();
            string[] objLines = File.ReadAllLines(strFilePath);
            #region Editor

            AddNfoValue(objLines, "PUBLiSHER", "Editor", objResults);
            AddNfoValue(objLines, "PUBLISHER :", "Editor", objResults);
            AddNfoValue(objLines, "PROD STUDIO:", "Editor", objResults);
            AddNfoValue(objLines, "Label:", "Editor", objResults);
            AddNfoValue(objLines, "LABEL", "Editor", objResults);
            AddNfoValue(objLines, "Company ..:", "Editor", objResults);

            #endregion
            #region Language

            AddNfoValue(objLines, "LANGUAGES", "Language", objResults);
            AddNfoValue(objLines, "language", "Language", objResults);
            AddNfoValue(objLines, "Language", "Language", objResults);
            AddNfoValue(objLines, "Langue", "Language", objResults);
            AddNfoValue(objLines, "Region", "Language", objResults);

            #endregion
            #region Type

            AddNfoValue(objLines, "Genere", "Type", objResults);
            AddNfoValue(objLines, "Game Genre   :", "Type", objResults);
            AddNfoValue(objLines, "Game Type :", "Type", objResults);
            AddNfoValue(objLines, "GAME.TYPE", "Type", objResults);
            AddNfoValue(objLines, "type.......:", "Type", objResults);
            AddNfoValue(objLines, "TYPE.........:", "Type", objResults);
            AddNfoValue(objLines, "Program Type ....:", "Type", objResults);
            AddNfoValue(objLines, "Program Typ", "Type", objResults);
            AddNfoValue(objLines, "GENRE", "Type", objResults);
            AddNfoValue(objLines, "Genre", "Type", objResults);

            #endregion
            #region Released Date

            AddNfoValue(objLines, "And released on:", "Released", objResults);
            AddNfoValue(objLines, "Release-Date", "Released", objResults);
            AddNfoValue(objLines, "Release Date ....:", "Released", objResults);
            AddNfoValue(objLines, "release", "Released", objResults);
            AddNfoValue(objLines, "Release date.:", "Released", objResults);
            AddNfoValue(objLines, "Release Date", "Released", objResults);
            AddNfoValue(objLines, "Release date", "Released", objResults);
            AddNfoValue(objLines, "RELEASE DATE", "Released", objResults);
            AddNfoValue(objLines, "RELEASEDATE:", "Released", objResults);
            AddNfoValue(objLines, "Rip.Date", "Released", objResults);
            AddNfoValue(objLines, "Rip-Date", "Released", objResults);
            AddNfoValue(objLines, "date.......:", "Released", objResults);
            AddNfoValue(objLines, "Date.....:", "Released", objResults);
            AddNfoValue(objLines, "Date :", "Released", objResults);
            AddNfoValue(objLines, "DATE", "Released", objResults);

            #endregion
            #region URL

            AddNfoValue(objLines, @"http://", "Links", objResults);
            AddNfoValue(objLines, "URL..........:", "Links", objResults);
            AddNfoValue(objLines, "URL", "Links", objResults);

            #endregion
            AddNfoValue(objLines, "Store Date", "StreetDate", objResults);
            AddNfoValue(objLines, "YEAR", "StreetDate", objResults);
            AddNfoValue(objLines, "date", "StreetDate", objResults);

            AddNfoValue(objLines, "RUNTiME", "Runtime", objResults);
            AddNfoValue(objLines, "Duree........:", "Runtime", objResults);
            AddNfoValue(objLines, "Durée :", "Runtime", objResults);
            AddNfoValue(objLines, "Duree", "Runtime", objResults);

            AddNfoValue(objLines, "iMDB", "Imdb", objResults);

            AddNfoValue(objLines, "Artist", "Artist", objResults);
            AddNfoValue(objLines, "ARTIST", "Artist", objResults);
            AddNfoValue(objLines, "AUTHOR", "Artist", objResults);

            AddNfoValue(objLines, "TITLE", "Album", objResults);
            AddNfoValue(objLines, "Title", "Album", objResults);

            AddNfoValue(objLines, "FORMAT :", "FileFormat", objResults);

            return objResults;
        }
        private static void AddNfoValue(IEnumerable<string> objLines, string strValue, string strKey,
                                        Hashtable objResults)
        {
            var results = from items in objLines
                          where items.Contains(strValue)
                          select items;

            if (results.Any())
                if (objResults.ContainsKey(strKey) == false)
                    switch (strKey)
                    {
                        case "Links":
                            objResults.Add(strKey, Util.PurgNfoForUrl(strValue, results.ToList()[0]));
                            break;
                        case "Imdb":
                            objResults.Add(strKey,
                                           Util.PurgNfo(strValue, results.ToList()[0], @"[^a-zA-Z0-9 .]"));
                            break;
                        case "Type":
                            objResults.Add(strKey,
                                           Util.PurgNfo(strValue, results.ToList()[0], @"[^a-zA-Z0-9 ,]"));
                            break;
                        case "Released":
                        case "StreetDate":
                            objResults.Add(strKey,
                                           Util.PurgNfo(strValue, results.ToList()[0], @"[^a-zA-Z0-9 /]"));
                            break;
                        default:
                            objResults.Add(strKey, Util.PurgNfo(strValue, results.ToList()[0], @"[^a-zA-Z0-9 ]"));
                            break;
                    }
        }

        #endregion
        #region Apps

        public void AddApps(Apps entity)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO Apps(
                                                                      Id, Title, BarCode, Language_Id, Editor_Id, Media_Id, Version, Description, IsDeleted,
                                                                      IsWhish, IsComplete, AddedDate, ReleaseDate, FilePath, FileName, Rating, IsTested, Comments,
                                                                      ToBeDeleted,smallCover,ToTest, PublicRating, NumID)
                                                     VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,
                                                            @param11,@param12,@param13,@param14,@param15,@param16,@param17,@param18,@param19,@param20,@param21,@param22
                                                            ,@param23)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Apps 
                                                        SET Title=@param2,BarCode=@param3,Language_Id=@param4,Editor_Id=@param5,Media_Id=@param6,Version=@param7,
                                                            Description=@param8, IsDeleted=@param9, IsWhish=@param10, IsComplete=@param11, AddedDate=@param12, 
                                                            ReleaseDate=@param13, FilePath=@param14, FileName=@param15, Rating=@param16, IsTested=@param17, 
                                                            Comments=@param18, ToBeDeleted=@param19, smallCover=@param20, ToTest=@param21, PublicRating=@param22, NumID=@param23
                                                         WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = entity.Title;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = entity.BarCode;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            if (entity.Language != null)
                param4.Value = entity.Language.Id;
            else
                param4.Value = DBNull.Value;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
            if (entity.Publisher != null)
                param5.Value = entity.Publisher.Id;
            else
                param5.Value = DBNull.Value;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = entity.Media.Id;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
            param7.Value = entity.Version;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
            param8.Value = entity.Description;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.Boolean);
            param9.Value = entity.IsDeleted;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.Boolean);
            param10.Value = entity.IsWhish;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.Boolean);
            param11.Value = entity.IsComplete;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.DateTime);
            param12.Value = entity.AddedDate;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.DateTime);
            param13.Value = entity.ReleaseDate;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.String);
            param14.Value = entity.FilePath;

            SQLiteParameter param15 = new SQLiteParameter("@param15", DbType.String);
            param15.Value = entity.FileName;

            SQLiteParameter param16 = new SQLiteParameter("@param16", DbType.Int32);
            param16.Value = entity.MyRating;

            SQLiteParameter param17 = new SQLiteParameter("@param17", DbType.Boolean);
            param17.Value = entity.Watched;

            SQLiteParameter param18 = new SQLiteParameter("@param18", DbType.String);
            param18.Value = entity.Comments;

            SQLiteParameter param19 = new SQLiteParameter("@param19", DbType.Boolean);
            param19.Value = entity.ToBeDeleted;

            SQLiteParameter param20 = new SQLiteParameter("@param20", DbType.Binary);
            param20.Value = entity.Cover;

            SQLiteParameter param21 = new SQLiteParameter("@param21", DbType.Boolean);
            param21.Value = entity.ToWatch;

            SQLiteParameter param22 = new SQLiteParameter("@param22", DbType.Double);
            param22.Value = entity.PublicRating;

            SQLiteParameter param23 = new SQLiteParameter("@param23", DbType.Int32);
            param23.Value = entity.NumId;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);
            objCommand.Parameters.Add(param15);
            objCommand.Parameters.Add(param16);
            objCommand.Parameters.Add(param17);
            objCommand.Parameters.Add(param18);
            objCommand.Parameters.Add(param19);
            objCommand.Parameters.Add(param20);
            objCommand.Parameters.Add(param21);
            objCommand.Parameters.Add(param22);
            objCommand.Parameters.Add(param23);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        private ThumbItem AppsToThumb(SQLiteDataReader reader)
        {

            string fullPath = string.Empty;
            if (reader.IsDBNull(14) == false && reader.IsDBNull(13) == false)
                if (string.IsNullOrWhiteSpace(reader.GetString(14)) == false &&
                    string.IsNullOrWhiteSpace(reader.GetString(13)) == false)
                    fullPath = Path.Combine(reader.GetString(14), reader.GetString(13));

            string description = string.Empty;
            if (reader.IsDBNull(6) == false)
                description = reader.GetString(6);

            byte[] cover = null;
            if (reader.IsDBNull(2) == false)
                cover = (byte[])reader.GetValue(2);

            bool tested = false;
            if (reader.IsDBNull(11) == false)
                tested = reader.GetBoolean(11);

            int? rating = null;
            if (reader.IsDBNull(5) == false)
                rating = reader.GetInt32(5);

            double? publicRating = null;
            if (reader.IsDBNull(16) == false)
                publicRating = reader.GetDouble(16);

            int? numId = null;
            if (reader.IsDBNull(17) == false)
                numId = reader.GetInt32(17);

            return new ThumbItem(reader.GetString(1), reader.GetString(0), cover, reader.GetDateTime(3),
                                    null, string.Empty, reader.GetBoolean(4),
                                    EntityType.Apps, rating, publicRating, description,
                                    reader.GetBoolean(7), reader.GetString(8), reader.GetBoolean(9),
                                    reader.GetBoolean(10), tested, reader.GetString(15),
                                    string.Empty, string.Empty, reader.GetBoolean(12), fullPath, numId);
        }

        public IList GetApps()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Apps> items = new List<Apps>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Apps
                                                     INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                     WHERE Apps.IsDeleted <> 'false';", ConstApps);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToApps(objResults, false));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Apps GetApps(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Apps item = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Apps
                                                      INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                      WHERE Apps.IsDeleted<>'false' 
                                                      AND Apps.Id='{1}';", ConstApps, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                item = ResultsToApps(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (item != null)
                GetChild(item);

            return item;
        }
        public Apps GetApps(string mediaName, string filePath, string fileName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Apps item = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Apps
                                                      INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                      WHERE Apps.IsDeleted<>'false' 
                                                      AND Media.Name =  @param1 AND Apps.FileName = @param2 AND Apps.FilePath =  @param3;", ConstApps);

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = fileName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = filePath;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                item = ResultsToApps(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (item != null)
                GetChild(item);

            return item;
        }
        public IList GetAppsByMedia(string mediaName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Apps> items = new List<Apps>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Apps
                                                      INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                      WHERE Apps.IsDeleted<>'false' 
                                                      AND Media.Name = '{1}';", ConstApps, mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(ResultsToApps(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public int GetAppsCountByType(string strGenre)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int number = 0;
            if (strGenre == string.Empty)
                return number;
            else
            {
                objCommand.CommandText = string.Format(@"SELECT Count(Apps.Title)
                                                        FROM Apps_AppType
                                                        INNER JOIN Apps ON (Apps_AppType.Apps_Id = Apps.Id)
                                                        INNER JOIN AppType ON (Apps_AppType.AppType_Id = AppType.Id)
                                                        WHERE AppType.DisplayName = ""{0}"";", strGenre);

                SQLiteDataReader objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                {
                    objResults.Read();
                    number = objResults.GetInt32(0);
                }
                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            return number;
        }
        public void GetChild(Apps item)
        {
            item.Links = GetLinks("App_Links", "App_Id", item.Id);
            item.Ressources = GetRessources("Apps_Ressources", "Apps_Id", item.Id);

            if (item.Publisher != null)
                item.Publisher = GetPublisher(item.Publisher.Id, "App_Editor", "Id");
            item.Genres = GetGenres("AppType", "Apps_AppType", "Apps_Id", item.Id, "AppType_Id");

            if (item.Language != null)
                item.Language = GetLanguageById(item.Language.Id);
        }
        public IList GetDupeApps()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM
                                                        (Select Apps.Title
                                                        From Apps
                                                        Group By Apps.Title
                                                        Having (Count(Apps.Title) > 1)) AS GRP
                                                      INNER JOIN Apps ON Apps.TITLE = GRP.TITLE 
                                                      INNER JOIN Media ON (Apps.Media_Id = Media.Id);", ConstAppsThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(AppsToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public Apps GetFirstApps()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Apps item = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Apps
                                                      INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                      WHERE Apps.IsDeleted<>'false';", ConstApps);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                item = ResultsToApps(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (item != null)
                GetChild(item);

            return item;
        }
        public IList GetNoSmallCoverApps()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Apps> items = new List<Apps>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Apps
                                                     INNER Apps Media ON (Apps.Media_Id = Media.Id)
                                                     WHERE Apps.Cover IS NULL;", ConstApps);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToApps(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItem GetThumbApp(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItem items = null;

            objCommand.CommandText = string.Format(@"SELECT {0} 
                                                    FROM Apps
                                                    INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                    WHERE Apps.IsDeleted<>'false' 
                                                    AND Apps.Id='{1}';", ConstAppsThumb, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = AppsToThumb(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetThumbApps()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0} 
                                                    FROM Apps
                                                    INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                    WHERE Apps.IsDeleted=0", ConstAppsThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(AppsToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItems GetBigThumbApps()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT Apps.Id, Title, BigCover, AddedDate, IsDeleted, Rating, Description, TobeDeleted, Media.Name, IsWhish, IsComplete, 
                                                            IsTested, ToTest, FileName, FilePath, 'None'
                                                      FROM 
                                                            (SELECT Apps_Id as AppsId, Ressource as BigCover
                                                             FROM Apps_Ressources
                                                             WHERE IsDefault=1
                                                             ), Apps
                                                       INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                        WHERE AppsId=Apps.Id;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(AppsToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public IList GetThumbAppsByTypes()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = @"SELECT Distinct Apps.Id, Title, smallCover, AddedDate, IsDeleted, Rating, Description, TobeDeleted, Media.Name, 
                                              IsWhish, IsComplete, IsTested, ToTest, FileName, FilePath, AppType.DisplayName 
                                       FROM AppType
                                       INNER JOIN Apps_AppType ON AppType.Id = Apps_AppType.AppType_Id
                                       INNER JOIN Apps ON Apps_AppType.Apps_Id = Apps.Id
                                       INNER JOIN Media ON Apps.Media_Id = Media.Id
                                       WHERE Apps.IsDeleted<>'false'
                                       AND AppType.DisplayName <>'';";

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(AppsToThumb(objResults));

            objResults.Close();
            objResults.Dispose();

            objCommand.CommandText = @"SELECT Apps.Id, Title, smallCover, AddedDate, IsDeleted, Rating, Description, TobeDeleted, Media.Name, 
                                              IsWhish, IsComplete, IsTested, ToTest, FileName, FilePath, 'None' 
                                       FROM Apps
                                       INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                       WHERE Apps.IsDeleted<>'false'
                                       AND NOT EXISTS (SELECT * 
	                                                   FROM [Apps_AppType]
	                                                   WHERE [Apps].[Id] = [Apps_AppType].[Apps_Id]) ;";


            objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(AppsToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }

        public int PurgeApps()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Apps> items = new List<Apps>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Apps
                                                     INNER JOIN Media ON (Apps.Media_Id = Media.Id)
                                                     WHERE Apps.IsDeleted='true';", ConstApps);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToApps(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            foreach (Apps item in items)
                PurgeApps(item);

            return items.Count;
        }
        public void PurgeApps(Apps item)
        {
            DeleteById("Apps_Ressources", "Apps_Id", item.Id);
            DeleteById("App_Links", "App_Id", item.Id);
            DeleteById("Apps", "Id", item.Id);
            if (item.Publisher != null)
                if (string.IsNullOrWhiteSpace(item.Publisher.Id) == false)
                    DeleteOrphelans("App_Editor", item.Publisher.Id, "Apps", "Editor_Id");
        }
        //TODO with not empty DB
        public void PurgeAppsType()
        {

            //var query = from items in _objEntities.Apps_AppType
            //            group items by new { items.Apps_Id, items.AppType_Id } into g
            //            where g.Count() > 1
            //            select g.Key;

            //foreach (var item in query)
            //{
            //    if (item != null)
            //    {
            //        string id = item.Apps_Id;
            //        string typeId = item.AppType_Id;

            //        Apps_AppType duplicates = (from items in _objEntities.Apps_AppType
            //                                   where items.Apps_Id == id && items.AppType_Id == typeId
            //                                   select items).First();


            //        _objEntities.DeleteObject(duplicates);
            //    }
            //}

            //_objEntities.SaveChanges();
        }

        private Apps ResultsToApps(SQLiteDataReader reader, bool getLastId = true)
        {
            Apps item = new Apps();

            item.Id = reader.GetString(0);
            item.Title = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.BarCode = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
            {
                item.Language = new Language();
                item.Language.Id = reader.GetString(3);
            }

            if (reader.IsDBNull(4) == false)
            {
                item.Publisher = new Publisher();
                item.Publisher.Id = reader.GetString(4);
            }

            item.Media = new Media();
            item.Media.Id = reader.GetString(5);
            item.Media.Name = reader.GetString(21);

            if (reader.IsDBNull(6) == false)
                item.Version = reader.GetString(6);

            if (reader.IsDBNull(7) == false)
                item.Description = reader.GetString(7);

            if (reader.IsDBNull(8) == false)
                item.IsDeleted = reader.GetBoolean(8);

            if (reader.IsDBNull(9) == false)
                item.IsWhish = reader.GetBoolean(9);

            if (reader.IsDBNull(10) == false)
                item.IsComplete = reader.GetBoolean(10);

            if (reader.IsDBNull(11) == false)
                item.AddedDate = reader.GetDateTime(11);

            if (reader.IsDBNull(12) == false)
                item.ReleaseDate = reader.GetDateTime(12);

            if (reader.IsDBNull(13) == false)
                item.FilePath = reader.GetString(13);

            if (reader.IsDBNull(14) == false)
                item.FileName = reader.GetString(14);

            if (reader.IsDBNull(15) == false)
                item.MyRating = reader.GetInt32(15);

            if (reader.IsDBNull(16) == false)
                item.Watched = reader.GetBoolean(16);

            if (reader.IsDBNull(17) == false)
                item.Comments = reader.GetString(17);

            if (reader.IsDBNull(18) == false)
                item.ToBeDeleted = reader.GetBoolean(18);

            if (reader.IsDBNull(19) == false)
                item.Cover = (byte[])reader.GetValue(19);

            if (reader.IsDBNull(20) == false)
                item.ToWatch = reader.GetBoolean(20);

            if (reader.IsDBNull(22) == false)
                item.PublicRating = reader.GetDouble(22);

            if (reader.IsDBNull(23) == false)
                item.NumId = reader.GetInt32(23);
            else if (getLastId)
                item.NumId = GetLastCollectionNumber(EntityType.Apps);

            return item;
        }

        #endregion
        #region Books
        public void AddBook(Books entity)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();


            #region FileFormat
            if (string.IsNullOrWhiteSpace(entity.FileFormat.Id))
            {
                entity.FileFormat.Id = Guid.NewGuid().ToString();

                objCommand.CommandText = string.Format(@"INSERT INTO BookFormat(Id, Name, Image)
                                                          VALUES(@param1,@param2,@param3)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE BookFormat 
                                                        SET Name=@param2,Image=@param3
                                                         WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.FileFormat.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = entity.FileFormat.Name;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.Binary);
            param3.Value = entity.FileFormat.Image;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();
            #endregion
            #region Book
            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO Books(Id, Title, BarCode, Editor_Id, Format_Id, Media_Id, Language_Id, ISBN, NbrPages, IsRead,
                                                                            IsDeleted, IsWhish, IsComplete, Description, FileName, FilePath, AddedDate, ReleaseDate,
                                                                            ToBeDeleted, Rating, Comments, Rated, smallCover,ToRead, PublicRating,NumID)
                                                          VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,
                                                                 @param11,@param12,@param13,@param14,@param15,@param16,@param17,@param18,@param19,@param20,@param21,@param22,
                                                                @param23,@param24,@param25,@param26)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Books 
                                                        SET Title=@param2,BarCode=@param3,Editor_Id=@param4,Format_Id=@param5,Media_Id=@param6,Language_Id=@param7,
                                                            ISBN=@param8, NbrPages=@param9, IsRead=@param10, IsDeleted=@param11, IsWhish=@param12, 
                                                            IsComplete=@param13, Description=@param14, FileName=@param15, FilePath=@param16, AddedDate=@param17, 
                                                            ReleaseDate=@param18, ToBeDeleted=@param19, Rating=@param20, Comments=@param21,Rated=@param22,
                                                            smallCover=@param23, ToRead=@param24,PublicRating=@param25,NumID=@param26
                                                         WHERE Id=@param1");

            param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.Id;

            param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = entity.Title;

            param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = entity.BarCode;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            if (entity.Publisher != null)
                param4.Value = entity.Publisher.Id;
            else
                param4.Value = DBNull.Value;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
            param5.Value = entity.FileFormat.Id;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = entity.Media.Id;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
            if (entity.Language != null)
                param7.Value = entity.Language.Id;
            else
                param7.Value = DBNull.Value;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
            param8.Value = entity.Isbn;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.Int32);
            param9.Value = entity.NbrPages;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.Boolean);
            param10.Value = entity.Watched;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.Boolean);
            param11.Value = entity.IsDeleted;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.Boolean);
            param12.Value = entity.IsWhish;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.Boolean);
            param13.Value = entity.IsComplete;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.String);
            param14.Value = entity.Description;

            SQLiteParameter param15 = new SQLiteParameter("@param15", DbType.String);
            param15.Value = entity.FileName;

            SQLiteParameter param16 = new SQLiteParameter("@param16", DbType.String);
            param16.Value = entity.FilePath;

            SQLiteParameter param17 = new SQLiteParameter("@param17", DbType.DateTime);
            param17.Value = entity.AddedDate;

            SQLiteParameter param18 = new SQLiteParameter("@param18", DbType.DateTime);
            param18.Value = entity.ReleaseDate;

            SQLiteParameter param19 = new SQLiteParameter("@param19", DbType.Boolean);
            param19.Value = entity.ToBeDeleted;

            SQLiteParameter param20 = new SQLiteParameter("@param20", DbType.Int32);
            param20.Value = entity.MyRating;

            SQLiteParameter param21 = new SQLiteParameter("@param21", DbType.String);
            param21.Value = entity.Comments;

            SQLiteParameter param22 = new SQLiteParameter("@param22", DbType.String);
            param22.Value = entity.Rated;

            SQLiteParameter param23 = new SQLiteParameter("@param23", DbType.Binary);
            param23.Value = entity.Cover;

            SQLiteParameter param24 = new SQLiteParameter("@param24", DbType.Boolean);
            param24.Value = entity.ToWatch;

            SQLiteParameter param25 = new SQLiteParameter("@param25", DbType.Double);
            param25.Value = entity.PublicRating;

            SQLiteParameter param26 = new SQLiteParameter("@param26", DbType.Int32);
            param26.Value = entity.NumId;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);
            objCommand.Parameters.Add(param15);
            objCommand.Parameters.Add(param16);
            objCommand.Parameters.Add(param17);
            objCommand.Parameters.Add(param18);
            objCommand.Parameters.Add(param19);
            objCommand.Parameters.Add(param20);
            objCommand.Parameters.Add(param21);
            objCommand.Parameters.Add(param22);
            objCommand.Parameters.Add(param23);
            objCommand.Parameters.Add(param24);
            objCommand.Parameters.Add(param25);
            objCommand.Parameters.Add(param26);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            #endregion
        }

        private ThumbItem BooksToThumb(SQLiteDataReader reader)
        {
            string fullPath = string.Empty;
            if (reader.IsDBNull(14) == false && reader.IsDBNull(13) == false)
                if (string.IsNullOrWhiteSpace(reader.GetString(14)) == false &&
                    string.IsNullOrWhiteSpace(reader.GetString(13)) == false)
                    fullPath = Path.Combine(reader.GetString(14), reader.GetString(13));

            string description = string.Empty;
            if (reader.IsDBNull(6) == false)
                description = reader.GetString(6);

            byte[] cover = null;
            if (reader.IsDBNull(2) == false)
                cover = (byte[])reader.GetValue(2);

            bool tested = false;
            if (reader.IsDBNull(11) == false)
                tested = reader.GetBoolean(11);

            int? rating = null;
            if (reader.IsDBNull(5) == false)
                rating = reader.GetInt32(5);

            double? publicRating = null;
            if (reader.IsDBNull(16) == false)
                publicRating = reader.GetDouble(16);

            int? numId = null;
            if (reader.IsDBNull(17) == false)
                numId = reader.GetInt32(17);

            string firstName = string.Empty;
            if (reader.IsDBNull(18) == false)
                firstName = reader.GetString(18);

            string lastName = string.Empty;
            if (reader.IsDBNull(19) == false)
                lastName = reader.GetString(19);

            string artist = firstName + " " + lastName;

            //FIX 2.8.9.0
            string type;
            if (reader.IsDBNull(21) == false)
                type = reader.GetString(21);
            else
                type = reader.GetString(15);

            return new ThumbItem(reader.GetString(1), reader.GetString(0), cover, reader.GetDateTime(3),
                                    null, string.Empty, reader.GetBoolean(4),
                                    EntityType.Books, rating, publicRating, description,
                                    reader.GetBoolean(7), reader.GetString(8), reader.GetBoolean(9),
                                    reader.GetBoolean(10), tested, type,
                                    string.Empty, artist, reader.GetBoolean(12), fullPath, numId);
        }

        public IList GetArtistThumbBooks()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT Book_Artist_Job.Artist_Id, Artist.FirstName, Artist.LastName, Artist.BirthDay, Artist.Picture,
                                                          Artist.Bio, Artist.PlaceBirth, Artist.WebSite, Artist.YearsActive, Artist.Ethnicity, Artist.Breast,
                                                          Artist.FulleName, Artist.Aka, Artist.Sex
                                                    FROM Book_Artist_Job
                                                    INNER JOIN Artist ON (Book_Artist_Job.Artist_Id = Artist.Id)");

            List<Artist> artist = new List<Artist>();
            SQLiteDataReader objResults = objCommand.ExecuteReader();

            if (objResults.HasRows == true)
                while (objResults.Read())
                    artist.Add(ResultsToArtist(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return artist;
        }
        public IList GetBooks()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Books> items = new List<Books>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Books
                                                     INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                     WHERE Books.IsDeleted <> 'false';", ConstBooks);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToBooks(objResults, false));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Books GetBooks(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Books items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Books
                                                      INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                      WHERE Books.IsDeleted<>'false' 
                                                      AND Books.Id='{1}';", ConstBooks, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToBooks(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();


            if (items != null)
                GetChild(items, false);

            return items;
        }
        public void GetChild(Books items, bool getArtistCredits)
        {
            items.Links = GetLinks("Book_Links", "Books_Id", items.Id);
            items.Ressources = GetRessources("Books_Ressources", "Books_Id", items.Id);

            if (items.Publisher != null)
                items.Publisher = GetPublisher(items.Publisher.Id, "App_Editor", "Id");

            items.Artists = GetArtists(items.Id, EntityType.Books, getArtistCredits);
            items.Genres = GetGenres("BookType", "Books_BookType", "Books_Id", items.Id, "BookType_Id");

            if (items.Language != null)
                items.Language = GetLanguageById(items.Language.Id);

            if (items.FileFormat != null)
                items.FileFormat = GetBookFormatById(items.FileFormat.Id);
        }
        public Books GetBooks(string mediaName, string filePath, string fileName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Books items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Books
                                                      INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                      WHERE Books.IsDeleted<>'false' 
                                                      AND Media.Name = @param1 AND Books.FileName =@param2 AND Books.FilePath = @param3;", ConstBooks);

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = fileName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = filePath;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToBooks(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public Books GetBooks(string mediaName, string title)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Books items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Books
                                                      INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                      WHERE Books.IsDeleted<>'false' 
                                                      AND Media.Name = '{1}';", ConstBooks, mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToBooks(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetBooksByMedia(string mediaName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Books> items = new List<Books>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM  Books
                                                      INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                      WHERE Books.IsDeleted<>'false' 
                                                      AND Media.Name = '{1}';", ConstBooks, mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToBooks(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public int GetBookCountByType(string strGenre)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int number = 0;
            if (strGenre == string.Empty)
                return number;
            else
            {
                objCommand.CommandText = string.Format(@"SELECT Count(Books.Title)
                                                        FROM Books_BookType
                                                        INNER JOIN Books ON (Books_BookType.Books_Id = Books.Id)
                                                        INNER JOIN BookType ON (Books_BookType.BookType_Id = BookType.Id)
                                                        WHERE BookType.DisplayName = ""{0}"";", strGenre);

                SQLiteDataReader objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                {
                    objResults.Read();
                    number = objResults.GetInt32(0);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            return number;
        }
        public IList GetDupeBooks()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM
                                                        (Select Books.Title
                                                        From Books
                                                        Group By Books.Title
                                                        Having (Count(Books.Title) > 1)) AS GRP
                                                      INNER JOIN Books ON Books.TITLE = GRP.TITLE  
                                                      INNER JOIN Media ON (Books.Media_Id = Media.Id);", ConstBooksThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(BooksToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public Books GetFirstBook()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Books items = null;
            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Books
                                                      INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                      WHERE Books.IsDeleted<>'false' 
                                                      LIMIT 1;", ConstBooks);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToBooks(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetNoSmallCoverBooks()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Books> items = new List<Books>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Books
                                                     INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                     WHERE Books.Cover IS NULL;", ConstBooks);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToBooks(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItem GetThumbBook(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItem items = null;

            objCommand.CommandText = string.Format(@"SELECT {0} {1}
                                                     FROM Books
                                                     LEFT OUTER JOIN Book_Artist_Job ON (Books.Id=Book_Artist_Job.Book_Id)
                                                     LEFT OUTER JOIN Artist ON (Book_Artist_Job.Artist_Id = Artist.Id)
                                                     LEFT OUTER JOIN Job ON (Book_Artist_Job.Job_Id = Job.Id)
                                                     INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                     WHERE Books.IsDeleted<>'false' 
                                                     AND Books.Id='{2}';", ConstBooksThumb, ConstBookThumbArtistPart, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = BooksToThumb(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetThumbBook()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0} {1}
                                                     FROM Books
                                                     LEFT OUTER JOIN Book_Artist_Job ON (Books.Id=Book_Artist_Job.Book_Id)
                                                     LEFT OUTER JOIN Artist ON (Book_Artist_Job.Artist_Id = Artist.Id)
                                                     LEFT OUTER JOIN Job ON (Book_Artist_Job.Job_Id = Job.Id)
                                                     INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                     WHERE Books.IsDeleted<>'false'", ConstBooksThumb, ConstBookThumbArtistPart);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(BooksToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItems GetBigThumbBooks()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT Books.Id, Title, BigCover, AddedDate, IsDeleted, Rating, Description, ToBeDeleted, Media.Name,
                                                            IsWhish, IsComplete, IsRead, ToRead, FileName, FilePath, 'None'
                                                      FROM 
                                                            (SELECT Books_Id as BooksId, Ressource as BigCover
                                                             FROM Books_Ressources
                                                             WHERE IsDefault=1
                                                             ), Books
                                                       INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                        WHERE BooksId=Books.Id;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(BooksToThumb(objResults));


            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public IList GetThumbBooksByTypes()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            //FIX 2.8.9.0
            objCommand.CommandText = string.Format(@"SELECT Distinct {0}{1}{2}
                                                     FROM Books
                                                     LEFT OUTER JOIN Book_Artist_Job ON (Books.Id=Book_Artist_Job.Book_Id)
                                                     LEFT OUTER JOIN Artist ON (Book_Artist_Job.Artist_Id = Artist.Id)
                                                     LEFT OUTER JOIN Job ON (Book_Artist_Job.Job_Id = Job.Id)
                                                     INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                     INNER JOIN BookType ON BookType.Id = Books_BookType.BookType_Id
                                                     INNER JOIN Books_BookType ON Books_BookType.Books_Id = Books.Id
                                                     WHERE Books.IsDeleted<>'false'
                                                     AND BookType.DisplayName <>'' ;", ConstBooksThumb, ConstBookThumbArtistPart, ConstBookThumbTypePart);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(BooksToThumb(objResults));

            objResults.Close();

            //FIX Since 2.7.12.0
            objCommand.CommandText = string.Format(@"SELECT {0} {1}
                                                       FROM Books
                                                       LEFT OUTER JOIN Book_Artist_Job ON (Books.Id=Book_Artist_Job.Book_Id)
                                                       LEFT OUTER JOIN Artist ON (Book_Artist_Job.Artist_Id = Artist.Id)
                                                       LEFT OUTER JOIN Job ON (Book_Artist_Job.Job_Id = Job.Id)
                                                       INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                       WHERE Books.IsDeleted<>'false'
                                                       AND NOT EXISTS (SELECT * 
	                                                       FROM [Books_BookType]
	                                                       WHERE [Books].[Id] = [Books_BookType].[Books_Id]);", ConstBooksThumb, ConstBookThumbArtistPart);


            objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(BooksToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }

        public int PurgeBooks()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Books> items = new List<Books>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Books
                                                     INNER JOIN Media ON (Books.Media_Id = Media.Id)
                                                     WHERE Books.IsDeleted='true';", ConstBooks);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToBooks(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            foreach (Books item in items)
                PurgeBooks(item);

            return items.Count;
        }
        public void PurgeBooks(Books item)
        {
            DeleteById("Books_Ressources", "Books_Id", item.Id);
            DeleteById("Book_Links", "Books_Id", item.Id);
            DeleteById("Book_Artist_Job", "Book_Id", item.Id);
            DeleteById("Books", "Id", item.Id);

            if (item.Publisher != null)
                DeleteOrphelans("App_Editor", item.Publisher.Id, "Books", "Editor_Id");

            if (item.FileFormat != null)
                DeleteOrphelans("BookFormat", item.FileFormat.Id, "Books", "Format_Id");

            foreach (Artist artist in item.Artists)
                DeleteOrphelanArtist(artist.Id);

        }
        public void PurgeBooksType()
        {
            //TODO
            //var query = from items in _objEntities.Books_BookType
            //            group items by new { items.Books_Id, items.BookType_Id } into g
            //            where g.Count() > 1
            //            select g.Key;

            //foreach (var item in query)
            //{
            //    var item1 = item;
            //    var duplicates = (from items in _objEntities.Books_BookType
            //                      where items.Books_Id == item1.Books_Id && items.BookType_Id == item1.BookType_Id
            //                      select items).First();

            //    _objEntities.DeleteObject(duplicates);
            //}

            //_objEntities.SaveChanges();
        }

        private Books ResultsToBooks(SQLiteDataReader reader, bool getLastId = true)
        {
            Books item = new Books();

            item.Id = reader.GetString(0);
            item.Title = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.BarCode = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
            {
                item.Publisher = new Publisher();
                item.Publisher.Id = reader.GetString(3);
            }

            if (reader.IsDBNull(4) == false)
            {
                item.FileFormat = new FileFormat();
                item.FileFormat.Id = reader.GetString(4);
            }

            item.Media = new Media();
            item.Media.Id = reader.GetString(5);
            item.Media.Name = reader.GetString(24);

            if (reader.IsDBNull(6) == false)
            {
                item.Language = new Language();
                item.Language.Id = reader.GetString(6);
            }

            if (reader.IsDBNull(7) == false)
                item.Isbn = reader.GetString(7);

            if (reader.IsDBNull(8) == false)
                item.NbrPages = reader.GetInt32(8);

            if (reader.IsDBNull(9) == false)
                item.Watched = reader.GetBoolean(9);

            if (reader.IsDBNull(10) == false)
                item.IsDeleted = reader.GetBoolean(10);

            if (reader.IsDBNull(11) == false)
                item.IsWhish = reader.GetBoolean(11);

            if (reader.IsDBNull(12) == false)
                item.IsComplete = reader.GetBoolean(12);

            if (reader.IsDBNull(13) == false)
                item.Description = reader.GetString(13);

            if (reader.IsDBNull(14) == false)
                item.FileName = reader.GetString(14);

            if (reader.IsDBNull(15) == false)
                item.FilePath = reader.GetString(15);

            if (reader.IsDBNull(16) == false)
                item.AddedDate = reader.GetDateTime(16);

            if (reader.IsDBNull(17) == false)
                item.ReleaseDate = reader.GetDateTime(17);

            if (reader.IsDBNull(18) == false)
                item.ToBeDeleted = reader.GetBoolean(18);

            if (reader.IsDBNull(19) == false)
                item.MyRating = reader.GetInt32(19);

            if (reader.IsDBNull(20) == false)
                item.Comments = reader.GetString(20);

            if (reader.IsDBNull(21) == false)
                item.Rated = reader.GetString(21);

            if (reader.IsDBNull(22) == false)
                item.Cover = (byte[])reader.GetValue(22);

            if (reader.IsDBNull(23) == false)
                item.ToWatch = reader.GetBoolean(23);

            if (reader.IsDBNull(25) == false)
                item.PublicRating = reader.GetDouble(25);

            if (reader.IsDBNull(26) == false)
                item.NumId = reader.GetInt32(26);
            else if (getLastId)
                item.NumId = GetLastCollectionNumber(EntityType.Books);

            return item;
        }

        #endregion
        #region Games
        public void AddGame(Gamez entity)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO Gamez(Id, Language_Id, Media_Id, Editor_Id, Title, BarCode, Rating, Descriptions, ReleaseDate,
                                                                            Comments, FileName, AddedDate, IsDeleted, IsWhish, IsComplete, FilePath, IsTested,
                                                                            ToBeDeleted, Rated, smallCover, Platform_Id, Price, ToTest, PublicRating,NumID)
                                                            VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,
                                                                    @param11,@param12,@param13,@param14,@param15,@param16,@param17,@param18,@param19,@param20,@param21,@param22,
                                                                    @param23,@param24,@param25)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Gamez 
                                                        SET Language_Id=@param2,Media_Id=@param3,Editor_Id=@param4,Title=@param5,BarCode=@param6,Rating=@param7,
                                                            Descriptions=@param8, ReleaseDate=@param9, Comments=@param10, FileName=@param11, AddedDate=@param12, 
                                                            IsDeleted=@param13, IsWhish=@param14, IsComplete=@param15, FilePath=@param16, IsTested=@param17, 
                                                            ToBeDeleted=@param18, Rated=@param19, smallCover=@param20, Platform_Id=@param21,Price=@param22,
                                                            ToTest=@param23,PublicRating=@param24,NumID=@param25
                                                        WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            if (entity.Language != null)
                param2.Value = entity.Language.Id;
            else
                param2.Value = DBNull.Value;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = entity.Media.Id;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            if (entity.Publisher != null)
                param4.Value = entity.Publisher.Id;
            else
                param4.Value = DBNull.Value;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
            param5.Value = entity.Title;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = entity.BarCode;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.Int32);
            param7.Value = entity.MyRating;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
            param8.Value = entity.Description;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.DateTime);
            param9.Value = entity.ReleaseDate;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.String);
            param10.Value = entity.Comments;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.String);
            param11.Value = entity.FileName;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.DateTime);
            param12.Value = entity.AddedDate;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.Boolean);
            param13.Value = entity.IsDeleted;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Boolean);
            param14.Value = entity.IsWhish;

            SQLiteParameter param15 = new SQLiteParameter("@param15", DbType.Boolean);
            param15.Value = entity.IsComplete;

            SQLiteParameter param16 = new SQLiteParameter("@param16", DbType.String);
            param16.Value = entity.FilePath;

            SQLiteParameter param17 = new SQLiteParameter("@param17", DbType.Boolean);
            param17.Value = entity.Watched;

            SQLiteParameter param18 = new SQLiteParameter("@param18", DbType.Boolean);
            param18.Value = entity.ToBeDeleted;

            SQLiteParameter param19 = new SQLiteParameter("@param19", DbType.String);
            param19.Value = entity.Rated;

            SQLiteParameter param20 = new SQLiteParameter("@param20", DbType.Binary);
            param20.Value = entity.Cover;

            SQLiteParameter param21 = new SQLiteParameter("@param21", DbType.String);
            if (entity.Platform != null)
                param21.Value = entity.Platform.Id;
            else
                param21.Value = DBNull.Value;

            SQLiteParameter param22 = new SQLiteParameter("@param22", DbType.Int32);
            param22.Value = entity.Price;

            SQLiteParameter param23 = new SQLiteParameter("@param23", DbType.Boolean);
            param23.Value = entity.ToWatch;

            SQLiteParameter param24 = new SQLiteParameter("@param24", DbType.Double);
            param24.Value = entity.PublicRating;

            SQLiteParameter param25 = new SQLiteParameter("@param25", DbType.Int32);
            param25.Value = entity.NumId;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);
            objCommand.Parameters.Add(param15);
            objCommand.Parameters.Add(param16);
            objCommand.Parameters.Add(param17);
            objCommand.Parameters.Add(param18);
            objCommand.Parameters.Add(param19);
            objCommand.Parameters.Add(param20);
            objCommand.Parameters.Add(param21);
            objCommand.Parameters.Add(param22);
            objCommand.Parameters.Add(param23);
            objCommand.Parameters.Add(param24);
            objCommand.Parameters.Add(param25);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        private ThumbItem GamesToThumb(SQLiteDataReader reader)
        {
            string fullPath = string.Empty;
            if (reader.IsDBNull(14) == false && reader.IsDBNull(13) == false)
                if (string.IsNullOrWhiteSpace(reader.GetString(14)) == false &&
                    string.IsNullOrWhiteSpace(reader.GetString(13)) == false)
                    fullPath = Path.Combine(reader.GetString(14), reader.GetString(13));

            string description = string.Empty;
            if (reader.IsDBNull(6) == false)
                description = reader.GetString(6);

            byte[] cover = null;
            if (reader.IsDBNull(2) == false)
                cover = (byte[])reader.GetValue(2);

            bool tested = false;
            if (reader.IsDBNull(11) == false)
                tested = reader.GetBoolean(11);

            int? rating = null;
            if (reader.IsDBNull(5) == false)
                rating = reader.GetInt32(5);

            double? publicRating = null;
            if (reader.IsDBNull(16) == false)
                publicRating = reader.GetDouble(16);

            int? numId = null;
            if (reader.IsDBNull(17) == false)
                numId = reader.GetInt32(17);


            return new ThumbItem(reader.GetString(1), reader.GetString(0), cover, reader.GetDateTime(3),
                                    null, string.Empty, reader.GetBoolean(4), EntityType.Games, rating, publicRating, description,
                                    reader.GetBoolean(7), reader.GetString(8), reader.GetBoolean(9), reader.GetBoolean(10), tested, reader.GetString(15),
                                    string.Empty, string.Empty, reader.GetBoolean(12), fullPath, numId);
        }
        public void GetChild(Gamez items)
        {
            items.Links = GetLinks("Gamez_Links", "Gamez_Id", items.Id);
            items.Ressources = GetRessources("Gamez_Ressources", "Gamez_Id", items.Id);

            if (items.Publisher != null)
                items.Publisher = GetPublisher(items.Publisher.Id, "App_Editor", "Id");

            items.Genres = GetGenres("GamezType", "Gamez_GamezType", "Gamez_Id", items.Id, "GamezType_Id");

            if (items.Language != null)
                items.Language = GetLanguageById(items.Language.Id);

            if (items.Platform != null)
                items.Platform = GetPlatformById(items.Platform.Id);
        }
        public IList GetDupeGames()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM
                                                        (Select Gamez.Title
                                                        From Gamez
                                                        Group By Gamez.Title
                                                        Having (Count(Gamez.Title) > 1)) AS GRP
                                                      INNER JOIN Gamez ON Gamez.TITLE = GRP.TITLE  
                                                      INNER JOIN Media ON (Gamez.Media_Id = Media.Id);", ConstGamesThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(GamesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Gamez GetFirstGame()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Gamez items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Gamez
                                                      INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                      WHERE Gamez.IsDeleted<>'false' 
                                                      LIMIT 1;", ConstGames);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToGames(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items);

            return items;
        }
        public IList GetGames()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Gamez> items = new List<Gamez>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Gamez
                                                     INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                     WHERE Gamez.IsDeleted <> 'false';", ConstGames);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToGames(objResults, false));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Gamez GetGames(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Gamez items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Gamez
                                                      INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                      WHERE Gamez.IsDeleted<>'false' 
                                                      AND Gamez.Id='{1}';", ConstGames, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToGames(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items);

            return items;
        }
        public Gamez GetGames(string mediaName, string filePath, string fileName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Gamez items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Gamez
                                                      INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                      WHERE Gamez.IsDeleted<>'false' 
                                                      AND Media.Name = @param1 AND Gamez.FileName = @param2 AND Gamez.FilePath = @param3;", ConstGames);

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = fileName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = filePath;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToGames(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items);

            return items;
        }
        public IList GetGamesByMedia(string mediaName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Gamez> items = new List<Gamez>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM  Gamez
                                                      INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                      WHERE Gamez.IsDeleted=0 
                                                      AND Media.Name = @param1;", ConstGames);

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            objCommand.Parameters.Add(param1);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToGames(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public int GetGamesCountByType(string strGenre)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int number = 0;
            if (strGenre == string.Empty)
                return number;
            else
            {
                objCommand.CommandText = string.Format(@"SELECT Count(Gamez.Title)
                                                        FROM Gamez_GamezType
                                                        INNER JOIN Gamez ON (Gamez_GamezType.Gamez_Id = Gamez.Id)
                                                        INNER JOIN GamezType ON (Gamez_GamezType.GamezType_Id = GamezType.Id)
                                                        WHERE GamezType.DisplayName = ""{0}"";", strGenre);

                SQLiteDataReader objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                {
                    objResults.Read();
                    number = objResults.GetInt32(0);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            return number;
        }
        public IList GetNoSmallCoverGames()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Gamez> items = new List<Gamez>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Gamez
                                                     INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                     WHERE Gamez.Cover IS NULL;", ConstGames);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToGames(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItem GetThumbGame(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItem items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM Gamez
                                                     INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                     WHERE Gamez.IsDeleted<>'false' 
                                                     AND Gamez.Id='{1}';", ConstGamesThumb, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = GamesToThumb(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public IList GetThumbGames()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM Gamez
                                                     INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                     WHERE Gamez.IsDeleted<>'false'", ConstGamesThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(GamesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItems GetBigThumbGames()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT Gamez.Id, Title, BigCover, AddedDate, IsDeleted, Rating, Descriptions, ToBeDeleted, Media.Name,
                                                            IsWhish, IsComplete, IsTested, ToTest, FileName, FilePath, 'None'
                                                      FROM 
                                                            (SELECT Gamez_Ressources.Gamez_Id as GamezId, Gamez_Ressources.Ressource as BigCover
                                                             FROM Gamez_Ressources
                                                             WHERE Gamez_Ressources.IsDefault=1
                                                             ), Gamez
                                                       INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                        WHERE GamezId=Gamez.Id;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(GamesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetThumbGamesByTypes()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = @"SELECT Distinct Gamez.Id, Title, smallCover, AddedDate, IsDeleted, Rating, Descriptions, ToBeDeleted, Media.Name,
                                                            IsWhish, IsComplete, IsTested, ToTest, FileName, FilePath, GamezType.DisplayName
                                                     FROM Gamez
                                                     INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                     INNER JOIN GamezType ON GamezType.Id = Gamez_GamezType.GamezType_Id
                                                     INNER JOIN Gamez_GamezType ON Gamez_GamezType.Gamez_Id = Gamez.Id
                                                     WHERE Gamez.IsDeleted<>'false'
                                                     AND GamezType.DisplayName <>'' ;";

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(GamesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                       FROM Gamez
                                       INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                       WHERE Gamez.IsDeleted<>'false'
                                       AND NOT EXISTS (SELECT * 
	                                                   FROM [Gamez_GamezType]
	                                                   WHERE [Gamez].[Id] = [Gamez_GamezType].[Gamez_Id]) ;", ConstGamesThumb);


            objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(GamesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
            return items;

        }
        public int PurgeGames()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Gamez> items = new List<Gamez>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Gamez
                                                     INNER JOIN Media ON (Gamez.Media_Id = Media.Id)
                                                     WHERE Gamez.IsDeleted='true';", ConstGames);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToGames(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            foreach (Gamez item in items)
                PurgeGames(item);

            return items.Count;
        }
        public void PurgeGames(Gamez item)
        {
            DeleteById("Gamez_Ressources", "Gamez_Id", item.Id);
            DeleteById("Gamez_Links", "Gamez_Id", item.Id);
            DeleteById("Gamez", "Id", item.Id);
            if (item.Publisher != null)
                if (string.IsNullOrWhiteSpace(item.Publisher.Id) == false)
                    DeleteOrphelans("App_Editor", item.Publisher.Id, "Gamez", "Editor_Id");
        }
        public void PurgeGamesType()
        {
            //TODO
            //var query = from items in _objEntities.Gamez_GamezType
            //            group items by new { items.Gamez_Id, items.GamezType_Id } into g
            //            where g.Count() > 1
            //            select g.Key;

            //foreach (var item in query)
            //{
            //    var item1 = item;
            //    Gamez_GamezType duplicates = (from items in _objEntities.Gamez_GamezType
            //                                  where items.Gamez_Id == item1.Gamez_Id && items.GamezType_Id == item1.GamezType_Id
            //                                  select items).First();

            //    _objEntities.DeleteObject(duplicates);
            //}

            //_objEntities.SaveChanges();

        }
        private Gamez ResultsToGames(SQLiteDataReader reader, bool getLastId = true)
        {
            Gamez item = new Gamez();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
            {
                item.Language = new Language();
                item.Language.Id = reader.GetString(1);
            }

            item.Media = new Media();
            item.Media.Id = reader.GetString(2);
            item.Media.Name = reader.GetString(23);

            if (reader.IsDBNull(3) == false)
            {
                item.Publisher = new Publisher();
                item.Publisher.Id = reader.GetString(3);
            }

            item.Title = reader.GetString(4);

            if (reader.IsDBNull(5) == false)
                item.BarCode = reader.GetString(5);

            if (reader.IsDBNull(6) == false)
                item.MyRating = reader.GetInt32(6);

            if (reader.IsDBNull(7) == false)
                item.Description = reader.GetString(7);

            if (reader.IsDBNull(8) == false)
                item.ReleaseDate = reader.GetDateTime(8);

            if (reader.IsDBNull(9) == false)
                item.Comments = reader.GetString(9);

            if (reader.IsDBNull(10) == false)
                item.FileName = reader.GetString(10);

            if (reader.IsDBNull(11) == false)
                item.AddedDate = reader.GetDateTime(11);

            if (reader.IsDBNull(12) == false)
                item.IsDeleted = reader.GetBoolean(12);

            if (reader.IsDBNull(13) == false)
                item.IsWhish = reader.GetBoolean(13);

            if (reader.IsDBNull(14) == false)
                item.IsComplete = reader.GetBoolean(14);

            if (reader.IsDBNull(15) == false)
                item.FilePath = reader.GetString(15);

            if (reader.IsDBNull(16) == false)
                item.Watched = reader.GetBoolean(16);

            if (reader.IsDBNull(17) == false)
                item.ToBeDeleted = reader.GetBoolean(17);

            if (reader.IsDBNull(18) == false)
                item.Rated = reader.GetString(18);

            if (reader.IsDBNull(19) == false)
                item.Cover = (byte[])reader.GetValue(19);

            if (reader.IsDBNull(20) == false)
            {
                item.Platform = new Platform();
                item.Platform.Id = reader.GetString(20);
            }

            if (reader.IsDBNull(21) == false)
                item.Price = reader.GetInt32(21);

            if (reader.IsDBNull(22) == false)
                item.ToWatch = reader.GetBoolean(22);

            if (reader.IsDBNull(24) == false)
                item.PublicRating = reader.GetDouble(24);

            if (reader.IsDBNull(25) == false)
                item.NumId = reader.GetInt32(25);
            else if (getLastId)
                item.NumId = GetLastCollectionNumber(EntityType.Games);

            return item;
        }

        #endregion
        #region Movie
        public void AddMovie(Movie entity)
        {
            SQLiteConnection objConnection = null;
            SQLiteCommand objCommand = null;
            try
            {

                #region FileFormat
                if (entity.FileFormat != null && string.IsNullOrWhiteSpace(entity.FileFormat.Id))
                    AddFileFormat(entity.FileFormat);
                #endregion
                #region AspectRatio
                if (entity.AspectRatio != null && string.IsNullOrWhiteSpace(entity.AspectRatio.Id))
                    AddAspectRatio(entity.AspectRatio);
                #endregion

                objConnection = new SQLiteConnection(_connectionString);
                objCommand = objConnection.CreateCommand();
                objCommand.CommandType = CommandType.Text;
                objConnection.Open();

                if (string.IsNullOrWhiteSpace(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                    objCommand.CommandText =
                        string.Format(
                            @"INSERT INTO Movie( Id, Studio_Id, Media_Id, FileFormat_Id, AspectRatio_Id, Title, BarCode, Imdb, AlloCine,
                                                                             Rating, Description, Runtime, Tagline, Rated, ReleaseDate, Seen, IsDeleted, IsWhish,
                                                                             IsComplete, AddedDate, FileName, FilePath, OriginalTitle, TobeDeleted, Comments,
                                                                             Country, smallCover, ToWatch, Goofs, PublicRating,NumID)
                                                          VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10, @param11,@param12,
                                                                 @param13,@param14,@param15,@param16,@param17,@param18,@param19,@param20,@param21,@param22,@param23,
                                                                 @param24,@param25,@param26,@param27,@param28,@param29,@param30,@param31)");
                }
                else
                    objCommand.CommandText = string.Format(@"UPDATE Movie 
                                                        SET Studio_Id=@param2,Media_Id=@param3,FileFormat_Id=@param4,AspectRatio_Id=@param5,Title=@param6,BarCode=@param7,
                                                            Imdb=@param8, AlloCine=@param9, Rating=@param10, Description=@param11, Runtime=@param12, 
                                                            Tagline=@param13, Rated=@param14, ReleaseDate=@param15, Seen=@param16, IsDeleted=@param17, 
                                                            IsWhish=@param18, IsComplete=@param19, AddedDate=@param20, FileName=@param21, FilePath=@param22, 
                                                            OriginalTitle=@param23, TobeDeleted=@param24, Comments=@param25, Country=@param26, smallCover=@param27,
                                                            ToWatch=@param28, Goofs=@param29, PublicRating=@param30,NumID=@param31
                                                         WHERE id=@param1");

                SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
                param1.Value = entity.Id;

                SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
                if (entity.Publisher != null)
                    param2.Value = entity.Publisher.Id;
                else
                    param2.Value = DBNull.Value;

                SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
                param3.Value = entity.Media.Id;

                SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
                if (entity.FileFormat != null)
                    param4.Value = entity.FileFormat.Id;
                else
                    param4.Value = DBNull.Value;

                SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
                if (entity.AspectRatio != null)
                    param5.Value = entity.AspectRatio.Id;
                else
                    param5.Value = DBNull.Value;

                SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
                param6.Value = entity.Title;

                SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
                param7.Value = entity.BarCode;

                SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
                param8.Value = entity.Imdb;

                SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.String);
                param9.Value = entity.AlloCine;

                SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.Int32);
                param10.Value = entity.MyRating;

                SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.String);
                param11.Value = entity.Description;

                SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.Int32);
                param12.Value = entity.Runtime;

                SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.String);
                param13.Value = entity.Tagline;

                SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Int32);
                param14.Value = entity.Rated;

                SQLiteParameter param15 = new SQLiteParameter("@param15", DbType.DateTime);
                param15.Value = entity.ReleaseDate;

                SQLiteParameter param16 = new SQLiteParameter("@param16", DbType.Boolean);
                param16.Value = entity.Watched;

                SQLiteParameter param17 = new SQLiteParameter("@param17", DbType.Boolean);
                param17.Value = entity.IsDeleted;

                SQLiteParameter param18 = new SQLiteParameter("@param18", DbType.Boolean);
                param18.Value = entity.IsWhish;

                SQLiteParameter param19 = new SQLiteParameter("@param19", DbType.Boolean);
                param19.Value = entity.IsComplete;

                SQLiteParameter param20 = new SQLiteParameter("@param20", DbType.DateTime);
                param20.Value = entity.AddedDate;

                SQLiteParameter param21 = new SQLiteParameter("@param21", DbType.String);
                param21.Value = entity.FileName;

                SQLiteParameter param22 = new SQLiteParameter("@param22", DbType.String);
                param22.Value = entity.FilePath;

                SQLiteParameter param23 = new SQLiteParameter("@param23", DbType.String);
                param23.Value = entity.OriginalTitle;

                SQLiteParameter param24 = new SQLiteParameter("@param24", DbType.Boolean);
                param24.Value = entity.ToBeDeleted;

                SQLiteParameter param25 = new SQLiteParameter("@param25", DbType.String);
                param25.Value = entity.Comments;

                SQLiteParameter param26 = new SQLiteParameter("@param26", DbType.String);
                param26.Value = entity.Country;

                SQLiteParameter param27 = new SQLiteParameter("@param27", DbType.Binary);
                param27.Value = entity.Cover;

                SQLiteParameter param28 = new SQLiteParameter("@param28", DbType.Boolean);
                param28.Value = entity.ToWatch;

                SQLiteParameter param29 = new SQLiteParameter("@param29", DbType.String);
                param29.Value = entity.Goofs;

                SQLiteParameter param30 = new SQLiteParameter("@param30", DbType.Double);
                param30.Value = entity.PublicRating;

                SQLiteParameter param31 = new SQLiteParameter("@param31", DbType.Int32);
                param31.Value = entity.NumId;

                objCommand.Parameters.Add(param1);
                objCommand.Parameters.Add(param2);
                objCommand.Parameters.Add(param3);
                objCommand.Parameters.Add(param4);
                objCommand.Parameters.Add(param5);
                objCommand.Parameters.Add(param6);
                objCommand.Parameters.Add(param7);
                objCommand.Parameters.Add(param8);
                objCommand.Parameters.Add(param9);
                objCommand.Parameters.Add(param10);
                objCommand.Parameters.Add(param11);
                objCommand.Parameters.Add(param12);
                objCommand.Parameters.Add(param13);
                objCommand.Parameters.Add(param14);
                objCommand.Parameters.Add(param15);
                objCommand.Parameters.Add(param16);
                objCommand.Parameters.Add(param17);
                objCommand.Parameters.Add(param18);
                objCommand.Parameters.Add(param19);
                objCommand.Parameters.Add(param20);
                objCommand.Parameters.Add(param21);
                objCommand.Parameters.Add(param22);
                objCommand.Parameters.Add(param23);
                objCommand.Parameters.Add(param24);
                objCommand.Parameters.Add(param25);
                objCommand.Parameters.Add(param26);
                objCommand.Parameters.Add(param27);
                objCommand.Parameters.Add(param28);
                objCommand.Parameters.Add(param29);
                objCommand.Parameters.Add(param30);
                objCommand.Parameters.Add(param31);

                objCommand.ExecuteNonQuery();
                objCommand.Parameters.Clear();

                #region SubTitle

                DeleteById("Movie_SubTitle", "Movie_Id", entity.Id);

                if (entity.Subtitles != null)
                    foreach (Language item in entity.Subtitles)
                        AddSubTitle(item, entity.Id);
                #endregion
                #region Audios

                DeleteById("Movie_Audio", "Movie_Id", entity.Id);

                if (entity.Audios != null)
                    foreach (Audio item in entity.Audios)
                        AddAudio(item, entity.Id);
                #endregion
            }
            catch (Exception ex)
            {
                string name = string.Empty;
                if (entity != null)
                    name = entity.Title;
                Util.LogException(ex, name);
                throw;
            }
            finally
            {
                if (objCommand != null)
                    objCommand.Dispose();

                if (objConnection != null)
                {
                    objConnection.Close();
                    objConnection.Dispose();
                }
            }
        }
        public void DeleteAllMovie()
        {
            PurgeTable("Movie_MovieGenre");
            PurgeTable("Movie_Genre");
            PurgeTable("Movie_Ressources");
            PurgeTable("Movie_Links");
            PurgeTable("Movie_Artist_Job");
            PurgeTable("Movie");
            PurgeTable("Movie_Studio");
            PurgeTable("Movie_Audio");
            PurgeTable("Movie_SubTitle");
            PurgeTable("AspectRatio");
            PurgeTable("AudioType");
        }
        public void GetChild(Movie items, bool getArtistCredits)
        {
            items.Links = GetLinks("Movie_Links", "Movie_Id", items.Id);
            items.Ressources = GetRessources("Movie_Ressources", "Movie_Id", items.Id);
            if (items.Publisher != null)
                items.Publisher = GetPublisher(items.Publisher.Id, "Movie_Studio", "Id");
            items.Artists = GetArtists(items.Id, EntityType.Movie, getArtistCredits);
            items.Genres = GetGenres("Movie_Genre", "Movie_MovieGenre", "Movie_Id", items.Id, "MovieGenre");
            items.Audios = GetAudios(items.Id);
            items.Subtitles = GetSubTitle(items.Id);
            items.MetaDatas = GetMetaDatas(items.Id);

            if (items.FileFormat != null)
                items.FileFormat = GetFileFormatById(items.FileFormat.Id);

            if (items.AspectRatio != null)
                items.AspectRatio = GetAspectRatioById(items.AspectRatio.Id);
        }
        public IList GetDupeMovie()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();
            #region Query
            objCommand.CommandText = string.Format(@"SELECT DISTINCT *
                                                      FROM (
                                                            SELECT DISTINCT mov.Id, mov.Title, smallCover, AddedDate, Runtime, OriginalTitle, IsDeleted, Imdb, AlloCine, Rating, Description, TobeDeleted, Media.Name, 
                                                                            IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, 'None' 
                                                            FROM Movie as mov
                                                            INNER JOIN Media ON (mov.Media_Id = Media.Id)
                                                            WHERE mov.OriginalTitle IN (
                                                                        SELECT mov2.OriginalTitle
                                                                        FROM Movie as mov2
                                                                        WHERE mov2.OriginalTitle is not null AND mov2.OriginalTitle <> ''
                                                                        GROUP BY mov2.OriginalTitle
                                                                        HAVING count(*)>1)
                                                            UNION ALL
                                                            -- Query to get duplicates on Title
                                                            SELECT DISTINCT mov.Id, mov.Title, smallCover, AddedDate, Runtime, OriginalTitle, IsDeleted, Imdb, AlloCine, Rating, Description, TobeDeleted, Media.Name, 
                                                                            IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, 'None' 
                                                            FROM Movie as mov
                                                            INNER JOIN Media ON (mov.Media_Id = Media.Id)
                                                            WHERE mov.Title in(
                                                                        SELECT mov2.title
                                                                        FROM Movie as mov2
                                                                        GROUP BY mov2.title
                                                                        HAVING count(*)>1)
                                                            UNION ALL
                                                            -- Query to get Orignaltitle same as Title
                                                            SELECT DISTINCT mov3.Id, mov3.Title, mov3.smallCover, mov3.AddedDate, mov3.Runtime, mov3.OriginalTitle, mov3.IsDeleted, mov3.Imdb, mov3.AlloCine, mov3.Rating, mov3.Description, mov3.TobeDeleted, Media.Name, 
                                                                            mov3.IsWhish, mov3.IsComplete,mov3.Seen, mov3.ToWatch, mov3.FileName, mov3.FilePath, 'None'
                                                            FROM Movie mov3
	                                                        JOIN Movie s3 ON mov3.OriginalTitle = s3.Title
	                                                        INNER JOIN Media ON (mov3.Media_Id = Media.Id)
                                                            WHERE mov3.Id <> s3.Id AND mov3.OriginalTitle <> '' AND mov3.OriginalTitle is not null
                                                            UNION
                                                            -- Query to get title same as Original Title
                                                            SELECT DISTINCT mov3.Id, mov3.Title, mov3.smallCover, mov3.AddedDate, mov3.Runtime, mov3.OriginalTitle, mov3.IsDeleted, mov3.Imdb, mov3.AlloCine, mov3.Rating, mov3.Description, mov3.TobeDeleted, Media.Name, 
                                                                            mov3.IsWhish, mov3.IsComplete,mov3.Seen, mov3.ToWatch, mov3.FileName, mov3.FilePath, 'None'
                                                            FROM Movie mov3
                                                            JOIN Movie s4 on mov3.Title = s4.OriginalTitle
                                                            INNER JOIN Media ON (mov3.Media_Id = Media.Id)
                                                            WHERE mov3.id <> s4.Id
                                                        )
                                                        ORDER by Title");
            #endregion
            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MovieToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public Movie GetFirstMovie()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Movie items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Movie
                                                      INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                      WHERE Movie.IsDeleted=0
                                                      LIMIT 1;", ConstMovie);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToMovie(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public Movie GetMovies(string id)
        {
            Movie items = null;

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();


            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Movie
                                                      INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                      WHERE Movie.IsDeleted<>'false' 
                                                      AND Movie.Id='{1}';", ConstMovie, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToMovie(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetMovies()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Movie> items = new List<Movie>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Movie
                                                     INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                     WHERE Movie.IsDeleted <> 'false'
                                                     ORDER BY Movie.Title;", ConstMovie);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMovie(objResults, false));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();


            return items;
        }
        public Movie GetMovies(string mediaName, string filePath, string fileName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Movie items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Movie
                                                      INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                      WHERE Movie.IsDeleted=0
                                                      AND Media.Name = @param1 AND  Movie.FileName = @param2 AND  Movie.FilePath = @param3;", ConstMovie);

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = fileName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = filePath;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToMovie(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetMoviesByMedia(string mediaName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Movie> items = new List<Movie>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM  Movie
                                                      INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                      WHERE Movie.IsDeleted=0 
                                                      AND Media.Name = ""{1}"";", ConstMovie, mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMovie(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public int GetMovieCountByType(string strGenre)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int number = 0;
            if (strGenre == string.Empty)
                return number;
            else
            {
                objCommand.CommandText = string.Format(@"SELECT Count(Movie.Title)
                                                        FROM Movie_MovieGenre
                                                        INNER JOIN Movie ON (Movie_MovieGenre.Movie_Id = Movie.Id)
                                                        INNER JOIN Movie_Genre ON (Movie_MovieGenre.MovieGenre = Movie_Genre.Id)
                                                        WHERE Movie_Genre.DisplayName = ""{0}"";", strGenre);

                SQLiteDataReader objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                {
                    objResults.Read();
                    number = objResults.GetInt32(0);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            return number;
        }
        public ThumbItem GetThumbMovie(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItem items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                    FROM Movie
                                                    INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                    WHERE Movie.IsDeleted<>'false' 
                                                    AND Movie.Id='{1}';", ConstMovieThumb, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = MovieToThumb(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItems GetThumbMovies()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                   FROM Movie
                                                   INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                   WHERE Movie.IsDeleted<>'false';", ConstMovieThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MovieToThumb(objResults));


            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItems GetBigThumbMovies()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT Movie.Id, Title, BigCover, AddedDate, Runtime, OriginalTitle, IsDeleted, Imdb, AlloCine, Rating, Description, TobeDeleted, Media.Name, 
                                                            IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, 'None'
                                                      FROM 
                                                            (SELECT Movie_Ressources.Movie_Id as MovieID, Movie_Ressources.Ressource as BigCover
                                                             FROM Movie_Ressources
                                                             WHERE Movie_Ressources.IsDefault=1
                                                             ), Movie
                                                       INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                        WHERE MovieID=Movie.Id;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MovieToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        private ThumbItem MovieToThumb(SQLiteDataReader reader)
        {

            double? publicRating = null;
            double? myRating = null;
            double alloCine = -1;
            double imdb = -1;

            if (reader.IsDBNull(8) == false && string.IsNullOrWhiteSpace(reader.GetString(8)) == false)
                double.TryParse(reader.GetString(8), out alloCine);

            if (reader.IsDBNull(7) == false && string.IsNullOrWhiteSpace(reader.GetString(7)) == false)
                double.TryParse(reader.GetString(7), out imdb);


            if (reader.IsDBNull(20) == false)
                publicRating = reader.GetDouble(20);
            else if (imdb >= 0 && alloCine >= 0)
                publicRating = (imdb + alloCine) / 2;
            else if (imdb >= 0)
                publicRating = imdb;
            else if (alloCine >= 0)
                publicRating = alloCine;

            if (reader.IsDBNull(9) == false)
                myRating = reader.GetDouble(9);

            string originalTitle = string.Empty;
            if (reader.IsDBNull(5) == false)
                originalTitle = reader.GetString(5);


            string fullPath = string.Empty;
            try
            {
                if (reader.IsDBNull(18) == false && reader.IsDBNull(17) == false)
                    if (string.IsNullOrWhiteSpace(reader.GetString(18)) == false &&
                        string.IsNullOrWhiteSpace(reader.GetString(17)) == false)
                        fullPath = Path.Combine(reader.GetString(18), reader.GetString(17));
            }
            //FIX 2.8.9.0
            catch (Exception ex)
            {
                string param17 = string.Empty;
                string param18 = string.Empty;

                if (reader.IsDBNull(18) == false && string.IsNullOrWhiteSpace(reader.GetString(18)) == false)
                    param18 = reader.GetString(18);
                if (reader.IsDBNull(17) == false && string.IsNullOrWhiteSpace(reader.GetString(17)) == false)
                    param18 = reader.GetString(17);

                Util.LogException(ex, param17 + " " + param18);
            }


            string description = string.Empty;
            if (reader.IsDBNull(10) == false)
                description = reader.GetString(10);

            int? runtime = null;
            if (reader.IsDBNull(4) == false)
                runtime = reader.GetInt32(4);

            byte[] cover = null;
            if (reader.IsDBNull(2) == false)
                cover = (byte[])reader.GetValue(2);

            int? numId = null;
            if (reader.IsDBNull(21) == false)
                numId = reader.GetInt32(21);

            string firstName = string.Empty;
            if (reader.IsDBNull(22) == false)
                firstName = reader.GetString(22);

            string lastName = string.Empty;
            if (reader.IsDBNull(23) == false)
                lastName = reader.GetString(23);

            string artist = firstName + " " + lastName;

            return new ThumbItem(reader.GetString(1), reader.GetString(0), cover, reader.GetDateTime(3),
                                    runtime, originalTitle, reader.GetBoolean(6),
                                    EntityType.Movie, myRating, publicRating, description,
                                    reader.GetBoolean(11), reader.GetString(12), reader.GetBoolean(13),
                                    reader.GetBoolean(14), reader.GetBoolean(15), reader.GetString(19),
                                    string.Empty, artist.Trim(), reader.GetBoolean(16), fullPath, numId);
        }

        public IList GetThumbMoviesByArtist()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<ThumbItem> thumbItems = new List<ThumbItem>();

            objCommand.CommandText = string.Format(@"SELECT {0} {1}
                                                      FROM Movie_Artist_Job
                                                      INNER JOIN Movie ON (Movie_Artist_Job.Movie_Id = Movie.Id)
                                                      INNER JOIN Artist ON (Movie_Artist_Job.Artist_Id = Artist.Id)
                                                      INNER JOIN Job ON (Movie_Artist_Job.Job_Id = Job.Id)
                                                      INNER JOIN Media ON (Movie.Media_Id = Media.Id);", ConstMovieThumb, ConstMovieThumbArtistPart);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    thumbItems.Add(MovieToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return thumbItems;

        }
        public IList GetThumbMoviesByTypes()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = @"SELECT Distinct Movie.Id, Title, smallCover, AddedDate, Runtime, OriginalTitle, IsDeleted, Imdb, AlloCine, Rating, Description, TobeDeleted, Media.Name, 
                                              IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, Movie_Genre.DisplayName 
                                       FROM Movie_Genre
                                       INNER JOIN Movie_MovieGenre ON Movie_Genre.Id = Movie_MovieGenre.MovieGenre
                                       INNER JOIN Movie ON Movie_MovieGenre.Movie_Id = Movie.Id
                                       INNER JOIN Media ON Movie.Media_Id = Media.Id
                                       WHERE Movie.IsDeleted<>'false'
                                       AND Movie_Genre.DisplayName <>'';";


            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MovieToThumb(objResults));

            objResults.Close();

            objCommand.CommandText = string.Format(@"SELECT {0} 
                                       FROM Movie
                                       INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                       WHERE Movie.IsDeleted<>'false'
                                       AND NOT EXISTS (SELECT * 
	                                                   FROM [Movie_MovieGenre]
	                                                   WHERE [Movie].[Id] = [Movie_MovieGenre].[Movie_Id]) ;", ConstMovieThumb);


            objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MovieToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }

        public IList GetNoSmallCoverMovies()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Movie> items = new List<Movie>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Movie
                                                     INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                     WHERE Movie.Cover IS NULL;", ConstMovie);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMovie(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
            return items;
        }

        public int PurgeMovie()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Movie> items = new List<Movie>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Movie
                                                     INNER JOIN Media ON (Movie.Media_Id = Media.Id)
                                                     WHERE Movie.IsDeleted='true';", ConstMovie);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMovie(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            foreach (Movie item in items)
                PurgeMovie(item);

            return items.Count;
        }
        public void PurgeMovie(Movie item)
        {
            DeleteById("Movie_Ressources", "Movie_Id", item.Id);
            DeleteById("Movie_Links", "Movie_Id", item.Id);
            DeleteById("Movie", "Id", item.Id);
            if (item.Publisher != null)
                if (string.IsNullOrWhiteSpace(item.Publisher.Id) == false)
                    DeleteOrphelans("Movie_Studio", item.Publisher.Id, "Movie", "Studio_Id");
        }
        public void PurgeMovieType()
        {
            //TODO
            //var query = from items in _objEntities.Movie_MovieGenre
            //            group items by new { items.Movie_Id, items.MovieGenre } into g
            //            where g.Count() > 1
            //            select g.Key;


            //foreach (var item in query)
            //{
            //    if (item != null)
            //    {
            //        string id = item.Movie_Id;
            //        string genre = item.MovieGenre;

            //        Movie_MovieGenre duplicates = (from items in _objEntities.Movie_MovieGenre
            //                                       where items.Movie_Id == id && items.MovieGenre == genre
            //                                       select items).First();

            //        _objEntities.DeleteObject(duplicates);
            //    }
            //}

            //_objEntities.SaveChanges();
        }

        private Movie ResultsToMovie(SQLiteDataReader reader, bool getLastId = true)
        {
            Movie item = new Movie();

            item.Id = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
            {
                item.Publisher = new Publisher();
                item.Publisher.Id = reader.GetString(1);
            }

            item.Media = new Media();
            item.Media.Id = reader.GetString(2);
            item.Media.Name = reader.GetString(29);

            if (reader.IsDBNull(3) == false)
            {
                item.FileFormat = new FileFormat();
                item.FileFormat.Id = reader.GetString(3);
            }

            if (reader.IsDBNull(4) == false)
            {
                item.AspectRatio = new AspectRatio();
                item.AspectRatio.Id = reader.GetString(4);
            }

            item.Title = reader.GetString(5);

            if (reader.IsDBNull(6) == false)
                item.BarCode = reader.GetString(6);

            if (reader.IsDBNull(7) == false)
                item.Imdb = reader.GetString(7);

            if (reader.IsDBNull(8) == false)
                item.AlloCine = reader.GetString(8);

            if (reader.IsDBNull(9) == false)
                item.MyRating = reader.GetInt32(9);

            if (reader.IsDBNull(10) == false)
                item.Description = reader.GetString(10);

            if (reader.IsDBNull(11) == false)
                item.Runtime = reader.GetInt32(11);

            if (reader.IsDBNull(12) == false)
                item.Tagline = reader.GetString(12);

            if (reader.IsDBNull(13) == false)
                item.Rated = reader.GetInt32(13).ToString(CultureInfo.InvariantCulture);

            if (reader.IsDBNull(14) == false)
                item.ReleaseDate = reader.GetDateTime(14);

            if (reader.IsDBNull(15) == false)
                item.Watched = reader.GetBoolean(15);

            if (reader.IsDBNull(16) == false)
                item.IsDeleted = reader.GetBoolean(16);

            if (reader.IsDBNull(17) == false)
                item.IsWhish = reader.GetBoolean(17);

            if (reader.IsDBNull(18) == false)
                item.IsComplete = reader.GetBoolean(18);

            if (reader.IsDBNull(19) == false)
                item.FileName = reader.GetString(19);

            if (reader.IsDBNull(20) == false)
                item.AddedDate = reader.GetDateTime(20);

            if (reader.IsDBNull(21) == false)
                item.FilePath = reader.GetString(21);

            if (reader.IsDBNull(22) == false)
                item.OriginalTitle = reader.GetString(22);

            if (reader.IsDBNull(23) == false)
                item.ToBeDeleted = reader.GetBoolean(23);

            if (reader.IsDBNull(24) == false)
                item.Comments = reader.GetString(24);

            if (reader.IsDBNull(25) == false)
                item.Country = reader.GetString(25);

            if (reader.IsDBNull(26) == false)
                item.Cover = (byte[])reader.GetValue(26);

            if (reader.IsDBNull(27) == false)
                item.ToWatch = reader.GetBoolean(27);

            if (reader.IsDBNull(28) == false)
                item.Goofs = reader.GetString(28);

            if (reader.IsDBNull(30) == false)
                item.PublicRating = reader.GetDouble(30);

            if (reader.IsDBNull(31) == false)
                item.NumId = reader.GetInt32(31);
            else if (getLastId)
                item.NumId = GetLastCollectionNumber(EntityType.Movie);

            return item;
        }

        #endregion
        #region Music
        public void AddMusic(Music entity)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO Music(Id, Title, BarCode, Media_Id, Studio_Id, Genre_Id, FileFormat_Id, Rating, Album, Length,
                                                                            BitRate, AddedDate, IsDeleted, IsWhish, IsComplete, IsHear, ReleaseDate, FileName, FilePath, ToBeDeleted,
                                                                            Comments, smallCover, ToHear, PublicRating,NumID)
                                                        VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,
                                                               @param11,@param12,@param13,@param14,@param15,@param16,@param17,@param18,@param19,@param20,
                                                                @param21,@param22,@param23,@param24,@param25)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Music 
                                                        SET Title=@param2,BarCode=@param3,Media_Id=@param4,Studio_Id=@param5,Genre_Id=@param6,FileFormat_Id=@param7,
                                                            Rating=@param8, Album=@param9, Length=@param10, BitRate=@param11, AddedDate=@param12, 
                                                            IsDeleted=@param13, IsWhish=@param14, IsComplete=@param15, IsHear=@param16, ReleaseDate=@param17, 
                                                            FileName=@param18, FilePath=@param19, ToBeDeleted=@param20, Comments=@param21, 
                                                            smallCover=@param22, ToHear=@param23,PublicRating=@param24,NumID=@param25
                                                         WHERE id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = entity.Title;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = entity.BarCode;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            param4.Value = entity.Media.Id;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
            if (entity.Publisher != null)
                param5.Value = entity.Publisher.Id;
            else
                param5.Value = DBNull.Value;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = null;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
            if (entity.FileFormat != null)
                param7.Value = entity.FileFormat.Id;
            else
                param7.Value = DBNull.Value;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.Int32);
            param8.Value = entity.MyRating;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.String);
            param9.Value = entity.Album;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.Int32);
            param10.Value = entity.Runtime;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.Int32);
            param11.Value = entity.BitRate;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.DateTime);
            param12.Value = entity.AddedDate;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.Boolean);
            param13.Value = entity.IsDeleted;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Boolean);
            param14.Value = entity.IsWhish;

            SQLiteParameter param15 = new SQLiteParameter("@param15", DbType.Boolean);
            param15.Value = entity.IsComplete;

            SQLiteParameter param16 = new SQLiteParameter("@param16", DbType.Boolean);
            param16.Value = entity.Watched;

            SQLiteParameter param17 = new SQLiteParameter("@param17", DbType.DateTime);
            param17.Value = entity.ReleaseDate;

            SQLiteParameter param18 = new SQLiteParameter("@param18", DbType.String);
            if (string.IsNullOrWhiteSpace(entity.FileName) == false)
                param18.Value = entity.FileName;
            else
                param18.Value = string.Empty;

            SQLiteParameter param19 = new SQLiteParameter("@param19", DbType.String);
            param19.Value = entity.FilePath;

            SQLiteParameter param20 = new SQLiteParameter("@param20", DbType.Boolean);
            param20.Value = entity.ToBeDeleted;

            SQLiteParameter param21 = new SQLiteParameter("@param21", DbType.String);
            param21.Value = entity.Comments;

            SQLiteParameter param22 = new SQLiteParameter("@param22", DbType.Binary);
            param22.Value = entity.Cover;

            SQLiteParameter param23 = new SQLiteParameter("@param23", DbType.Boolean);
            param23.Value = entity.ToWatch;

            SQLiteParameter param24 = new SQLiteParameter("@param24", DbType.Double);
            param24.Value = entity.PublicRating;

            SQLiteParameter param25 = new SQLiteParameter("@param25", DbType.Int32);
            param25.Value = entity.NumId;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);
            objCommand.Parameters.Add(param15);
            objCommand.Parameters.Add(param16);
            objCommand.Parameters.Add(param17);
            objCommand.Parameters.Add(param18);
            objCommand.Parameters.Add(param19);
            objCommand.Parameters.Add(param20);
            objCommand.Parameters.Add(param21);
            objCommand.Parameters.Add(param22);
            objCommand.Parameters.Add(param23);
            objCommand.Parameters.Add(param24);
            objCommand.Parameters.Add(param25);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }
        public void DeleteAllMusic()
        {
            PurgeTable("Music_Tracks");
            PurgeTable("Music_MusicGenre");
            PurgeTable("Music_Genre");
            PurgeTable("Music_Ressources");
            PurgeTable("Music_Links");
            PurgeTable("Music_Artist_Job");
            PurgeTable("Music");
            PurgeTable("Music_Studio");
        }
        public void GetChild(Music items, bool getArtistCredits)
        {
            items.Links = GetLinks("Music_Links", "Music_Id", items.Id);
            items.Ressources = GetRessources("Music_Ressources", "Music_Id", items.Id);
            if (items.Publisher != null)
                items.Publisher = GetPublisher(items.Publisher.Id, "Music_Studio", "Id");
            items.Artists = GetArtists(items.Id, EntityType.Music, getArtistCredits);
            items.Genres = GetGenres("Music_Genre", "Music_MusicGenre", "Music_Id", items.Id, "MusicGenre_Id");
            items.Tracks = GetTracks(items.Id);
        }
        public IList GetDupeMusic()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM
                                                        (Select Music.Title
                                                       From Music
                                                        Inner JOIN Music_Artist_JOB On Music_Artist_JOB.Music_Id = Music.Id
                                                        Inner JOIN Artist On Music_Artist_JOB.Artist_Id = Artist.Id
                                                        Group By Music.Title,artist.FulleName
                                                        Having (Count(Music.Title) > 1)) AS GRP
                                                      INNER JOIN Music ON Music.TITLE = GRP.TITLE  
                                                      INNER JOIN Media ON (Music.Media_Id = Media.Id);", ConstMusicThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MusicToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public Music GetFirstMusic()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Music items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Music
                                                      INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                      WHERE Music.IsDeleted<>'false' 
                                                      LIMIT 1;", ConstMusic);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToMusic(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetMusics()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Music> items = new List<Music>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Music
                                                     INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                     WHERE Music.IsDeleted <> 'false';", ConstMusic);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMusic(objResults, false));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Music GetMusics(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Music items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Music
                                                      INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                      WHERE Music.IsDeleted<>'false' 
                                                      AND Music.Id='{1}';", ConstMusic, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToMusic(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);


            return items;
        }
        public Music GetMusics(string mediaName, string title, string firstName, string lastName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Music items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Music_Artist_Job
                                                      INNER JOIN Music ON (Music_Artist_Job.Music_Id = Music.Id)
                                                      INNER JOIN Artist ON (Music_Artist_Job.Artist_Id = Artist.Id)
                                                      INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                      WHERE Music.IsDeleted<>'false' 
                                                      AND Media.Name =  @param1 AND  Artist.FirstName = @param2 
                                                      AND   Artist.LastName = @param3;", ConstMusic);

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = firstName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = lastName;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToMusic(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public Music GetMusics(string mediaName, string filePath, string fileName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Music items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Music
                                                      INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                      WHERE Music.IsDeleted<>'false' 
                                                      AND Media.Name = @param1 AND  Music.FileName = @param2 AND  Music.FilePath = @param3;", ConstMusic);

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = fileName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = filePath;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToMusic(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetMusicsByMedia(string mediaName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Music> items = new List<Music>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM  Music
                                                      INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                      WHERE Music.IsDeleted=0 
                                                      AND Media.Name = ""{1}"";", ConstMusic, mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMusic(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public int GetMusicCountByType(string strGenre)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int number = 0;
            if (strGenre == string.Empty)
                return number;
            else
            {
                objCommand.CommandText = string.Format(@"SELECT Count(Music.Title)
                                                        FROM Music_MusicGenre
                                                        INNER JOIN Music ON (Music_MusicGenre.Music_Id = Music.Id)
                                                        INNER JOIN Music_Genre ON (Music_MusicGenre.MusicGenre_Id = Music_Genre.Id)
                                                        WHERE Music_Genre.DisplayName = ""{0}"";", strGenre);

                SQLiteDataReader objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                {
                    objResults.Read();
                    number = objResults.GetInt32(0);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            return number;
        }
        public ThumbItem GetThumbMusic(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItem items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}{1}
                                                     FROM Music
                                                     LEFT OUTER JOIN Music_Artist_Job ON (Music.Id=Music_Artist_Job.Music_Id)
                                                     LEFT OUTER JOIN Artist ON (Music_Artist_Job.Artist_Id = Artist.Id)
                                                     LEFT OUTER JOIN Job ON (Music_Artist_Job.Job_Id = Job.Id)
                                                     INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                     WHERE Music.IsDeleted<>'false' 
                                                     AND Music.Id='{2}';", ConstMusicThumb, ConstMusicThumbArtistPart, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = MusicToThumb(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetThumbMusic()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}{1}
                                                     FROM Music
                                                     LEFT OUTER JOIN Music_Artist_Job ON (Music.Id=Music_Artist_Job.Music_Id)
                                                     LEFT OUTER JOIN Artist ON (Music_Artist_Job.Artist_Id = Artist.Id)
                                                     LEFT OUTER JOIN Job ON (Music_Artist_Job.Job_Id = Job.Id)
                                                     INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                     WHERE Music.IsDeleted<>'false' ;", ConstMusicThumb, ConstMusicThumbArtistPart);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MusicToThumb(objResults));


            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItems GetBigThumbMusics()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT Music.Id, Title, BigCover, AddedDate, IsDeleted, Rating, Comments, ToBeDeleted, Media.Name,
                                                             IsWhish, IsComplete, IsHear, ToHear, FileName, FilePath, 'None'
                                                      FROM 
                                                            (SELECT Music_Id as MusicId, Ressource as BigCover
                                                             FROM Music_Ressources
                                                             WHERE IsDefault=1
                                                             ), Music
                                                       INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                        WHERE MusicId=Music.Id;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MusicToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public IList GetThumbMusicByTypes()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT Distinct {0}{1}{2}
                                                     FROM Music
                                                     LEFT OUTER JOIN Music_Artist_Job ON (Music.Id=Music_Artist_Job.Music_Id)
                                                     LEFT OUTER JOIN Artist ON (Music_Artist_Job.Artist_Id = Artist.Id)
                                                     LEFT OUTER JOIN Job ON (Music_Artist_Job.Job_Id = Job.Id)
                                                     INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                     INNER JOIN Music_Genre ON Music_Genre.Id = Music_MusicGenre.MusicGenre_Id
                                                     INNER JOIN Music_MusicGenre ON Music_MusicGenre.Music_Id = Music.Id
                                                     WHERE Music.IsDeleted<>'false'
                                                     AND Music_Genre.DisplayName <>'' ;", ConstMusicThumb, ConstMusicThumbArtistPart, ConstMusicThumbTypePart);


            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MusicToThumb(objResults));

            objResults.Close();

            objCommand.CommandText = string.Format(@"SELECT {0} {1}
                                       FROM Music
                                       LEFT OUTER JOIN Music_Artist_Job ON (Music.Id=Music_Artist_Job.Music_Id)
                                       LEFT OUTER JOIN Artist ON (Music_Artist_Job.Artist_Id = Artist.Id)
                                       LEFT OUTER JOIN Job ON (Music_Artist_Job.Job_Id = Job.Id)
                                       INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                       WHERE Music.IsDeleted<>'false'
                                       AND NOT EXISTS (SELECT * 
	                                                   FROM [Music_MusicGenre]
	                                                   WHERE [Music].[Id] = [Music_MusicGenre].[Music_Id]) ;", ConstMusicThumb, ConstMusicThumbArtistPart);


            objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(MusicToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public IList GetNoSmallCoverMusic()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Music> items = new List<Music>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Music
                                                     INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                     WHERE Music.Cover IS NULL;", ConstMusic);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMusic(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        private ThumbItem MusicToThumb(SQLiteDataReader reader)
        {

            string fullPath = string.Empty;
            if (reader.IsDBNull(14) == false && reader.IsDBNull(13) == false)
                if (string.IsNullOrWhiteSpace(reader.GetString(14)) == false &&
                    string.IsNullOrWhiteSpace(reader.GetString(13)) == false)
                    fullPath = Path.Combine(reader.GetString(14), reader.GetString(13));

            string description = string.Empty;
            if (reader.IsDBNull(6) == false)
                description = reader.GetString(6);

            byte[] cover = null;
            if (reader.IsDBNull(2) == false)
                cover = (byte[])reader.GetValue(2);

            bool tested = false;
            if (reader.IsDBNull(11) == false)
                tested = reader.GetBoolean(11);

            int? rating = null;
            if (reader.IsDBNull(5) == false)
                rating = reader.GetInt32(5);

            double? publicRating = null;
            if (reader.IsDBNull(16) == false)
                publicRating = reader.GetDouble(16);

            int? numId = null;
            if (reader.IsDBNull(17) == false)
                numId = reader.GetInt32(17);

            string firstName = string.Empty;
            if (reader.IsDBNull(18) == false)
                firstName = reader.GetString(18);

            string lastName = string.Empty;
            if (reader.IsDBNull(19) == false)
                lastName = reader.GetString(19);

            string artist = firstName + " " + lastName;

            string album = string.Empty;
            if (reader.IsDBNull(20) == false)
                album = reader.GetString(20);

            //FIX 2.8.10.0
            string type;
            if (reader.IsDBNull(21) == false)
                type = reader.GetString(21);
            else
                type = reader.GetString(15);

            return new ThumbItem(reader.GetString(1), reader.GetString(0), cover, reader.GetDateTime(3),
                                    null, string.Empty, reader.GetBoolean(4),
                                    EntityType.Music, rating, publicRating, description,
                                    reader.GetBoolean(7), reader.GetString(8), reader.GetBoolean(9),
                                    reader.GetBoolean(10), tested, type,
                                    album, artist, reader.GetBoolean(12), fullPath, numId);
        }

        public int PurgeMusic()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();


            List<Music> items = new List<Music>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Music
                                                     INNER JOIN Media ON (Music.Media_Id = Media.Id)
                                                     WHERE Music.IsDeleted='true';", ConstMusic);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToMusic(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            foreach (Music item in items)
                PurgeMusic(item);

            return items.Count;
        }
        public void PurgeMusic(Music item)
        {
            DeleteById("Music_Ressources", "Music_Id", item.Id);
            DeleteById("Music_Links", "Music_Id", item.Id);
            DeleteById("Music", "Id", item.Id);
            if (item.Publisher != null)
                if (string.IsNullOrWhiteSpace(item.Publisher.Id) == false)
                    DeleteOrphelans("Music_Studio", item.Publisher.Id, "Music", "Studio_Id");
        }

        private Music ResultsToMusic(SQLiteDataReader reader, bool getLastId = true)
        {
            Music item = new Music();

            item.Id = reader.GetString(0);
            item.Title = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.BarCode = reader.GetString(2);

            item.Media = new Media();
            item.Media.Id = reader.GetString(3);
            item.Media.Name = reader.GetString(23);

            if (reader.IsDBNull(4) == false)
            {
                item.Publisher = new Publisher();
                item.Publisher.Id = reader.GetString(4);
            }

            if (reader.IsDBNull(6) == false)
            {
                item.FileFormat = new FileFormat();
                item.FileFormat.Id = reader.GetString(6);
            }

            if (reader.IsDBNull(7) == false)
                item.MyRating = reader.GetInt32(7);

            if (reader.IsDBNull(8) == false)
                item.Album = reader.GetString(8);

            if (reader.IsDBNull(9) == false)
                item.Runtime = reader.GetInt32(9);

            if (reader.IsDBNull(10) == false)
                item.BitRate = reader.GetInt32(10);

            if (reader.IsDBNull(11) == false)
                item.AddedDate = reader.GetDateTime(11);

            if (reader.IsDBNull(12) == false)
                item.IsDeleted = reader.GetBoolean(12);

            if (reader.IsDBNull(13) == false)
                item.IsWhish = reader.GetBoolean(13);

            if (reader.IsDBNull(14) == false)
                item.IsComplete = reader.GetBoolean(14);

            if (reader.IsDBNull(15) == false)
                item.Watched = reader.GetBoolean(15);

            if (reader.IsDBNull(16) == false)
                item.ReleaseDate = reader.GetDateTime(16);

            if (reader.IsDBNull(17) == false)
                item.FileName = reader.GetString(17);

            if (reader.IsDBNull(18) == false)
                item.FilePath = reader.GetString(18);

            if (reader.IsDBNull(19) == false)
                item.ToBeDeleted = reader.GetBoolean(19);

            if (reader.IsDBNull(20) == false)
                item.Comments = reader.GetString(20);

            if (reader.IsDBNull(21) == false)
                item.Cover = (byte[])reader.GetValue(21);

            if (reader.IsDBNull(22) == false)
                item.ToWatch = reader.GetBoolean(22);

            if (reader.IsDBNull(24) == false)
                item.PublicRating = reader.GetDouble(24);

            if (reader.IsDBNull(25) == false)
                item.NumId = reader.GetInt32(25);
            else if (getLastId)
                item.NumId = GetLastCollectionNumber(EntityType.Music);

            return item;
        }

        #endregion
        #region Nds

        public void AddNds(Nds entity)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO Nds(Id, Title, BarCode, Editor_Id, Language_Id, Media_Id, Rating, Description, ReleaseDate, Comments,
                                                                          FileName, FilePath, AddedDate, IsDeleted, IsWhish, IsComplete, ToBeDeleted, IsTested, Rated,
                                                                          smallCover,ToTest, PublicRating,NumID)
                                                          VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,
                                                                 @param11,@param12,@param13,@param14,@param15,@param16,@param17,@param18,@param19,@param20,
                                                                 @param21,@param22,@param23)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Nds 
                                                        SET Title=@param2,BarCode=@param3,Editor_Id=@param4,Language_Id=@param5,Media_Id=@param6,Rating=@param7,
                                                            Description=@param8, ReleaseDate=@param9, Comments=@param10, FileName=@param11, FilePath=@param12, 
                                                            AddedDate=@param13, IsDeleted=@param14, IsWhish=@param15, IsComplete=@param16, ToBeDeleted=@param17, 
                                                            IsTested=@param18, Rated=@param19, smallCover=@param20, ToTest=@param21 ,PublicRating=@param22,
                                                            NumID=@param23
                                                         WHERE Id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = entity.Title;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = entity.BarCode;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            if (entity.Publisher != null)
                param4.Value = entity.Publisher.Id;
            else
                param4.Value = DBNull.Value;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
            if (entity.Language != null)
                param5.Value = entity.Language.Id;
            else
                param5.Value = DBNull.Value;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = entity.Media.Id;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.Int32);
            param7.Value = entity.MyRating;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
            param8.Value = entity.Description;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.DateTime);
            param9.Value = entity.ReleaseDate;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.String);
            param10.Value = entity.Comments;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.String);
            param11.Value = entity.FileName;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.String);
            param12.Value = entity.FilePath;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.DateTime);
            param13.Value = entity.AddedDate;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Boolean);
            param14.Value = entity.IsDeleted;

            SQLiteParameter param15 = new SQLiteParameter("@param15", DbType.Boolean);
            param15.Value = entity.IsWhish;

            SQLiteParameter param16 = new SQLiteParameter("@param16", DbType.Boolean);
            param16.Value = entity.IsComplete;

            SQLiteParameter param17 = new SQLiteParameter("@param17", DbType.Boolean);
            param17.Value = entity.ToBeDeleted;

            SQLiteParameter param18 = new SQLiteParameter("@param18", DbType.Boolean);
            param18.Value = entity.Watched;

            SQLiteParameter param19 = new SQLiteParameter("@param19", DbType.String);
            param19.Value = entity.Rated;

            SQLiteParameter param20 = new SQLiteParameter("@param20", DbType.Binary);
            param20.Value = entity.Cover;

            SQLiteParameter param21 = new SQLiteParameter("@param21", DbType.Boolean);
            param21.Value = entity.ToWatch;

            SQLiteParameter param22 = new SQLiteParameter("@param22", DbType.Double);
            param22.Value = entity.PublicRating;

            SQLiteParameter param23 = new SQLiteParameter("@param23", DbType.Int32);
            param23.Value = entity.NumId;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);
            objCommand.Parameters.Add(param15);
            objCommand.Parameters.Add(param16);
            objCommand.Parameters.Add(param17);
            objCommand.Parameters.Add(param18);
            objCommand.Parameters.Add(param19);
            objCommand.Parameters.Add(param20);
            objCommand.Parameters.Add(param21);
            objCommand.Parameters.Add(param22);
            objCommand.Parameters.Add(param23);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public void GetChild(Nds items)
        {
            items.Links = GetLinks("Nds_Links", "Nds_ID", items.Id);
            items.Ressources = GetRessources("Nds_Ressources", "Nds_Id", items.Id);
            if (items.Publisher != null)
                items.Publisher = GetPublisher(items.Publisher.Id, "App_Editor", "Id");
            items.Genres = GetGenres("GamezType", "Nds_GamezType", "Nds_Id", items.Id, "GamezType_Id");
            if (items.Language != null)
                items.Language = GetLanguageById(items.Language.Id);
        }
        public Nds GetFirstNds()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Nds items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Nds
                                                      INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                      WHERE Nds.IsDeleted<>'false' 
                                                      LIMIT 1;", ConstNds);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToNds(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items);

            return items;
        }
        public IList GetNdss()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Nds> items = new List<Nds>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Nds
                                                     INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                     WHERE Nds.IsDeleted <> 'false';", ConstNds);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToNds(objResults, false));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public Nds GetNdss(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Nds items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Nds
                                                      INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                      WHERE Nds.IsDeleted<>'false' 
                                                      AND Nds.Id='{1}';", ConstNds, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToNds(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items);

            return items;
        }
        public Nds GetNdss(string mediaName, string filePath, string fileName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            Nds items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Nds
                                                      INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                      WHERE Nds.IsDeleted<>'false' 
                                                      AND Media.Name = @param1 AND  Nds.FileName = @param2 AND  Nds.FilePath =  @param3;", ConstNds);
            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = fileName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = filePath;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToNds(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items);

            return items;
        }
        public IList GetNdsByMedia(string mediaName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Nds> items = new List<Nds>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM  Nds
                                                      INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                      WHERE Nds.IsDeleted=0 
                                                      AND Media.Name = ""{1}"";", ConstNds, mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToNds(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public int GetNdsCountByType(string strGenre)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int number = 0;
            if (strGenre == string.Empty)
                return number;
            else
            {
                objCommand.CommandText = string.Format(@"SELECT Count(Nds.Title)
                                                        FROM Nds_GamezType
                                                        INNER JOIN Nds ON (Nds_GamezType.Nds_Id = Nds.Id)
                                                        INNER JOIN GamezType ON (Nds_GamezType.GamezType_Id = GamezType.Id)
                                                        WHERE GamezType.DisplayName = ""{0}"";", strGenre);

                SQLiteDataReader objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                {
                    objResults.Read();
                    number = objResults.GetInt32(0);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            return number;
        }
        public IList GetDupeNds()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM
                                                        (Select Nds.Title
                                                        From Nds
                                                        Group By Nds.Title
                                                        Having (Count(Nds.Title) > 1)) AS GRP
                                                      INNER JOIN Nds ON Nds.TITLE = GRP.TITLE  
                                                      INNER JOIN Media ON (Nds.Media_Id = Media.Id);", ConstNdsThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(XxxToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItem GetThumbNds(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItem items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM Nds
                                                     INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                     WHERE Nds.IsDeleted<>'false' 
                                                     AND Nds.Id='{1}';", ConstNdsThumb, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = NdsToThumb(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetThumbNds()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM Nds
                                                     INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                     WHERE Nds.IsDeleted<>'false'", ConstNdsThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(NdsToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItems GetBigThumbNds()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT Nds.Id, Title, BigCover, AddedDate, IsDeleted, Rating, Description, ToBeDeleted, Media.Name,
                                                            IsWhish, IsComplete, IsTested, ToTest, FileName, FilePath, 'None'
                                                      FROM 
                                                            (SELECT Nds_Id as NdsId, Ressource as BigCover
                                                             FROM Nds_Ressources
                                                             WHERE IsDefault=1
                                                             ), Nds
                                                       INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                        WHERE NdsId=Nds.Id;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(NdsToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public IList GetThumbNdsByTypes()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = @"SELECT Distinct Nds.Id, Title, smallCover, AddedDate, IsDeleted, Rating, Description, ToBeDeleted, Media.Name,
                                              IsWhish, IsComplete, IsTested, ToTest, FileName, FilePath, GamezType.DisplayName
                                       FROM Nds
                                       INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                       INNER JOIN GamezType ON GamezType.Id = Nds_GamezType.GamezType_Id
                                       INNER JOIN Nds_GamezType ON Nds_GamezType.Nds_Id = Nds.Id
                                       WHERE Nds.IsDeleted<>'false'
                                       AND GamezType.DisplayName <>'' ;";


            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(NdsToThumb(objResults));

            objResults.Close();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                       FROM Nds
                                       INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                       WHERE Nds.IsDeleted<>'false'
                                       AND NOT EXISTS (SELECT * 
	                                                   FROM [Nds_GamezType]
	                                                   WHERE [Nds].[Id] = [Nds_GamezType].[Nds_Id]) ;", ConstNdsThumb);


            objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(NdsToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetNoSmallCoverNds()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Nds> items = new List<Nds>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Nds
                                                     INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                     WHERE Nds.Cover IS NULL;", ConstNds);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToNds(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        private ThumbItem NdsToThumb(SQLiteDataReader reader)
        {

            string fullPath = string.Empty;
            if (reader.IsDBNull(14) == false && reader.IsDBNull(13) == false)
                if (string.IsNullOrWhiteSpace(reader.GetString(14)) == false &&
                    string.IsNullOrWhiteSpace(reader.GetString(13)) == false)
                    fullPath = Path.Combine(reader.GetString(14), reader.GetString(13));

            string description = string.Empty;
            if (reader.IsDBNull(6) == false)
                description = reader.GetString(6);

            byte[] cover = null;
            if (reader.IsDBNull(2) == false)
                cover = (byte[])reader.GetValue(2);

            bool tested = false;
            if (reader.IsDBNull(11) == false)
                tested = reader.GetBoolean(11);

            int? rating = null;
            if (reader.IsDBNull(5) == false)
                rating = reader.GetInt32(5);

            double? publicRating = null;
            if (reader.IsDBNull(16) == false)
                publicRating = reader.GetDouble(16);

            int? numId = null;
            if (reader.IsDBNull(17) == false)
                numId = reader.GetInt32(17);

            return new ThumbItem(reader.GetString(1), reader.GetString(0), cover, reader.GetDateTime(3),
                                    null, string.Empty, reader.GetBoolean(4),
                                    EntityType.Nds, rating, publicRating, description,
                                    reader.GetBoolean(7), reader.GetString(8), reader.GetBoolean(9),
                                    reader.GetBoolean(10), tested, reader.GetString(15),
                                    string.Empty, string.Empty, reader.GetBoolean(12), fullPath, numId);
        }

        public int PurgeNds()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<Nds> items = new List<Nds>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  Nds
                                                     INNER JOIN Media ON (Nds.Media_Id = Media.Id)
                                                     WHERE Nds.IsDeleted='true';", ConstNds);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToNds(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            foreach (Nds item in items)
                PurgeNds(item);

            return items.Count;
        }
        public void PurgeNds(Nds item)
        {
            DeleteById("Nds_Ressources", "Nds_Id", item.Id);
            DeleteById("Nds_Links", "Nds_ID", item.Id);
            DeleteById("Nds", "Id", item.Id);
            if (item.Publisher != null)
                if (string.IsNullOrWhiteSpace(item.Publisher.Id) == false)
                    DeleteOrphelans("App_Editor", item.Publisher.Id, "Nds", "Editor_Id");
        }
        public void PurgeNdsType()
        {
            //TODO
            //var query = from items in _objEntities.Nds_GamezType
            //            group items by new { items.Nds_Id, items.GamezType_Id } into g
            //            where g.Count() > 1
            //            select g.Key;

            //foreach (var item in query)
            //{
            //    var item1 = item;
            //    Nds_GamezType duplicates = (from items in _objEntities.Nds_GamezType
            //                                where items.Nds_Id == item1.Nds_Id && items.GamezType_Id == item1.GamezType_Id
            //                                select items).First();

            //    _objEntities.DeleteObject(duplicates);
            //}

            //_objEntities.SaveChanges();
        }
        private Nds ResultsToNds(SQLiteDataReader reader, bool getLastId = true)
        {
            Nds item = new Nds();

            item.Id = reader.GetString(0);
            item.Title = reader.GetString(1);

            if (reader.IsDBNull(2) == false)
                item.BarCode = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
            {
                item.Publisher = new Publisher();
                item.Publisher.Id = reader.GetString(3);
            }

            if (reader.IsDBNull(4) == false)
            {
                item.Language = new Language();
                item.Language.Id = reader.GetString(4);
            }

            item.Media = new Media();
            item.Media.Id = reader.GetString(5);
            item.Media.Name = reader.GetString(21);

            if (reader.IsDBNull(6) == false)
                item.MyRating = reader.GetInt32(6);

            if (reader.IsDBNull(7) == false)
                item.Description = reader.GetString(7);

            if (reader.IsDBNull(8) == false)
                item.ReleaseDate = reader.GetDateTime(8);

            if (reader.IsDBNull(9) == false)
                item.Comments = reader.GetString(9);

            if (reader.IsDBNull(10) == false)
                item.FileName = reader.GetString(10);

            if (reader.IsDBNull(11) == false)
                item.FilePath = reader.GetString(11);

            if (reader.IsDBNull(12) == false)
                item.AddedDate = reader.GetDateTime(12);

            if (reader.IsDBNull(13) == false)
                item.IsDeleted = reader.GetBoolean(13);

            if (reader.IsDBNull(14) == false)
                item.IsWhish = reader.GetBoolean(14);

            if (reader.IsDBNull(15) == false)
                item.IsComplete = reader.GetBoolean(15);

            if (reader.IsDBNull(16) == false)
                item.ToBeDeleted = reader.GetBoolean(16);

            if (reader.IsDBNull(17) == false)
                item.Watched = reader.GetBoolean(17);

            if (reader.IsDBNull(18) == false)
                item.Rated = reader.GetString(18);

            if (reader.IsDBNull(19) == false)
                item.Cover = (byte[])reader.GetValue(19);

            if (reader.IsDBNull(20) == false)
                item.ToWatch = reader.GetBoolean(20);

            if (reader.IsDBNull(22) == false)
                item.PublicRating = reader.GetDouble(22);

            if (reader.IsDBNull(23) == false)
                item.NumId = reader.GetInt32(23);
            else if (getLastId)
                item.NumId = GetLastCollectionNumber(EntityType.Nds);

            return item;
        }

        #endregion
        #region Series

        public void AddSeriesSeason(SeriesSeason entity)
        {

            #region FileFormat
            if (entity.FileFormat != null && string.IsNullOrWhiteSpace(entity.FileFormat.Id))
                AddFileFormat(entity.FileFormat);
            #endregion
            #region AspectRatio
            if (entity.AspectRatio != null && string.IsNullOrWhiteSpace(entity.AspectRatio.Id))
                AddAspectRatio(entity.AspectRatio);
            #endregion

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(entity.SerieId))
            {
                entity.SerieId = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO Series(Id, Studio_Id, Title, IsInProduction, Country, Description, OfficialWebSite, RunTime, Rated)
                                                            VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Series 
                                                        SET Studio_Id=@param2,Title=@param3,IsInProduction=@param4,Country=@param5,Description=@param6,OfficialWebSite=@param7,
                                                            RunTime=@param8, Rated=@param9
                                                         WHERE id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.SerieId;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            if (entity.Publisher != null)
                param2.Value = entity.Publisher.Id;
            else
                param2.Value = DBNull.Value;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = entity.Title;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.Boolean);
            param4.Value = entity.IsInProduction;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
            param5.Value = entity.Country;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = entity.Description;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.String);
            param7.Value = entity.OfficialWebSite;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.Int32);
            param8.Value = entity.Runtime;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.String);
            param9.Value = entity.Rated;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO Series_Season(Id, Series_Id, Media_Id, Season, BarCode, TotalEpisodes, AvailableEpisodes, MissingEpisodes, Seen, AddedDate, IsComplete,
                                                                                    FilesPath, IsWhish, ToBeDeleted, Rating, ReleaseDate, Comments, smallCover, 
                                                                                    ToWatch, PublicRating,NumID)
                                                          VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,@param12,@param13,@param14,
                                                                 @param15,@param16,@param17,@param18,@param19,@param20,@param21)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE Series_Season 
                                                        SET Series_Id=@param2,Media_Id=@param3,Season=@param4,BarCode=@param5,TotalEpisodes=@param6,AvailableEpisodes=@param7,
                                                            MissingEpisodes=@param8, Seen=@param9, AddedDate=@param10, IsComplete=@param11, FilesPath=@param12, 
                                                            IsWhish=@param13, ToBeDeleted=@param14, Rating=@param15, ReleaseDate=@param16, 
                                                            Comments=@param17, smallCover=@param18, ToWatch=@param19,PublicRating=@param20,NumID=@param21
                                                         WHERE id=@param1");

            param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.Id;

            param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = entity.SerieId;

            param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = entity.Media.Id;

            param4 = new SQLiteParameter("@param4", DbType.Int32);
            param4.Value = entity.Season;

            param5 = new SQLiteParameter("@param5", DbType.String);
            param5.Value = entity.BarCode;

            param6 = new SQLiteParameter("@param6", DbType.Int32);
            param6.Value = entity.TotalEpisodes;

            param7 = new SQLiteParameter("@param7", DbType.Int32);
            param7.Value = entity.AvailableEpisodes;

            param8 = new SQLiteParameter("@param8", DbType.Int32);
            param8.Value = entity.MissingEpisodes;

            param9 = new SQLiteParameter("@param9", DbType.Boolean);
            param9.Value = entity.Watched;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.DateTime);
            param10.Value = entity.AddedDate;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.Boolean);
            param11.Value = entity.IsComplete;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.String);
            param12.Value = entity.FilePath;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.Boolean);
            param13.Value = entity.IsWhish;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Boolean);
            param14.Value = entity.ToBeDeleted;

            SQLiteParameter param15 = new SQLiteParameter("@param15", DbType.Int32);
            param15.Value = entity.MyRating;

            SQLiteParameter param16 = new SQLiteParameter("@param16", DbType.DateTime);
            param16.Value = entity.ReleaseDate;

            SQLiteParameter param17 = new SQLiteParameter("@param17", DbType.String);
            param17.Value = entity.Comments;

            SQLiteParameter param18 = new SQLiteParameter("@param18", DbType.Binary);
            param18.Value = entity.Cover;

            SQLiteParameter param19 = new SQLiteParameter("@param19", DbType.Boolean);
            param19.Value = entity.ToWatch;

            SQLiteParameter param20 = new SQLiteParameter("@param20", DbType.Double);
            param20.Value = entity.PublicRating;

            SQLiteParameter param21 = new SQLiteParameter("@param21", DbType.Int32);
            param21.Value = entity.NumId;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);
            objCommand.Parameters.Add(param15);
            objCommand.Parameters.Add(param16);
            objCommand.Parameters.Add(param17);
            objCommand.Parameters.Add(param18);
            objCommand.Parameters.Add(param19);
            objCommand.Parameters.Add(param20);
            objCommand.Parameters.Add(param21);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            #region SubTitle

            DeleteById("Movie_SubTitle", "Movie_Id", entity.Id);

            if (entity.Subtitles != null)
                foreach (Language item in entity.Subtitles)
                    AddSubTitle(item, entity.Id);
            #endregion
            #region Audios

            DeleteById("Movie_Audio", "Movie_Id", entity.Id);

            if (entity.Audios != null)
                foreach (Audio item in entity.Audios)
                    AddAudio(item, entity.Id);
            #endregion

        }

        public void GetChild(SeriesSeason items, bool getArtistCredits)
        {
            items.Links = GetLinks("Series_Links", "Series_Id", items.SerieId);
            items.Ressources = GetRessources("SeriesSeason_Ressources", "SeriesSeason_Id", items.Id);
            if (items.Publisher != null)
                items.Publisher = GetPublisher(items.Publisher.Id, "Movie_Studio", "Id");
            items.Artists = GetArtists(items.SerieId, items.ObjectType, getArtistCredits);
            items.Genres = GetGenres("Movie_Genre", "Series_MovieGenre", "Series_Id", items.SerieId, "Genre_Id");
            items.Audios = GetAudios(items.Id);
            items.Subtitles = GetSubTitle(items.Id);
            items.MetaDatas = GetMetaDatas(items.Id);

            if (items.FileFormat != null)
                items.FileFormat = GetFileFormatById(items.FileFormat.Id);

            if (items.AspectRatio != null)
                items.AspectRatio = GetAspectRatioById(items.AspectRatio.Id);

        }
        public IList GetDupeSeries()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM
                                                        (Select Series.Title
                                                        From Series
                                                        INNER JOIN Series_Season ON Series.Id = Series_Season.Series_Id  
                                                        Group By Series.Title,Series_Season.Season
                                                        Having (Count(Series.Title) > 1)) AS GRP
                                                        INNER JOIN Series ON Series.TITLE = GRP.TITLE
                                                        INNER JOIN Series_Season ON Series.Id = Series_Season.Series_Id  
                                                        INNER JOIN Media ON (Series_Season.Media_Id = Media.Id);", ConstSerieThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(SeriesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public SeriesSeason GetFirstSeries()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            SeriesSeason items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Series_Season
                                                      INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                      INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                      WHERE Series_Season.IsDeleted<>'false' 
                                                      LIMIT 1;", ConstSerie);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToSeriesSeason(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetSeriesByMedia(string mediaName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<SeriesSeason> items = new List<SeriesSeason>();
            //FIX 2.8.5.0
            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Series_Season
                                                      INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                      INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                      WHERE Series_Season.IsDeleted='False' 
                                                      AND Media.Name = ""{1}"";", ConstSerie, mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToSeriesSeason(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public string GetSeriesIdByName(string title)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            string id = string.Empty;

            objCommand.CommandText = string.Format(@"SELECT Series.Id
                                                        FROM Series
                                                        WHERE Series.Title = @param1;");

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = title;

            objCommand.Parameters.Add(param1);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                objResults.Read();
                id = objResults.GetString(0);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return id;

        }
        public int GetSerieCountByType(string strGenre)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int number = 0;
            if (strGenre == string.Empty)
                return number;
            else
            {
                objCommand.CommandText = string.Format(@"SELECT Count(Series.Title)
                                                        FROM Series_MovieGenre
                                                        INNER JOIN Series ON (Series_MovieGenre.Series_Id = Series.Id)
                                                        INNER JOIN Movie_Genre ON (Series_MovieGenre.Genre_Id = Movie_Genre.Id)
                                                        WHERE Movie_Genre.DisplayName = ""{0}"";", strGenre);

                SQLiteDataReader objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                {
                    objResults.Read();
                    number = objResults.GetInt32(0);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            return number;
        }
        public IList getSeries_Seasons()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<SeriesSeason> items = new List<SeriesSeason>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Series_Season
                                                      INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                      INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                      WHERE Series_Season.IsDeleted<>'false';", ConstSerie);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToSeriesSeason(objResults, false));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public SeriesSeason GetSeries_Seasons(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            SeriesSeason items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Series_Season
                                                      INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                      INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                       WHERE (Series_Season.IsDeleted<>'false' OR Series_Season.IsDeleted is NULL) 
                                                      AND Series_Season.Id='{1}';", ConstSerie, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToSeriesSeason(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetSeries_SeasonsBySerieID(string serieId)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<SeriesSeason> items = new List<SeriesSeason>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Series_Season
                                                      INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                      INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                      WHERE (Series_Season.IsDeleted<>'false' OR Series_Season.IsDeleted is NULL)
                                                      AND Series_Season.Series_Id='{1}';", ConstSerie, serieId);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(ResultsToSeriesSeason(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public SeriesSeason GetSeriesSeason(string mediaName, string filePath, long season)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            SeriesSeason items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM Series_Season
                                                      INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                      INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                       WHERE (Series_Season.IsDeleted<>'false' OR Series_Season.IsDeleted is NULL)
                                                      AND Media.Name = @param1 AND Series_Season.Season = @param2 AND  Series_Season.FilesPath =@param3;", ConstSerie);

            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = season;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = filePath;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToSeriesSeason(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public ThumbItem getThumbSeries_Season(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItem items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                    FROM Series_Season
                                                    INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                    INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                    WHERE (Series_Season.IsDeleted<>'false' OR Series_Season.IsDeleted isNull)
                                                    AND Series_Season.Id='{1}';", ConstSerieThumb, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = SeriesToThumb(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public IList getThumbSeries_Seasons()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                    FROM Series_Season
                                                    INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                    INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                    WHERE Series_Season.IsDeleted<>'false' OR Series_Season.IsDeleted isNull ;", ConstSerieThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(SeriesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public ThumbItems GetBigThumbSeries_Seasons()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT Series_Season.Id, Series.Title, BigCover, AddedDate, Series.Runtime, IsDeleted, Rating, Series.Description, TobeDeleted, Media.Name, 
                                                            IsWhish, IsComplete,Seen, ToWatch, FilesPath, 'None',Series_Season.Season
                                                      FROM 
                                                            (SELECT SeriesSeason_Id as SeriesSeasonId, Ressource as BigCover
                                                             FROM SeriesSeason_Ressources
                                                             WHERE IsDefault=1
                                                             ), Series_Season
                                                       INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                       INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                       WHERE SeriesSeasonId=Series_Season.Id;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(SeriesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetThumbSeriesByTypes()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = @"SELECT Distinct Series_Season.Id, Series.Title, smallCover, AddedDate, Series.Runtime, IsDeleted, 
                                              Rating, Series.Description, TobeDeleted, Media.Name, 
                                              IsWhish, IsComplete,Seen, ToWatch, FilesPath,  Movie_Genre.DisplayName,Series_Season.Season 
                                       FROM Movie_Genre
                                       INNER JOIN Series_Season ON (Series_Season.Series_Id = Series.Id)
                                       INNER JOIN Series_MovieGenre ON Movie_Genre.Id = Series_MovieGenre.Genre_Id
                                       INNER JOIN Series ON Series_MovieGenre.Series_Id = Series.Id
                                       INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                       WHERE (Series_Season.IsDeleted<>'false' OR Series_Season.IsDeleted isNull)
                                       AND Movie_Genre.DisplayName <>'';";


            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(SeriesToThumb(objResults));

            objResults.Close();

            objCommand.CommandText = @"SELECT Series_Season.Id, Series.Title, smallCover, AddedDate, Series.Runtime, IsDeleted, 
                                              Rating, Series.Description, TobeDeleted, Media.Name, 
                                              IsWhish, IsComplete,Seen, ToWatch, FilesPath,  'None',Series_Season.Season 
                                       FROM Series_Season
                                       INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                       INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                       WHERE (Series_Season.IsDeleted<>'false' OR Series_Season.IsDeleted isNull)
                                       AND NOT EXISTS (SELECT * 
	                                                   FROM [Series_MovieGenre]
	                                                   WHERE [Series].[Id] = [Series_MovieGenre].[Series_Id]) ;";


            objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(SeriesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public IList GetThumbSeriesByArtist()
        {

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<ThumbItem> thumbItems = new List<ThumbItem>();
            //Fixed 2.7.12.0
            objCommand.CommandText = string.Format(@"SELECT Series_Season.Id, Series.Title, Series_Season.smallCover, Series_Season.AddedDate, Series.Runtime, Series_Season.IsDeleted, 
                                                            Series_Season.Rating, Series.Description, Series_Season.TobeDeleted, Media.Name, 
                                                            Series_Season.IsWhish, Series_Season.IsComplete,Series_Season.Seen, Series_Season.ToWatch, Series_Season.FilesPath, 'None',Series_Season.Season, Artist.FirstName, Artist.LastName, Job.Name 
                                                          FROM Series_Season
                                                          LEFT OUTER JOIN Series_Artist_Job ON (Series.Id=Series_Artist_Job.Series_Id)
                                                          LEFT OUTER JOIN Artist ON (Series_Artist_Job.Artist_Id = Artist.Id)
                                                          LEFT OUTER JOIN Job ON (Series_Artist_Job.Job_Id = Job.Id)
                                                          INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                          INNER JOIN Media ON (Series_Season.Media_Id = Media.Id);");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    thumbItems.Add(SeriesToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
            return thumbItems;

        }
        public IList GetNoSmallCoverSeries()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<SeriesSeason> items = new List<SeriesSeason>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM Series_Season
                                                     INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                     INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                     WHERE Series_Season.smallCover IS NULL;", ConstSerie);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToSeriesSeason(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public int PurgeSeries()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<SeriesSeason> items = new List<SeriesSeason>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM Series_Season
                                                     INNER JOIN Series ON (Series_Season.Series_Id = Series.Id)
                                                     INNER JOIN Media ON (Series_Season.Media_Id = Media.Id)
                                                     WHERE Series_Season.IsDeleted='true';", ConstSerie);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToSeriesSeason(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            foreach (SeriesSeason item in items)
                PurgeSeries(item.Id);

            return items.Count;

        }
        public void PurgeSeries(string id)
        {
            DeleteById("SeriesSeason_Ressources", "SeriesSeason_Id", id);
            DeleteById("Series_Links", "Series_Id",id);
            DeleteById("Series_Season", "Id", id);

            //REmoved to increase performance
            //if (item.Publisher != null)
            //    if (string.IsNullOrWhiteSpace(item.Publisher.Id) == false)
            //        DeleteOrphelans("Movie_Studio", item.Publisher.Id, "Series", "Studio_Id");

        }
        //TODO
        public void PurgeSeriesType()
        {

            //var query = from items in _objEntities.Series_MovieGenre
            //            group items by new { items.Series_Id, items.Genre_Id } into g
            //            where g.Count() > 1
            //            select g.Key;

            //foreach (var item in query)
            //{
            //    var item1 = item;
            //    Series_MovieGenre duplicates = (from items in _objEntities.Series_MovieGenre
            //                                    where items.Series_Id == item1.Series_Id && items.Genre_Id == item1.Genre_Id
            //                                    select items).FirstOrDefault();

            //    if (duplicates != null)
            //        _objEntities.DeleteObject(duplicates);
            //}

            //_objEntities.SaveChanges();
        }

        private SeriesSeason ResultsToSeriesSeason(SQLiteDataReader reader, bool getLastId = true)
        {
            SeriesSeason item = new SeriesSeason();
            item.ObjectType = EntityType.Series;
            item.SerieId = reader.GetString(0);

            if (reader.IsDBNull(1) == false)
            {
                item.Publisher = new Publisher();
                item.Publisher.Id = reader.GetString(1);
            }

            item.Title = reader.GetString(2);

            if (reader.IsDBNull(3) == false)
                item.IsInProduction = reader.GetBoolean(3);

            if (reader.IsDBNull(4) == false)
                item.Country = reader.GetString(4);

            if (reader.IsDBNull(5) == false)
                item.Description = reader.GetString(5);

            if (reader.IsDBNull(6) == false)
                item.OfficialWebSite = reader.GetString(6);

            if (reader.IsDBNull(7) == false)
                item.Runtime = reader.GetInt32(7);

            if (reader.IsDBNull(8) == false)
                item.Rated = reader.GetString(8);

            item.Id = reader.GetString(9);

            item.Media = new Media();
            item.Media.Id = reader.GetString(11);
            item.Media.Name = reader.GetString(29);

            item.Season = reader.GetInt32(12);

            if (reader.IsDBNull(13) == false)
                item.BarCode = reader.GetString(13);

            if (reader.IsDBNull(14) == false)
                item.TotalEpisodes = reader.GetInt32(14);

            if (reader.IsDBNull(15) == false)
                item.AvailableEpisodes = reader.GetInt32(15);

            if (reader.IsDBNull(16) == false)
                item.MissingEpisodes = reader.GetInt32(16);

            if (reader.IsDBNull(17) == false)
                item.Watched = reader.GetBoolean(17);

            if (reader.IsDBNull(18) == false)
                item.AddedDate = reader.GetDateTime(18);

            if (reader.IsDBNull(19) == false)
                item.IsComplete = reader.GetBoolean(19);

            if (reader.IsDBNull(20) == false)
                item.FilePath = reader.GetString(20);

            if (reader.IsDBNull(21) == false)
                item.IsDeleted = (bool)reader.GetValue(21);

            if (reader.IsDBNull(22) == false)
                item.IsWhish = reader.GetBoolean(22);

            if (reader.IsDBNull(23) == false)
                item.ToBeDeleted = reader.GetBoolean(23);

            if (reader.IsDBNull(24) == false)
                item.MyRating = reader.GetInt32(24);

            if (reader.IsDBNull(25) == false)
                item.ReleaseDate = reader.GetDateTime(25);

            if (reader.IsDBNull(26) == false)
                item.Comments = reader.GetString(26);

            if (reader.IsDBNull(27) == false)
                item.Cover = (byte[])reader.GetValue(27);

            if (reader.IsDBNull(28) == false)
                item.ToWatch = (bool)reader.GetValue(28);

            if (reader.IsDBNull(30) == false)
                item.PublicRating = reader.GetDouble(30);

            if (reader.IsDBNull(31) == false)
                item.NumId = reader.GetInt32(31);
            else if (getLastId)
                item.NumId = GetLastCollectionNumber(EntityType.Series);

            return item;
        }

        private ThumbItem SeriesToThumb(SQLiteDataReader reader)
        {

            DateTime addedDate;
            if (reader.IsDBNull(3) == true)
                addedDate = DateTime.Now;
            else
                addedDate = reader.GetDateTime(3);

            bool isDeleted = false;
            if (reader.IsDBNull(5) == false)
                isDeleted = (bool)reader.GetValue(5);

            bool toBuy = false;
            if (reader.IsDBNull(10) == false)
                toBuy = reader.GetBoolean(10);

            string fullPath = string.Empty;
            if (reader.IsDBNull(14) == false)
                fullPath = reader.GetString(14);

            string description = string.Empty;
            if (reader.IsDBNull(7) == false)
                description = reader.GetString(7);

            int? runtime = null;
            if (reader.IsDBNull(4) == false)
                runtime = reader.GetInt32(4);

            byte[] cover = null;
            if (reader.IsDBNull(2) == false)
                cover = (byte[])reader.GetValue(2);

            int? rating = null;
            if (reader.IsDBNull(6) == false)
                rating = reader.GetInt32(6);

            string title = reader.GetString(1) + " - s" + reader.GetInt32(16).ToString(CultureInfo.InvariantCulture);

            string mediaName = "None";
            if (reader.IsDBNull(9) == false)
                mediaName = reader.GetString(9);

            double? publicRating = null;
            if (reader.IsDBNull(17) == false)
                publicRating = reader.GetDouble(17);

            int? numId = null;
            if (reader.IsDBNull(18) == false)
                numId = reader.GetInt32(18);

            string firstName = string.Empty;
            if (reader.IsDBNull(19) == false)
                firstName = reader.GetString(19);

            string lastName = string.Empty;
            if (reader.IsDBNull(20) == false)
                lastName = reader.GetString(20);

            string artist = firstName + " " + lastName;

            ThumbItem thumb = new ThumbItem(title, reader.GetString(0), cover, addedDate,
                                    runtime, string.Empty, isDeleted,
                                    EntityType.Series, rating, publicRating, description,
                                    reader.GetBoolean(8), mediaName, toBuy,
                                    reader.GetBoolean(11), reader.GetBoolean(12), reader.GetString(15),
                                    string.Empty, artist, reader.GetBoolean(13), fullPath, numId);

            thumb.SerieName = reader.GetString(1);

            return thumb;
        }

        #endregion
        #region XXX
        public void AddXxx(XXX entity)
        {
            #region FileFormat
            if (entity.FileFormat != null && string.IsNullOrWhiteSpace(entity.FileFormat.Id))
                AddFileFormat(entity.FileFormat);
            #endregion
            #region AspectRatio
            if (entity.AspectRatio != null && string.IsNullOrWhiteSpace(entity.AspectRatio.Id))
                AddAspectRatio(entity.AspectRatio);
            #endregion

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
                objCommand.CommandText = string.Format(@"INSERT INTO XXX 
                                                        (Id,Media_Id,Language_Id, FileFormat_Id, Title, BarCode, Rating, Description, Runtime, 
                                                         Seen, ReleaseDate, Comments, IsDeleted, IsComplete, IsWhish, AddedDate, FileName, 
                                                         FilePath, ToBeDeleted, Studio_Id, Country, smallCover, ToWatch, PublicRating,NumID)
                                                     VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,
                                                            @param11,@param12,@param13,@param14,@param15,@param16,@param17,@param18,@param19,@param20,@param21,
                                                            @param22,@param23,@param24,@param25)");
            }
            else
                objCommand.CommandText = string.Format(@"UPDATE XXX 
                                                        SET Media_Id=@param2,Language_Id=@param3,FileFormat_Id=@param4,Title=@param5,BarCode=@param6,Rating=@param7,
                                                            Description=@param8, Runtime=@param9, Seen=@param10, ReleaseDate=@param11, Comments=@param12, 
                                                            IsDeleted=@param13, IsComplete=@param14, IsWhish=@param15, AddedDate=@param16, FileName=@param17, 
                                                            FilePath=@param18, ToBeDeleted=@param19, Studio_Id=@param20, Country=@param21, 
                                                            smallCover=@param22, ToWatch=@param23 ,PublicRating=@param24,NumID=@param25
                                                         WHERE id=@param1");

            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = entity.Id;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = entity.Media.Id;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            if (entity.Language != null)
                param3.Value = entity.Language.Id;
            else
                param3.Value = DBNull.Value;

            SQLiteParameter param4 = new SQLiteParameter("@param4", DbType.String);
            if (entity.FileFormat != null)
                param4.Value = entity.FileFormat.Id;
            else
                param4.Value = DBNull.Value;

            SQLiteParameter param5 = new SQLiteParameter("@param5", DbType.String);
            param5.Value = entity.Title;

            SQLiteParameter param6 = new SQLiteParameter("@param6", DbType.String);
            param6.Value = entity.BarCode;

            SQLiteParameter param7 = new SQLiteParameter("@param7", DbType.Int32);
            param7.Value = entity.MyRating;

            SQLiteParameter param8 = new SQLiteParameter("@param8", DbType.String);
            param8.Value = entity.Description;

            SQLiteParameter param9 = new SQLiteParameter("@param9", DbType.Int32);
            param9.Value = entity.Runtime;

            SQLiteParameter param10 = new SQLiteParameter("@param10", DbType.Boolean);
            param10.Value = entity.Watched;

            SQLiteParameter param11 = new SQLiteParameter("@param11", DbType.DateTime);
            param11.Value = entity.ReleaseDate;

            SQLiteParameter param12 = new SQLiteParameter("@param12", DbType.String);
            param12.Value = entity.Comments;

            SQLiteParameter param13 = new SQLiteParameter("@param13", DbType.Boolean);
            param13.Value = entity.IsDeleted;

            SQLiteParameter param14 = new SQLiteParameter("@param14", DbType.Boolean);
            param14.Value = entity.IsComplete;

            SQLiteParameter param15 = new SQLiteParameter("@param15", DbType.Boolean);
            param15.Value = entity.IsWhish;

            SQLiteParameter param16 = new SQLiteParameter("@param16", DbType.DateTime);
            param16.Value = entity.AddedDate;

            SQLiteParameter param17 = new SQLiteParameter("@param17", DbType.String);
            param17.Value = entity.FileName;

            SQLiteParameter param18 = new SQLiteParameter("@param18", DbType.String);
            param18.Value = entity.FilePath;

            SQLiteParameter param19 = new SQLiteParameter("@param19", DbType.Boolean);
            param19.Value = entity.ToBeDeleted;

            SQLiteParameter param20 = new SQLiteParameter("@param20", DbType.String);
            if (entity.Publisher != null)
                param20.Value = entity.Publisher.Id;
            else
                param20.Value = DBNull.Value;

            SQLiteParameter param21 = new SQLiteParameter("@param21", DbType.String);
            param21.Value = entity.Country;

            SQLiteParameter param22 = new SQLiteParameter("@param22", DbType.Binary);
            param22.Value = entity.Cover;

            SQLiteParameter param23 = new SQLiteParameter("@param23", DbType.Boolean);
            param23.Value = entity.ToWatch;

            SQLiteParameter param24 = new SQLiteParameter("@param24", DbType.Double);
            param24.Value = entity.PublicRating;

            SQLiteParameter param25 = new SQLiteParameter("@param25", DbType.Int32);
            param25.Value = entity.NumId;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);
            objCommand.Parameters.Add(param4);
            objCommand.Parameters.Add(param5);
            objCommand.Parameters.Add(param6);
            objCommand.Parameters.Add(param7);
            objCommand.Parameters.Add(param8);
            objCommand.Parameters.Add(param9);
            objCommand.Parameters.Add(param10);
            objCommand.Parameters.Add(param11);
            objCommand.Parameters.Add(param12);
            objCommand.Parameters.Add(param13);
            objCommand.Parameters.Add(param14);
            objCommand.Parameters.Add(param15);
            objCommand.Parameters.Add(param16);
            objCommand.Parameters.Add(param17);
            objCommand.Parameters.Add(param18);
            objCommand.Parameters.Add(param19);
            objCommand.Parameters.Add(param20);
            objCommand.Parameters.Add(param21);
            objCommand.Parameters.Add(param22);
            objCommand.Parameters.Add(param23);
            objCommand.Parameters.Add(param24);
            objCommand.Parameters.Add(param25);

            objCommand.ExecuteNonQuery();
            objCommand.Parameters.Clear();

            #region SubTitle

            DeleteById("Movie_SubTitle", "Movie_Id", entity.Id);

            if (entity.Subtitles != null)
                foreach (Language item in entity.Subtitles)
                    AddSubTitle(item, entity.Id);
            #endregion
            #region Audios

            DeleteById("Movie_Audio", "Movie_Id", entity.Id);

            if (entity.Audios != null)
                foreach (Audio item in entity.Audios)
                    AddAudio(item, entity.Id);
            #endregion
        }

        public void DeleteAllXxx()
        {
            PurgeTable("XXX_XXXGenre");
            PurgeTable("XXX_Genre");
            PurgeTable("XXX_Ressources");
            PurgeTable("XXX_Links");
            PurgeTable("XXX_Artist_Job");
            PurgeTable("XXX");
            PurgeTable("XXX_Studio");
        }

        public void GetChild(XXX items, bool getArtistCredits)
        {
            items.Links = GetLinks("XXX_Links", "XXX_Id", items.Id);
            items.Ressources = GetRessources("XXX_Ressources", "XXX_Id", items.Id);
            if (items.Publisher != null)
                items.Publisher = GetPublisher(items.Publisher.Id, "XXX_Studio", "Id");
            items.Artists = GetArtists(items.Id, EntityType.XXX, getArtistCredits);
            items.Genres = GetGenres("XXX_Genre", "XXX_XXXGenre", "XXX_Id", items.Id, "Genre_Id");
            if (items.Language != null)
                items.Language = GetLanguageById(items.Language.Id);
            items.Audios = GetAudios(items.Id);
            items.Subtitles = GetSubTitle(items.Id);
            items.MetaDatas = GetMetaDatas(items.Id);

            if (items.FileFormat != null)
                items.FileFormat = GetFileFormatById(items.FileFormat.Id);

            if (items.AspectRatio != null)
                items.AspectRatio = GetAspectRatioById(items.AspectRatio.Id);
        }
        public IList GetDupeXxx()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"{0}
                                                      FROM
                                                        (Select XXX.Title
                                                        From XXX
                                                        Group By XXX.Title
                                                        Having (Count(XXX.Title) > 1)) AS GRP
                                                      INNER JOIN XXX ON XXX.TITLE = GRP.TITLE 
                                                      INNER JOIN Media ON (XXX.Media_Id = Media.Id);", ConstXxxThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(XxxToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
            return items;
        }
        public XXX GetFirstXxx()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            XXX items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM XXX
                                                      INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                      WHERE XXX.IsDeleted<>'false' 
                                                      LIMIT 1;", ConstXxx);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToXxx(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }
        public IList GetNoSmallCoverXxx()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<XXX> items = new List<XXX>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  XXX
                                                     INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                     WHERE XXX.Cover IS NULL;", ConstXxx);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToXxx(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItem GetThumbXxx(string id)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItem items = null;

            objCommand.CommandText = string.Format(@"{0} 
                                                    FROM XXX
                                                    INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                    WHERE XXX.IsDeleted<>'false' 
                                                    AND XXX.Id='{1}';", ConstXxxThumb, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = XxxToThumb(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public IList GetThumbXxxByArtist()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<ThumbItem> thumbItems = new List<ThumbItem>();

            objCommand.CommandText = string.Format(@"{0}{1}
                                                          FROM XXX
                                                          LEFT OUTER JOIN XXX_Artist_Job ON (XXX.Id=XXX_Artist_Job.XXX_Id)
                                                          LEFT OUTER JOIN Artist ON (XXX_Artist_Job.Artist_Id = Artist.Id)
                                                          LEFT OUTER JOIN Job ON (XXX_Artist_Job.Job_Id = Job.Id)
                                                          INNER JOIN Media ON (XXX.Media_Id = Media.Id);", ConstXxxThumb, ConstXxxThumbArtistPart);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    thumbItems.Add(XxxToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return thumbItems;

        }
        public IList GetXxxByMedia(string mediaName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<XXX> items = new List<XXX>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM  XXX
                                                      INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                      WHERE XXX.IsDeleted=0
                                                      AND Media.Name = ""{1}"";", ConstXxx, mediaName);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToXxx(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public int GetXxxCountByType(string strGenre)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            int number = 0;
            if (strGenre == string.Empty)
                return number;
            else
            {
                objCommand.CommandText = string.Format(@"SELECT Count(XXX.Title)
                                                        FROM XXX_XXXGenre
                                                        INNER JOIN XXX ON (XXX_XXXGenre.XXX_Id = XXX.Id)
                                                        INNER JOIN XXX_Genre ON (XXX_XXXGenre.Genre_Id = XXX_Genre.Id)
                                                        WHERE XXX_Genre.DisplayName = ""{0}"";", strGenre);

                SQLiteDataReader objResults = objCommand.ExecuteReader();
                if (objResults.HasRows)
                {
                    objResults.Read();
                    number = objResults.GetInt32(0);
                }

                objResults.Close();
                objResults.Dispose();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            return number;
        }
        public IList GetThumbXxxByTypes()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = @"SELECT Distinct XXX.Id, Title, smallCover, AddedDate, Runtime, IsDeleted, Rating, Description, ToBeDeleted, Media.Name, 
                                                       IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, XXX_Genre.DisplayName 
                                       FROM XXX_Genre
                                       INNER JOIN XXX_XXXGenre ON XXX_Genre.Id = XXX_XXXGenre.Genre_Id
                                       INNER JOIN XXX ON XXX_XXXGenre.XXX_Id = XXX.Id
                                       INNER JOIN Media ON XXX.Media_Id = Media.Id
                                       WHERE XXX.IsDeleted<>'false'
                                       AND XXX_Genre.DisplayName <>'';";


            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(XxxToThumb(objResults));

            objResults.Close();

            objCommand.CommandText = @"SELECT XXX.Id, Title, smallCover, AddedDate, Runtime, IsDeleted, Rating, Description, ToBeDeleted, Media.Name, 
                                              IsWhish, IsComplete,Seen, ToWatch, FileName, FilePath, 'None' 
                                       FROM XXX
                                       INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                       WHERE XXX.IsDeleted<>'false'
                                       AND NOT EXISTS (SELECT * 
	                                                   FROM [XXX_XXXGenre]
	                                                   WHERE [XXX].[Id] = [XXX_XXXGenre].[XXX_Id]) ;";

            objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(XxxToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;

        }
        public IList GetThumbXxXs()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"{0}
                                                      FROM XXX
                                                      INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                      WHERE XXX.IsDeleted<>'false' ;", ConstXxxThumb);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(XxxToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public ThumbItems GetBigThumbXxx()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            ThumbItems items = new ThumbItems();

            objCommand.CommandText = string.Format(@"SELECT XXX.Id, XXX.Title, BigCover, XXX.AddedDate, XXX.Runtime, XXX.IsDeleted, XXX.Rating, XXX.Description, 
                                                            XXX.ToBeDeleted, Media.Name, XXX.IsWhish, XXX.IsComplete,XXX.Seen, XXX.ToWatch, XXX.FileName, XXX.FilePath, 'None'
                                                      FROM 
                                                            (SELECT XXX_Id as XXXId, Ressource as BigCover
                                                             FROM XXX_Ressources
                                                             WHERE IsDefault=1
                                                             ), XXX
                                                       INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                       WHERE XXXId=XXX.Id;");

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
                while (objResults.Read())
                    items.Add(XxxToThumb(objResults));

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }

        public IList GetXxXs()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<XXX> items = new List<XXX>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  XXX
                                                     INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                     WHERE XXX.IsDeleted <> 'false';", ConstXxx);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToXxx(objResults, false));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            return items;
        }
        public XXX GetXxXs(string id)
        {
            XXX items = null;

            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM XXX
                                                      INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                      WHERE XXX.IsDeleted=0 
                                                      AND XXX.Id=""{1}"";", ConstXxx, id);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToXxx(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);



            return items;
        }
        public XXX GetXxXs(string mediaName, string filePath, string fileName)
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            XXX items = null;

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                      FROM XXX
                                                      INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                      WHERE XXX.IsDeleted<>'false' 
                                                      AND Media.Name = @param1 AND  XXX.FileName = @param2 AND  XXX.FilePath = @param3;", ConstXxx);
            //FIX 2.8.8.0
            SQLiteParameter param1 = new SQLiteParameter("@param1", DbType.String);
            param1.Value = mediaName;

            SQLiteParameter param2 = new SQLiteParameter("@param2", DbType.String);
            param2.Value = fileName;

            SQLiteParameter param3 = new SQLiteParameter("@param3", DbType.String);
            param3.Value = filePath;

            objCommand.Parameters.Add(param1);
            objCommand.Parameters.Add(param2);
            objCommand.Parameters.Add(param3);

            SQLiteDataReader objResults = objCommand.ExecuteReader(CommandBehavior.SingleRow);
            if (objResults.HasRows)
            {
                objResults.Read();
                items = ResultsToXxx(objResults);
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            if (items != null)
                GetChild(items, false);

            return items;
        }

        public int PurgeXxx()
        {
            SQLiteConnection objConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand objCommand = objConnection.CreateCommand();
            objCommand.CommandType = CommandType.Text;
            objConnection.Open();

            List<XXX> items = new List<XXX>();

            objCommand.CommandText = string.Format(@"SELECT {0}
                                                     FROM  XXX
                                                     INNER JOIN Media ON (XXX.Media_Id = Media.Id)
                                                     WHERE XXX.IsDeleted='true';", ConstXxx);

            SQLiteDataReader objResults = objCommand.ExecuteReader();
            if (objResults.HasRows)
            {
                while (objResults.Read())
                    items.Add(ResultsToXxx(objResults));
            }

            objResults.Close();
            objResults.Dispose();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();

            foreach (XXX item in items)
                PurgeXxx(item);

            return items.Count;
        }
        public void PurgeXxx(XXX item)
        {
            DeleteById("XXX_Ressources", "XXX_Id", item.Id);
            DeleteById("XXX_Links", "XXX_Id", item.Id);
            DeleteById("XXX", "Id", item.Id);
            if (item.Publisher != null)
                if (string.IsNullOrWhiteSpace(item.Publisher.Id) == false)
                    DeleteOrphelans("XXX_Studio", item.Publisher.Id, "XXX", "Studio_Id");

        }
        //TODO
        public void PurgeXxxType()
        {

            //var query = from items in _objEntities.XXX_XXXGenre
            //            group items by new { items.XXX_Id, items.Genre_Id } into g
            //            where g.Count() > 1
            //            select g.Key;

            //foreach (var item in query)
            //{
            //    var item1 = item;
            //    var duplicates = from items in _objEntities.XXX_XXXGenre
            //                     where items.XXX_Id == item1.XXX_Id && items.Genre_Id == item1.Genre_Id
            //                     select items;

            //    bool first = true;
            //    foreach (XXX_XXXGenre toDelete in duplicates)
            //    {
            //        if (first == false)
            //            _objEntities.DeleteObject(toDelete);
            //        else
            //            first = false;
            //    }
            //}

            //_objEntities.SaveChanges();

        }

        private XXX ResultsToXxx(SQLiteDataReader reader, bool getLastId = true)
        {
            XXX item = new XXX();

            item.Title = reader.GetString(0);
            item.Id = reader.GetString(1);

            item.Media = new Media();
            item.Media.Id = reader.GetString(2);
            item.Media.Name = reader.GetString(23);

            if (reader.IsDBNull(3) == false)
            {
                item.Language = new Language();
                item.Language.Id = reader.GetString(3);
            }

            if (reader.IsDBNull(4) == false)
            {
                item.FileFormat = new FileFormat();
                item.FileFormat.Id = reader.GetString(4);
            }

            if (reader.IsDBNull(5) == false)
                item.BarCode = reader.GetString(5);

            if (reader.IsDBNull(6) == false)
                item.MyRating = reader.GetInt32(6);

            if (reader.IsDBNull(7) == false)
                item.Description = reader.GetString(7);

            if (reader.IsDBNull(8) == false)
                item.Runtime = reader.GetInt32(8);

            if (reader.IsDBNull(9) == false)
                item.Watched = reader.GetBoolean(9);

            if (reader.IsDBNull(10) == false)
                item.ReleaseDate = reader.GetDateTime(10);

            if (reader.IsDBNull(11) == false)
                item.Comments = reader.GetString(11);

            if (reader.IsDBNull(12) == false)
                item.IsDeleted = reader.GetBoolean(12);

            if (reader.IsDBNull(13) == false)
                item.IsComplete = reader.GetBoolean(13);

            if (reader.IsDBNull(14) == false)
                item.IsWhish = reader.GetBoolean(14);

            if (reader.IsDBNull(15) == false)
                item.AddedDate = reader.GetDateTime(15);

            if (reader.IsDBNull(16) == false)
                item.FileName = reader.GetString(16);

            if (reader.IsDBNull(17) == false)
                item.FilePath = reader.GetString(17);

            if (reader.IsDBNull(18) == false)
                item.ToBeDeleted = reader.GetBoolean(18);

            if (reader.IsDBNull(19) == false)
            {
                item.Publisher = new Publisher();
                item.Publisher.Id = reader.GetString(19);
            }

            if (reader.IsDBNull(20) == false)
                item.Country = reader.GetString(20);

            if (reader.IsDBNull(21) == false)
                item.Cover = (byte[])reader.GetValue(21);

            if (reader.IsDBNull(22) == false)
                item.ToWatch = reader.GetBoolean(22);

            if (reader.IsDBNull(24) == false)
                item.PublicRating = reader.GetDouble(24);

            if (reader.IsDBNull(25) == false)
                item.NumId = reader.GetInt32(25);
            else if (getLastId)
                item.NumId = GetLastCollectionNumber(EntityType.XXX);

            return item;
        }

        private ThumbItem XxxToThumb(SQLiteDataReader reader)
        {
            int? myRating = null;
            if (reader.IsDBNull(6) == false)
                myRating = reader.GetInt32(6);

            string fullPath = string.Empty;
            if (reader.IsDBNull(15) == false && reader.IsDBNull(14) == false)
                if (string.IsNullOrWhiteSpace(reader.GetString(15)) == false &&
                    string.IsNullOrWhiteSpace(reader.GetString(14)) == false)
                    fullPath = Path.Combine(reader.GetString(15), reader.GetString(14));

            string description = string.Empty;
            if (reader.IsDBNull(7) == false)
                description = reader.GetString(7);

            int? runtime = null;
            if (reader.IsDBNull(4) == false)
                runtime = reader.GetInt32(4);

            byte[] cover = null;
            if (reader.IsDBNull(2) == false)
                cover = (byte[])reader.GetValue(2);

            double? publicRating = null;
            if (reader.IsDBNull(19) == false)
                publicRating = reader.GetDouble(19);

            int? numId = null;
            if (reader.IsDBNull(20) == false)
                numId = reader.GetInt32(20);

            string firstName = string.Empty;
            if (reader.IsDBNull(22) == false)
                firstName = reader.GetString(22);

            string lastName = string.Empty;
            if (reader.IsDBNull(23) == false)
                lastName = reader.GetString(23);

            string artist = firstName + " " + lastName;

            return new ThumbItem(reader.GetString(1), reader.GetString(0), cover, reader.GetDateTime(3),
                                    runtime, string.Empty, reader.GetBoolean(5),
                                    EntityType.XXX, myRating, publicRating, description,
                                    reader.GetBoolean(8), reader.GetString(9), reader.GetBoolean(10),
                                    reader.GetBoolean(11), reader.GetBoolean(12), reader.GetString(16),
                                    string.Empty, artist, reader.GetBoolean(13), fullPath, numId);
        }

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}