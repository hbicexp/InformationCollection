CREATE TABLE [dbo].[SourceRegex]
(
	[SourceRegexId] INT NOT NULL IDENTITY, 
    [SourceRegexGroupId] INT NOT NULL, 
    [RegexType] VARCHAR(500) NOT NULL, 
    [Regex] NVARCHAR(500) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [IsMatched] BIT NOT NULL, 
    CONSTRAINT [PK_SourceRegex] PRIMARY KEY ([SourceRegexId]), 
    CONSTRAINT [FK_SourceRegex_SourceRegexGroup] FOREIGN KEY ([SourceRegexGroupId]) REFERENCES [SourceRegexGroup]([SourceRegexGroupId])
)
