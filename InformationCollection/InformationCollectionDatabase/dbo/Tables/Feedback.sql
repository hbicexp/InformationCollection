CREATE TABLE [dbo].[Feedback]
(
	[FeedbackId] INT NOT NULL IDENTITY, 
	[UserId] Int NOT null,
	[SourceId] Int Not null,
	[Content] nvarchar(4000) null,
	[SubmitTime] datetime not null,
	[Response] nvarchar(4000) null,
	[ResponseTime] datetime null,
    CONSTRAINT [PK_Feedback] PRIMARY KEY ([FeedbackId]) 
)
