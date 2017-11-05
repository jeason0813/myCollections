using System;
using System.Collections;
using System.Collections.Generic;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Services
{
    static class MediaServices
    {
        public static bool DeleteMedia(string strMediaName)
        {
            Media noneMedia = Get("None", true);
            return Dal.GetInstance.DeleteMedia(strMediaName,noneMedia.Id);
        }

        public static Media Get(string strMedia, bool save)
        {
            if (string.IsNullOrEmpty(strMedia))
                strMedia = "None";

            Media objMedia = Dal.GetInstance.GetMediaByName(strMedia);
            if (objMedia == null)
            {
                objMedia = new Media();
                objMedia.Name = strMedia;
                if (save)
                    Dal.GetInstance.AddMedia(objMedia);
            }

            return objMedia;
        }

        public static Media Get(string strMedia, string mediaType, string path, bool cleanTitle, EntityType entityType, string lastPattern, bool searchSub,
                                    bool localImage, bool useNfo, bool save)
        {
            Media objMedia = Get(strMedia, false);
            if (string.IsNullOrWhiteSpace(mediaType) == false)
            {
                MediaType objMediaType = Dal.GetInstance.GetMediaType(mediaType);
                if (objMediaType == null)
                {
                    objMediaType = new MediaType();
                    objMedia.MediaType = new MediaType();
                    objMedia.MediaType.Name = mediaType;

                }

                objMedia.MediaType = objMediaType;
            }

            if (string.IsNullOrWhiteSpace(path) == false)
                objMedia.Path = path;

            objMedia.CleanTitle = cleanTitle;
            objMedia.EntityType = entityType;

            if (string.IsNullOrWhiteSpace(lastPattern) == false)
                objMedia.LastPattern = lastPattern;

            objMedia.SearchSub = searchSub;
            objMedia.LocalImage = localImage;
            objMedia.UseNfo = useNfo;

            if (save==true)
                Dal.GetInstance.AddMedia(objMedia);

            return objMedia;
        }

        public static IList<string> GetNames()
        {
            return Dal.GetInstance.GetMediaList();
        }

        public static IList<string> GetTypes()
        {
            return Dal.GetInstance.GetMediaTypeList();
        }
        public static void UpdateInfo(string strMediaName, string strPath, string mediaType, bool useNfo,
                                        EntityType entityType, bool cleanTitle, string lastPattern, bool localImage, bool searchSub)
        {
            Media objMedia = Get(strMediaName, false);

            int free;
            int total;
            Util.GetDriveInfo(strPath, out free, out total);
            if (total > -1)
                objMedia.TotalSpace = total;

            if (free > -1)
                objMedia.FreeSpace = free;

            objMedia.Path = strPath;
            objMedia.EntityType = entityType;
            objMedia.LastUpdate = DateTime.Now;
            objMedia.MediaType = Dal.GetInstance.GetMediaType(mediaType);
            objMedia.CleanTitle = cleanTitle;
            objMedia.LastPattern = lastPattern;
            objMedia.LocalImage = localImage;
            objMedia.SearchSub = searchSub;
            objMedia.UseNfo = useNfo;

            Dal.GetInstance.AddMedia(objMedia);
        }
        public static void UpdatePath(string oldname, string newname, string strMediaPath)
        {
            Media objMedia = Dal.GetInstance.GetMediaByName(oldname);
            if (objMedia != null && objMedia.Path != null)
            {
                objMedia.Name = newname;

                IList items = Dal.GetInstance.GetAppsByMedia(oldname);
                foreach (Apps item in items)
                {
                    if (item != null && item.FilePath != null && item.FilePath != item.FilePath.Replace(objMedia.Path, strMediaPath))
                    {
                        item.FilePath = item.FilePath.Replace(objMedia.Path, strMediaPath);
                        Dal.GetInstance.AddApps(item);
                    }
                }

                items = Dal.GetInstance.GetBooksByMedia(oldname);
                foreach (Books item in items)
                {
                    if (item != null && item.FilePath != null && item.FilePath != item.FilePath.Replace(objMedia.Path, strMediaPath))
                    {
                        item.FilePath = item.FilePath.Replace(objMedia.Path, strMediaPath);
                        Dal.GetInstance.AddBook(item);
                    }
                }

                items = Dal.GetInstance.GetGamesByMedia(oldname);
                foreach (Gamez item in items)
                {
                    if (item != null && item.FilePath != null && item.FilePath != item.FilePath.Replace(objMedia.Path, strMediaPath))
                    {
                        item.FilePath = item.FilePath.Replace(objMedia.Path, strMediaPath);
                        Dal.GetInstance.AddGame(item);
                    }
                }

                items = Dal.GetInstance.GetMoviesByMedia(oldname);
                foreach (Movie item in items)
                {
                    if (item != null && item.FilePath != null && item.FilePath != item.FilePath.Replace(objMedia.Path, strMediaPath))
                    {
                        item.FilePath = item.FilePath.Replace(objMedia.Path, strMediaPath);
                        Dal.GetInstance.AddMovie(item);
                    }
                }

                items = Dal.GetInstance.GetMusicsByMedia(oldname);
                foreach (Music item in items)
                {
                    if (item != null && item.FilePath != null && item.FilePath != item.FilePath.Replace(objMedia.Path, strMediaPath))
                    {
                        item.FilePath = item.FilePath.Replace(objMedia.Path, strMediaPath);
                        Dal.GetInstance.AddMusic(item);
                    }
                }

                items = Dal.GetInstance.GetNdsByMedia(oldname);
                foreach (Nds item in items)
                {
                    if (item != null && item.FilePath != null && item.FilePath != item.FilePath.Replace(objMedia.Path, strMediaPath))
                    {
                        item.FilePath = item.FilePath.Replace(objMedia.Path, strMediaPath);
                        Dal.GetInstance.AddNds(item);
                    }
                }

                items = Dal.GetInstance.GetSeriesByMedia(oldname);
                foreach (SeriesSeason item in items)
                {
                    if (item != null && item.FilePath != null && item.FilePath != item.FilePath.Replace(objMedia.Path, strMediaPath))
                    {
                        item.FilePath = item.FilePath.Replace(objMedia.Path, strMediaPath);
                        Dal.GetInstance.AddSeriesSeason(item);
                    }
                }

                items = Dal.GetInstance.GetXxxByMedia(oldname);
                foreach (XXX item in items)
                {
                    if (item != null && item.FilePath != null && item.FilePath != item.FilePath.Replace(objMedia.Path, strMediaPath))
                    {
                        item.FilePath = item.FilePath.Replace(objMedia.Path, strMediaPath);
                        Dal.GetInstance.AddXxx(item);
                    }
                }

                objMedia.Path = strMediaPath;
                Dal.GetInstance.AddMedia(objMedia);
            }
        }


    }
}
