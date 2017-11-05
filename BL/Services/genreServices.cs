using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Services
{
    class GenreServices
    {
        public static void AddGenres(IEnumerable<string> lstGenres, IMyCollectionsData objEntity, bool save)
        {
            try
            {
                //FIX since 2.7.12.0
                if (lstGenres != null)
                {
                    foreach (string item in lstGenres)
                    {
                        //Fix 2.8.0.0
                        if (String.IsNullOrWhiteSpace(item) == false)
                        {
                            string strTemp = Util.PurgeHtml(item.Trim());
                            bool bFind = objEntity.Genres.Any(objType => objType.DisplayName == strTemp);

                            if (bFind == false && save == false)
                            {
                                Genre genre = GetGenre(item, objEntity.ObjectType);
                                objEntity.Genres.Add(genre);
                            }
                            if (save == true)
                            {
                                Genre genre = GetGenre(item, objEntity.ObjectType);
                                Dal.GetInstance.AddGenre(genre, objEntity.ObjectType);
                                Dal.GetInstance.LinkGenre(objEntity, genre.Id);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string param = string.Empty;
                if (objEntity != null)
                    param = objEntity.Title;
                Util.LogException(exception, param);
                throw;
            }
        }
        public static void AddGenres(IEnumerable<Genre> lstGenres, IMyCollectionsData objEntity, bool save)
        {
            foreach (Genre item in lstGenres)
            {
                if (String.IsNullOrWhiteSpace(item.RealName.Trim()) == false)
                {
                    bool bFind = objEntity.Genres.Any(objType => objType.RealName == item.RealName);

                    if (bFind == false && save == false)
                        objEntity.Genres.Add(item);

                    if (save == true)
                    {
                        Dal.GetInstance.AddGenre(item, objEntity.ObjectType);
                        Dal.GetInstance.LinkGenre(objEntity, item.Id);
                    }
                }
            }
        }
        public static void AddGenre(string strType, IMyCollectionsData entity, bool save)
        {
            List<string> lstTypes = new List<string>();
            string[] strTemp;

            if (strType.Contains("|"))
                strTemp = strType.Split("|".ToCharArray());
            else
                strTemp = strType.Split(",".ToCharArray());

            foreach (string item in strTemp)
            {
                string strValue = Util.GetValueBetween(item, ">", "</a>");
                if (String.IsNullOrWhiteSpace(strValue) == false)
                    lstTypes.Add(strValue);
            }

            AddGenres(lstTypes, entity, save);
        }
        public void AddGenre(Genre genre, EntityType entityType)
        {
            if (genre == null) return;

            Genre bookType = GetGenre(genre.RealName, entityType);
            //FIX 2.7.12.0
            if (bookType == null || string.IsNullOrWhiteSpace(bookType.Id)==true)
            {
                if (genre.DisplayName == null)
                    genre.DisplayName = string.Empty;

                Dal.GetInstance.AddGenre(genre, entityType);
            }
            else
            {
                if (bookType.DisplayName != genre.DisplayName)
                {
                    if (genre.DisplayName == null)
                        genre.DisplayName = string.Empty;

                    bookType.DisplayName = genre.DisplayName;
                    Dal.GetInstance.AddGenre(bookType, entityType);
                }
            }
        }

        public static void DeleteGenre(Genre objItem, EntityType entityType)
        {
            Genre genre = GetGenre(objItem.RealName, entityType);

            if (string.IsNullOrWhiteSpace(genre.Id) != false)
                return;


            switch (entityType)
            {
                case EntityType.Apps:
                    Dal.GetInstance.DeleteById("Apps_AppType", "AppType_Id", genre.Id);
                    Dal.GetInstance.DeleteById("AppType", "Id", objItem.Id);
                    break;
                case EntityType.Books:
                    Dal.GetInstance.DeleteById("Books_BookType", "BookType_Id", genre.Id);
                    Dal.GetInstance.DeleteById("BookType", "Id", objItem.Id);
                    break;
                case EntityType.Games:
                    Dal.GetInstance.DeleteById("Gamez_GamezType", "GamezType_Id", genre.Id);
                    Dal.GetInstance.DeleteById("GamezType", "Id", genre.Id);
                    break;
                case EntityType.Movie:
                    Dal.GetInstance.DeleteById("Movie_MovieGenre", "MovieGenre", genre.Id);
                    Dal.GetInstance.DeleteById("Movie_Genre", "Id", objItem.Id);
                    break;
                case EntityType.Music:
                    Dal.GetInstance.DeleteById("Music_MusicGenre", "MusicGenre_Id", genre.Id);
                    Dal.GetInstance.DeleteById("Music_Genre", "Id", objItem.Id);
                    break;
                case EntityType.Nds:
                    Dal.GetInstance.DeleteById("Nds_GamezType", "GamezType_Id", genre.Id);
                    Dal.GetInstance.DeleteById("GamezType", "Id", objItem.Id);
                    break;
                case EntityType.Series:
                    Dal.GetInstance.DeleteById("Series_MovieGenre", "Genre_Id", genre.Id);
                    Dal.GetInstance.DeleteById("Movie_Genre", "Id", objItem.Id);
                    break;
                case EntityType.XXX:
                    Dal.GetInstance.DeleteById("XXX_XXXGenre", "Genre_Id", genre.Id);
                    Dal.GetInstance.DeleteById("XXX_Genre", "Id", genre.Id);
                    break;
            }
        }

        public static IList GetGenres(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Apps:
                    return Dal.GetInstance.GetGenres("AppType");
                case EntityType.Books:
                    return Dal.GetInstance.GetGenres("BookType");
                case EntityType.Nds:
                case EntityType.Games:
                    return Dal.GetInstance.GetGenres("GamezType");
                case EntityType.Series:
                case EntityType.Movie:
                    return Dal.GetInstance.GetGenres("Movie_Genre");
                case EntityType.Music:
                    return Dal.GetInstance.GetGenres("Music_Genre");
                case EntityType.XXX:
                    return Dal.GetInstance.GetGenres("XXX_Genre");
            }
            return null;
        }
        public static Genre GetGenre(string genreName, EntityType entityType)
        {
            if (String.IsNullOrEmpty(genreName) == true)
                return null;

            Genre objTypes = null;
            switch (entityType)
            {
                case EntityType.Apps:
                    objTypes = Dal.GetInstance.GetGenre(genreName, "AppType");
                    break;
                case EntityType.Books:
                    objTypes = Dal.GetInstance.GetGenre(genreName, "BookType");
                    break;
                case EntityType.Nds:
                case EntityType.Games:
                    objTypes = Dal.GetInstance.GetGenre(genreName, "GamezType");
                    break;
                case EntityType.Series:
                case EntityType.Movie:
                    objTypes = Dal.GetInstance.GetGenre(genreName, "Movie_Genre");
                    break;
                case EntityType.Music:
                    objTypes = Dal.GetInstance.GetGenre(genreName, "Music_Genre");
                    break;
                case EntityType.XXX:
                    objTypes = Dal.GetInstance.GetGenre(genreName, "XXX_Genre");
                    break;
            }


            if (objTypes == null)
                objTypes = new Genre(genreName, genreName);

            return objTypes;
        }
        public static IList<string> GetGenres(IMyCollectionsData entity)
        {
            List<string> genres = new List<string>();

            if (entity != null)
            {
                if (entity.Genres.Count > 0)
                {
                    foreach (Genre item in entity.Genres)
                    {
                        if (item != null)
                            if (genres.Contains(item.DisplayName) == false)
                                genres.Add(item.DisplayName);
                    }
                }
            }

            return genres;
        }


        public static void UnlinkGenre(IMyCollectionsData entity)
        {
            Dal.GetInstance.UnlinkGenre(entity);
        }
    }
}
