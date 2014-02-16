CREATE TABLE [dbo].[SourceRegex]
(
	[SourceRegexId] INT NOT NULL IDENTITY, 
    [Regex] VARCHAR(500) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [IsMatched] BIT NOT NULL, 
    CONSTRAINT [PK_SourceRegex] PRIMARY KEY ([SourceRegexId])
)
