namespace myCollections.Data.SqlLite
{
    static class UpgradeScripts
    {
        #region Apps
        public const string AddSmallCoverAppsColumn = @"ALTER TABLE Apps ADD COLUMN Cover blob;";
        public const string AddToTestAppsColumn = @"ALTER TABLE Apps ADD COLUMN ToTest bit NOT NULL default 0;";
        public const string AddPublicRatingAppsColumn = @"ALTER TABLE Apps ADD COLUMN PublicRating float(50);";
        public const string AddNumIdAppsColumn = @"ALTER TABLE Apps ADD COLUMN NumID integer;";
        #endregion
        #region Books
        public const string AddSmallCoverBooksColumn = @"ALTER TABLE Books ADD COLUMN Cover blob";
        public const string AddToReadBooksColumn = @"ALTER TABLE Books ADD COLUMN ToRead bit NOT NULL default 0";
        public const string AddPublicRatingBooksColumn = @"ALTER TABLE Books ADD COLUMN PublicRating float(50)";
        public const string AddNumIdBooksColumn = @"ALTER TABLE Books ADD COLUMN NumID integer;";
        #endregion
        #region Games
        public const string AddSmallCoverGamesColumn = @"ALTER TABLE Gamez ADD COLUMN Cover blob";
        public const string AddPlateformTable = @"CREATE TABLE Platform (
                                                  Id    varchar(50) PRIMARY KEY NOT NULL,
                                                  Name  varchar(50) NOT NULL UNIQUE
                                                );";
        public const string AddPlateformColumn = @"ALTER TABLE Gamez
                                                          ADD COLUMN Platform_Id varchar(50);";
        public const string AddPriceColumn = @"ALTER TABLE Gamez
                                                          ADD COLUMN Price integer;";
        public const string UpdateGamesWithPlateformPrice = @"CREATE TABLE Gamez01(
                                                              Id,
                                                              Language_Id,
                                                              Media_Id,
                                                              Editor_Id,
                                                              Title,
                                                              BarCode,
                                                              Rating,
                                                              Descriptions,
                                                              ReleaseDate,
                                                              Comments,
                                                              FileName,
                                                              AddedDate,
                                                              IsDeleted,
                                                              IsWhish,
                                                              IsComplete,
                                                              FilePath,
                                                              IsTested,
                                                              ToBeDeleted,
                                                              Rated,
                                                              Cover,
                                                              Platform_Id,
                                                              Price
                                                            );

                                                            INSERT INTO Gamez01
                                                            SELECT
                                                              Id,
                                                              Language_Id,
                                                              Media_Id,
                                                              Editor_Id,
                                                              Title,
                                                              BarCode,
                                                              Rating,
                                                              Descriptions,
                                                              ReleaseDate,
                                                              Comments,
                                                              FileName,
                                                              AddedDate,
                                                              IsDeleted,
                                                              IsWhish,
                                                              IsComplete,
                                                              FilePath,
                                                              IsTested,
                                                              ToBeDeleted,
                                                              Rated,
                                                              Cover,
                                                              Platform_Id,
                                                              Price
                                                            FROM Gamez;

                                                            DROP TABLE Gamez;

                                                            CREATE TABLE Gamez (
                                                              Id            varchar PRIMARY KEY NOT NULL,
                                                              Language_Id   varchar,
                                                              Media_Id      varchar,
                                                              Editor_Id     varchar(50),
                                                              Title         varchar NOT NULL COLLATE NOCASE,
                                                              BarCode       varchar(50),
                                                              Rating        int,
                                                              Descriptions  text COLLATE NOCASE,
                                                              ReleaseDate   timestamp,
                                                              Comments      text COLLATE NOCASE,
                                                              FileName      varchar COLLATE NOCASE,
                                                              AddedDate     timestamp NOT NULL,
                                                              IsDeleted     bit NOT NULL,
                                                              IsWhish       bit NOT NULL,
                                                              IsComplete    bit NOT NULL,
                                                              FilePath      varchar(200),
                                                              IsTested      boolean NOT NULL DEFAULT False,
                                                              ToBeDeleted   boolean NOT NULL DEFAULT False,
                                                              Rated         integer,
                                                              Cover    blob,
                                                              Platform_Id   varchar(50),
                                                              Price         integer,
                                                              /* Foreign keys */
                                                              FOREIGN KEY (Media_Id)
                                                                REFERENCES Media(Id), 
                                                              FOREIGN KEY (Language_Id)
                                                                REFERENCES Language(Id), 
                                                              FOREIGN KEY (Editor_Id)
                                                                REFERENCES App_Editor(Id), 
                                                              FOREIGN KEY (Platform_Id)
                                                                REFERENCES Platform(Id)
                                                            );

                                                            INSERT INTO Gamez
                                                              (Id,
                                                              Language_Id,
                                                              Media_Id,
                                                              Editor_Id,
                                                              Title,
                                                              BarCode,
                                                              Rating,
                                                              Descriptions,
                                                              ReleaseDate,
                                                              Comments,
                                                              FileName,
                                                              AddedDate,
                                                              IsDeleted,
                                                              IsWhish,
                                                              IsComplete,
                                                              FilePath,
                                                              IsTested,
                                                              ToBeDeleted,
                                                              Rated,
                                                              Cover,
                                                              Platform_Id,
                                                              Price)
                                                            SELECT
                                                              Id,
                                                              Language_Id,
                                                              Media_Id,
                                                              Editor_Id,
                                                              Title,
                                                              BarCode,
                                                              Rating,
                                                              Descriptions,
                                                              ReleaseDate,
                                                              Comments,
                                                              FileName,
                                                              AddedDate,
                                                              IsDeleted,
                                                              IsWhish,
                                                              IsComplete,
                                                              FilePath,
                                                              IsTested,
                                                              ToBeDeleted,
                                                              Rated,
                                                              Cover,
                                                              Platform_Id,
                                                              Price
                                                            FROM Gamez01;

                                                            DROP TABLE Gamez01;";
        public const string AddToTestGamesColumn = @"ALTER TABLE Gamez ADD COLUMN ToTest bit NOT NULL default 0";
        public const string AddPlateformDisplayNameColumn = @"ALTER TABLE Platform ADD COLUMN DisplayName text;";
        public const string AddPublicRatingGamesColumn = @"ALTER TABLE Gamez ADD COLUMN PublicRating float(50)";
        public const string AddNumIdGamesColumn = @"ALTER TABLE Gamez ADD COLUMN NumID integer;";
        #endregion
        #region Movie
        public const string AddSmallCoverMovieColumn = @"ALTER TABLE Movie ADD COLUMN Cover blob";
        public const string AddAddedJobMovieColumn = @"ALTER TABLE Movie_Artist_Job  ADD COLUMN Added datetime;";
        public const string CreateAspectRatioTable = @"CREATE TABLE AspectRatio (
                                                            Id     varchar PRIMARY KEY NOT NULL,
                                                            Name   varchar COLLATE NOCASE,
                                                            Image  blob
                                                            );
                                                        CREATE UNIQUE INDEX AspectRatio_AspectRatioNameUnique
                                                          ON AspectRatio
                                                          (Name);";
        public const string AddAspectRatioMovieColumn = @"CREATE TABLE Movie01(
                                                      Id,
                                                      Studio_Id,
                                                      Media_Id,
                                                      FileFormat_Id,
                                                      Title,
                                                      BarCode,
                                                      Imdb,
                                                      AlloCine,
                                                      Rating,
                                                      Description,
                                                      Runtime,
                                                      Tagline,
                                                      Rated,
                                                      ReleaseDate,
                                                      Seen,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      FileName,
                                                      FilePath,
                                                      OriginalTitle,
                                                      TobeDeleted,
                                                      Comments,
                                                      Country,
                                                      Cover
                                                      );

                                                    INSERT INTO Movie01
                                                    SELECT
                                                      Id,
                                                      Studio_Id,
                                                      Media_Id,
                                                      FileFormat_Id,
                                                      Title,
                                                      BarCode,
                                                      Imdb,
                                                      AlloCine,
                                                      Rating,
                                                      Description,
                                                      Runtime,
                                                      Tagline,
                                                      Rated,
                                                      ReleaseDate,
                                                      Seen,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      FileName,
                                                      FilePath,
                                                      OriginalTitle,
                                                      TobeDeleted,
                                                      Comments,
                                                      Country,
                                                      Cover
                                                    FROM Movie;

                                                    DROP TABLE Movie;

                                                    CREATE TABLE Movie (
                                                      Id              varchar(50) PRIMARY KEY NOT NULL,
                                                      Studio_Id       varchar(50),
                                                      Media_Id        varchar(50),
                                                      FileFormat_Id   varchar(50),
                                                      AspectRatio_Id  varchar(50),
                                                      Title           varchar(250) NOT NULL,
                                                      BarCode         varchar(50),
                                                      Imdb            varchar(5),
                                                      AlloCine        varchar(5),
                                                      Rating          int,
                                                      Description     text,
                                                      Runtime         int,
                                                      Tagline         text,
                                                      Rated           int,
                                                      ReleaseDate     datetime,
                                                      Seen            bit NOT NULL,
                                                      IsDeleted       bit NOT NULL,
                                                      IsWhish         bit NOT NULL,
                                                      IsComplete      bit NOT NULL,
                                                      AddedDate       timestamp NOT NULL,
                                                      FileName        varchar(250),
                                                      FilePath        varchar(250),
                                                      OriginalTitle   varchar(250),
                                                      TobeDeleted     boolean NOT NULL DEFAULT 0,
                                                      Comments        text,
                                                      Country         varchar(50),
                                                      Cover      blob,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id), 
                                                      FOREIGN KEY (Studio_Id)
                                                        REFERENCES Movie_Studio(Id), 
                                                      FOREIGN KEY (FileFormat_Id)
                                                        REFERENCES FileFormat(Id), 
                                                      FOREIGN KEY (AspectRatio_Id)
                                                        REFERENCES AspectRatio(Id)
                                                    );

                                                    INSERT INTO Movie
                                                      (Id,
                                                      Studio_Id,
                                                      Media_Id,
                                                      FileFormat_Id,
                                                      Title,
                                                      BarCode,
                                                      Imdb,
                                                      AlloCine,
                                                      Rating,
                                                      Description,
                                                      Runtime,
                                                      Tagline,
                                                      Rated,
                                                      ReleaseDate,
                                                      Seen,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      FileName,
                                                      FilePath,
                                                      OriginalTitle,
                                                      TobeDeleted,
                                                      Comments,
                                                      Country,
                                                      Cover)
                                                    SELECT
                                                      Id,
                                                      Studio_Id,
                                                      Media_Id,
                                                      FileFormat_Id,
                                                      Title,
                                                      BarCode,
                                                      Imdb,
                                                      AlloCine,
                                                      Rating,
                                                      Description,
                                                      Runtime,
                                                      Tagline,
                                                      Rated,
                                                      ReleaseDate,
                                                      Seen,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      FileName,
                                                      FilePath,
                                                      OriginalTitle,
                                                      TobeDeleted,
                                                      Comments,
                                                      Country,
                                                      Cover
                                                    FROM Movie01;

                                                    DROP TABLE Movie01;";
        public const string AddToWatchMoviesColumn = @"ALTER TABLE Movie ADD COLUMN ToWatch bit NOT NULL default 0";
        public const string AddGoofsMoviesColumn = @"ALTER TABLE Movie ADD COLUMN Goofs text";
        public const string AddPublicRatingMoviesColumn = @"ALTER TABLE Movie ADD COLUMN PublicRating float(50)";
        public const string AddNumIdMoviesColumn = @"ALTER TABLE Movie ADD COLUMN NumID integer;";

        #endregion
        #region Music
        public const string AddSmallCoverMusicColumn = @"ALTER TABLE Music ADD COLUMN Cover blob";
        public const string CreateMusicMusicGenreTable = @"CREATE TABLE IF NOT EXISTS Music_MusicGenre (
                                                          Id          varchar(50) PRIMARY KEY NOT NULL,
                                                          Music_Id    varchar(50) NOT NULL,
                                                          MusicGenre_Id  varchar(50) NOT NULL,
                                                          FOREIGN KEY (MusicGenre_Id)
                                                            REFERENCES ""[Music_Genre]""(Id), 
                                                          FOREIGN KEY (Music_Id)
                                                            REFERENCES ""[Music]""(Id)
                                                            );";

        public const string CreateMusicTracksTable = @"CREATE TABLE IF NOT EXISTS Track (
                                                          Id        varchar(50) PRIMARY KEY NOT NULL,
                                                          Music_Id  varchar(50) NOT NULL,
                                                          Title     nvarchar(250) NOT NULL,
                                                          Path      text,
                                                          /* Foreign keys */
                                                          FOREIGN KEY (Music_Id)
                                                            REFERENCES Music(Id)
                                                        );";
        public const string AddToHearMusicColumn = @"ALTER TABLE Music ADD COLUMN ToHear bit NOT NULL default 0";
        public const string AddPublicRatingMusicColumn = @"ALTER TABLE Music ADD COLUMN PublicRating float(50)";
        public const string AddNumIdMusicColumn = @"ALTER TABLE Music ADD COLUMN NumID integer;";

        #endregion
        #region Nds
        public const string AddSmallCoverNdsColumn = @"ALTER TABLE Nds ADD COLUMN Cover blob";
        public const string AddToTestNdsColumn = @"ALTER TABLE Nds ADD COLUMN ToTest bit NOT NULL default 0";
        public const string AddPublicRatingNdsColumn = @"ALTER TABLE Nds ADD COLUMN PublicRating float(50)";
        public const string AddNumIdNdsColumn = @"ALTER TABLE Nds ADD COLUMN NumID integer;";

        #endregion
        #region Series
        #region Delete
        public const string DeleteSeriesEpisodes = "DROP TABLE Series_Episode;";
        public const string DeleteSeriesSeason = "DROP TABLE Series_Season;";
        public const string DeleteSeries = "DROP TABLE Series;";
        public const string DeleteSeriesArtistJob = "DROP TABLE Series_Artist_Job;";
        public const string DeleteSeriesRessources = "DROP TABLE Series_Ressources;";
        public const string AddSmallCoverSeriesColumn = @"ALTER TABLE Series_Season ADD COLUMN Cover blob";
        #endregion
        #region Create
        #region Series_Season
        public const string CreateSeriesSeason = @"CREATE TABLE Series_Season ( 
                                              Id varchar(50) PRIMARY KEY NOT NULL,
                                              Series_Id          varchar(50) NOT NULL,
                                              Media_Id           varchar(50) NOT NULL,
                                              Season             integer NOT NULL,
                                              BarCode            varchar(50),
                                              TotalEpisodes      integer,
                                              AvailableEpisodes  integer,
                                              MissingEpisodes    integer,
                                              Seen               boolean NOT NULL DEFAULT False,
                                              AddedDate          datetime,
                                              IsComplete         boolean NOT NULL DEFAULT False,
                                              FilesPath          varchar(50),
                                              IsDeleted          boolean DEFAULT False,
                                              IsWhish            boolean DEFAULT False,
                                              ToBeDeleted        boolean NOT NULL DEFAULT False,
                                              Rating             integer,
                                              ReleaseDate        datetime,
                                              Comments           text,

                                              FOREIGN KEY (Media_Id)
                                                REFERENCES Media(Id), 
                                              FOREIGN KEY (Series_Id)
                                                REFERENCES Series(Id)
                                                );";
        #endregion
        #region Series
        public const string CreateSeries = @"CREATE TABLE Series (
                                              Id               varchar(50) PRIMARY KEY NOT NULL,
                                              Studio_Id        varchar(50),
                                              Title            varchar(50) NOT NULL,
                                              IsInProduction   boolean,
                                              Country          varchar(50),
                                              Description      text,
                                              OfficialWebSite  varchar(500),
                                              RunTime          integer,
                                              Rated            integer,
                                             
                                              FOREIGN KEY (Studio_Id)
                                                REFERENCES Movie_Studio(Id)
                                            );";
        #endregion
        #region Series_Artist_Job
        public const string CreateSeriesArtistJob = @"CREATE TABLE Series_Artist_Job (
                                                      Series_Id  varchar(50) NOT NULL,
                                                      Artist_Id  varchar(50) NOT NULL,
                                                      Job_Id     varchar(50) NOT NULL,
                                                      PRIMARY KEY (Series_Id, Artist_Id, Job_Id),
                                                      
                                                      FOREIGN KEY (Job_Id)
                                                        REFERENCES Job(Id), 
                                                      FOREIGN KEY (Artist_Id)
                                                        REFERENCES Artist(Id), 
                                                      FOREIGN KEY (Series_Id)
                                                        REFERENCES Series(Id)
                                                        );";
        #endregion
        public const string CreateSeriesSeasonRessources = @"CREATE TABLE Ressource (
                                                              Id                varchar(50) PRIMARY KEY NOT NULL,
                                                              SeriesSeason_Id         varchar(50) NOT NULL,
                                                              RessourceType_ID  varchar(50) NOT NULL,
                                                              Ressource         blob,
                                                              Link              varchar(500),
                                                              IsDefault         boolean NOT NULL,
                                                              /* Foreign keys */
                                                              FOREIGN KEY (SeriesSeason_Id)
                                                                REFERENCES Series_Season(Id), 
                                                              FOREIGN KEY (RessourceType_ID)
                                                                REFERENCES ResourcesType(Id)
                                                            );";
        #endregion
        public const string AddAddedJobSerieColumn = @"ALTER TABLE Series_Artist_Job  ADD COLUMN Added datetime;";
        public const string AddToWatchSeriesSeasonColumn = @"ALTER TABLE Series_Season ADD COLUMN ToWatch bit NOT NULL default 0";
        public const string AddPublicRatingSeriesSeasonColumn = @"ALTER TABLE Series_Season ADD COLUMN PublicRating float(50)";
        public const string AddNumIdSeriesSeasonColumn = @"ALTER TABLE Series_Season ADD COLUMN NumID integer;";
        #endregion
        #region XXX
        public const string AddSmallCoverXXXColumn = @"ALTER TABLE XXX ADD COLUMN Cover blob";
        public const string DeleteXXXArtistJobId = @"Begin Transaction;
                                                    Create  TABLE MAIN.[Temp_898596585](
                                                    [XXX_Id] varchar NOT NULL REFERENCES [XXX] ([Id]),
                                                    [Artist_Id] varchar NOT NULL REFERENCES [Artist] ([Id]),
                                                    [Job_Id] varchar(50) NOT NULL REFERENCES [Job] ([Id]) 
                                                    );
                                                    Insert Into MAIN.[Temp_898596585] ([XXX_Id],[Artist_Id],[Job_Id]) 
                                                     Select [XXX_Id],[Artist_Id],[Job_Id] From MAIN.[XXX_Artist_Job];
                                                    Drop Table MAIN.[XXX_Artist_Job];
                                                    Alter Table MAIN.[Temp_898596585] Rename To [XXX_Artist_Job];
                                                    Commit Transaction;
                                                    ";
        public const string UpdateXXXArtistJob = @"CREATE TABLE XXX_Artist_Job01(XXX_Id, Artist_Id, Job_Id);

                                                    INSERT INTO XXX_Artist_Job01
                                                    SELECT Distinct
                                                      XXX_Id,
                                                      Artist_Id,
                                                      Job_Id
                                                    FROM XXX_Artist_Job;

                                                    DROP TABLE XXX_Artist_Job;

                                                    CREATE TABLE XXX_Artist_Job (
                                                      XXX_Id     varchar(50) NOT NULL,
                                                      Artist_Id  varchar(50) NOT NULL,
                                                      Job_Id     varchar(50) NOT NULL,
                                                      Added      datetime,
                                                      PRIMARY KEY (XXX_Id, Artist_Id,Job_Id)
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Job_Id)
                                                        REFERENCES Job(Id)
                                                      FOREIGN KEY (Artist_Id)
                                                        REFERENCES Artist(Id)
                                                      FOREIGN KEY (XXX_Id)
                                                        REFERENCES XXX(Id)
                                                    );

                                                    INSERT INTO XXX_Artist_Job
                                                      (XXX_Id,
                                                      Artist_Id,
                                                      Job_Id)
                                                    SELECT
                                                      XXX_Id,
                                                      Artist_Id,
                                                      Job_Id
                                                    FROM XXX_Artist_Job01;

                                                    DROP TABLE XXX_Artist_Job01;";

        public const string AddToWatchXXXColumn = @"ALTER TABLE XXX ADD COLUMN ToWatch bit NOT NULL default 0";
        public const string AddPublicRatingXXXColumn = @"ALTER TABLE XXX ADD COLUMN PublicRating float(50)";
        public const string AddNumIdXXXColumn = @"ALTER TABLE XXX ADD COLUMN NumID integer;";

        #endregion
        #region Group
        #region deleteGroupFromApps
        public const string DeleteGroupFromApps = @"CREATE TABLE Apps01(
                                                      Id,
                                                      Title,
                                                      Language_Id,
                                                      Editor_Id,
                                                      Media_Id,
                                                      Version,
                                                      Description,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      FilePath,
                                                      FileName,
                                                      Rating,
                                                      IsTested,
                                                      Comments,
                                                      ToBeDeleted
                                                    );

                                                    INSERT INTO Apps01
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Language_Id,
                                                      Editor_Id,
                                                      Media_Id,
                                                      Version,
                                                      Description,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      FilePath,
                                                      FileName,
                                                      Rating,
                                                      IsTested,
                                                      Comments,
                                                      ToBeDeleted
                                                    FROM Apps;

                                                    DROP TABLE Apps;

                                                    CREATE TABLE Apps (
                                                      Id           varchar PRIMARY KEY NOT NULL,
                                                      Title        varchar NOT NULL,
                                                      BarCode      varchar(50),
                                                      Language_Id  varchar,
                                                      Editor_Id    varchar(50),
                                                      Media_Id     varchar(50) NOT NULL ON CONFLICT ROLLBACK,
                                                      Version      varchar,
                                                      Description  text,
                                                      IsDeleted    bit NOT NULL,
                                                      IsWhish      bit NOT NULL,
                                                      IsComplete   bit NOT NULL,
                                                      AddedDate    timestamp NOT NULL,
                                                      ReleaseDate  timestamp,
                                                      FilePath     varchar(50),
                                                      FileName     varchar(50),
                                                      Rating       integer,
                                                      IsTested     boolean DEFAULT False,
                                                      Comments     text,
                                                      ToBeDeleted  boolean NOT NULL DEFAULT False,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id), 
                                                      FOREIGN KEY (Language_Id)
                                                        REFERENCES Language(Id), 
                                                      FOREIGN KEY (Editor_Id)
                                                        REFERENCES App_Editor(Id)
                                                    );

                                                    INSERT INTO Apps
                                                      (Id,
                                                      Title,
                                                      Language_Id,
                                                      Editor_Id,
                                                      Media_Id,
                                                      Version,
                                                      Description,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      FilePath,
                                                      FileName,
                                                      Rating,
                                                      IsTested,
                                                      Comments,
                                                      ToBeDeleted)
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Language_Id,
                                                      Editor_Id,
                                                      Media_Id,
                                                      Version,
                                                      Description,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      FilePath,
                                                      FileName,
                                                      Rating,
                                                      IsTested,
                                                      Comments,
                                                      ToBeDeleted
                                                    FROM Apps01;

                                                    DROP TABLE Apps01;

                                                    CREATE TABLE Apps01(
                                                      Id,
                                                      Title,
                                                      Language_Id,
                                                      Editor_Id,
                                                      Media_Id,
                                                      Version,
                                                      Description,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      FilePath,
                                                      FileName,
                                                      Rating,
                                                      IsTested,
                                                      Comments,
                                                      ToBeDeleted
                                                    );

                                                    INSERT INTO Apps01
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Language_Id,
                                                      Editor_Id,
                                                      Media_Id,
                                                      Version,
                                                      Description,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      FilePath,
                                                      FileName,
                                                      Rating,
                                                      IsTested,
                                                      Comments,
                                                      ToBeDeleted
                                                    FROM Apps;

                                                    DROP TABLE Apps;

                                                    CREATE TABLE Apps (
                                                      Id           varchar PRIMARY KEY NOT NULL,
                                                      Title        varchar NOT NULL,
                                                      BarCode      varchar(50),
                                                      Language_Id  varchar,
                                                      Editor_Id    varchar(50),
                                                      Media_Id     varchar(50) NOT NULL ON CONFLICT ROLLBACK,
                                                      Version      varchar,
                                                      Description  text,
                                                      IsDeleted    bit NOT NULL,
                                                      IsWhish      bit NOT NULL,
                                                      IsComplete   bit NOT NULL,
                                                      AddedDate    timestamp NOT NULL,
                                                      ReleaseDate  timestamp,
                                                      FilePath     varchar(50),
                                                      FileName     varchar(50),
                                                      Rating       integer,
                                                      IsTested     boolean DEFAULT False,
                                                      Comments     text,
                                                      ToBeDeleted  boolean NOT NULL DEFAULT False,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id), 
                                                      FOREIGN KEY (Language_Id)
                                                        REFERENCES Language(Id), 
                                                      FOREIGN KEY (Editor_Id)
                                                        REFERENCES App_Editor(Id)
                                                    );

                                                    INSERT INTO Apps
                                                      (Id,
                                                      Title,
                                                      Language_Id,
                                                      Editor_Id,
                                                      Media_Id,
                                                      Version,
                                                      Description,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      FilePath,
                                                      FileName,
                                                      Rating,
                                                      IsTested,
                                                      Comments,
                                                      ToBeDeleted)
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Language_Id,
                                                      Editor_Id,
                                                      Media_Id,
                                                      Version,
                                                      Description,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      FilePath,
                                                      FileName,
                                                      Rating,
                                                      IsTested,
                                                      Comments,
                                                      ToBeDeleted
                                                    FROM Apps01;

                                                    DROP TABLE Apps01;";
        #endregion
        #region deleteGroupFromBooks
        public const string DeleteGroupFromBooks = @"CREATE TABLE Books01(
                                                      Id,
                                                      Title,
                                                      Editor_Id,
                                                      Format_Id,
                                                      Media_Id,
                                                      Language_Id,
                                                      ISBN,
                                                      NbrPages,
                                                      IsRead,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      Description,
                                                      FileName,
                                                      FilePath,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      ToBeDeleted,
                                                      Rating,
                                                      Comments,
                                                      Rated
                                                    );

                                                    INSERT INTO Books01
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Editor_Id,
                                                      Format_Id,
                                                      Media_Id,
                                                      Language_Id,
                                                      ISBN,
                                                      NbrPages,
                                                      IsRead,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      Description,
                                                      FileName,
                                                      FilePath,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      ToBeDeleted,
                                                      Rating,
                                                      Comments,
                                                      Rated
                                                    FROM Books;

                                                    DROP TABLE Books;

                                                    CREATE TABLE Books (
                                                      Id           varchar(50) PRIMARY KEY NOT NULL,
                                                      Title        varchar(50) NOT NULL,
                                                      Editor_Id    varchar(50),
                                                      Format_Id    varchar(50) NOT NULL,
                                                      Media_Id     varchar(50) NOT NULL,
                                                      Language_Id  varchar(50),
                                                      ISBN         varchar(50),
                                                      NbrPages     integer,
                                                      IsRead       boolean NOT NULL,
                                                      IsDeleted    boolean NOT NULL,
                                                      IsWhish      boolean NOT NULL,
                                                      IsComplete   boolean NOT NULL,
                                                      Description  text,
                                                      FileName     varchar(100),
                                                      FilePath     varchar(200),
                                                      AddedDate    datetime NOT NULL,
                                                      ReleaseDate  datetime,
                                                      ToBeDeleted  boolean NOT NULL,
                                                      Rating       integer,
                                                      Comments     text,
                                                      Rated        integer,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Editor_Id)
                                                        REFERENCES App_Editor(Id), 
                                                      FOREIGN KEY (Format_Id)
                                                        REFERENCES FileFormat(Id), 
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id), 
                                                      FOREIGN KEY (Language_Id)
                                                        REFERENCES Language(Id)
                                                    );

                                                    INSERT INTO Books
                                                      (Id,
                                                      Title,
                                                      Editor_Id,
                                                      Format_Id,
                                                      Media_Id,
                                                      Language_Id,
                                                      ISBN,
                                                      NbrPages,
                                                      IsRead,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      Description,
                                                      FileName,
                                                      FilePath,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      ToBeDeleted,
                                                      Rating,
                                                      Comments,
                                                      Rated)
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Editor_Id,
                                                      Format_Id,
                                                      Media_Id,
                                                      Language_Id,
                                                      ISBN,
                                                      NbrPages,
                                                      IsRead,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      Description,
                                                      FileName,
                                                      FilePath,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      ToBeDeleted,
                                                      Rating,
                                                      Comments,
                                                      Rated
                                                    FROM Books01;

                                                    DROP TABLE Books01;

                                                    CREATE TABLE Books01(
                                                      Id,
                                                      Title,
                                                      Editor_Id,
                                                      Format_Id,
                                                      Media_Id,
                                                      Language_Id,
                                                      ISBN,
                                                      NbrPages,
                                                      IsRead,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      Description,
                                                      FileName,
                                                      FilePath,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      ToBeDeleted,
                                                      Rating,
                                                      Comments,
                                                      Rated
                                                    );

                                                    INSERT INTO Books01
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Editor_Id,
                                                      Format_Id,
                                                      Media_Id,
                                                      Language_Id,
                                                      ISBN,
                                                      NbrPages,
                                                      IsRead,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      Description,
                                                      FileName,
                                                      FilePath,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      ToBeDeleted,
                                                      Rating,
                                                      Comments,
                                                      Rated
                                                    FROM Books;

                                                    DROP TABLE Books;

                                                    CREATE TABLE Books (
                                                      Id           varchar(50) PRIMARY KEY NOT NULL,
                                                      Title        varchar(50) NOT NULL,
                                                      BarCode      varchar(50),
                                                      Editor_Id    varchar(50),
                                                      Format_Id    varchar(50) NOT NULL,
                                                      Media_Id     varchar(50) NOT NULL,
                                                      Language_Id  varchar(50),
                                                      ISBN         varchar(50),
                                                      NbrPages     integer,
                                                      IsRead       boolean NOT NULL,
                                                      IsDeleted    boolean NOT NULL,
                                                      IsWhish      boolean NOT NULL,
                                                      IsComplete   boolean NOT NULL,
                                                      Description  text,
                                                      FileName     varchar(100),
                                                      FilePath     varchar(200),
                                                      AddedDate    datetime NOT NULL,
                                                      ReleaseDate  datetime,
                                                      ToBeDeleted  boolean NOT NULL,
                                                      Rating       integer,
                                                      Comments     text,
                                                      Rated        integer,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Editor_Id)
                                                        REFERENCES App_Editor(Id), 
                                                      FOREIGN KEY (Format_Id)
                                                        REFERENCES FileFormat(Id), 
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id), 
                                                      FOREIGN KEY (Language_Id)
                                                        REFERENCES Language(Id)
                                                    );

                                                    INSERT INTO Books
                                                      (Id,
                                                      Title,
                                                      Editor_Id,
                                                      Format_Id,
                                                      Media_Id,
                                                      Language_Id,
                                                      ISBN,
                                                      NbrPages,
                                                      IsRead,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      Description,
                                                      FileName,
                                                      FilePath,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      ToBeDeleted,
                                                      Rating,
                                                      Comments,
                                                      Rated)
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Editor_Id,
                                                      Format_Id,
                                                      Media_Id,
                                                      Language_Id,
                                                      ISBN,
                                                      NbrPages,
                                                      IsRead,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      Description,
                                                      FileName,
                                                      FilePath,
                                                      AddedDate,
                                                      ReleaseDate,
                                                      ToBeDeleted,
                                                      Rating,
                                                      Comments,
                                                      Rated
                                                    FROM Books01;

                                                    DROP TABLE Books01;";
        #endregion
        #region deleteGroupFromGamez
        public const string DeleteGroupFromGamez = @"CREATE TABLE Gamez01(
                                                      Id,
                                                      Language_Id,
                                                      Media_Id,
                                                      Editor_Id,
                                                      Title,
                                                      Rating,
                                                      Descriptions,
                                                      ReleaseDate,
                                                      Comments,
                                                      FileName,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      FilePath,
                                                      IsTested,
                                                      ToBeDeleted,
                                                      Rated
                                                    );

                                                    INSERT INTO Gamez01
                                                    SELECT
                                                      Id,
                                                      Language_Id,
                                                      Media_Id,
                                                      Editor_Id,
                                                      Title,
                                                      Rating,
                                                      Descriptions,
                                                      ReleaseDate,
                                                      Comments,
                                                      FileName,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      FilePath,
                                                      IsTested,
                                                      ToBeDeleted,
                                                      Rated
                                                    FROM Gamez;

                                                    DROP TABLE Gamez;

                                                    CREATE TABLE Gamez (
                                                      Id            varchar PRIMARY KEY NOT NULL,
                                                      Language_Id   varchar,
                                                      Media_Id      varchar,
                                                      Editor_Id     varchar(50),
                                                      Title         varchar NOT NULL COLLATE NOCASE,
                                                      Rating        int,
                                                      Descriptions  text COLLATE NOCASE,
                                                      ReleaseDate   timestamp,
                                                      Comments      text COLLATE NOCASE,
                                                      FileName      varchar COLLATE NOCASE,
                                                      AddedDate     timestamp NOT NULL,
                                                      IsDeleted     bit NOT NULL,
                                                      IsWhish       bit NOT NULL,
                                                      IsComplete    bit NOT NULL,
                                                      FilePath      varchar(200),
                                                      IsTested      boolean NOT NULL DEFAULT False,
                                                      ToBeDeleted   boolean NOT NULL DEFAULT False,
                                                      Rated         integer,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Editor_Id)
                                                        REFERENCES App_Editor(Id), 
                                                      FOREIGN KEY (Language_Id)
                                                        REFERENCES Language(Id), 
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id)
                                                    );

                                                    INSERT INTO Gamez
                                                      (Id,
                                                      Language_Id,
                                                      Media_Id,
                                                      Editor_Id,
                                                      Title,
                                                      Rating,
                                                      Descriptions,
                                                      ReleaseDate,
                                                      Comments,
                                                      FileName,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      FilePath,
                                                      IsTested,
                                                      ToBeDeleted,
                                                      Rated)
                                                    SELECT
                                                      Id,
                                                      Language_Id,
                                                      Media_Id,
                                                      Editor_Id,
                                                      Title,
                                                      Rating,
                                                      Descriptions,
                                                      ReleaseDate,
                                                      Comments,
                                                      FileName,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      FilePath,
                                                      IsTested,
                                                      ToBeDeleted,
                                                      Rated
                                                    FROM Gamez01;

                                                    DROP TABLE Gamez01;

                                                    CREATE TABLE Gamez01(
                                                      Id,
                                                      Language_Id,
                                                      Media_Id,
                                                      Editor_Id,
                                                      Title,
                                                      Rating,
                                                      Descriptions,
                                                      ReleaseDate,
                                                      Comments,
                                                      FileName,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      FilePath,
                                                      IsTested,
                                                      ToBeDeleted,
                                                      Rated
                                                    );

                                                    INSERT INTO Gamez01
                                                    SELECT
                                                      Id,
                                                      Language_Id,
                                                      Media_Id,
                                                      Editor_Id,
                                                      Title,
                                                      Rating,
                                                      Descriptions,
                                                      ReleaseDate,
                                                      Comments,
                                                      FileName,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      FilePath,
                                                      IsTested,
                                                      ToBeDeleted,
                                                      Rated
                                                    FROM Gamez;

                                                    DROP TABLE Gamez;

                                                    CREATE TABLE Gamez (
                                                      Id            varchar PRIMARY KEY NOT NULL,
                                                      Language_Id   varchar,
                                                      Media_Id      varchar,
                                                      Editor_Id     varchar(50),
                                                      Title         varchar NOT NULL COLLATE NOCASE,
                                                      BarCode      varchar(50),
                                                      Rating        int,
                                                      Descriptions  text COLLATE NOCASE,
                                                      ReleaseDate   timestamp,
                                                      Comments      text COLLATE NOCASE,
                                                      FileName      varchar COLLATE NOCASE,
                                                      AddedDate     timestamp NOT NULL,
                                                      IsDeleted     bit NOT NULL,
                                                      IsWhish       bit NOT NULL,
                                                      IsComplete    bit NOT NULL,
                                                      FilePath      varchar(200),
                                                      IsTested      boolean NOT NULL DEFAULT False,
                                                      ToBeDeleted   boolean NOT NULL DEFAULT False,
                                                      Rated         integer,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Editor_Id)
                                                        REFERENCES App_Editor(Id), 
                                                      FOREIGN KEY (Language_Id)
                                                        REFERENCES Language(Id), 
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id)
                                                    );

                                                    INSERT INTO Gamez
                                                      (Id,
                                                      Language_Id,
                                                      Media_Id,
                                                      Editor_Id,
                                                      Title,
                                                      Rating,
                                                      Descriptions,
                                                      ReleaseDate,
                                                      Comments,
                                                      FileName,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      FilePath,
                                                      IsTested,
                                                      ToBeDeleted,
                                                      Rated)
                                                    SELECT
                                                      Id,
                                                      Language_Id,
                                                      Media_Id,
                                                      Editor_Id,
                                                      Title,
                                                      Rating,
                                                      Descriptions,
                                                      ReleaseDate,
                                                      Comments,
                                                      FileName,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      FilePath,
                                                      IsTested,
                                                      ToBeDeleted,
                                                      Rated
                                                    FROM Gamez01;

                                                    DROP TABLE Gamez01;";
        #endregion
        #region deleteGroupFromMovies
        public const string DeleteGroupFromMovies = @"CREATE TABLE Movie01(
                                      Id,
                                      Studio_Id,
                                      Media_Id,
                                      FileFormat_Id,
                                      Title,
                                      Imdb,
                                      AlloCine,
                                      Rating,
                                      Description,
                                      Runtime,
                                      Tagline,
                                      Rated,
                                      ReleaseDate,
                                      Seen,
                                      IsDeleted,
                                      IsWhish,
                                      IsComplete,
                                      AddedDate,
                                      FileName,
                                      FilePath,
                                      OriginalTitle,
                                      TobeDeleted,
                                      Comments,
                                      Country
                                    );

                                    INSERT INTO Movie01
                                    SELECT
                                      Id,
                                      Studio_Id,
                                      Media_Id,
                                      FileFormat_Id,
                                      Title,
                                      Imdb,
                                      AlloCine,
                                      Rating,
                                      Description,
                                      Runtime,
                                      Tagline,
                                      Rated,
                                      ReleaseDate,
                                      Seen,
                                      IsDeleted,
                                      IsWhish,
                                      IsComplete,
                                      AddedDate,
                                      FileName,
                                      FilePath,
                                      OriginalTitle,
                                      TobeDeleted,
                                      Comments,
                                      Country
                                    FROM Movie;

                                    DROP TABLE Movie;

                                    CREATE TABLE Movie (
                                      Id             varchar(50) PRIMARY KEY NOT NULL,
                                      Studio_Id      varchar(50),
                                      Media_Id       varchar(50),
                                      FileFormat_Id  varchar(50),
                                      Title          varchar(250) NOT NULL,
                                      Imdb           varchar(5),
                                      AlloCine       varchar(5),
                                      Rating         int,
                                      Description    text,
                                      Runtime        int,
                                      Tagline        text,
                                      Rated          int,
                                      ReleaseDate    datetime,
                                      Seen           bit NOT NULL,
                                      IsDeleted      bit NOT NULL,
                                      IsWhish        bit NOT NULL,
                                      IsComplete     bit NOT NULL,
                                      AddedDate      timestamp NOT NULL,
                                      FileName       varchar(250),
                                      FilePath       varchar(250),
                                      OriginalTitle  varchar(250),
                                      TobeDeleted    boolean NOT NULL DEFAULT 0,
                                      Comments       text,
                                      Country        varchar(50),
                                      /* Foreign keys */
                                      FOREIGN KEY (FileFormat_Id)
                                        REFERENCES FileFormat(Id), 
                                      FOREIGN KEY (Studio_Id)
                                        REFERENCES Movie_Studio(Id), 
                                      FOREIGN KEY (Media_Id)
                                        REFERENCES Media(Id)
                                    );

                                    INSERT INTO Movie
                                      (Id,
                                      Studio_Id,
                                      Media_Id,
                                      FileFormat_Id,
                                      Title,
                                      Imdb,
                                      AlloCine,
                                      Rating,
                                      Description,
                                      Runtime,
                                      Tagline,
                                      Rated,
                                      ReleaseDate,
                                      Seen,
                                      IsDeleted,
                                      IsWhish,
                                      IsComplete,
                                      AddedDate,
                                      FileName,
                                      FilePath,
                                      OriginalTitle,
                                      TobeDeleted,
                                      Comments,
                                      Country)
                                    SELECT
                                      Id,
                                      Studio_Id,
                                      Media_Id,
                                      FileFormat_Id,
                                      Title,
                                      Imdb,
                                      AlloCine,
                                      Rating,
                                      Description,
                                      Runtime,
                                      Tagline,
                                      Rated,
                                      ReleaseDate,
                                      Seen,
                                      IsDeleted,
                                      IsWhish,
                                      IsComplete,
                                      AddedDate,
                                      FileName,
                                      FilePath,
                                      OriginalTitle,
                                      TobeDeleted,
                                      Comments,
                                      Country
                                    FROM Movie01;

                                    DROP TABLE Movie01;

                                    CREATE TABLE Movie01(
                                      Id,
                                      Studio_Id,
                                      Media_Id,
                                      FileFormat_Id,
                                      Title,
                                      Imdb,
                                      AlloCine,
                                      Rating,
                                      Description,
                                      Runtime,
                                      Tagline,
                                      Rated,
                                      ReleaseDate,
                                      Seen,
                                      IsDeleted,
                                      IsWhish,
                                      IsComplete,
                                      AddedDate,
                                      FileName,
                                      FilePath,
                                      OriginalTitle,
                                      TobeDeleted,
                                      Comments,
                                      Country
                                    );

                                    INSERT INTO Movie01
                                    SELECT
                                      Id,
                                      Studio_Id,
                                      Media_Id,
                                      FileFormat_Id,
                                      Title,
                                      Imdb,
                                      AlloCine,
                                      Rating,
                                      Description,
                                      Runtime,
                                      Tagline,
                                      Rated,
                                      ReleaseDate,
                                      Seen,
                                      IsDeleted,
                                      IsWhish,
                                      IsComplete,
                                      AddedDate,
                                      FileName,
                                      FilePath,
                                      OriginalTitle,
                                      TobeDeleted,
                                      Comments,
                                      Country
                                    FROM Movie;

                                    DROP TABLE Movie;

                                    CREATE TABLE Movie (
                                      Id             varchar(50) PRIMARY KEY NOT NULL,
                                      Studio_Id      varchar(50),
                                      Media_Id       varchar(50),
                                      FileFormat_Id  varchar(50),
                                      Title          varchar(250) NOT NULL,
                                      BarCode      varchar(50),
                                      Imdb           varchar(5),
                                      AlloCine       varchar(5),
                                      Rating         int,
                                      Description    text,
                                      Runtime        int,
                                      Tagline        text,
                                      Rated          int,
                                      ReleaseDate    datetime,
                                      Seen           bit NOT NULL,
                                      IsDeleted      bit NOT NULL,
                                      IsWhish        bit NOT NULL,
                                      IsComplete     bit NOT NULL,
                                      AddedDate      timestamp NOT NULL,
                                      FileName       varchar(250),
                                      FilePath       varchar(250),
                                      OriginalTitle  varchar(250),
                                      TobeDeleted    boolean NOT NULL DEFAULT 0,
                                      Comments       text,
                                      Country        varchar(50),
                                      /* Foreign keys */
                                      FOREIGN KEY (FileFormat_Id)
                                        REFERENCES FileFormat(Id), 
                                      FOREIGN KEY (Studio_Id)
                                        REFERENCES Movie_Studio(Id), 
                                      FOREIGN KEY (Media_Id)
                                        REFERENCES Media(Id)
                                    );

                                    INSERT INTO Movie
                                      (Id,
                                      Studio_Id,
                                      Media_Id,
                                      FileFormat_Id,
                                      Title,
                                      Imdb,
                                      AlloCine,
                                      Rating,
                                      Description,
                                      Runtime,
                                      Tagline,
                                      Rated,
                                      ReleaseDate,
                                      Seen,
                                      IsDeleted,
                                      IsWhish,
                                      IsComplete,
                                      AddedDate,
                                      FileName,
                                      FilePath,
                                      OriginalTitle,
                                      TobeDeleted,
                                      Comments,
                                      Country)
                                    SELECT
                                      Id,
                                      Studio_Id,
                                      Media_Id,
                                      FileFormat_Id,
                                      Title,
                                      Imdb,
                                      AlloCine,
                                      Rating,
                                      Description,
                                      Runtime,
                                      Tagline,
                                      Rated,
                                      ReleaseDate,
                                      Seen,
                                      IsDeleted,
                                      IsWhish,
                                      IsComplete,
                                      AddedDate,
                                      FileName,
                                      FilePath,
                                      OriginalTitle,
                                      TobeDeleted,
                                      Comments,
                                      Country
                                    FROM Movie01;

                                    DROP TABLE Movie01;";
        #endregion
        #region deleteGroupFromMusic
        public const string DeleteGroupFromMusic = @"CREATE TABLE Music01(
                                                      Id,
                                                      Title,
                                                      Media_Id,
                                                      Studio_Id,
                                                      Genre_Id,
                                                      FileFormat_Id,
                                                      Rating,
                                                      Album,
                                                      Length,
                                                      BitRate,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      IsHear,
                                                      ReleaseDate,
                                                      FileName,
                                                      FilePath,
                                                      ToBeDeleted,
                                                      Comments
                                                    );

                                                    INSERT INTO Music01
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Media_Id,
                                                      Studio_Id,
                                                      Genre_Id,
                                                      FileFormat_Id,
                                                      Rating,
                                                      Album,
                                                      Length,
                                                      BitRate,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      IsHear,
                                                      ReleaseDate,
                                                      FileName,
                                                      FilePath,
                                                      ToBeDeleted,
                                                      Comments
                                                    FROM Music;

                                                    DROP TABLE Music;

                                                    CREATE TABLE Music (
                                                      Id             varchar(50) PRIMARY KEY NOT NULL,
                                                      Title          varchar(250) NOT NULL,
                                                      Media_Id       varchar(50) NOT NULL,
                                                      Studio_Id      varchar(50),
                                                      Genre_Id       varchar(50),
                                                      FileFormat_Id  varchar(50),
                                                      Rating         int,
                                                      Album          varchar(250),
                                                      Length         int,
                                                      BitRate        int,
                                                      AddedDate      timestamp NOT NULL,
                                                      IsDeleted      bit NOT NULL,
                                                      IsWhish        bit NOT NULL,
                                                      IsComplete     bit NOT NULL,
                                                      IsHear         bit NOT NULL,
                                                      ReleaseDate    timestamp,
                                                      FileName       varchar(250) NOT NULL,
                                                      FilePath       varchar(250) NOT NULL,
                                                      ToBeDeleted    bit NOT NULL,
                                                      Comments       text,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id), 
                                                      FOREIGN KEY (Genre_Id)
                                                        REFERENCES Music_Genre(Id), 
                                                      FOREIGN KEY (Studio_Id)
                                                        REFERENCES Music_Studio(Id), 
                                                      FOREIGN KEY (FileFormat_Id)
                                                        REFERENCES FileFormat(Id)
                                                    );

                                                    INSERT INTO Music
                                                      (Id,
                                                      Title,
                                                      Media_Id,
                                                      Studio_Id,
                                                      Genre_Id,
                                                      FileFormat_Id,
                                                      Rating,
                                                      Album,
                                                      Length,
                                                      BitRate,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      IsHear,
                                                      ReleaseDate,
                                                      FileName,
                                                      FilePath,
                                                      ToBeDeleted,
                                                      Comments)
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Media_Id,
                                                      Studio_Id,
                                                      Genre_Id,
                                                      FileFormat_Id,
                                                      Rating,
                                                      Album,
                                                      Length,
                                                      BitRate,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      IsHear,
                                                      ReleaseDate,
                                                      FileName,
                                                      FilePath,
                                                      ToBeDeleted,
                                                      Comments
                                                    FROM Music01;

                                                    DROP TABLE Music01;

                                                    CREATE TABLE Music01(
                                                      Id,
                                                      Title,
                                                      Media_Id,
                                                      Studio_Id,
                                                      Genre_Id,
                                                      FileFormat_Id,
                                                      Rating,
                                                      Album,
                                                      Length,
                                                      BitRate,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      IsHear,
                                                      ReleaseDate,
                                                      FileName,
                                                      FilePath,
                                                      ToBeDeleted,
                                                      Comments
                                                    );

                                                    INSERT INTO Music01
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Media_Id,
                                                      Studio_Id,
                                                      Genre_Id,
                                                      FileFormat_Id,
                                                      Rating,
                                                      Album,
                                                      Length,
                                                      BitRate,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      IsHear,
                                                      ReleaseDate,
                                                      FileName,
                                                      FilePath,
                                                      ToBeDeleted,
                                                      Comments
                                                    FROM Music;

                                                    DROP TABLE Music;

                                                    CREATE TABLE Music (
                                                      Id             varchar(50) PRIMARY KEY NOT NULL,
                                                      Title          varchar(250) NOT NULL,
                                                      BarCode      varchar(50),
                                                      Media_Id       varchar(50) NOT NULL,
                                                      Studio_Id      varchar(50),
                                                      Genre_Id       varchar(50),
                                                      FileFormat_Id  varchar(50),
                                                      Rating         int,
                                                      Album          varchar(250),
                                                      Length         int,
                                                      BitRate        int,
                                                      AddedDate      timestamp NOT NULL,
                                                      IsDeleted      bit NOT NULL,
                                                      IsWhish        bit NOT NULL,
                                                      IsComplete     bit NOT NULL,
                                                      IsHear         bit NOT NULL,
                                                      ReleaseDate    timestamp,
                                                      FileName       varchar(250) NOT NULL,
                                                      FilePath       varchar(250) NOT NULL,
                                                      ToBeDeleted    bit NOT NULL,
                                                      Comments       text,
                                                      /* Foreign keys */
                                                      FOREIGN KEY (Media_Id)
                                                        REFERENCES Media(Id), 
                                                      FOREIGN KEY (Genre_Id)
                                                        REFERENCES Music_Genre(Id), 
                                                      FOREIGN KEY (Studio_Id)
                                                        REFERENCES Music_Studio(Id), 
                                                      FOREIGN KEY (FileFormat_Id)
                                                        REFERENCES FileFormat(Id)
                                                    );

                                                    INSERT INTO Music
                                                      (Id,
                                                      Title,
                                                      Media_Id,
                                                      Studio_Id,
                                                      Genre_Id,
                                                      FileFormat_Id,
                                                      Rating,
                                                      Album,
                                                      Length,
                                                      BitRate,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      IsHear,
                                                      ReleaseDate,
                                                      FileName,
                                                      FilePath,
                                                      ToBeDeleted,
                                                      Comments)
                                                    SELECT
                                                      Id,
                                                      Title,
                                                      Media_Id,
                                                      Studio_Id,
                                                      Genre_Id,
                                                      FileFormat_Id,
                                                      Rating,
                                                      Album,
                                                      Length,
                                                      BitRate,
                                                      AddedDate,
                                                      IsDeleted,
                                                      IsWhish,
                                                      IsComplete,
                                                      IsHear,
                                                      ReleaseDate,
                                                      FileName,
                                                      FilePath,
                                                      ToBeDeleted,
                                                      Comments
                                                    FROM Music01;

                                                    DROP TABLE Music01;";
        #endregion
        #region deleteGroupFromNds
        public const string DeleteGroupFromNds = @"
                                                CREATE TABLE Nds01(
                                                  Id,
                                                  Title,
                                                  Editor_Id,
                                                  Language_Id,
                                                  Media_Id,
                                                  Rating,
                                                  Description,
                                                  ReleaseDate,
                                                  Comments,
                                                  FileName,
                                                  FilePath,
                                                  AddedDate,
                                                  IsDeleted,
                                                  IsWhish,
                                                  IsComplete,
                                                  ToBeDeleted,
                                                  IsTested,
                                                  Rated
                                                );

                                                INSERT INTO Nds01
                                                SELECT
                                                  Id,
                                                  Title,
                                                  Editor_Id,
                                                  Language_Id,
                                                  Media_Id,
                                                  Rating,
                                                  Description,
                                                  ReleaseDate,
                                                  Comments,
                                                  FileName,
                                                  FilePath,
                                                  AddedDate,
                                                  IsDeleted,
                                                  IsWhish,
                                                  IsComplete,
                                                  ToBeDeleted,
                                                  IsTested,
                                                  Rated
                                                FROM Nds;

                                                DROP TABLE Nds;

                                                CREATE TABLE Nds (
                                                  Id           varchar(50) PRIMARY KEY NOT NULL,
                                                  Title        varchar(250) NOT NULL,
                                                  Editor_Id    varchar(50),
                                                  Language_Id  varchar(50),
                                                  Media_Id     varchar(50) NOT NULL,
                                                  Rating       int,
                                                  Description  text,
                                                  ReleaseDate  datetime,
                                                  Comments     text,
                                                  FileName     varchar(250),
                                                  FilePath     varchar(250),
                                                  AddedDate    datetime NOT NULL,
                                                  IsDeleted    bit NOT NULL,
                                                  IsWhish      bit NOT NULL,
                                                  IsComplete   bit NOT NULL,
                                                  ToBeDeleted  boolean NOT NULL,
                                                  IsTested     boolean NOT NULL,
                                                  Rated        integer,
                                                  /* Foreign keys */
                                                  FOREIGN KEY (Editor_Id)
                                                    REFERENCES App_Editor(Id), 
                                                  FOREIGN KEY (Language_Id)
                                                    REFERENCES Language(Id), 
                                                  FOREIGN KEY (Media_Id)
                                                    REFERENCES Media(Id)
                                                );

                                                INSERT INTO Nds
                                                  (Id,
                                                  Title,
                                                  Editor_Id,
                                                  Language_Id,
                                                  Media_Id,
                                                  Rating,
                                                  Description,
                                                  ReleaseDate,
                                                  Comments,
                                                  FileName,
                                                  FilePath,
                                                  AddedDate,
                                                  IsDeleted,
                                                  IsWhish,
                                                  IsComplete,
                                                  ToBeDeleted,
                                                  IsTested,
                                                  Rated)
                                                SELECT
                                                  Id,
                                                  Title,
                                                  Editor_Id,
                                                  Language_Id,
                                                  Media_Id,
                                                  Rating,
                                                  Description,
                                                  ReleaseDate,
                                                  Comments,
                                                  FileName,
                                                  FilePath,
                                                  AddedDate,
                                                  IsDeleted,
                                                  IsWhish,
                                                  IsComplete,
                                                  ToBeDeleted,
                                                  IsTested,
                                                  Rated
                                                FROM Nds01;

                                                DROP TABLE Nds01;

                                                CREATE TABLE Nds01(
                                                  Id,
                                                  Title,
                                                  Editor_Id,
                                                  Language_Id,
                                                  Media_Id,
                                                  Rating,
                                                  Description,
                                                  ReleaseDate,
                                                  Comments,
                                                  FileName,
                                                  FilePath,
                                                  AddedDate,
                                                  IsDeleted,
                                                  IsWhish,
                                                  IsComplete,
                                                  ToBeDeleted,
                                                  IsTested,
                                                  Rated
                                                );

                                                INSERT INTO Nds01
                                                SELECT
                                                  Id,
                                                  Title,
                                                  Editor_Id,
                                                  Language_Id,
                                                  Media_Id,
                                                  Rating,
                                                  Description,
                                                  ReleaseDate,
                                                  Comments,
                                                  FileName,
                                                  FilePath,
                                                  AddedDate,
                                                  IsDeleted,
                                                  IsWhish,
                                                  IsComplete,
                                                  ToBeDeleted,
                                                  IsTested,
                                                  Rated
                                                FROM Nds;

                                                DROP TABLE Nds;

                                                CREATE TABLE Nds (
                                                  Id           varchar(50) PRIMARY KEY NOT NULL,
                                                  Title        varchar(250) NOT NULL,
                                                  BarCode      varchar(50),
                                                  Editor_Id    varchar(50),
                                                  Language_Id  varchar(50),
                                                  Media_Id     varchar(50) NOT NULL,
                                                  Rating       int,
                                                  Description  text,
                                                  ReleaseDate  datetime,
                                                  Comments     text,
                                                  FileName     varchar(250),
                                                  FilePath     varchar(250),
                                                  AddedDate    datetime NOT NULL,
                                                  IsDeleted    bit NOT NULL,
                                                  IsWhish      bit NOT NULL,
                                                  IsComplete   bit NOT NULL,
                                                  ToBeDeleted  boolean NOT NULL,
                                                  IsTested     boolean NOT NULL,
                                                  Rated        integer,
                                                  /* Foreign keys */
                                                  FOREIGN KEY (Editor_Id)
                                                    REFERENCES App_Editor(Id), 
                                                  FOREIGN KEY (Language_Id)
                                                    REFERENCES Language(Id), 
                                                  FOREIGN KEY (Media_Id)
                                                    REFERENCES Media(Id)
                                                );

                                                INSERT INTO Nds
                                                  (Id,
                                                  Title,
                                                  Editor_Id,
                                                  Language_Id,
                                                  Media_Id,
                                                  Rating,
                                                  Description,
                                                  ReleaseDate,
                                                  Comments,
                                                  FileName,
                                                  FilePath,
                                                  AddedDate,
                                                  IsDeleted,
                                                  IsWhish,
                                                  IsComplete,
                                                  ToBeDeleted,
                                                  IsTested,
                                                  Rated)
                                                SELECT
                                                  Id,
                                                  Title,
                                                  Editor_Id,
                                                  Language_Id,
                                                  Media_Id,
                                                  Rating,
                                                  Description,
                                                  ReleaseDate,
                                                  Comments,
                                                  FileName,
                                                  FilePath,
                                                  AddedDate,
                                                  IsDeleted,
                                                  IsWhish,
                                                  IsComplete,
                                                  ToBeDeleted,
                                                  IsTested,
                                                  Rated
                                                FROM Nds01;

                                                DROP TABLE Nds01;";
        #endregion
        #region deleteGroupFromXXX
        public const string DeleteGroupFromXXX = @"CREATE TABLE XXX01(
                                              Id,
                                              Media_Id,
                                              Language_Id,
                                              FileFormat_Id,
                                              Title,
                                              Rating,
                                              Description,
                                              Runtime,
                                              Seen,
                                              ReleaseDate,
                                              Comments,
                                              IsDeleted,
                                              IsComplete,
                                              IsWhish,
                                              AddedDate,
                                              FileName,
                                              FilePath,
                                              ToBeDeleted,
                                              Studio_Id,
                                              Country
                                            );

                                            INSERT INTO XXX01
                                            SELECT
                                              Id,
                                              Media_Id,
                                              Language_Id,
                                              FileFormat_Id,
                                              Title,
                                              Rating,
                                              Description,
                                              Runtime,
                                              Seen,
                                              ReleaseDate,
                                              Comments,
                                              IsDeleted,
                                              IsComplete,
                                              IsWhish,
                                              AddedDate,
                                              FileName,
                                              FilePath,
                                              ToBeDeleted,
                                              Studio_Id,
                                              Country
                                            FROM XXX;

                                            DROP TABLE XXX;

                                            CREATE TABLE XXX (
                                              Id             varchar PRIMARY KEY NOT NULL,
                                              Media_Id       varchar NOT NULL,
                                              Language_Id    varchar(50),
                                              FileFormat_Id  varchar(50),
                                              Title          varchar NOT NULL COLLATE NOCASE,
                                              Rating         int,
                                              Description    text COLLATE NOCASE,
                                              Runtime        int,
                                              Seen           bit NOT NULL,
                                              ReleaseDate    timestamp,
                                              Comments       text COLLATE NOCASE,
                                              IsDeleted      bit NOT NULL,
                                              IsComplete     bit NOT NULL,
                                              IsWhish        bit NOT NULL,
                                              AddedDate      timestamp NOT NULL,
                                              FileName       varchar COLLATE NOCASE,
                                              FilePath       varchar NOT NULL COLLATE NOCASE,
                                              ToBeDeleted    bit NOT NULL DEFAULT 0,
                                              Studio_Id      varchar(50),
                                              Country        varchar(50),
                                              /* Foreign keys */
                                              FOREIGN KEY (FileFormat_Id)
                                                REFERENCES FileFormat(Id), 
                                              FOREIGN KEY (Language_Id)
                                                REFERENCES Language(Id), 
                                              FOREIGN KEY (Media_Id)
                                                REFERENCES Media(Id), 
                                              FOREIGN KEY (Studio_Id)
                                                REFERENCES XXX_Studio(Id)
                                            );

                                            INSERT INTO XXX
                                              (Id,
                                              Media_Id,
                                              Language_Id,
                                              FileFormat_Id,
                                              Title,
                                              Rating,
                                              Description,
                                              Runtime,
                                              Seen,
                                              ReleaseDate,
                                              Comments,
                                              IsDeleted,
                                              IsComplete,
                                              IsWhish,
                                              AddedDate,
                                              FileName,
                                              FilePath,
                                              ToBeDeleted,
                                              Studio_Id,
                                              Country)
                                            SELECT
                                              Id,
                                              Media_Id,
                                              Language_Id,
                                              FileFormat_Id,
                                              Title,
                                              Rating,
                                              Description,
                                              Runtime,
                                              Seen,
                                              ReleaseDate,
                                              Comments,
                                              IsDeleted,
                                              IsComplete,
                                              IsWhish,
                                              AddedDate,
                                              FileName,
                                              FilePath,
                                              ToBeDeleted,
                                              Studio_Id,
                                              Country
                                            FROM XXX01;

                                            DROP TABLE XXX01;

                                            CREATE TABLE XXX01(
                                              Id,
                                              Media_Id,
                                              Language_Id,
                                              FileFormat_Id,
                                              Title,
                                              Rating,
                                              Description,
                                              Runtime,
                                              Seen,
                                              ReleaseDate,
                                              Comments,
                                              IsDeleted,
                                              IsComplete,
                                              IsWhish,
                                              AddedDate,
                                              FileName,
                                              FilePath,
                                              ToBeDeleted,
                                              Studio_Id,
                                              Country
                                            );

                                            INSERT INTO XXX01
                                            SELECT
                                              Id,
                                              Media_Id,
                                              Language_Id,
                                              FileFormat_Id,
                                              Title,
                                              Rating,
                                              Description,
                                              Runtime,
                                              Seen,
                                              ReleaseDate,
                                              Comments,
                                              IsDeleted,
                                              IsComplete,
                                              IsWhish,
                                              AddedDate,
                                              FileName,
                                              FilePath,
                                              ToBeDeleted,
                                              Studio_Id,
                                              Country
                                            FROM XXX;

                                            DROP TABLE XXX;

                                            CREATE TABLE XXX (
                                              Id             varchar PRIMARY KEY NOT NULL,
                                              Media_Id       varchar NOT NULL,
                                              Language_Id    varchar(50),
                                              FileFormat_Id  varchar(50),
                                              Title          varchar NOT NULL COLLATE NOCASE,
                                              BarCode      varchar(50),
                                              Rating         int,
                                              Description    text COLLATE NOCASE,
                                              Runtime        int,
                                              Seen           bit NOT NULL,
                                              ReleaseDate    timestamp,
                                              Comments       text COLLATE NOCASE,
                                              IsDeleted      bit NOT NULL,
                                              IsComplete     bit NOT NULL,
                                              IsWhish        bit NOT NULL,
                                              AddedDate      timestamp NOT NULL,
                                              FileName       varchar COLLATE NOCASE,
                                              FilePath       varchar NOT NULL COLLATE NOCASE,
                                              ToBeDeleted    bit NOT NULL DEFAULT 0,
                                              Studio_Id      varchar(50),
                                              Country        varchar(50),
                                              /* Foreign keys */
                                              FOREIGN KEY (FileFormat_Id)
                                                REFERENCES FileFormat(Id), 
                                              FOREIGN KEY (Language_Id)
                                                REFERENCES Language(Id), 
                                              FOREIGN KEY (Media_Id)
                                                REFERENCES Media(Id), 
                                              FOREIGN KEY (Studio_Id)
                                                REFERENCES XXX_Studio(Id)
                                            );

                                            INSERT INTO XXX
                                              (Id,
                                              Media_Id,
                                              Language_Id,
                                              FileFormat_Id,
                                              Title,
                                              Rating,
                                              Description,
                                              Runtime,
                                              Seen,
                                              ReleaseDate,
                                              Comments,
                                              IsDeleted,
                                              IsComplete,
                                              IsWhish,
                                              AddedDate,
                                              FileName,
                                              FilePath,
                                              ToBeDeleted,
                                              Studio_Id,
                                              Country)
                                            SELECT
                                              Id,
                                              Media_Id,
                                              Language_Id,
                                              FileFormat_Id,
                                              Title,
                                              Rating,
                                              Description,
                                              Runtime,
                                              Seen,
                                              ReleaseDate,
                                              Comments,
                                              IsDeleted,
                                              IsComplete,
                                              IsWhish,
                                              AddedDate,
                                              FileName,
                                              FilePath,
                                              ToBeDeleted,
                                              Studio_Id,
                                              Country
                                            FROM XXX01;

                                            DROP TABLE XXX01;";
        #endregion
        #endregion
        #region LoanManager
        public const string DeleteItemeType = "DROP TABLE ItemsType;";
        public const string DeleteLoan = "DROP TABLE Loan;";

        public const string CreateItemType = @"CREATE TABLE ItemsType (
                                            Id    varchar(50) PRIMARY KEY NOT NULL UNIQUE,
                                            Name  varchar(50) NOT NULL UNIQUE
                                            );";

        public const string CreateFriends = @"CREATE TABLE Friends (
                                              Id              varchar(50) PRIMARY KEY NOT NULL UNIQUE,
                                              Alias           varchar(50) NOT NULL UNIQUE,
                                              FirstName       varchar(50),
                                              LastName        varchar(50),
                                              Sex             boolean,
                                              BirthDate       date,
                                              Adresse         varchar(500),
                                              PhoneNumber     varchar(50),
                                              eMail           varchar(200),
                                              nbrCurrentLoan  integer NOT NULL DEFAULT 0,
                                              nbrMaxLoan      integer NOT NULL DEFAULT 10,
                                              Picture         blob,
                                              Comments        text
                                            );";
        public const string UpdateTypeMovie = @"UPDATE Type SET Name='Movies' WHERE Type_id='1';";

        public const string CreateLoan = @"CREATE TABLE Loan (
                                      Id          varchar(50) PRIMARY KEY NOT NULL UNIQUE,
                                      ItemTypeId  integer NOT NULL,
                                      ItemId      varchar(50) NOT NULL,
                                      FriendId    varchar(50) NOT NULL,
                                      StartDate   date NOT NULL,
                                      EndDate     date NOT NULL,
                                      IsBack      boolean NOT NULL,
                                      BackDate    date,
                                      /* Foreign keys */
                                      FOREIGN KEY (ItemTypeId)
                                        REFERENCES Type(Id), 
                                      FOREIGN KEY (FriendId)
                                        REFERENCES Friends(Id)
                                    );";

        #endregion
        #region Media
        public const string AddFreeSpaceColumn = @"ALTER TABLE Media ADD COLUMN FreeSpace integer";
        public const string AddTotalSpaceColumn = @"ALTER TABLE Media ADD COLUMN TotalSpace integer";
        public const string AddLastUpdateColumn = @"ALTER TABLE Media ADD COLUMN LastUpdate datetime";
        public const string UpdateMediaSettings = @"CREATE TABLE Media01(
                                                  Id,
                                                  MediaType_Id,
                                                  Name,
                                                  Path,
                                                  Image,
                                                  FreeSpace,
                                                  TotalSpace,
                                                  LastUpdate);

                                                INSERT INTO Media01
                                                SELECT
                                                  Id,
                                                  MediaType_Id,
                                                  Name,
                                                  Path,
                                                  Image,
                                                  FreeSpace,
                                                  TotalSpace,
                                                  LastUpdate
                                                FROM Media;

                                                DROP TABLE Media;

                                                CREATE TABLE Media (
                                                  Id            varchar(50) PRIMARY KEY NOT NULL,
                                                  MediaType_Id  varchar(50),
                                                  Name          varchar(50),
                                                  Path          varchar,
                                                  Image         blob,
                                                  FreeSpace     integer,
                                                  TotalSpace    integer,
                                                  LastUpdate    datetime,
                                                  EntityType  varchar(50),
                                                  LastPattern   varchar(50),
                                                  SearchSub     boolean,
                                                  LocalImage    boolean,
                                                  UseNfo        boolean,
                                                  CleanTitle    boolean,
                                                  /* Foreign keys */
                                                  FOREIGN KEY (MediaType_Id)
                                                    REFERENCES MediaType(Id)
                                                );

                                                CREATE INDEX Media_UniqueName
                                                  ON Media
                                                  (Name);

                                                INSERT INTO Media
                                                  (Id,
                                                  MediaType_Id,
                                                  Name,
                                                  Path,
                                                  Image,
                                                  FreeSpace,
                                                  TotalSpace,
                                                  LastUpdate)
                                                SELECT
                                                  Id,
                                                  MediaType_Id,
                                                  Name,
                                                  Path,
                                                  Image,
                                                  FreeSpace,
                                                  TotalSpace,
                                                  LastUpdate
                                                FROM Media01;

                                                DROP TABLE Media01;";
        #endregion
        #region CleanTitle
        public const string CreateCleanTitleTable = @"CREATE TABLE CleanTitle (
                                                    Id        varchar(50) PRIMARY KEY NOT NULL UNIQUE,
                                                    Value     varchar(250) NOT NULL UNIQUE,
                                                    Category  varchar(50) NOT NULL);";
        #endregion
        #region FileFormat
        public const string AddBluRayFileFormat = @"INSERT INTO FileFormat (Id,Name) VALUES ('77bc2a98-1e94-40ad-90ab-737bf21c4c19','BlueRay');";
        #endregion
        #region Artist_Credit
        public const string CreateArtistCreditTable = @"CREATE TABLE Artist_Credits (
                                                          Id         varchar(50) PRIMARY KEY NOT NULL UNIQUE,
                                                          Artist_Id  varchar(50) NOT NULL,
                                                          EntityType  varchar(50) NOT NULL,
                                                          Title      text NOT NULL,
                                                          Notes      text,
                                                          /* Foreign keys */
                                                          FOREIGN KEY (Artist_Id)
                                                            REFERENCES Artist(Id)
                                                        );";

        public const string AddReleaseDateArtistCredit = @"ALTER TABLE Artist_Credits
                                                ADD COLUMN ReleaseDate datetime;";

        public const string AddBuyLinkArtistCredit = @"ALTER TABLE Artist_Credits
                                                ADD COLUMN BuyLink text;";
        #endregion
        #region Artist
        public const string AddBioArtist = @"ALTER TABLE Artist
                                                ADD COLUMN Bio text;";
        public const string AddPlaceBirthArtist = @"ALTER TABLE Artist
                                                        ADD COLUMN PlaceBirth text;";
        public const string AddWebSiteArtist = @"ALTER TABLE Artist
                                                    ADD COLUMN WebSite text;";
        public const string AddYearsActiveArtist = @"ALTER TABLE Artist
                                                    ADD COLUMN YearsActive varchar(50);";
        public const string AddEthnicityArtist = @"ALTER TABLE Artist
                                                    ADD COLUMN Ethnicity varchar(50);";
        public const string AddBreastArtist = @"ALTER TABLE Artist
                                                    ADD COLUMN Breast varchar(50);";
        public const string AddFullNameArtist = @"ALTER TABLE Artist
                                                ADD COLUMN FulleName text;";
        public const string AddAkaArtist = @"ALTER TABLE Artist
                                                ADD COLUMN Aka text;";
        public const string AddSexArtistColumn = @"ALTER TABLE Artist
                                                ADD COLUMN Sex bit;";
        #endregion
        #region MetaData

        public const string AddMetaDataTable = @"CREATE TABLE MetaData (
                                                    Id        varchar(50) PRIMARY KEY NOT NULL,
                                                    Name      varchar(50) NOT NULL,
                                                    Category  varchar(50) NOT NULL);";

        public const string AddItemMetaDataTable = @"CREATE TABLE Item_Metadata (
                                                            Id             varchar(50) PRIMARY KEY NOT NULL UNIQUE,
                                                            Item_Id        varchar(50) NOT NULL,
                                                            MetaData_Id    varchar(50) NOT NULL,
                                                            Item_Category  varchar(50) NOT NULL,
                                                            /* Foreign keys */
                                                            FOREIGN KEY (MetaData_Id)
                                                            REFERENCES MetaData(Id)
                                                            ON DELETE NO ACTION
                                                            ON UPDATE NO ACTION
                                                        );";

        public const string Add1080I = @"INSERT OR IGNORE INTO MetaData(Id,Name,Category) VALUES('00028FC7-B968-4BD1-A063-BDD9F46BC746','1080i','Movie')";
        public const string Add720p = @"INSERT OR IGNORE INTO MetaData(Id,Name,Category) VALUES('A9BFB7F4-FEDA-404A-99E3-44A819210C59','720p','Movie')";
        public const string Add3D = @"INSERT OR IGNORE INTO MetaData(Id,Name,Category) VALUES('F9DB3988-9630-463E-9485-F45E3B973374','3D','Movie')";
        public const string AddLossless = @"INSERT OR IGNORE INTO MetaData(Id,Name,Category) VALUES('6E78F078-21EC-4DF7-8154-2E1849D40F5E','Lossless','Music')";

        public const string AddClean2013 = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('623B3636-EEC2-4E25-9DA4-E1EE55D184FE','2013','All')";
        public const string AddCleanDvdRip = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('536334E3-3F94-450B-B1B5-F71E71A1F472','dvdrip','Movie')";
        public const string AddClean720p = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('8130F1F3-9492-409F-BC57-1A5BF373DE17','720p','Movie')";
        public const string AddClean1080p = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('951FB56F-854B-46BE-8BAE-F80747BF1AC0','1080p','Movie')";
        public const string AddClean1080i = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('4FE129B6-F778-429A-878A-D1CB4CD23444','1080i','Movie')";
        public const string AddCleanBdRip = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('88FC70B1-E5A6-4467-8B03-127CD99C8236','Bdrip','Movie')";
        public const string AddCleanRepack = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('5DB79A2E-6F13-477F-97A0-4B86A073B4A9','REPACK','All')";
        public const string AddCleanXvid = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('DD1AB877-C0DC-436D-BDA7-54D869CD7697','Xvid','Movie')";
        public const string AddCleanAC3 = @"INSERT OR IGNORE INTO CleanTitle(Id,Value,Category) VALUES('117D2803-40E8-4E21-948E-93A6E0F07E85','AC3','Movie')";

        #endregion

    }
}
