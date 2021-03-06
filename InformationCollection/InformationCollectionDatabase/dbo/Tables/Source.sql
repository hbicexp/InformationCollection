﻿CREATE TABLE [dbo].[Source]
(
	[SourceId] INT NOT NULL  IDENTITY, 
	[Company] INT NOT NULL DEFAULT 0,
    [SourceType] INT NOT NULL DEFAULT 0, 
    [SourceName] NVARCHAR(500) NOT NULL, 
	[Domain] NVARCHAR(500) Not null,
    [Url] NVARCHAR(4000) NOT NULL, 
    [Interval] INT NOT NULL, 
    [CreateTime] DATETIME NOT NULL, 
    CONSTRAINT [PK_Source] PRIMARY KEY ([SourceId])
)
