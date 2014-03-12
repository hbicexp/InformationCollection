CREATE TABLE [dbo].[SourceUrl]
(
	[SourceUrlId] INT NOT NULL IDENTITY , 
	[SourceId] INT NOT NULL, 
    [Url] NVARCHAR(4000) NOT NULL, 
    [Enabled] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_SourceUrl] PRIMARY KEY ([SourceUrlId]), 
    CONSTRAINT [FK_SourceUrl_Source] FOREIGN KEY ([SourceId]) REFERENCES [Source]([SourceId]), 
)
