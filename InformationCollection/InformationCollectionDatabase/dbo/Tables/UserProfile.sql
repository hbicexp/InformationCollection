CREATE TABLE [dbo].[UserProfile] (
    [UserId]   INT            IDENTITY (1, 1) NOT NULL,
    [UserName] NVARCHAR (MAX) NULL, 
    CONSTRAINT [PK_UserProfile] PRIMARY KEY ([UserId])
);

