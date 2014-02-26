CREATE TABLE [dbo].[SourceRegexGroup]
(
	[SourceRegexGroupId] INT NOT NULL IDENTITY ,
    [Name] NVARCHAR(50) NOT NULL, 
    [Domain] VARCHAR(500) NOT NULL DEFAULT 'All', 
    [Regex] NVARCHAR(500) NOT NULL DEFAULT '', 
    [Enabled] BIT NOT NULL DEFAULT 1, 
    [Decode] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_SourceRegexGroup] PRIMARY KEY ([SourceRegexGroupId])
)
