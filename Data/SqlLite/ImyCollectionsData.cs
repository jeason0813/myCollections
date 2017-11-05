using System.Collections.Generic;
using System.ComponentModel;
using myCollections.Utils;
using System;

namespace myCollections.Data.SqlLite
{
    public interface IMyCollectionsData : INotifyPropertyChanged
    {
        DateTime AddedDate { get; set; }
        AspectRatio AspectRatio { get; set; }
        string BarCode { get; set; }
        int? MyRating { get; set; }
        double? PublicRating { get; set; }
        string Comments { get; set; }
        byte[] Cover { get; set; }
        string CoverTheme { get; }
        string Description { get; set; }
        string FileName { get; set; }
        string FilePath { get; set; }
        string Id { get; set; }
        bool IsComplete { get; set; }
        bool IsDeleted { get; set; }
        bool IsWhish { get; set; }
        EntityType ObjectType { get; }
        string OriginalTitle { get; set; }
        int? NumId { get; set; }
        int? Runtime { get; set; }
        DateTime? ReleaseDate { get; set; }
        string SerieId { get; set; }
        Publisher Publisher { get; set; }
        string Title { get; set; }
        bool ToBeDeleted { get; set; }
        bool ToWatch { get; set; }
        bool Watched { get; set; }

        IList<Artist> Artists { get; set; }
        IList<Audio> Audios { get; set; }
        IList<Ressource> Ressources { get; set; }
        IList<Genre> Genres { get; set; }
        IList<Links> Links { get; set; }
        IList<Language> Subtitles { get; set; }
        IList<Track> Tracks { get; set; }

        Language Language { get; set; }
        FileFormat FileFormat { get; set; }
        Platform Platform { get; set; }

        Media Media { get; set; }
        IList<MetaData> MetaDatas { get; set; }

    }
}
