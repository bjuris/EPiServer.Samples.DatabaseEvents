CREATE TABLE [dbo].[tblDatabaseEvents](
	[pkId] [int] IDENTITY(1,1) NOT NULL,
	[SerializedMessage] [varbinary](max) NULL,
	[Source] [uniqueidentifier] NOT NULL,
	[CreatedDateUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_tblDatabaseEvents] PRIMARY KEY CLUSTERED 
(
	[pkId] ASC
)
)
GO
ALTER TABLE [dbo].[tblDatabaseEvents] ADD  CONSTRAINT [DF_tblDatabaseEvents_CreatedDateUtc]  DEFAULT (getutcdate()) FOR [CreatedDateUtc]
GO