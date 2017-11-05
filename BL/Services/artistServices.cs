using System;
using System.Collections;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using myCollections.BL.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myCollections.BL.Services
{
    static class ArtistServices
    {
        public static void AddArtists(IEnumerable<string> artists, IMyCollectionsData entity)
        {
            if (artists != null)
                foreach (string item in artists)
                {
                    if (String.IsNullOrWhiteSpace(item) == false)
                    {
                        if (entity.Artists == null || entity.Artists.Any(x => x.FulleName.ToUpper() == item.ToUpper()) == false)
                        {
                            bool isNew;
                            SaveArtist(Get(item, out isNew), entity);
                        }
                        else
                        {
                            Artist artist = entity.Artists.First(x => x.FulleName.ToUpper() == item.ToUpper() && x.IsOld == false);
                            SaveArtist(artist, entity);
                        }
                    }
                }
        }

        public static void AddArtist(IEnumerable<Artist> lstItems, IMyCollectionsData entity)
        {
            if (lstItems == null) return;

            if (entity.Artists == null)
                entity.Artists = new List<Artist>();

            Parallel.ForEach(lstItems, item =>
            {
                try
                {
                    if (item != null && item.IsOld == false)
                    {
                        try
                        {
                            bool bFind = false;
                            for (int i = 0; i < entity.Artists.Count; i++)
                            {
                                Artist artist = entity.Artists.ElementAt(i);

                                if (artist != null)
                                {
                                    if (
                                        (String.Equals(artist.FirstName, item.FirstName,
                                            StringComparison.CurrentCultureIgnoreCase) &&
                                         String.Equals(artist.LastName, item.LastName,
                                             StringComparison.CurrentCultureIgnoreCase)) ||
                                        String.Equals(artist.FulleName, item.FulleName,
                                            StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        if (artist.ArtistCredits == null)
                                            artist.ArtistCredits = new List<ArtistCredits>();

                                        if (artist.BirthDay == null)
                                            artist.BirthDay = item.BirthDay;

                                        if (item.Picture != null)
                                            if (artist.Picture == null || artist.Picture.Length < item.Picture.Length)
                                                artist.Picture = item.Picture;

                                        IEnumerable<ArtistCredits> diffCredits = null;
                                        if (artist.ArtistCredits != null && item.ArtistCredits != null)
                                            diffCredits = item.ArtistCredits.Except(artist.ArtistCredits);
                                        else if (item.ArtistCredits != null)
                                            diffCredits = item.ArtistCredits;
                                        else if (artist.ArtistCredits != null)
                                            diffCredits = artist.ArtistCredits;

                                        if (diffCredits != null)
                                            for (int j = 0; j < diffCredits.Count(); j++)
                                                artist.ArtistCredits.Add(diffCredits.ElementAt(j));

                                        if (String.IsNullOrWhiteSpace(artist.Bio))
                                            artist.Bio = item.Bio;

                                        if (String.IsNullOrWhiteSpace(artist.PlaceBirth))
                                            artist.PlaceBirth = item.PlaceBirth;

                                        artist.IsOld = false;

                                        bFind = true;
                                        break;
                                    }
                                }
                            }

                            if (bFind == false)
                            {

                                if (String.IsNullOrWhiteSpace(item.FulleName))
                                    item.FulleName = item.FirstName + " " + item.LastName;

                                bool isNew;
                                Artist artist = Get(item.FulleName, out isNew);

                                if (artist != null)
                                {
                                    if (artist.BirthDay == null)
                                        artist.BirthDay = item.BirthDay;

                                    if (item.Picture != null)
                                        if (artist.Picture == null || artist.Picture.Length < item.Picture.Length)
                                            artist.Picture = item.Picture;

                                    IEnumerable<ArtistCredits> diffCredits = null;
                                    if (artist.ArtistCredits != null && item.ArtistCredits != null)
                                        diffCredits = item.ArtistCredits.Except(artist.ArtistCredits);
                                    else if (item.ArtistCredits != null)
                                        diffCredits = item.ArtistCredits;
                                    else if (artist.ArtistCredits != null)
                                        diffCredits = artist.ArtistCredits;

                                    if (diffCredits != null)
                                        for (int i = 0; i < diffCredits.Count(); i++)
                                            artist.ArtistCredits.Add(diffCredits.ElementAt(i));

                                    if (String.IsNullOrWhiteSpace(artist.Bio))
                                        artist.Bio = item.Bio;

                                    if (String.IsNullOrWhiteSpace(artist.PlaceBirth))
                                        artist.PlaceBirth = item.PlaceBirth;

                                    Job objJob = GetJob(entity.ObjectType);
                                    artist.Added = DateTime.UtcNow;
                                    artist.Job = objJob;
                                    entity.Artists.Add(artist);
                                }
                            }
                        }
                        //FIX 2.8.8.0
                        catch (InvalidOperationException) { }
                        catch (Exception ex)
                        {
                            if (item != null && entity != null)
                                Util.LogException(ex, item.FulleName + " " + entity.Title + " " + entity.OriginalTitle);
                            else
                                Util.LogException(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //FIX 2.8.9.0
                    Util.LogException(ex, entity.Title);
                }
            });
        }
        public static void AddArtist(string artistName, IMyCollectionsData objEntity)
        {
            bool isNew;
            if (String.IsNullOrWhiteSpace(artistName)) return;

            SaveArtist(Get(artistName, out isNew), objEntity);

        }

        private static void AddCredits(IEnumerable<ArtistCredits> credits, Artist objItem)
        {

            if (credits != null)
            {
                List<ArtistCredits> titles = Dal.GetInstance.GetArtistCreditbyId(objItem.Id) as List<ArtistCredits>;

                foreach (ArtistCredits movie in credits)
                {
                    if (titles == null || titles.Any(x => x.Title.ToUpperInvariant() == movie.Title.ToUpperInvariant()) == false)
                        objItem.ArtistCredits.Add(movie);
                }
            }
        }

        public static void AddDirector(IEnumerable<Artist> lstItems, Movie objEntity)
        {
            if (lstItems == null || lstItems.Any() == false) return;

            foreach (Artist item in lstItems)
            {
                bool bFind = false;
                if (item != null)
                {
                    for (int i = 0; i < objEntity.Artists.Count; i++)
                    {
                        Artist objMovieArtistJob = objEntity.Artists.ElementAt(i);

                        if (objMovieArtistJob != null)
                        {
                            if (((String.Equals(objMovieArtistJob.FirstName, item.FirstName, StringComparison.CurrentCultureIgnoreCase) &&
                                  String.Equals(objMovieArtistJob.LastName, item.LastName, StringComparison.CurrentCultureIgnoreCase)) ||
                                 String.Equals(objMovieArtistJob.FulleName, item.FulleName, StringComparison.CurrentCultureIgnoreCase)) &&
                                objMovieArtistJob.Job.Name == "Director")
                            {
                                if (objMovieArtistJob.BirthDay == null)
                                    objMovieArtistJob.BirthDay = item.BirthDay;

                                if (item.Picture != null)
                                    if (objMovieArtistJob.Picture == null ||
                                        objMovieArtistJob.Picture.Length < item.Picture.Length)
                                        objMovieArtistJob.Picture = item.Picture;

                                IEnumerable<ArtistCredits> diffCredits = null;
                                if (objMovieArtistJob.ArtistCredits != null && item.ArtistCredits != null)
                                    diffCredits = item.ArtistCredits.Except(objMovieArtistJob.ArtistCredits);
                                else if (item.ArtistCredits != null)
                                    diffCredits = item.ArtistCredits;
                                else if (objMovieArtistJob.ArtistCredits != null)
                                    diffCredits = objMovieArtistJob.ArtistCredits;

                                if (diffCredits != null)
                                {
                                    //FIX 2.7.12.0
                                    if (objMovieArtistJob.ArtistCredits == null)
                                        objMovieArtistJob.ArtistCredits = new List<ArtistCredits>();

                                    for (int j = 0; j < diffCredits.Count(); j++)
                                        objMovieArtistJob.ArtistCredits.Add(diffCredits.ElementAt(j));
                                }

                                if (String.IsNullOrWhiteSpace(objMovieArtistJob.Bio))
                                    objMovieArtistJob.Bio = item.Bio;

                                if (String.IsNullOrWhiteSpace(objMovieArtistJob.PlaceBirth))
                                    objMovieArtistJob.PlaceBirth = item.PlaceBirth;

                                objMovieArtistJob.IsOld = false;
                                bFind = true;
                                break;
                            }
                        }
                    }

                    if (bFind == false)
                    {
                        if (String.IsNullOrWhiteSpace(item.FulleName))
                            item.FulleName = item.FirstName + " " + item.LastName;

                        bool isNew;
                        Artist artist = Get(item.FulleName, out isNew);
                        if (artist != null)
                        {
                            if (artist.BirthDay == null)
                                artist.BirthDay = item.BirthDay;

                            if (item.Picture != null)
                                if (artist.Picture == null || artist.Picture.Length < item.Picture.Length)
                                    artist.Picture = item.Picture;

                            IEnumerable<ArtistCredits> diffCredits = item.ArtistCredits.Except(artist.ArtistCredits);

                            for (int i = 0; i < diffCredits.Count(); i++)
                                artist.ArtistCredits.Add(diffCredits.ElementAt(i));

                            if (String.IsNullOrWhiteSpace(artist.Bio))
                                artist.Bio = item.Bio;

                            if (String.IsNullOrWhiteSpace(artist.PlaceBirth))
                                artist.PlaceBirth = item.PlaceBirth;

                            Job objJob = GetJob("Director");
                            if (objJob != null)
                            {
                                artist.Job = objJob;
                                artist.Added = DateTime.UtcNow;
                                objEntity.Artists.Add(artist);
                            }
                        }
                    }
                }
            }
        }

        public static void Delete(string id)
        {
            Dal.GetInstance.DeleteArtist(id);
        }
        public static void DeleteCredits(Artist artist)
        {
            foreach (ArtistCredits credit in artist.ArtistCredits)
                credit.IsOld = true;
        }

        public static void Fill(Hashtable objResults, Artist objItem, string fullname)
        {

            if (objResults != null)
            {
                if (objResults.ContainsKey("Image"))
                {
                    byte[] image = Util.GetImage(objResults["Image"].ToString());
                    if (objItem.Picture == null || objItem.Picture.Length < image.Length)
                        objItem.Picture = image;
                }

                if (objResults.ContainsKey("Credits"))
                {
                    List<ArtistCredits> credits = objResults["Credits"] as List<ArtistCredits>;
                    AddCredits(credits, objItem);
                }

                if (objResults.ContainsKey("Aka"))
                    objItem.Aka = objResults["Aka"].ToString();

                if (objResults.ContainsKey("Birthday"))
                    objItem.BirthDay = objResults["Birthday"] as DateTime?;

                if (objResults.ContainsKey("Bio"))
                    objItem.Bio = objResults["Bio"].ToString();
                else if (objResults.ContainsKey("Comments"))
                    objItem.Bio = objResults["Comments"].ToString();

                if (objResults.ContainsKey("Measurements"))
                    objItem.Breast = objResults["Measurements"].ToString();

                if (objResults.ContainsKey("Ethnicity"))
                    objItem.Ethnicity = objResults["Ethnicity"].ToString();

                objItem.FulleName = fullname;

                if (objResults.ContainsKey("Birthplace"))
                    objItem.PlaceBirth = objResults["Birthplace"].ToString();

                if (objResults.ContainsKey("Website"))
                    objItem.WebSite = objResults["Website"].ToString();

                if (objResults.ContainsKey("YearsActive"))
                    objItem.YearsActive = objResults["YearsActive"].ToString();
            }
        }
        public static IList Find(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            return Dal.GetInstance.FindArtist(name);

        }

        public static Artist Get(string fullName, out bool isNew)
        {
            isNew = false;
            if (String.IsNullOrWhiteSpace(fullName))
                return null;

            string firstName;
            string lastName;

            fullName = fullName.Trim();
            SplitName(fullName, out firstName, out lastName);

            Artist objArtist = Dal.GetInstance.GetArtist(firstName, lastName);
            if (objArtist == null)
            {
                isNew = true;
                objArtist = new Artist();
                objArtist.FirstName = firstName;
                objArtist.LastName = lastName;
                objArtist.FulleName = fullName;
                objArtist.ArtistCredits = new List<ArtistCredits>();
            }

            if (String.IsNullOrWhiteSpace(objArtist.FulleName))
                objArtist.FulleName = fullName;

            return objArtist;
        }
        public static Artist GetById(string id)
        {
            return Dal.GetInstance.GetArtistById(id);
        }
        public static IList<string> GetCast(IMyCollectionsData entity)
        {
            List<string> cast = new List<string>();

            if (entity != null)
            {
                if (entity.Artists.Count > 0)
                {
                    foreach (Artist item in entity.Artists)
                    {
                        if (item != null)
                            if (cast.Contains(item.FulleName) == false)
                                cast.Add(item.FulleName);
                    }
                }
            }

            return cast;
        }

        public static IList GetFullNames()
        {
            return Dal.GetInstance.GetArtistFullNames();
        }
        public static void GetInfoFromWeb(Artist objItem, bool usePartialMatch, EntityType entityType, out string errorMessage, bool log)
        {
            errorMessage = String.Empty;
            objItem.Picture = null;

            if (entityType == EntityType.XXX)
            {
                GetInfoFromWeb(objItem, usePartialMatch, Provider.AduldtDvdEmpire, out errorMessage, log);
                if (objItem.Picture == null)
                    GetInfoFromWeb(objItem, usePartialMatch, Provider.Iafd, out errorMessage, log);
            }

            if (objItem.Picture == null)
                GetInfoFromWeb(objItem, usePartialMatch, Provider.Tmdb, out errorMessage, log);

            if (objItem.Picture == null)
                GetInfoFromWeb(objItem, usePartialMatch, Provider.Bing, out errorMessage, log);

        }
        public static void GetInfoFromWeb(Artist objItem, bool usePartialMatch, Provider provider, out string errorMessage, bool log)
        {
            errorMessage = String.Empty;
            //Fix 2.6.7.0
            if (objItem != null)
            {

                errorMessage = String.Empty;
                string strSearch = (objItem.FirstName + " " + objItem.LastName).Trim();
                Hashtable objResults = null;
                Artist results = null;

                if (log == true)
                    Task.Factory.StartNew(() => Util.NotifyEvent("getInfoFromWeb:Artist " + provider + " " + strSearch));

                try
                {
                    switch (provider)
                    {
                        case Provider.Bing:
                            objResults = BingServices.SearchPortrait(strSearch, usePartialMatch);
                            break;
                        case Provider.Iafd:
                            objResults = IafdServices.SearchPortrait(strSearch, usePartialMatch);
                            break;
                        case Provider.Tmdb:
                            results = TheMovieDbServices.SearchPortrait(strSearch, usePartialMatch, LanguageType.EN);
                            break;
                        case Provider.AlloCine:
                            results = AlloCineServices.SearchPortrait(strSearch, usePartialMatch, LanguageType.FR);
                            break;
                        case Provider.AduldtDvdEmpire:
                            objResults = AdultdvdempireServices.SearchPortrait(strSearch, usePartialMatch);
                            break;
                    }
                    if (objResults != null)
                        Fill(objResults, objItem, strSearch);

                    #region Artist

                    if (results != null)
                    {
                        if (results.Picture != null)
                            if (objItem.Picture == null || objItem.Picture.Length < results.Picture.Length)
                                objItem.Picture = results.Picture;

                        //FIX 2.8.0.0
                        if (results.ArtistCredits != null)
                        {
                            if (objItem.ArtistCredits == null)
                                objItem.ArtistCredits = new List<ArtistCredits>();

                            for (int i = 0; i < results.ArtistCredits.Count; i++)
                            {
                                ArtistCredits item = results.ArtistCredits.ElementAt(i);
                                if (
                                    objItem.ArtistCredits.Any(
                                        x => x.Title.ToUpperInvariant() == item.Title.ToUpperInvariant()) == false)
                                    objItem.ArtistCredits.Add(item);
                            }
                        }

                        if (results.BirthDay != null)
                            objItem.BirthDay = results.BirthDay;

                        if (String.IsNullOrWhiteSpace(results.Bio) == false)
                            objItem.Bio = results.Bio;

                        if (String.IsNullOrWhiteSpace(results.Breast) == false)
                            objItem.Breast = results.Breast;

                        if (String.IsNullOrWhiteSpace(results.Ethnicity) == false)
                            objItem.Ethnicity = results.Ethnicity;

                        objItem.FulleName = strSearch;

                        if (String.IsNullOrWhiteSpace(results.PlaceBirth) == false)
                            objItem.PlaceBirth = results.PlaceBirth;

                        if (String.IsNullOrWhiteSpace(results.WebSite) == false)
                            objItem.WebSite = results.WebSite;

                        if (String.IsNullOrWhiteSpace(results.YearsActive) == false)
                            objItem.YearsActive = results.YearsActive;
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    Util.LogException(ex);
                }
            }
        }
        public static IList<ArtistCredits> GetCredits(Artist artist)
        {
            if (String.IsNullOrWhiteSpace(artist.Id))
                return Dal.GetInstance.GetArtistCredit(artist.FulleName);
            else
                return Dal.GetInstance.GetArtistCreditbyId(artist.Id);
        }
        public static Job GetJob(string strJobName)
        {
            Job job = Dal.GetInstance.GetJob(strJobName);
            if (job == null)
            {
                job = new Job();
                job.Name = strJobName;
                Dal.GetInstance.AddJob(job);
            }
            return job;
        }
        public static Job GetJob(EntityType entityType)
        {
            string jobName = String.Empty;
            switch (entityType)
            {
                case EntityType.Books:
                    jobName = "Author";
                    break;
                case EntityType.Music:
                    jobName = "Singer";
                    break;
                case EntityType.Movie:
                case EntityType.Series:
                case EntityType.XXX:
                    jobName = "Actor";
                    break;

            }
            return Dal.GetInstance.GetJob(jobName);
        }
        public static IList<string> GetOwnedBooks(Artist artist)
        {
            if (String.IsNullOrWhiteSpace(artist.Id))
                return Dal.GetInstance.GetArtistsOwndedByName(artist.FulleName, "Books", "Book_Artist_Job", "Book_Id");
            else
                return Dal.GetInstance.GetArtistsOwndedById(artist.Id, "Books", "Book_Artist_Job", "Book_Id");
        }
        public static IList<string> GetOwnedMovies(Artist artist)
        {
            if (String.IsNullOrWhiteSpace(artist.Id))
                return Dal.GetInstance.GetArtistsOwndedByName(artist.FulleName, "Movie", "Movie_Artist_Job", "Movie_Id");
            else
                return Dal.GetInstance.GetArtistsOwndedById(artist.Id, "Movie", "Movie_Artist_Job", "Movie_Id");

        }
        public static IList<string> GetOwnedSeries(Artist artist)
        {
            if (String.IsNullOrWhiteSpace(artist.Id))
                return Dal.GetInstance.GetArtistsOwndedByName(artist.FulleName, "Series", "Series_Artist_Job", "Series_Id");
            else
                return Dal.GetInstance.GetArtistsOwndedById(artist.Id, "Series", "Series_Artist_Job", "Series_Id");
        }
        public static IList<string> GetOwnedXxx(Artist artist)
        {
            if (String.IsNullOrWhiteSpace(artist.Id))
                return Dal.GetInstance.GetArtistsOwndedByName(artist.FulleName, "XXX", "XXX_Artist_Job", "XXX_Id");
            else
                return Dal.GetInstance.GetArtistsOwndedById(artist.Id, "XXX", "XXX_Artist_Job", "XXX_Id");

        }
        public static IList<string> GetOwnedMusic(Artist artist)
        {
            if (String.IsNullOrWhiteSpace(artist.Id))
                return Dal.GetInstance.GetArtistsOwndedByName(artist.FulleName, "Music", "Music_Artist_Job", "Music_Id");
            else
                return Dal.GetInstance.GetArtistsOwndedById(artist.Id, "Music", "Music_Artist_Job", "Music_Id");
        }

        public static void SaveArtist(Artist artist, IMyCollectionsData entity)
        {
            if (artist.IsOld == false)
            {
                if (artist.Job == null)
                {
                    Job objJob = GetJob(entity.ObjectType);
                    artist.Job = objJob;
                }

                Dal.GetInstance.AddArtist(artist, entity);
            }
        }

        private static void SplitName(string strName, out string strFirstName, out string strLastName)
        {

            strFirstName = null;
            strLastName = null;

            if (String.IsNullOrEmpty(strName))
                return;

            strLastName = " ";

            foreach (string item in strName.Split(' '))
            {
                if (String.IsNullOrEmpty(strFirstName))
                    strFirstName = item;
                else
                    strLastName += item + " ";
            }
            if (strFirstName != null)
            {
                strFirstName = Util.PurgeHtml(strFirstName.Trim());
                strFirstName = strFirstName.Replace(',', ' ').Trim();
            }

            strLastName = Util.PurgeHtml(strLastName.Trim());
            strLastName = strLastName.Replace(',', ' ').Trim();
        }

        public static void Update(Artist artist)
        {
            Dal.GetInstance.AddArtist(artist);
        }
    }
}
