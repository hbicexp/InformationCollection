CREATE TABLE [dbo].[UserSourceContentFavorLink]
(
	[UserSourceContentFavorLinkId] INT NOT NULL IDENTITY, 
    [UserId] INT NOT NULL, 
    [SourceContentId] INT NOT NULL, 
    CONSTRAINT [PK_UserSourceContentFavorLink] PRIMARY KEY ([UserSourceContentFavorLinkId]), 
    CONSTRAINT [FK_UserSourceContentFavorLink_UserProfile] FOREIGN KEY ([UserId]) REFERENCES [UserProfile]([UserId]), 
    CONSTRAINT [FK_UserSourceContentFavorLink_SourceContent] FOREIGN KEY ([SourceContentId]) REFERENCES [SourceContent]([SourceContentId]) 
)
