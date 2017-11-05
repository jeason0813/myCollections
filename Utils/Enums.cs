namespace myCollections.Utils
{
    public enum BrowserType
    {
        None,
        Firefox4,
        Firefox8,
        Firefox10
    }
    public enum EntityType
    {
        Apps,
        Artist,
        Books,
        Games,
        LateLoan,
        Loan,
        Movie,
        Music,
        Nds,
        Series,
        XXX
    }
    public enum EntityService
    {
        AppServices,
        BookServices,
        GameServices,
        MovieServices,
        MusicServices,
        NdsServices,
        SerieServices,
        XxxServices
    }
    public enum Filter
    {
        All,
        Deleted,
        ToBeDeleted,
        Wish,
        Complete,
        NotComplete,
        NoCovers,
        Covers,
        NotSeen,
        Seen,
        Find,
        ToWatch
    }
    public enum Provider
    {
        AdoroCinema,
        AduldtBluRayHDDvd,
        AduldtDvdEmpire,
        AlloCine,
        Amazon,
        Bing,
        CdUniverse,
        Dorcel,
        Fnac,
        FilmStarts,
        GraceNote,
        HotMovies,
        Iafd,
        IMDB,
        JeuxVideo,
        LastFM,
        MusicBrainz,
        NokiaMusic,
        Orgazmik,
        Softpedia,
        Softonic,
        SugarVod,
        TheGamesDB,
        Tmdb,
        Tucows,
        Tvdb
    }
    public enum Order
    {
        Name,
        Added,
        Runtime,
        Media,
        MyRating,
        Artist,
        Type,
        PublicRating,
        NumId
    }
    public enum SearchType
    {
        Apps,
        Artist,
        Books,
        Games,
        Movie,
        Music,
        Nds,
        Series,
        XXX
    }
    public enum SearchMode
    {
        All,
        Provider
    }
    public enum EntityAction
    {
        Show,
        Dupe,
        Updated,
        Search,
        None,
        Added,
        Deleted
    }
    public enum View
    {
        Cover,
        Card,
        CoverFlow,
        Cube,
        Artist
    }
    public enum GroupBy
    {
        Media,
        Album,
        Serie,
        None,
        Type,
        Artist
    }
    public enum LanguageType
    {
        FR,
        EN,
        DU,
        DE,
        ES,
        BR,
        RU,
        UK,
        IT,
        CN,
        TK,
        ZH,
        PT,
        PE
    }

    public enum AmazonIndex
    {
        Books,
        Music,
        DVD,
        Classical,
        MusicTracks,
        Software,
        VideoGames
    }
    public enum AmazonCountry
    {
        com,
        de,
        fr,
        it,
        cn,
        es
    }
    public enum AmazonBrowserNode
    {
        DSUs = 11075831,
        DSFr = 13913851,
        DSDe = 13840371,
        DSIt = 435493031,
        DSEs = 911247031,
        None = 0
    }
    public enum CurrentUc
    {
        Info,
        Track,
        Cast,
        Types,
        Technical
    }
    public enum ManageItemsType
    {
        XxxTypes,
        MusicTypes,
        MovieTypes,
        GameTypes,
        BookType,
        AppType
    }
    public enum CleanTitleType
    {
        All,
        Apps,
        Books,
        Games,
        Movie,
        Music,
        Nds,
        Series,
        XXX
    }

    public enum GamesPlateform
    {
       All,
       Nds
    }

    public enum SupportedDevice
    {
        Dune,
        Mede8er,
        MyMovies,
        Tvix,
        WDHDTV,
        WindowsMediaCenter,
        XBMC
    }

    public enum BooksImport
    {
        BibTex,
        XML
    }

    public enum MusicImport
    {
        Csv,
        XML
    }

    public enum MoviesImport
    {
        FilmotechXml,
        FilmotechCsv
    }

    public enum GraceNoteLanguage
    {
        fre,
        eng,
        dut,
        ger,
        spa,
        por,
        rus,
        ita,
        qtd,
        tur
    }

    public enum Jobs
    {
        Singer,
        Author,
        Actor,
        Director
    }

}

