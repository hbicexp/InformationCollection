CREATE TABLE [dbo].[Source]
(
	[SourceId] INT NOT NULL  IDENTITY, 
    [SourceName] NVARCHAR(500) NOT NULL, 
	[Domain] VARCHAR(500) Not null,
    [Url] VARCHAR(4000) NOT NULL, 
    [Interval] INT NOT NULL, 
    [CreateTime] DATETIME NOT NULL, 
    CONSTRAINT [PK_Source] PRIMARY KEY ([SourceId])
)
