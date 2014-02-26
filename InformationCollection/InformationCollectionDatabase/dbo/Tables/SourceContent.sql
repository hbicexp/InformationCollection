CREATE TABLE [dbo].[SourceContent]
(
	[SourceContentId] INT NOT NULL IDENTITY, 
    [SourceId] INT NOT NULL, 
    [Content] NVARCHAR(4000) NOT NULL, 
    [ContentType] INT NOT NULL, 
    [Url] VARCHAR(5000) NOT NULL, 
    [AddTime] DATETIME NOT NULL, 
    [AddDate] DATETIME NOT NULL DEFAULT '2014-01-01', 
    [AddHour] int NOT NULL DEFAULT 0, 
    [SourceDate] DATETIME NOT NULL DEFAULT '2014-01-01', 
    CONSTRAINT [PK_SourceContent] PRIMARY KEY ([SourceContentId]), 
    CONSTRAINT [FK_SourceContent_Source] FOREIGN KEY ([SourceId]) REFERENCES [Source]([SourceId]) 
)
