CREATE TABLE [dbo].[UserSourceLink]
(
	[UserSourceLinkId] INT NOT NULL  IDENTITY, 
    [SourceId] INT NOT NULL, 
    [SourceName] NVARCHAR(500) NOT NULL DEFAULT '', 
    [UserId] INT NOT NULL, 
    [Interval] INT NOT NULL DEFAULT 1, 
    [CreateTime] DATETIME NOT NULL, 
    [UpdateTime] DATETIME NULL, 
    CONSTRAINT [FK_UserSourceLink_Source] FOREIGN KEY ([SourceId]) REFERENCES [Source]([SourceId]), 
    CONSTRAINT [FK_UserSourceLink_UserProfile] FOREIGN KEY ([UserId]) REFERENCES [UserProfile]([UserId]), 
    CONSTRAINT [PK_UserSourceLink] PRIMARY KEY ([UserSourceLinkId])
)
