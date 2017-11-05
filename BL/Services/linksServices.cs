using System;
using System.Collections.Generic;
using System.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Services
{
    class LinksServices
    {
        public static void AddLinks(IEnumerable<string> lstItem, IMyCollectionsData objEntity, bool toBeSaved)
        {
            foreach (string item in lstItem)
                AddLinks(item, objEntity, toBeSaved);
        }
        public static void AddLinks(IEnumerable<Links> lstItem, IMyCollectionsData objEntity, bool toBeSaved)
        {
            foreach (Links item in lstItem)
                if (toBeSaved == true && item.IsOld == false)
                    SaveLinks(item.Path, objEntity);
        }
        public static void AddLinks(string link, IMyCollectionsData objEntity, bool toBeSaved)
        {
            string cleanLink = link.Trim();
            if (String.IsNullOrWhiteSpace(cleanLink) == false)
            {
                Links objLink = GetLink(cleanLink, objEntity);
                if (objLink == null)
                {
                    if (toBeSaved == true)
                        objLink = SaveLinks(cleanLink, objEntity);
                    else
                    {
                        objLink = new Links();
                        objLink.ItemId = objEntity.Id;
                        objLink.Path = cleanLink;
                        objLink.Type = "Url";
                    }

                    if (objEntity.Links == null)
                        objEntity.Links = new List<Links>();

                    objEntity.Links.Add(objLink);
                }
                else
                {
                    Links oldLinks = objEntity.Links.FirstOrDefault(x => x.Id == objLink.Id);
                    if (oldLinks != null)
                        oldLinks.IsOld = false;
                }
            }
        }

        public static void DeleteLinks(IMyCollectionsData entity)
        {
            switch (entity.ObjectType)
            {
                case EntityType.Apps:
                    Dal.GetInstance.DeleteById("App_Links", "App_Id", entity.Id);
                    break;
                case EntityType.Books:
                    Dal.GetInstance.DeleteById("Book_Links", "Books_Id", entity.Id);
                    break;
                case EntityType.Games:
                    Dal.GetInstance.DeleteById("Gamez_Links", "Gamez_Id", entity.Id);
                    break;
                case EntityType.Movie:
                    Dal.GetInstance.DeleteById("Movie_Links", "Movie_Id", entity.Id);
                    break;
                case EntityType.Music:
                    Dal.GetInstance.DeleteById("Music_Links", "Music_Id", entity.Id);
                    break;
                case EntityType.Nds:
                    Dal.GetInstance.DeleteById("Nds_Links", "Nds_ID", entity.Id);
                    break;
                case EntityType.Series:
                    Dal.GetInstance.DeleteById("Series_Links", "Series_Id", entity.SerieId);
                    break;
                case EntityType.XXX:
                    Dal.GetInstance.DeleteById("XXX_Links", "XXX_Id", entity.Id);
                    break;
            }
        }


        public static Links GetLink(string value, IMyCollectionsData entity)
        {
            switch (entity.ObjectType)
            {
                case EntityType.Apps:
                    return Dal.GetInstance.GetLink(value, entity.Id, "App_Links", "App_Id");
                case EntityType.Books:
                    return Dal.GetInstance.GetLink(value, entity.Id, "Book_Links", "Books_Id");
                case EntityType.Games:
                    return Dal.GetInstance.GetLink(value, entity.Id, "Gamez_Links", "Gamez_Id");
                case EntityType.Movie:
                    return Dal.GetInstance.GetLink(value, entity.Id, "Movie_Links", "Movie_Id");
                case EntityType.Music:
                    return Dal.GetInstance.GetLink(value, entity.Id, "Music_Links", "Music_Id");
                case EntityType.Nds:
                    return Dal.GetInstance.GetLink(value, entity.Id, "Nds_Links", "Nds_ID");
                case EntityType.Series:
                    return Dal.GetInstance.GetLink(value, entity.SerieId, "Series_Links", "Series_Id");
                case EntityType.XXX:
                    return Dal.GetInstance.GetLink(value, entity.Id, "XXX_Links", "XXX_Id");
            }

            return null;
        }

        private static Links SaveLinks(string value, IMyCollectionsData entity)
        {
            string entityId = entity.Id;

            switch (entity.ObjectType)
            {
                case EntityType.Apps:
                    return Dal.GetInstance.CreateLinks("App_Links", "App_Id", entityId, "Url", value);
                case EntityType.Books:
                    return Dal.GetInstance.CreateLinks("Book_Links", "Books_Id", entityId, "Url", value);
                case EntityType.Games:
                    return Dal.GetInstance.CreateLinks("Gamez_Links", "Gamez_Id", entityId, "Url", value);
                case EntityType.Movie:
                    return Dal.GetInstance.CreateLinks("Movie_Links", "Movie_Id", entityId, "Url", value);
                case EntityType.Music:
                    return Dal.GetInstance.CreateLinks("Music_Links", "Music_Id", entityId, "Url", value);
                case EntityType.Nds:
                    return Dal.GetInstance.CreateLinks("Nds_Links", "Nds_ID", entityId, "Url", value);
                case EntityType.Series:
                    return Dal.GetInstance.CreateLinks("Series_Links", "Series_Id", entity.SerieId, "Url", value);
                case EntityType.XXX:
                    return Dal.GetInstance.CreateLinks("XXX_Links", "XXX_Id", entityId, "Url", value);
            }
            return null;
        }

    }
}
