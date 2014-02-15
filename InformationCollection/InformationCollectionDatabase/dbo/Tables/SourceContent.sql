CREATE TABLE [dbo].[SourceContent]
(
	[SourceContentId] INT NOT NULL IDENTITY, 
    [SourceId] INT NOT NULL, 
    [Content] NVARCHAR(4000) NOT NULL, 
    [ContentType] INT NOT NULL, 
    [Url] VARCHAR(5000) NOT NULL, 
    [AddTime] DATETIME NOT NULL, 
    CONSTRAINT [PK_SourceContent] PRIMARY KEY ([SourceContentId]), 
    CONSTRAINT [FK_SourceContent_Source] FOREIGN KEY ([SourceId]) REFERENCES [Source]([SourceId]) 
)
