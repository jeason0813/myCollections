using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using Image = System.Drawing.Image;

namespace myCollections.BL.Services
{
    class RessourcesServices
    {
        public static void AddBackground(byte[] objImage, IMyCollectionsData entity)
        {
            if (objImage == null) return;

            Ressource objRessource = new Ressource();
            objRessource.IsDefault = false;
            objRessource.ItemId = entity.Id;
            objRessource.ResourcesType = CommonServices.GetRessourceType("Background");
            objRessource.Value = objImage;

            if (entity.Ressources == null)
                entity.Ressources = new List<Ressource>();

            entity.Ressources.Add(objRessource);
        }
        public static void AddImage(byte[] objImage, IMyCollectionsData entity, bool isDefault)
        {
            byte[] image = Util.CreateSmallCover(objImage, Util.ThumbHeight, Util.ThumbWidth);
            if (image != null)
            {

                Ressource objRessource = new Ressource();
                objRessource.ItemId = entity.Id;
                objRessource.ResourcesType = CommonServices.GetRessourceType("Image");
                objRessource.Value = objImage;

                if (isDefault == true)
                {
                    foreach (Ressource item in entity.Ressources)
                    {
                        if (item.IsDefault == true)
                        {
                            item.IsDefault = false;
                            break;
                        }
                    }
                    entity.Cover = image;
                }
                objRessource.IsDefault = isDefault;
                entity.Ressources.Add(objRessource);
            }
        }

        public static void DeleteImage(Ressource item, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Apps:
                    Dal.GetInstance.DeleteById("Apps_Ressources", "Id", item.Id);
                    break;
                case EntityType.Books:
                    Dal.GetInstance.DeleteById("Books_Ressources", "id", item.Id);
                    break;
                case EntityType.Games:
                    Dal.GetInstance.DeleteById("Gamez_Ressources", "Id", item.Id);
                    break;
                case EntityType.Movie:
                    Dal.GetInstance.DeleteById("Movie_Ressources", "Id", item.Id);
                    break;
                case EntityType.Music:
                    Dal.GetInstance.DeleteById("Music_Ressources", "Id", item.Id);
                    break;
                case EntityType.Nds:
                    Dal.GetInstance.DeleteById("Nds_Ressources", "Id", item.Id);
                    break;
                case EntityType.Series:
                    Dal.GetInstance.DeleteById("SeriesSeason_Ressources", "Id", item.Id);
                    break;
                case EntityType.XXX:
                    Dal.GetInstance.DeleteById("XXX_Ressources", "Id", item.Id);
                    break;
            }
        }
        public static void DeleteRessources(string entityId, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Apps:
                    Dal.GetInstance.DeleteById("Apps_Ressources", "Apps_Id", entityId);
                    break;
                case EntityType.Books:
                    Dal.GetInstance.DeleteById("Books_Ressources", "Books_Id", entityId);
                    break;
                case EntityType.Games:
                    Dal.GetInstance.DeleteById("Gamez_Ressources", "Gamez_Id", entityId);
                    break;
                case EntityType.Movie:
                    Dal.GetInstance.DeleteById("Movie_Ressources", "Movie_Id", entityId);
                    break;
                case EntityType.Music:
                    Dal.GetInstance.DeleteById("Music_Ressources", "Music_Id", entityId);
                    break;
                case EntityType.Nds:
                    Dal.GetInstance.DeleteById("Nds_Ressources", "Nds_Id", entityId);
                    break;
                case EntityType.Series:
                    Dal.GetInstance.DeleteById("SeriesSeason_Ressources", "SeriesSeason_Id", entityId);
                    break;
                case EntityType.XXX:
                    Dal.GetInstance.DeleteById("XXX_Ressources", "XXX_Id", entityId);
                    break;
            }
        }

        public static byte[] GetBackground(IMyCollectionsData entity)
        {
            byte[] objBackground = null;
            if (entity != null)
            {
                IEnumerable<Ressource> objRessources = entity.Ressources;

                if (objRessources != null)
                {
                    foreach (Ressource item in objRessources)
                    {
                        if (item.ResourcesType.Name == "Background")
                        {
                            objBackground = item.Value;
                            break;
                        }

                    }
                }
            }

            return objBackground;
        }
        public static byte[] GetDefaultCover(IMyCollectionsData entity, out int index)
        {
            index = 0;
            if (entity != null)
                foreach (Ressource item in entity.Ressources)
                {

                    if (item.IsDefault == true && item.IsOld == false)
                        return item.Value;
                    else
                        index++;
                }
            return null;
        }

        public static Ressource GetRessource(string id, EntityType entityType, string ressourceType)
        {
            switch (entityType)
            {
                case EntityType.Apps:
                    //FIX since 2.7.11.0
                    return Dal.GetInstance.GetRessource(id, "Apps_Ressources", "Apps_Id", ressourceType);
                case EntityType.Books:
                    return Dal.GetInstance.GetRessource(id, "Books_Ressources", "Books_Id", ressourceType);
                case EntityType.Games:
                    return Dal.GetInstance.GetRessource(id, "Gamez_Ressources", "Gamez_Id", ressourceType);
                case EntityType.Movie:
                    return Dal.GetInstance.GetRessource(id, "Movie_Ressources", "Movie_Id", ressourceType);
                case EntityType.Music:
                    return Dal.GetInstance.GetRessource(id, "Music_Ressources", "Music_Id", ressourceType);
                case EntityType.Nds:
                    return Dal.GetInstance.GetRessource(id, "Nds_Ressources", "Nds_Id", ressourceType);
                case EntityType.Series:
                    return Dal.GetInstance.GetRessource(id, "SeriesSeason_Ressources", "SeriesSeason_Id", ressourceType);
                case EntityType.XXX:
                    return Dal.GetInstance.GetRessource(id, "XXX_Ressources", "XXX_Id", ressourceType);
            }
            return null;
        }

        public static void SetDefaultCover(int index, IMyCollectionsData entity)
        {
            foreach (Ressource item in entity.Ressources)
            {
                if (item.IsDefault == true)
                {
                    item.IsDefault = false;
                    break;
                }
            }

            if (entity.Ressources.Count > 0)
            {
                Ressource image = entity.Ressources.ElementAt(index);
                image.IsDefault = true;
                entity.Cover = Util.CreateSmallCover(image.Value, Util.ThumbHeight, Util.ThumbWidth);
            }
        }
        public static void SetBackground(int index, IMyCollectionsData entity)
        {
            ResourcesType backgroundType = CommonServices.GetRessourceType("Background");
            ResourcesType imageType = CommonServices.GetRessourceType("Image");
            foreach (Ressource item in entity.Ressources)
            {
                if (item.ResourcesType.Id == backgroundType.Id)
                {
                    item.ResourcesType = imageType;
                    break;
                }
            }

            Ressource image = entity.Ressources[index];
            image.ResourcesType.Id = backgroundType.Id;
            image.ResourcesType.Name = backgroundType.Name;
            image.ResourcesType = backgroundType;
        }

        public static string Print(int index, IMyCollectionsData entity)
        {
            if (entity.Ressources.Any())
            {
                Ressource image = entity.Ressources[index];
                if (image != null)
                {
                    PrintDocument printDocument = new PrintDocument();
                    printDocument.PrintPage += (sender, args) => DrawImage(image, args.Graphics, entity, printDocument);
                    printDocument.Print();
                    return string.Empty;
                }
                return "Nothing to print";
            }
            else
                return "Nothing to print";
        }

        private static void DrawImage(Ressource image, Graphics graphics, IMyCollectionsData entity, PrintDocument printDocument)
        {
            MemoryStream objImageMemory = new MemoryStream(image.Value, 0, image.Value.Length);
            objImageMemory.Write(image.Value, 0, image.Value.Length);
            Image imgCover = Image.FromStream(objImageMemory, true);

            if ((entity.ObjectType == EntityType.Music || entity.ObjectType == EntityType.Games || entity.ObjectType == EntityType.Nds) && image.IsDefault == true)
            {
                graphics.DrawImage(imgCover, new Rectangle(100, 100, 473, 473));
            }
            else if (image.IsDefault == true)
            {
                graphics.DrawImage(imgCover, new Rectangle(100, 100, 524, 721));
            }
            else if (imgCover.Width > printDocument.DefaultPageSettings.PrintableArea.Width || imgCover.Height > printDocument.DefaultPageSettings.PrintableArea.Height)
            {
                graphics.DrawImage(imgCover, new Rectangle(100, 100, (int)printDocument.DefaultPageSettings.PrintableArea.Width - 200, (int)printDocument.DefaultPageSettings.PrintableArea.Height - 200));
            }
            else
            {
                graphics.DrawImage(imgCover, new Rectangle(100, 100, imgCover.Width - 200, imgCover.Height - 200));
            }
            imgCover.Dispose();
            objImageMemory.Dispose();
        }


        public static void UpdateRessources(IMyCollectionsData entity)
        {
            if (entity != null)
            {
                DeleteRessources(entity.Id, entity.ObjectType);
                if (entity.Ressources != null && entity.Ressources.Count > 0)
                {
                    foreach (Ressource ressource in entity.Ressources)
                        if (ressource.IsOld == false)
                            switch (entity.ObjectType)
                            {
                                case EntityType.Apps:
                                    Dal.GetInstance.AddRessource("Apps_Ressources", "Apps_Id", entity.Id, ressource.ResourcesType.Id, ressource.Value, ressource.Link, ressource.IsDefault);
                                    break;
                                case EntityType.Books:
                                    Dal.GetInstance.AddRessource("Books_Ressources", "Books_Id", entity.Id, ressource.ResourcesType.Id, ressource.Value, ressource.Link, ressource.IsDefault);
                                    break;
                                case EntityType.Games:
                                    Dal.GetInstance.AddRessource("Gamez_Ressources", "Gamez_Id", entity.Id, ressource.ResourcesType.Id, ressource.Value, ressource.Link, ressource.IsDefault);
                                    break;
                                case EntityType.Movie:
                                    Dal.GetInstance.AddRessource("Movie_Ressources", "Movie_Id", entity.Id, ressource.ResourcesType.Id, ressource.Value, ressource.Link, ressource.IsDefault);
                                    break;
                                case EntityType.Music:
                                    Dal.GetInstance.AddRessource("Music_Ressources", "Music_Id", entity.Id, ressource.ResourcesType.Id, ressource.Value, ressource.Link, ressource.IsDefault);
                                    break;
                                case EntityType.Nds:
                                    Dal.GetInstance.AddRessource("Nds_Ressources", "Nds_Id", entity.Id, ressource.ResourcesType.Id, ressource.Value, ressource.Link, ressource.IsDefault);
                                    break;
                                case EntityType.Series:
                                    Dal.GetInstance.AddRessource("SeriesSeason_Ressources", "SeriesSeason_Id", entity.Id, ressource.ResourcesType.Id, ressource.Value, ressource.Link, ressource.IsDefault);
                                    break;
                                case EntityType.XXX:
                                    Dal.GetInstance.AddRessource("XXX_Ressources", "XXX_Id", entity.Id, ressource.ResourcesType.Id, ressource.Value, ressource.Link, ressource.IsDefault);
                                    break;

                            }
                }
            }
        }


    }
}
