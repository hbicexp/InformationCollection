CREATE TABLE [dbo].[UserSourceContentLink]
(
	[UserSourceContentLinkId] INT NOT NULL IDENTITY, 
    [UserId] INT NOT NULL, 
    [SourceContentId] INT NOT NULL, 
    CONSTRAINT [PK_UserSourceContentLink] PRIMARY KEY ([UserSourceContentLinkId]), 
    CONSTRAINT [FK_UserSourceContentLink_UserProfile] FOREIGN KEY ([UserId]) REFERENCES [UserProfile]([UserId]), 
    CONSTRAINT [FK_UserSourceContentLink_SourceContent] FOREIGN KEY ([SourceContentId]) REFERENCES [SourceContent]([SourceContentId]) 
)
