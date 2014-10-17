CREATE TABLE [dbo].[ARTIST] (
    [ID]       INT            IDENTITY (1, 1) NOT NULL,
    [NAME]     VARCHAR (256)  NOT NULL,
    [COMMENTS] VARCHAR (1024) NULL,
    [RATING]   INT            NULL,
    [RADIO]    BIT            NULL,
    CONSTRAINT [PK_ARTISTS] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [NAME_IDX]
    ON [dbo].[ARTIST]([NAME] ASC);

CREATE TABLE [dbo].[JobLog] (
    [Id]      INT           IDENTITY (1, 1) NOT NULL,
    [DateRun] DATE          NOT NULL,
    [Details] VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[SONG] (
    [TITLE]     VARCHAR (256)  NOT NULL,
    [ARTIST]    VARCHAR (256)  NULL,
    [ALBUM]     VARCHAR (256)  NULL,
    [GENRE]     VARCHAR (256)  CONSTRAINT [DF_SONGS_GENRE] DEFAULT ('Unknown') NULL,
    [DURATION]  BIGINT         NULL,
    [BITRATE]   BIGINT         NULL,
    [FILESIZE]  BIGINT         NULL,
    [RATING]    INT            CONSTRAINT [DF_SONGS_RATING] DEFAULT ((0)) NOT NULL,
    [COMMENTS]  VARCHAR (256)  NULL,
    [ARTIST_ID] INT            NULL,
    [LOCATION]  VARCHAR (1024) NULL,
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [UPLOADED]  BIT            CONSTRAINT [DF_SONG_UPLOADED] DEFAULT ((0)) NOT NULL,
    [HASH]      BINARY (50)    NULL,
    [UPDATED]   DATE           NULL,
    [CREATED]   DATE           NULL,
    CONSTRAINT [PK_SONGS] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_SONG_GENRE]
    ON [dbo].[SONG]([GENRE] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SONG_UPLOADED]
    ON [dbo].[SONG]([UPLOADED] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SONG_ARTIST]
    ON [dbo].[SONG]([ARTIST] ASC);

go

CREATE TABLE [dbo].[SONG_HISTORY] (
    [ID]        INT      IDENTITY (1, 1) NOT NULL,
    [SONG_ID]   INT      NOT NULL,
    [PLAY_TIME] DATETIME NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SONG_HISTORY_SONG] FOREIGN KEY ([SONG_ID]) REFERENCES [dbo].[SONG] ([ID])
);

go

CREATE TABLE [dbo].[SONG_RATED] (
    [ID]      INT IDENTITY (1, 1) NOT NULL,
    [SONG_ID] INT NOT NULL,
    CONSTRAINT [PK_SONG_RATED] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SONG_RATED_SONG] FOREIGN KEY ([SONG_ID]) REFERENCES [dbo].[SONG] ([ID]) ON DELETE CASCADE ON UPDATE CASCADE
);

go

CREATE TABLE [dbo].[Sync] (
    [ID]            INT        IDENTITY (1, 1) NOT NULL,
    [COMPUTER_NAME] CHAR (255) NOT NULL,
    [DATE]          DATETIME   NOT NULL,
    CONSTRAINT [PK_SYNC] PRIMARY KEY CLUSTERED ([ID] ASC)
);

go

CREATE PROCEDURE [dbo].[InsertSong]
@title varchar(256),
@artist varchar(256),
@album varchar(256),
@genre varchar(256),
@duration bigint,
@filesize bigint,
@rating int,
@comments varchar(256),
@location varchar(1024),
@hash binary(50),
@id int OUTPUT

AS
BEGIN
	INSERT INTO SONG (title,artist,album,genre,duration,filesize,rating,comments,location,updated,created)
		values (@title,@artist,@album,@genre,@duration,@filesize,@rating,@comments,@location,GetDate(),GetDate());

	set @id = SCOPE_IDENTITY();

	INSERT INTO SONG_RATED (SONG_ID) VALUES (@id);
	
	SELECT TOP 1 * FROM SONG ORDER BY ID DESC;

	SELECT @id;
END

go

CREATE VIEW [dbo].[SongsV] 
AS
select s.TITLE,s.ARTIST,s.ALBUM,s.GENRE,s.DURATION,s.BITRATE,s.FILESIZE,s.COMMENTS,s.ARTIST_ID,s.ID,s.UPLOADED,s.LOCATION,s.HASH,s.UPDATED,s.CREATED,Song_Rating_Table.RATING,newid() as newid from [dbo].SONG s

INNER JOIN
	(SELECT SONG_ID, COUNT(*) AS RATING FROM SONG_RATED GROUP BY SONG_ID) 
	AS Song_Rating_Table ON S.ID = Song_Rating_Table.SONG_ID

go