SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Blogs]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_Blogs](
	[PortalID] [int] NOT NULL,
	[BlogID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[Public] [bit] NOT NULL,
	[AllowComments] [bit] NOT NULL,
	[AllowAnonymous] [bit] NOT NULL,
	[LastEntry] [datetime] NULL,
	[Created] [datetime] NOT NULL DEFAULT (getutcdate()),
	[ShowFullName] [bit] NOT NULL,
	[ParentBlogID] [int] NULL DEFAULT ((-1)),
	[Syndicated] [bit] NOT NULL,
	[SyndicateIndependant] [bit] NOT NULL,
	[SyndicationURL] [nvarchar](1024) NULL,
	[SyndicationEmail] [nvarchar](255) NULL,
	[EmailNotification] [bit] NULL,
	[AllowTrackbacks] [bit] NULL,
	[AutoTrackback] [bit] NULL,
	[MustApproveComments] [bit] NULL,
	[MustApproveAnonymous] [bit] NULL,
	[MustApproveTrackbacks] [bit] NULL,
	[UseCaptcha] [bit] NULL,
	[AuthorMode] [int] NULL,
 CONSTRAINT [PK_BLOGDEV_Blog_Blogs_BlogID] PRIMARY KEY CLUSTERED 
(
	[BlogID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Blogs]') AND name = N'IX_BLOGDEV_Blog_Blogs_PortalID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Blogs_PortalID] ON [dbo].[BLOGDEV_Blog_Blogs] 
(
	[PortalID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Blogs]') AND name = N'IX_BLOGDEV_Blog_Blogs_UserID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Blogs_UserID] ON [dbo].[BLOGDEV_Blog_Blogs] 
(
	[UserID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Categories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_Categories](
	[CatID] [int] IDENTITY(1,1) NOT NULL,
	[Category] [nvarchar](255) NOT NULL,
	[Slug] [nvarchar](255) NOT NULL,
	[ParentID] [int] NULL,
	[PortalID] [int] NOT NULL,
 CONSTRAINT [PK_BLOGDEV_Blog_Categories] PRIMARY KEY CLUSTERED 
(
	[CatID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Categories]') AND name = N'IX_BLOGDEV_Blog_Categories_PortalID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Categories_PortalID] ON [dbo].[BLOGDEV_Blog_Categories] 
(
	[PortalID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Comments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_Comments](
	[CommentID] [int] IDENTITY(1,1) NOT NULL,
	[EntryID] [int] NOT NULL,
	[UserID] [int] NULL,
	[Comment] [ntext] NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Approved] [bit] NULL,
	[Author] [nvarchar](50) NULL,
	[Website] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
 CONSTRAINT [PK_BLOGDEV_Blog_Comments_CommentID] PRIMARY KEY CLUSTERED 
(
	[CommentID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Comments]') AND name = N'IX_BLOGDEV_Blog_Comments_EntryID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Comments_EntryID] ON [dbo].[BLOGDEV_Blog_Comments] 
(
	[EntryID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entries]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_Entries](
	[BlogID] [int] NOT NULL,
	[EntryID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Entry] [ntext] NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[Published] [bit] NOT NULL,
	[Description] [ntext] NULL,
	[AllowComments] [bit] NULL,
	[DisplayCopyright] [bit] NOT NULL,
	[Copyright] [nvarchar](255) NULL,
	[PermaLink] [nvarchar](1024) NULL,
	[ContentItemId] [int] NULL,
	[CreatedUserId] [int] NULL,
	[ViewCount] [int] NULL,
 CONSTRAINT [PK_BLOGDEV_Blog_Entries_EntryID] PRIMARY KEY CLUSTERED 
(
	[EntryID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entries]') AND name = N'IX_BLOGDEV_Blog_Entries_AddedDate')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Entries_AddedDate] ON [dbo].[BLOGDEV_Blog_Entries] 
(
	[AddedDate] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entries]') AND name = N'IX_BLOGDEV_Blog_Entries_BlogID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Entries_BlogID] ON [dbo].[BLOGDEV_Blog_Entries] 
(
	[BlogID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Categories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_Entry_Categories](
	[EntryCatID] [int] IDENTITY(1,1) NOT NULL,
	[EntryID] [int] NULL,
	[CatID] [int] NULL,
 CONSTRAINT [PK_BLOGDEV_Blog_Entry_Categories] PRIMARY KEY CLUSTERED 
(
	[EntryCatID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Categories]') AND name = N'IX_BLOGDEV_Blog_Entry_Categories_CatID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Entry_Categories_CatID] ON [dbo].[BLOGDEV_Blog_Entry_Categories] 
(
	[CatID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Categories]') AND name = N'IX_BLOGDEV_Blog_Entry_Categories_EntryID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Entry_Categories_EntryID] ON [dbo].[BLOGDEV_Blog_Entry_Categories] 
(
	[EntryID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Tags]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_Entry_Tags](
	[EntryTagID] [int] IDENTITY(1,1) NOT NULL,
	[EntryID] [int] NOT NULL,
	[TagID] [int] NOT NULL,
 CONSTRAINT [PK_BLOGDEV_Blog_Entry_Tags] PRIMARY KEY CLUSTERED 
(
	[EntryTagID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Tags]') AND name = N'IX_BLOGDEV_Blog_Entry_Tags_EntryID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Entry_Tags_EntryID] ON [dbo].[BLOGDEV_Blog_Entry_Tags] 
(
	[EntryID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Tags]') AND name = N'IX_BLOGDEV_Blog_Entry_Tags_TagID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Entry_Tags_TagID] ON [dbo].[BLOGDEV_Blog_Entry_Tags] 
(
	[TagID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_MetaWeblogData]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_MetaWeblogData](
	[TempInstallUrl] [nvarchar](500) NULL
)
END
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Settings]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_Settings](
	[PortalID] [int] NOT NULL,
	[Key] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](1024) NOT NULL,
	[TabID] [int] NOT NULL,
 CONSTRAINT [PK_BLOGDEV_Blog_Settings] PRIMARY KEY CLUSTERED 
(
	[PortalID] ASC,
	[TabID] ASC,
	[Key] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Tags]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BLOGDEV_Blog_Tags](
	[TagID] [int] IDENTITY(1,1) NOT NULL,
	[Tag] [nvarchar](255) NOT NULL,
	[Slug] [nvarchar](255) NOT NULL,
	[Active] [bit] NOT NULL,
	[PortalID] [int] NOT NULL,
 CONSTRAINT [PK_BLOGDEV_Blog_Tags] PRIMARY KEY CLUSTERED 
(
	[TagID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Tags]') AND name = N'IX_BLOGDEV_Blog_Tags_PortalID')
CREATE NONCLUSTERED INDEX [IX_BLOGDEV_Blog_Tags_PortalID] ON [dbo].[BLOGDEV_Blog_Tags] 
(
	[PortalID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Blogs_Portals]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Blogs]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Blogs]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Blogs_Portals] FOREIGN KEY([PortalID])
REFERENCES [BLOGDEV_Portals] ([PortalID])
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Blogs_Portals]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Blogs]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Blogs] NOCHECK CONSTRAINT [FK_BLOGDEV_Blog_Blogs_Portals]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Blogs_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Blogs]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Blogs]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Blogs_Users] FOREIGN KEY([UserID])
REFERENCES [BLOGDEV_Users] ([UserID])
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Blogs_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Blogs]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Blogs] NOCHECK CONSTRAINT [FK_BLOGDEV_Blog_Blogs_Users]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Categories_Portals]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Categories]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Categories]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Categories_Portals] FOREIGN KEY([PortalID])
REFERENCES [BLOGDEV_Portals] ([PortalID])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Categories_Portals]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Categories]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Categories] CHECK CONSTRAINT [FK_BLOGDEV_Blog_Categories_Portals]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Comments]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Comments]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Comments]  WITH CHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Comments] FOREIGN KEY([EntryID])
REFERENCES [BLOGDEV_Blog_Entries] ([EntryID])
ON UPDATE CASCADE
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Comments]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Comments]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Comments] CHECK CONSTRAINT [FK_BLOGDEV_Blog_Comments]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Entry_Categories_Categories]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Categories]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Entry_Categories]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Entry_Categories_Categories] FOREIGN KEY([CatID])
REFERENCES [BLOGDEV_Blog_Categories] ([CatID])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Entry_Categories_Categories]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Categories]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Entry_Categories] CHECK CONSTRAINT [FK_BLOGDEV_Blog_Entry_Categories_Categories]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Entry_Categories_Entries]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Categories]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Entry_Categories]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Entry_Categories_Entries] FOREIGN KEY([EntryID])
REFERENCES [BLOGDEV_Blog_Entries] ([EntryID])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Entry_Categories_Entries]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Categories]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Entry_Categories] CHECK CONSTRAINT [FK_BLOGDEV_Blog_Entry_Categories_Entries]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Entry_Tags_Entries]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Tags]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Entry_Tags]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Entry_Tags_Entries] FOREIGN KEY([EntryID])
REFERENCES [BLOGDEV_Blog_Entries] ([EntryID])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Entry_Tags_Entries]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Tags]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Entry_Tags] CHECK CONSTRAINT [FK_BLOGDEV_Blog_Entry_Tags_Entries]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Entry_Tags_Tags]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Tags]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Entry_Tags]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Entry_Tags_Tags] FOREIGN KEY([TagID])
REFERENCES [BLOGDEV_Blog_Tags] ([TagID])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Entry_Tags_Tags]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Entry_Tags]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Entry_Tags] CHECK CONSTRAINT [FK_BLOGDEV_Blog_Entry_Tags_Tags]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Settings_Portals]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Settings]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Settings]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Settings_Portals] FOREIGN KEY([PortalID])
REFERENCES [BLOGDEV_Portals] ([PortalID])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Settings_Portals]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Settings]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Settings] CHECK CONSTRAINT [FK_BLOGDEV_Blog_Settings_Portals]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Tags_Portals]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Tags]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Tags]  WITH NOCHECK ADD  CONSTRAINT [FK_BLOGDEV_Blog_Tags_Portals] FOREIGN KEY([PortalID])
REFERENCES [BLOGDEV_Portals] ([PortalID])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BLOGDEV_Blog_Tags_Portals]') AND parent_object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Tags]'))
ALTER TABLE [dbo].[BLOGDEV_Blog_Tags] CHECK CONSTRAINT [FK_BLOGDEV_Blog_Tags_Portals]
GO



IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_vw_Blog_Terms]'))
DROP VIEW [dbo].[BLOGDEV_vw_Blog_Terms]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE VIEW dbo.[BLOGDEV_vw_Blog_Terms]
AS
	SELECT  TT.TermID ,
			TT.Name ,
			TT.ParentTermID ,
			TT.Description ,
			CI.CreatedOnDate ,
			TV.Name AS VocabularyName ,
			TV.VocabularyID ,
			CI.TabID ,
			CI.ModuleID ,
			CI.ContentTypeID ,
			CI.ContentItemID ,
			T.PortalID ,
			TT.Weight ,
			TT.TermLeft ,
			TT.TermRight
	FROM    dbo.BLOGDEV_Taxonomy_Vocabularies AS TV
			INNER JOIN dbo.BLOGDEV_Taxonomy_Terms AS TT ON TV.VocabularyID = TT.VocabularyID
			INNER JOIN dbo.BLOGDEV_ContentItems_Tags AS CIT ON TT.TermID = CIT.TermID
			INNER JOIN dbo.BLOGDEV_ContentItems AS CI ON CIT.ContentItemID = CI.ContentItemID
			INNER JOIN dbo.BLOGDEV_Tabs AS T ON CI.TabID = T.TabID
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_AddBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_AddBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_AddBlog
	@PortalID INT ,
	@ParentBlogID INT = -1 ,
	@UserID INT ,
	@Title NVARCHAR(512) ,
	@Description NVARCHAR(1024) ,
	@Public BIT ,
	@AllowComments BIT ,
	@AllowAnonymous BIT ,
	@ShowFullName BIT ,
	@Syndicated BIT ,
	@SyndicateIndependant BIT ,
	@SyndicationURL NVARCHAR(1024) ,
	@SyndicationEmail NVARCHAR(255) ,
	@EmailNotification BIT ,
	@AllowTrackbacks BIT ,
	@AutoTrackback BIT ,
	@MustApproveComments BIT ,
	@MustApproveAnonymous BIT ,
	@MustApproveTrackbacks BIT ,
	@UseCaptcha BIT ,
	@AuthorMode INT
AS 
	INSERT  INTO dbo.BLOGDEV_Blog_Blogs
			( [PortalID] ,
			  [ParentBlogID] ,
			  [UserID] ,
			  [Title] ,
			  [Description] ,
			  [Public] ,
			  [AllowComments] ,
			  [AllowAnonymous] ,
			  [ShowFullName] ,
			  [Created] ,
			  [Syndicated] ,
			  [SyndicateIndependant] ,
			  [SyndicationURL] ,
			  [SyndicationEmail] ,
			  [EmailNotification] ,
			  [AllowTrackbacks] ,
			  [AutoTrackback] ,
			  [MustApproveComments] ,
			  [MustApproveAnonymous] ,
			  [MustApproveTrackbacks] ,
			  [UseCaptcha] ,
			  [AuthorMode]
			)
	VALUES  ( @PortalID ,
			  @ParentBlogID ,
			  @UserID ,
			  @Title ,
			  @Description ,
			  @Public ,
			  @AllowComments ,
			  @AllowAnonymous ,
			  @ShowFullName ,
			  GETUTCDATE() ,
			  @Syndicated ,
			  @SyndicateIndependant ,
			  @SyndicationURL ,
			  @SyndicationEmail ,
			  @EmailNotification ,
			  @AllowTrackbacks ,
			  @AutoTrackback ,
			  @MustApproveComments ,
			  @MustApproveAnonymous ,
			  @MustApproveTrackbacks ,
			  @UseCaptcha ,
			  @AuthorMode
			)
	SELECT  SCOPE_IDENTITY()

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_AddComment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_AddComment]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_AddComment
	@EntryID int,
	@UserID int,
	@Title nvarchar(255),
	@Comment ntext,
	@Author nvarchar(50),
	@Approved bit,
	@Website nvarchar(255),
	@Email nvarchar(255),
@AddedDate datetime
AS
INSERT INTO dbo.BLOGDEV_Blog_Comments (
	[EntryID],
	[UserID],
	[Title],
	[Comment],
	[Author],
	[Approved],
	[AddedDate],
	[Website],
	[Email]
) VALUES (
	@EntryID,
	@UserID,
	@Title,
	@Comment,
	@Author,
	@Approved,
	COALESCE(@AddedDate, GetUTCDate()),
	@Website,
	@Email
)
select SCOPE_IDENTITY()
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_AddEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_AddEntry]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_AddEntry
	@BlogID int,
	@Title nvarchar(255),
	@Description ntext,
	@Entry ntext,
	@Published bit,
	@AllowComments bit,
	@AddedDate datetime,
	@DisplayCopyright bit,
	@Copyright nvarchar(256),
	@PermaLink nvarchar(1024),
	@CreatedUserId INT
AS
DECLARE @EntryID int
UPDATE  dbo.BLOGDEV_Blog_Blogs
	SET LastEntry = GetUTCDate()
WHERE [BlogID] = @BlogID
INSERT INTO dbo.BLOGDEV_Blog_Entries (
	[BlogID],
	[Title],
	[Description],
	[Entry],
	[AddedDate],
	[Published],
	[AllowComments],
	[DisplayCopyright],
	[Copyright],
	[PermaLink],
	[CreatedUserId],
	[ViewCount]
) VALUES (
	@BlogID,
	@Title,
	@Description,
	@Entry,
	@AddedDate,
	@Published,
	@AllowComments,
	@DisplayCopyright,
	@Copyright,
	null,
	@CreatedUserId,
	0
)
SET @EntryID = SCOPE_IDENTITY()
If NOT @PermaLink IS NULL
	UPDATE dbo.BLOGDEV_Blog_Entries SET PermaLink=@PermaLink + convert(nvarchar(10),EntryID) WHERE BlogID=@BlogID AND PermaLink IS NULL
SELECT @EntryID
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_DeleteBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_DeleteBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_DeleteBlog
  @BlogID INT,
	@PortalID INT
AS 
	DELETE  FROM dbo.BLOGDEV_Blog_Blogs
	WHERE   PortalID = @PortalID
			AND ( [BlogID] = @BlogID
				  OR [ParentBlogID] = @BlogID
				)

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_DeleteComment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_DeleteComment]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_DeleteComment
	@CommentID int
AS
DELETE FROM dbo.BLOGDEV_Blog_Comments
WHERE
	[CommentID] = @CommentID
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_DeleteEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_DeleteEntry]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_DeleteEntry
	@EntryID int
AS
DELETE FROM dbo.BLOGDEV_Blog_Entries
WHERE
	[EntryID] = @EntryID
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_DelUnAppCommByEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_DelUnAppCommByEntry]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_DelUnAppCommByEntry
	@EntryID int
AS
DELETE FROM
	dbo.BLOGDEV_Blog_Comments
WHERE
	[EntryID] = @EntryID
	AND [Approved] = 0
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetAllEntriesByBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetAllEntriesByBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE BLOGDEV_Blog_GetAllEntriesByBlog @BlogID INT
AS 
	SELECT  E.* ,
			B.[UserID] ,
			B.[SyndicationEmail] ,
			U.[UserName] ,
			U.[DisplayName] AS UserFullName ,
			( SELECT    COUNT(*)
			  FROM      dbo.BLOGDEV_Blog_Comments
			  WHERE     EntryID = E.EntryID
						AND Approved = 1
			) AS CommentCount ,
			CI.CreatedByUserID ,
			CI.CreatedOnDate ,
			CI.ContentKey ,
			CI.Indexed ,
			CI.Content ,
			CI.ContentItemID ,
			CI.LastModifiedByUserID ,
			CI.LastModifiedOnDate ,
			CI.ModuleID ,
			CI.TabID ,
			CI.ContentTypeID
	FROM    dbo.BLOGDEV_Blog_Blogs B
			INNER JOIN dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID]
			INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
			LEFT OUTER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
	WHERE   ( B.[BlogID] = @BlogID )
	ORDER BY E.AddedDate DESC

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetAllEntriesByPortal]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetAllEntriesByPortal]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetAllEntriesByPortal
	@PortalID INT ,
	@ShowNonPublic BIT = 0 ,
	@ShowNonPublished BIT = 0
AS 
	SELECT  E.* ,
			B.[UserID] ,
			B.[SyndicationEmail] ,
			U.[UserName] ,
			U.[DisplayName] AS UserFullName ,
			( SELECT    COUNT(*)
			  FROM      dbo.BLOGDEV_Blog_Comments
			  WHERE     EntryID = E.EntryID
						AND Approved = 1
			) AS CommentCount ,
			CI.CreatedByUserID ,
			CI.CreatedOnDate ,
			CI.ContentKey ,
			CI.Indexed ,
			CI.Content ,
			CI.ContentItemID ,
			CI.LastModifiedByUserID ,
			CI.LastModifiedOnDate ,
			CI.ModuleID ,
			CI.TabID ,
			CI.ContentTypeID
	FROM    dbo.BLOGDEV_Blog_Blogs B
			INNER JOIN dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID]
			INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
			LEFT OUTER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
	WHERE   B.PortalID = @PortalID
			AND ( E.[Published] = 1
				  OR E.[Published] <> @ShowNonPublished
				)
			AND ( B.[Public] = 1
				  OR B.[Public] <> @ShowNonPublic
				)
	ORDER BY E.AddedDate DESC

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetBlog @BlogID INT
AS 
	SELECT  B.* ,
			U.[UserName] ,
			U.[DisplayName] AS UserFullName ,
			( SELECT    COUNT(BlogID)
			  FROM      dbo.BLOGDEV_Blog_Blogs
			  WHERE     ParentBlogID = B.[BlogID]
			) AS ChildBlogCount ,
			( SELECT    COUNT(BlogID)
			  FROM      dbo.BLOGDEV_Blog_Entries
			  WHERE     BlogID = B.BlogID
						AND Published = 1
			) AS BlogPostCount
	FROM    dbo.BLOGDEV_Blog_Blogs B
			INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
	WHERE   [BlogID] = @BlogID

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetBlogDaysForMonth]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetBlogDaysForMonth]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_GetBlogDaysForMonth
(
	@PortalID int,
	@BlogID int,
	@BlogDate DateTime
)
AS
DECLARE @BlogMonth int
DECLARE @BlogYear int
SELECT @BlogMonth = 	DATEPART(mm, @BlogDate) 
SELECT @BlogYear = 	DATEPART(yy, @BlogDate) 
If @BlogID > -1
BEGIN
	SELECT
		E.[EntryID],
		E.[BlogID], 
		B.[ParentBlogID],
		E.[Title],
		E.[AddedDate],
		U.[Username],
		DATEPART(mm, E.AddedDate) as AddedMonth,
		DATEPART(yy, E.AddedDate) as AddedYear
	FROM   dbo.BLOGDEV_Blog_Blogs B INNER JOIN
		dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID] INNER JOIN
		dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
	
	WHERE B.[PortalID] = @PortalID AND
		 (B.[BlogID] = @BlogID OR  B.[ParentBlogID] = @BlogID) AND 
		DATEPART(yy, E.AddedDate) = @BlogYear AND 
		DATEPART(mm, E.AddedDate) = @BlogMonth
	
	ORDER BY E.AddedDate
END
ELSE
BEGIN
	SELECT
		E.[EntryID],
		E.[BlogID], 
		B.[ParentBlogID],
		E.[Title],
		E.[AddedDate],
		U.[Username],
		DATEPART(mm, E.AddedDate) as AddedMonth,
		DATEPART(yy, E.AddedDate) as AddedYear
	FROM   dbo.BLOGDEV_Blog_Blogs B INNER JOIN
		dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID] INNER JOIN
		dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
	
	WHERE B.[PortalID] = @PortalID AND 
		DATEPART(yy, E.AddedDate) = @BlogYear AND 
		DATEPART(mm, E.AddedDate) = @BlogMonth
	
	ORDER BY E.AddedDate
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetBlogMonths]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetBlogMonths]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_GetBlogMonths
(
	@PortalID int,
	@BlogID int
)
AS
If @BlogID > -1
BEGIN
	SELECT
		 DATEPART(mm, E.AddedDate) as AddedMonth,
		 DATEPART(yy, E.AddedDate) as AddedYear,
		 COUNT(EntryId) AS PostCount
	
	FROM dbo.BLOGDEV_Blog_Entries E INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON
		B.[BlogID] = E.[BlogID]
	
	WHERE B.[PortalID] = @PortalID 
	AND  (B.BlogID = @BlogID OR B.[ParentBlogID] = @BlogID) AND E.Published=1
	
	group by DATEPART(m, E.AddedDate), DATEPART(yy, E.AddedDate)
	order by AddedYear DESC, AddedMonth DESC
END
ELSE
BEGIN
	SELECT
		 DATEPART(mm, E.AddedDate) as AddedMonth,
		 DATEPART(yy, E.AddedDate) as AddedYear,
		 COUNT(EntryId) AS PostCount
	
	FROM dbo.BLOGDEV_Blog_Entries E INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON
		B.[BlogID] = E.[BlogID]
	
	WHERE B.[PortalID] = @PortalID AND E.Published=1
	
	group by DATEPART(m, E.AddedDate), DATEPART(yy, E.AddedDate)
	order by AddedYear DESC, AddedMonth DESC
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetBlogsByPortal]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetBlogsByPortal]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetBlogsByPortal @PortalID INT
AS 
	SELECT  B.* ,
			U.[UserName] ,
			U.[DisplayName] AS UserFullName ,
			( SELECT    COUNT(BlogID)
			  FROM      dbo.BLOGDEV_Blog_Blogs
			  WHERE     ParentBlogID = B.[BlogID]
			) AS ChildBlogCount ,
			( SELECT    COUNT(BlogID)
			  FROM      dbo.BLOGDEV_Blog_Entries
			  WHERE     BlogID = B.BlogID
						AND Published = 1
			) AS BlogPostCount
	FROM    dbo.BLOGDEV_Blog_Blogs B
			INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
	WHERE   [PortalID] = @PortalID

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetBlogViewEntryModuleID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetBlogViewEntryModuleID]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_GetBlogViewEntryModuleID 
	@TabID as Integer
AS
SELECT dbo.BLOGDEV_TabModules.ModuleID FROM dbo.BLOGDEV_TabModules INNER JOIN dbo.BLOGDEV_Modules ON dbo.BLOGDEV_TabModules.ModuleID = dbo.BLOGDEV_Modules.ModuleID WHERE dbo.BLOGDEV_TabModules.TabID=@TabID AND dbo.BLOGDEV_Modules.ModuleDefID=(SELECT ModuleDefID FROM dbo.BLOGDEV_ModuleControls WHERE ControlKey = 'View_Entry')
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetComment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetComment]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_GetComment
	@CommentID int
	
AS
SELECT
	C.[CommentID],
	C.[EntryID],
	C.[UserID],
	C.[Title],
	C.[Comment],
	C.[AddedDate],
	U.[UserName],
	U.[DisplayName] AS UserFullName,
	C.[Author],
	C.[Approved],
	C.[Website],
	C.[Email]
FROM
	dbo.BLOGDEV_Blog_Comments C
	LEFT JOIN 
	dbo.BLOGDEV_Users U ON C.[UserID] = U.[UserID]
WHERE
	C.[CommentID] = @CommentID
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetCommentsByBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetCommentsByBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_GetCommentsByBlog
	@BlogId INT,
	@ShowNonApproved BIT = 0,
	@MaxComments int = 10
AS
SET rowcount @MaxComments
SELECT
	C.[CommentID],
	C.[EntryID],
	C.[UserID],
	C.[Title],
	C.[Comment],
	C.[AddedDate],
	U.[UserName],
	U.[DisplayName] AS UserFullName,
	C.[Author],
	C.[Approved],
	C.[Website],
	C.[Email]
FROM
	dbo.BLOGDEV_Blog_Comments C
	LEFT OUTER JOIN 
	dbo.BLOGDEV_Users U ON C.[UserID] = U.[UserID]
	INNER JOIN
	dbo.BLOGDEV_Blog_Entries E ON C.EntryID = E.EntryID
	WHERE (C.[Approved] = 1 OR C.[Approved] <> @ShowNonApproved) AND
	E.BlogID = @BlogId
ORDER BY
	C.AddedDate desc
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetCommentsByEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetCommentsByEntry]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetCommentsByEntry
	@EntryID INT,
	@ShowNonApproved BIT
AS
	SELECT
		C.[CommentID],
		C.[EntryID],
		C.[UserID],
		C.[Title],
		C.[Comment],
		C.[AddedDate],
		U.[UserName],
		U.[DisplayName] AS UserFullName,
		C.[Author],
		C.[Approved],
		C.[Website],
		C.[Email]
	FROM dbo.BLOGDEV_Blog_Comments C
		LEFT JOIN dbo.BLOGDEV_Users U ON C.[UserID] = U.[UserID]
		WHERE [EntryID] = @EntryID AND (C.[Approved]=1 OR C.[Approved] <> @ShowNonApproved)
		ORDER By C.[CommentID]

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetCommentsByPortal]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetCommentsByPortal]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_GetCommentsByPortal
	@PortalId INT,
	@ShowNonApproved BIT = 0,
	@MaxComments int = 10
AS
SET rowcount @MaxComments
SELECT
	C.[CommentID],
	C.[EntryID],
	C.[UserID],
	C.[Title],
	C.[Comment],
	C.[AddedDate],
	U.[UserName],
	U.[DisplayName] AS UserFullName,
	C.[Author],
	C.[Approved],
	C.[Website],
	C.[Email]
FROM
	dbo.BLOGDEV_Blog_Comments C
	LEFT OUTER JOIN 
	dbo.BLOGDEV_Users U ON C.[UserID] = U.[UserID]
	INNER JOIN
	dbo.BLOGDEV_Blog_Entries E ON C.EntryID = E.EntryID
	INNER JOIN
	dbo.BLOGDEV_Blog_Blogs B ON E.BlogID = B.BlogID
	WHERE (C.[Approved] = 1 OR C.[Approved] <> @ShowNonApproved) AND
	B.PortalID = @PortalId
ORDER BY
	C.AddedDate desc
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetEntriesByBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetEntriesByBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetEntriesByBlog
    @BlogID INT ,
    @BlogDate DATETIME = NULL ,
    @PageSize INT ,
    @CurrentPage INT ,
    @ShowNonPublic BIT = 0 ,
    @ShowNonPublished BIT = 0
AS 
    IF @BlogDate IS NULL 
        BEGIN
            SET @BlogDate = GETUTCDATE()
        END
    DECLARE @RowStart INT 
    DECLARE @RowEnd INT 
  
    SET @RowStart = @PageSize * ( @CurrentPage ) + 1
    SET @RowEnd = @RowStart + @PageSize
 
    SELECT  *
    FROM    ( SELECT    E.* ,
                        B.[UserID] ,
                        B.[SyndicationEmail] ,
                        U.[UserName] ,
                        U.[DisplayName] AS UserFullName ,
                        ( SELECT    COUNT(*)
                          FROM      dbo.BLOGDEV_Blog_Comments
                          WHERE     EntryID = E.EntryID
                                    AND Approved = 1
                        ) AS CommentCount ,
                        CI.CreatedByUserID ,
                        CI.CreatedOnDate ,
                        CI.ContentKey ,
                        CI.Indexed ,
                        CI.Content ,
                        CI.LastModifiedByUserID ,
                        CI.LastModifiedOnDate ,
                        CI.ModuleID ,
                        CI.TabID ,
                        CI.ContentTypeID ,
                        ( SELECT    COUNT(BE.ContentItemID)
                          FROM      dbo.BLOGDEV_Blog_Blogs BB
                                    INNER JOIN dbo.BLOGDEV_Blog_Entries BE ON BB.[BlogID] = BE.[BlogID]
                                    INNER JOIN dbo.BLOGDEV_Users BU ON BB.[UserID] = BU.[UserID]
                                    LEFT OUTER JOIN dbo.BLOGDEV_ContentItems BCI ON BE.ContentItemId = BCI.ContentItemID
                          WHERE     ( BB.[BlogID] = @BlogID
                                      OR BB.[ParentBlogID] = @BlogID
                                    )
                                    AND BE.AddedDate <= @BlogDate
                                    AND ( BE.[Published] = 1
                                          OR BE.[Published] <> @ShowNonPublished
                                        )
                                    AND ( BB.[Public] = 1
                                          OR BB.[Public] <> @ShowNonPublic
                                        )
                        ) AS TotalRecords ,
                        ROW_NUMBER() OVER ( ORDER BY E.AddedDate DESC ) AS RowNumber
              FROM      dbo.BLOGDEV_Blog_Blogs B
                        INNER JOIN dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID]
                        INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
                        LEFT OUTER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
              WHERE     ( B.[BlogID] = @BlogID
                          OR B.[ParentBlogID] = @BlogID
                        )
                        AND E.AddedDate <= @BlogDate
                        AND ( E.[Published] = 1
                              OR E.[Published] <> @ShowNonPublished
                            )
                        AND ( B.[Public] = 1
                              OR B.[Public] <> @ShowNonPublic
                            )
            ) AS EntryInfo
    WHERE   RowNumber >= @RowStart
            AND RowNumber < @RowEnd

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetEntriesByDay]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetEntriesByDay]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetEntriesByDay
	@PortalID INT ,
	@BlogDate DATETIME = NULL ,
	@PageSize INT ,
	@CurrentPage INT ,
	@ShowNonPublic BIT = 0 ,
	@ShowNonPublished BIT = 0
AS 
	IF @BlogDate IS NULL 
		BEGIN
			SET @BlogDate = GETUTCDATE()
		END
		
	DECLARE @RowStart INT 
	DECLARE @RowEnd INT 
				
	SET @RowStart = @PageSize * ( @CurrentPage - 1 )
	SET @RowEnd = @RowStart + @PageSize
	
	SELECT  *
	FROM    ( SELECT    E.* ,
						B.[UserID] ,
						B.[SyndicationEmail] ,
						U.[UserName] ,
						U.[DisplayName] AS UserFullName ,
						( SELECT    COUNT(*)
						  FROM      dbo.BLOGDEV_Blog_Comments
						  WHERE     EntryID = E.EntryID
									AND Approved = 1
						) AS CommentCount ,
						CI.CreatedByUserID ,
						CI.CreatedOnDate ,
						CI.ContentKey ,
						CI.Indexed ,
						CI.Content ,
						CI.LastModifiedByUserID ,
						CI.LastModifiedOnDate ,
						CI.ModuleID ,
						CI.TabID ,
						CI.ContentTypeID ,
						( SELECT    COUNT(*)
						  FROM      dbo.BLOGDEV_Blog_Blogs BB
									INNER JOIN dbo.BLOGDEV_Blog_Entries BE ON BB.BlogID = BE.BlogID
									INNER JOIN dbo.BLOGDEV_Users BU ON BB.UserID = BU.UserID
						  WHERE     B.PortalID = @PortalID
									AND ( E.[Published] = 1
										  OR E.[Published] <> @ShowNonPublished
										)
									AND ( B.[Public] = 1
										  OR B.[Public] <> @ShowNonPublic
										)
						) AS TotalRecords ,
						ROW_NUMBER() OVER ( ORDER BY E.AddedDate DESC ) AS RowNumber
			  FROM      dbo.BLOGDEV_Blog_Blogs B
						INNER JOIN dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID]
						INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
						LEFT OUTER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
			  WHERE     B.PortalID = @PortalID
						AND E.AddedDate BETWEEN @BlogDate
										AND     DATEADD(dd, 1, @BlogDate)
						AND ( E.[Published] = 1
							  OR E.[Published] <> @ShowNonPublished
							)
						AND ( B.[Public] = 1
							  OR B.[Public] <> @ShowNonPublic
							)
			) AS EntryInfo
	WHERE   RowNumber >= @RowStart
			AND RowNumber <= @RowEnd 

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetEntriesByMonth]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetEntriesByMonth]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetEntriesByMonth
	@PortalID INT ,
	@BlogDate DATETIME = NULL ,
	@PageSize INT ,
	@CurrentPage INT ,
	@ShowNonPublic BIT = 0 ,
	@ShowNonPublished BIT = 0
AS 
	IF @BlogDate IS NULL 
		BEGIN
			SET @BlogDate = GETUTCDATE()
		END
		
	DECLARE @RowStart INT 
	DECLARE @RowEnd INT 
				
	SET @RowStart = @PageSize * ( @CurrentPage - 1 )
	SET @RowEnd = @RowStart + @PageSize
	
	SELECT  *
	FROM    ( SELECT    E.* ,
						B.[UserID] ,
						B.[SyndicationEmail] ,
						U.[UserName] ,
						U.[DisplayName] AS UserFullName ,
						( SELECT    COUNT(*)
						  FROM      dbo.BLOGDEV_Blog_Comments
						  WHERE     EntryID = E.EntryID
									AND Approved = 1
						) AS CommentCount ,
						CI.CreatedByUserID ,
						CI.CreatedOnDate ,
						CI.ContentKey ,
						CI.Indexed ,
						CI.Content ,
						CI.LastModifiedByUserID ,
						CI.LastModifiedOnDate ,
						CI.ModuleID ,
						CI.TabID ,
						CI.ContentTypeID ,
						( SELECT    COUNT(*)
						  FROM      dbo.BLOGDEV_Blog_Blogs BB
									INNER JOIN dbo.BLOGDEV_Blog_Entries BE ON BB.BlogID = BE.BlogID
									INNER JOIN dbo.BLOGDEV_Users BU ON BB.UserID = BU.UserID
						  WHERE     B.PortalID = @PortalID
									AND ( E.[Published] = 1
										  OR E.[Published] <> @ShowNonPublished
										)
									AND ( B.[Public] = 1
										  OR B.[Public] <> @ShowNonPublic
										)
						) AS TotalRecords ,
						ROW_NUMBER() OVER ( ORDER BY E.AddedDate DESC ) AS RowNumber
			  FROM      dbo.BLOGDEV_Blog_Blogs B
						INNER JOIN dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID]
						INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
						LEFT OUTER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
			  WHERE     B.PortalID = @PortalID
						AND E.AddedDate BETWEEN DATEADD(month,
														DATEDIFF(month, 0,
															  @BlogDate), 0)
										AND     @BlogDate
						AND E.AddedDate <= GETUTCDATE()
						AND ( E.[Published] = 1
							  OR E.[Published] <> @ShowNonPublished
							)
						AND ( B.[Public] = 1
							  OR B.[Public] <> @ShowNonPublic
							)
			) AS EntryInfo
	WHERE   RowNumber >= @RowStart
			AND RowNumber <= @RowEnd

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetEntriesByPortal]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetEntriesByPortal]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetEntriesByPortal
	@PortalID INT ,
	@BlogDate DATETIME = NULL ,
	@PageSize INT ,
	@CurrentPage INT ,
	@ShowNonPublic BIT = 0 ,
	@ShowNonPublished BIT = 0
AS 
	IF @BlogDate IS NULL 
		BEGIN
			SET @BlogDate = GETUTCDATE()
		END
	DECLARE @RowStart INT 
	DECLARE @RowEnd INT 
	
	SET @RowStart = @PageSize * ( @CurrentPage ) + 1
	SET @RowEnd = @RowStart + @PageSize
  
	SELECT  *
	FROM    ( SELECT    E.* ,
						B.[UserID] ,
						B.[SyndicationEmail] ,
						U.[UserName] ,
						U.[DisplayName] AS UserFullName ,
						( SELECT    COUNT(*)
						  FROM      dbo.BLOGDEV_Blog_Comments
						  WHERE     EntryID = E.EntryID
									AND Approved = 1
						) AS CommentCount ,
						CI.CreatedByUserID ,
						CI.CreatedOnDate ,
						CI.ContentKey ,
						CI.Indexed ,
						CI.Content ,
						CI.LastModifiedByUserID ,
						CI.LastModifiedOnDate ,
						CI.ModuleID ,
						CI.TabID ,
						CI.ContentTypeID ,
						( SELECT    COUNT(*)
						  FROM      dbo.BLOGDEV_Blog_Blogs BB
									INNER JOIN dbo.BLOGDEV_Blog_Entries BE ON BB.BlogID = BE.BlogID
									INNER JOIN dbo.BLOGDEV_Users BU ON BB.UserID = BU.UserID
						  WHERE     BB.PortalID = @PortalID
									AND ( @ShowNonPublished = 1
										  OR BE.AddedDate <= @BlogDate
										)
									AND ( BE.[Published] = 1
										  OR BE.[Published] <> @ShowNonPublished
										)
									AND ( BB.[Public] = 1
										  OR BB.[Public] <> @ShowNonPublic
										)
						) AS TotalRecords ,
						ROW_NUMBER() OVER ( ORDER BY E.AddedDate DESC ) AS RowNumber
			  FROM      dbo.BLOGDEV_Blog_Blogs B
						INNER JOIN dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID]
						INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
						LEFT OUTER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
			  WHERE     B.PortalID = @PortalID
						AND ( @ShowNonPublished = 1
							  OR E.AddedDate <= @BlogDate
							)
						AND ( E.[Published] = 1
							  OR E.[Published] <> @ShowNonPublished
							)
						AND ( B.[Public] = 1
							  OR B.[Public] <> @ShowNonPublic
							)
			) AS EntryInfo
	WHERE   RowNumber >= @RowStart
			AND RowNumber < @RowEnd 

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetEntriesByTerm]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetEntriesByTerm]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetEntriesByTerm
	@PortalID INT ,
	@BlogDate DATETIME = NULL ,
	@TagID INT ,
	@PageSize INT ,
	@CurrentPage INT ,
	@ShowNonPublic BIT = 0 ,
	@ShowNonPublished BIT = 0
AS 
	BEGIN
		IF @BlogDate IS NULL 
			BEGIN
				SET @BlogDate = GETUTCDATE()
			END
		DECLARE @RowStart INT 
		DECLARE @RowEnd INT 
  
		SET @RowStart = @PageSize * ( @CurrentPage ) + 1
		SET @RowEnd = @RowStart + @PageSize
 
		SELECT  *
		FROM    ( SELECT    E.* ,
							B.[UserID] ,
							B.[SyndicationEmail] ,
							U.[UserName] ,
							U.[DisplayName] AS UserFullName ,
							( SELECT    COUNT(*)
							  FROM      dbo.BLOGDEV_Blog_Comments
							  WHERE     EntryID = E.EntryID
										AND Approved = 1
							) AS CommentCount ,
							CI.CreatedByUserID ,
							CI.CreatedOnDate ,
							CI.ContentKey ,
							CI.Indexed ,
							CI.Content ,
							CI.LastModifiedByUserID ,
							CI.LastModifiedOnDate ,
							CI.ModuleID ,
							CI.TabID ,
							CI.ContentTypeID ,
							( SELECT    COUNT(BE.ContentItemID)
							  FROM      dbo.BLOGDEV_Blog_Blogs BB
										INNER JOIN dbo.BLOGDEV_Blog_Entries BE ON BB.BlogID = BE.BlogID
										INNER JOIN dbo.BLOGDEV_Users BU ON BB.UserID = BU.UserID
										INNER JOIN dbo.BLOGDEV_ContentItems BCI ON BE.ContentItemId = BCI.ContentItemID
										INNER JOIN dbo.BLOGDEV_ContentItems_Tags BCIT ON BE.ContentItemId = BCIT.ContentItemId
										INNER JOIN dbo.BLOGDEV_Taxonomy_Terms BTT ON BCIT.TermId = BTT.TermId
							  WHERE     BB.PortalID = @PortalID
										AND BTT.TermID = @TagID
										AND ( @ShowNonPublished = 1
											  OR BE.AddedDate <= @BlogDate
											)
										AND ( BE.[Published] = 1
											  OR BE.[Published] <> @ShowNonPublished
											)
										AND ( BB.[Public] = 1
											  OR BB.[Public] <> @ShowNonPublic
											)
							) AS TotalRecords ,
							ROW_NUMBER() OVER ( ORDER BY E.AddedDate DESC ) AS RowNumber
				  FROM      dbo.BLOGDEV_Blog_Blogs B
							INNER JOIN dbo.BLOGDEV_Blog_Entries E ON B.BlogID = E.BlogID
							INNER JOIN dbo.BLOGDEV_Users U ON B.UserID = U.UserID
							INNER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
							INNER JOIN dbo.BLOGDEV_ContentItems_Tags CIT ON E.ContentItemId = CIT.ContentItemId
							INNER JOIN dbo.BLOGDEV_Taxonomy_Terms TT ON CIT.TermId = TT.TermId
				  WHERE     B.PortalID = @PortalID
							AND TT.TermID = @TagID
							AND ( @ShowNonPublished = 1
								  OR E.AddedDate <= @BlogDate
								)
							AND ( E.[Published] = 1
								  OR E.[Published] <> @ShowNonPublished
								)
							AND ( B.[Public] = 1
								  OR B.[Public] <> @ShowNonPublic
								)
				) AS EntryInfo
		WHERE   RowNumber >= @RowStart
				AND RowNumber < @RowEnd 
	END

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetEntry]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetEntry
	@EntryID INT ,
	@PortalId INT
AS 
	SELECT  E.* ,
			B.[UserID] ,
			B.[SyndicationEmail] ,
			U.[UserName] ,
			U.[DisplayName] AS UserFullName ,
			( SELECT    COUNT(*)
			  FROM      dbo.BLOGDEV_Blog_Comments
			  WHERE     EntryID = E.EntryID
						AND Approved = 1
			) AS CommentCount ,
			CI.CreatedByUserID ,
			CI.CreatedOnDate ,
			CI.ContentKey ,
			CI.Indexed ,
			CI.Content ,
			CI.ContentItemID ,
			CI.LastModifiedByUserID ,
			CI.LastModifiedOnDate ,
			CI.ModuleID ,
			CI.TabID ,
			CI.ContentTypeID ,
			( SELECT    1
			) AS TotalRecords
	FROM    dbo.BLOGDEV_Blog_Entries E
			INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.BlogID = E.BlogID
			INNER JOIN dbo.BLOGDEV_Users U ON U.UserID = B.UserID
			LEFT OUTER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
	WHERE   E.[EntryID] = @EntryID
			AND B.PortalId = @PortalId

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetSettings]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetSettings]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetSettings
	@PortalID INT ,
	@TabID INT
AS 
	SELECT  [Key] ,
			[Value] ,
			PortalID ,
			TabID 
	FROM    dbo.BLOGDEV_Blog_Settings
	WHERE   PortalID = @PortalID
			AND TabID = @TabID

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetTermsByContentItem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetTermsByContentItem]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetTermsByContentItem
	@ContentItemId INT ,
	@VocabularyId INT
AS 
	SELECT TOP 100
			TermId ,
			Name ,
			ParentTermId ,
			[Description] ,
			[Weight] ,
			( SELECT    CreatedOnDate
			  FROM      dbo.BLOGDEV_Taxonomy_Terms T
			  WHERE     VRT.TermId = T.TermId
			) AS CreatedOnDate ,
			( SELECT    CreatedByUserId
			  FROM      dbo.BLOGDEV_Taxonomy_Terms T
			  WHERE     VRT.TermId = T.TermId
			) AS CreatedByUserId ,
			( SELECT    LastModifiedOnDate
			  FROM      dbo.BLOGDEV_Taxonomy_Terms T
			  WHERE     VRT.TermId = T.TermId
			) AS LastModifiedOnDate ,
			( SELECT    LastModifiedByUserId
			  FROM      dbo.BLOGDEV_Taxonomy_Terms T
			  WHERE     VRT.TermId = T.TermId
			) AS LastModifiedByUserId ,
			VocabularyId ,
			TermLeft ,
			TermRight ,
			( SELECT    COUNT(TermId)
			  FROM      dbo.BLOGDEV_vw_Blog_Terms
			  WHERE     TermId = VRT.TermId
						AND ContentItemId = @ContentItemId
			) AS TotalTermUsage ,
			( SELECT    COUNT(TermId)
			  FROM      dbo.BLOGDEV_vw_Blog_Terms
			  WHERE     TermId = VRT.TermId
						AND ContentItemId = @ContentItemId
						AND CreatedOnDate > DATEDIFF(day, GETDATE(), -30)
			) AS MonthTermUsage ,
			( SELECT    COUNT(TermId)
			  FROM      dbo.BLOGDEV_vw_Blog_Terms
			  WHERE     TermId = VRT.TermId
						AND ContentItemId = @ContentItemId
						AND CreatedOnDate > DATEDIFF(day, GETDATE(), -7)
			) AS WeekTermUsage ,
			( SELECT    COUNT(TermId)
			  FROM      dbo.BLOGDEV_vw_Blog_Terms
			  WHERE     TermId = VRT.TermId
						AND ContentItemId = @ContentItemId
						AND CreatedOnDate > DATEDIFF(day, GETDATE(), -1)
			) AS DayTermUsage
	FROM    dbo.BLOGDEV_vw_Blog_Terms AS VRT
	WHERE   ContentItemId = @ContentItemId
			AND VocabularyID = @VocabularyId
	GROUP BY TermId ,
			Name ,
			ParentTermId ,
			[Description] ,
			[Weight] ,
			VocabularyId ,
			TermLeft ,
			TermRight

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_GetTermsByContentType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_GetTermsByContentType]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_GetTermsByContentType
	@PortalId INT ,
	@ContentTypeId INT ,
	@VocabularyId INT
AS 
	SELECT TOP 1000
			TermID ,
			Name ,
			ParentTermID ,
			[Description] ,
			[Weight] ,
			( SELECT    CreatedOnDate
			  FROM      dbo.BLOGDEV_Taxonomy_Terms T
			  WHERE     VRT.TermID = T.TermID
			) AS CreatedOnDate ,
			( SELECT    CreatedByUserId
			  FROM      dbo.BLOGDEV_Taxonomy_Terms T
			  WHERE     VRT.TermID = T.TermID
			) AS CreatedByUserId ,
			( SELECT    LastModifiedOnDate
			  FROM      dbo.BLOGDEV_Taxonomy_Terms T
			  WHERE     VRT.TermID = T.TermID
			) AS LastModifiedOnDate ,
			( SELECT    LastModifiedByUserId
			  FROM      dbo.BLOGDEV_Taxonomy_Terms T
			  WHERE     VRT.TermID = T.TermID
			) AS LastModifiedByUserId ,
			VocabularyID ,
			TermLeft ,
			TermRight ,
			( SELECT    COUNT(TermID)
			  FROM      dbo.BLOGDEV_vw_Blog_Terms T
						INNER JOIN dbo.BLOGDEV_Blog_Entries P ON T.ContentItemID = P.ContentItemId
			  WHERE     TermID = VRT.TermID
						AND ContentTypeID = @ContentTypeID
						AND T.PortalID = @PortalID
						AND P.Published = 1
			) AS TotalTermUsage ,
			( SELECT    COUNT(TermID)
			  FROM      dbo.BLOGDEV_vw_Blog_Terms T
						INNER JOIN dbo.BLOGDEV_Blog_Entries P ON T.ContentItemID = P.ContentItemId
			  WHERE     TermID = VRT.TermID
						AND ContentTypeID = @ContentTypeID
						AND T.PortalID = @PortalID
						AND CreatedOnDate > DATEADD(day, -30, GETDATE())
						AND P.Published = 1
			) AS MonthTermUsage ,
			( SELECT    COUNT(TermID)
			  FROM      dbo.BLOGDEV_vw_Blog_Terms T
						INNER JOIN dbo.BLOGDEV_Blog_Entries P ON T.ContentItemID = P.ContentItemId
			  WHERE     TermID = VRT.TermID
						AND ContentTypeID = @ContentTypeID
						AND T.PortalID = @PortalID
						AND CreatedOnDate > DATEADD(day, -7, GETDATE())
						AND P.Published = 1
			) AS WeekTermUsage ,
			( SELECT    COUNT(TermID)
			  FROM      dbo.BLOGDEV_vw_Blog_Terms T
						INNER JOIN dbo.BLOGDEV_Blog_Entries P ON T.ContentItemID = P.ContentItemId
			  WHERE     TermID = VRT.TermID
						AND ContentTypeID = @ContentTypeID
						AND T.PortalID = @PortalID
						AND CreatedOnDate > DATEADD(day, -1, GETDATE())
						AND P.Published = 1
			) AS DayTermUsage
	FROM    dbo.BLOGDEV_vw_Blog_Terms VRT
	WHERE   VRT.PortalID = @PortalID
			AND VocabularyID = @VocabularyId
			AND ContentTypeID = @ContentTypeID
	GROUP BY TermID ,
			Name ,
			ParentTermID ,
			[Description] ,
			[Weight] ,
			VocabularyID ,
			TermLeft ,
			TermRight

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_MetaWeblog_Get_DesktopModule_FriendlyName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_MetaWeblog_Get_DesktopModule_FriendlyName]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_MetaWeblog_Get_DesktopModule_FriendlyName
	@ModuleDefinition nvarchar(50)
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	SET NOCOUNT ON
	SELECT DM.FriendlyName 
	FROM dbo.BLOGDEV_ModuleDefinitions MD
		JOIN dbo.BLOGDEV_DesktopModules DM ON MD.DesktopModuleID = DM.DesktopModuleID
	WHERE MD.FriendlyName = @ModuleDefinition
		
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_SearchByKeyWordAndBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_SearchByKeyWordAndBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE  PROCEDURE dbo.BLOGDEV_Blog_SearchByKeyWordAndBlog
	@BlogID int,
	@SearchString nvarchar(255),
	@ShowNonPublic bit,
	@ShowNonPublished bit
AS
DECLARE @separator char(1)
SET @separator = ' '
set nocount on
-- @SearchString is the array we wish to parse
-- @Separator is the separator charactor such as a comma
declare @separator_position int -- This is used to locate each separator character
declare @search_value nvarchar(255) -- this holds each array value as it is returned
declare @like_text nvarchar (257)
-- Build my Temp Table to hold results
CREATE TABLE #SearchResults (EntryID int)
-- For my loop to work I need an extra separator at the end.  I always look to the
-- left of the separator character for each array value
set @SearchString = @SearchString + @separator
-- Loop through the string searching for separtor characters
WHILE patindex('%' + @separator + '%' , @SearchString) <> 0 
BEGIN
	-- patindex matches the a pattern against a string
	select @separator_position =  patindex('%' + @separator + '%' , @SearchString)
	select @search_value = left(@SearchString, @separator_position - 1)
	
	select @like_text = '%' + @Search_value + '%'
	
	INSERT #SearchResults
	SELECT E.[EntryID]
	FROM dbo.BLOGDEV_Blog_Entries E 
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
	WHERE (B.[BlogID] = @BlogID OR B.[ParentBlogID] = @BlogID)
	AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
	AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
	AND  E.[Title] like @like_text
	INSERT #SearchResults
	SELECT E.[EntryID]
	FROM dbo.BLOGDEV_Blog_Entries E 
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
	WHERE (B.[BlogID] = @BlogID OR B.[ParentBlogID] = @BlogID)
	AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
	AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
	AND  E.[Description] like @like_text
	INSERT #SearchResults
	SELECT E.[EntryID]
	FROM dbo.BLOGDEV_Blog_Entries E 
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
	WHERE  (B.[BlogID] = @BlogID OR B.[ParentBlogID] = @BlogID)
	AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
	AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
	AND  E.[Entry] like @like_text
	  
	-- This replaces what we just processed with and empty string
	select @SearchString = stuff(@SearchString, 1, @separator_position, '')
END
SELECT S.EntryID, Rank = Count(*)
INTO #SearchResultsGrouped
FROM #SearchResults S
GROUP BY S.EntryID
	
SELECT  S.EntryID, 
	E.[BlogID], 
	B.[Title] as BlogTitle,
	E.[Title] As EntryTitle,
	IsNull(E.[Description], SubString(E.[Entry], 1, 2500)) As Summary,
	E.[AddedDate],
	E.[PermaLink],
	B.[UserID],
	U.[Username],
	U.[DisplayName] AS UserFullName,
	E.Published,
	S.Rank
	FROM   	#SearchResultsGrouped S 
		INNER JOIN dbo.BLOGDEV_Blog_Entries E ON S.EntryID = E.EntryID
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]  
		INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
Order by S.Rank DESC, E.AddedDate DESC, E.PermaLink DESC
set nocount off
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_SearchByKeyWordAndPortal]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_SearchByKeyWordAndPortal]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE  PROCEDURE dbo.BLOGDEV_Blog_SearchByKeyWordAndPortal
	@PortalID int,
	@SearchString nvarchar(255),
	@ShowNonPublic bit,
	@ShowNonPublished bit
AS
DECLARE @separator char(1)
SET @separator = ' '
set nocount on
-- @SearchString is the array we wish to parse
-- @Separator is the separator charactor such as a comma
declare @separator_position int -- This is used to locate each separator character
declare @search_value nvarchar(255) -- this holds each array value as it is returned
declare @like_text nvarchar (257)
-- Build my Temp Table to hold results
CREATE TABLE #SearchResults (EntryID int)
-- For my loop to work I need an extra separator at the end.  I always look to the
-- left of the separator character for each array value
set @SearchString = @SearchString + @separator
-- Loop through the string searching for separtor characters
WHILE patindex('%' + @separator + '%' , @SearchString) <> 0 
BEGIN
	-- patindex matches the a pattern against a string
	select @separator_position =  patindex('%' + @separator + '%' , @SearchString)
	select @search_value = left(@SearchString, @separator_position - 1)
	
	select @like_text = '%' + @Search_value + '%'
	INSERT #SearchResults
	SELECT E.[EntryID]
	FROM dbo.BLOGDEV_Blog_Entries E 
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
	WHERE B.[PortalID] = @PortalID 
	AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
	AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
	AND  E.[Title] like @like_text
	INSERT #SearchResults
	SELECT E.[EntryID]
	FROM dbo.BLOGDEV_Blog_Entries E 
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
	WHERE B.[PortalID] = @PortalID 
	AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
	AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
	AND  E.[Description] like @like_text
	INSERT #SearchResults
	SELECT E.[EntryID]
	FROM dbo.BLOGDEV_Blog_Entries E 
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
	WHERE B.[PortalID] = @PortalID 
	AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
	AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
	AND  E.[Entry] like @like_text
	  
	-- This replaces what we just processed with and empty string
	select @SearchString = stuff(@SearchString, 1, @separator_position, '')
END
SELECT S.EntryID, Rank = Count(*)
INTO #SearchResultsGrouped
FROM #SearchResults S
GROUP BY S.EntryID
	
SELECT  S.EntryID, 
	E.[BlogID], 
	B.[Title] as BlogTitle,
	E.[Title] As EntryTitle,
	IsNull(E.[Description], SubString(E.[Entry], 1, 2500)) As Summary,
	E.[AddedDate],
	E.[PermaLink],
	B.[UserID],
	U.[Username],
	U.[DisplayName] AS UserFullName,
	E.Published,
	S.Rank
	FROM   	#SearchResultsGrouped S 
		INNER JOIN dbo.BLOGDEV_Blog_Entries E ON S.EntryID = E.EntryID
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]  
		INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
Order by S.Rank DESC, E.AddedDate DESC, E.PermaLink DESC
set nocount off
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_SearchByPhraseAndBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_SearchByPhraseAndBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE  PROCEDURE dbo.BLOGDEV_Blog_SearchByPhraseAndBlog
	@BlogID int,
	@SearchString nvarchar(255),
	@ShowNonPublic bit,
	@ShowNonPublished bit
AS
set nocount on
-- Build Temp Table to hold results
CREATE TABLE #SearchResults (EntryID int)
declare @like_text nvarchar(257)
select @like_text = '%' + @SearchString + '%'
INSERT #SearchResults
SELECT E.[EntryID]
FROM dbo.BLOGDEV_Blog_Entries E 
	INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
WHERE  (B.[BlogID] = @BlogID OR B.[ParentBlogID] = @BlogID)
AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
AND  E.[Title] like @like_text
INSERT #SearchResults
SELECT E.[EntryID]
FROM dbo.BLOGDEV_Blog_Entries E 
	INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
WHERE  (B.[BlogID] = @BlogID OR B.[ParentBlogID] = @BlogID)
AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
AND  E.[Description] like @like_text
INSERT #SearchResults
SELECT E.[EntryID]
FROM dbo.BLOGDEV_Blog_Entries E 
	INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
WHERE  (B.[BlogID] = @BlogID OR B.[ParentBlogID] = @BlogID)
AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
AND  E.[Entry] like @like_text
	
SELECT S.EntryID, Rank = Count(*)
INTO #SearchResultsGrouped
FROM #SearchResults S
GROUP BY S.EntryID
	
SELECT  S.EntryID, 
	E.[BlogID], 
	B.[Title] as BlogTitle,
	E.[Title] As EntryTitle,
	IsNull(E.[Description], SubString(E.[Entry], 1, 2500)) As Summary,
	E.[AddedDate],
	E.[PermaLink],
	B.[UserID],
	U.[Username],
	U.[DisplayName] AS UserFullName,
	E.Published,
	S.Rank
	FROM   	#SearchResultsGrouped S 
		INNER JOIN dbo.BLOGDEV_Blog_Entries E ON S.EntryID = E.EntryID
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]  
		INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
Order by S.Rank DESC, E.AddedDate DESC, E.PermaLink DESC
set nocount off
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_SearchByPhraseAndPortal]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_SearchByPhraseAndPortal]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE  PROCEDURE dbo.BLOGDEV_Blog_SearchByPhraseAndPortal
	@PortalID int,
	@SearchString nvarchar(255),
	@ShowNonPublic bit,
	@ShowNonPublished bit
AS
set nocount on
-- Build Temp Table to hold results
CREATE TABLE #SearchResults (EntryID int)
declare @like_text nvarchar(257)
select @like_text = '%' + @SearchString + '%'
INSERT #SearchResults
SELECT E.[EntryID]
FROM dbo.BLOGDEV_Blog_Entries E 
	INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
WHERE B.[PortalID] = @PortalID 
AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
AND  E.[Title] like @like_text
INSERT #SearchResults
SELECT E.[EntryID]
FROM dbo.BLOGDEV_Blog_Entries E 
	INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
WHERE B.[PortalID] = @PortalID 
AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
AND  E.[Description] like @like_text
INSERT #SearchResults
SELECT E.[EntryID]
FROM dbo.BLOGDEV_Blog_Entries E 
	INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]
WHERE B.[PortalID] = @PortalID 
AND (E.[Published] = 1 OR E.[Published] <> @ShowNonPublished)
AND (B.[Public] = 1 OR B.[Public] <> @ShowNonPublic)
AND  E.[Entry] like @like_text
	
SELECT S.EntryID, Rank = Count(*)
INTO #SearchResultsGrouped
FROM #SearchResults S
GROUP BY S.EntryID
	
SELECT  S.EntryID, 
	E.[BlogID], 
	B.[Title] as BlogTitle,
	E.[Title] As EntryTitle,
	IsNull(E.[Description], SubString(E.[Entry], 1, 2500)) As Summary,
	E.[AddedDate],
	E.[PermaLink],
	B.[UserID],
	U.[Username],
	U.[DisplayName] AS UserFullName,
	E.Published,
	S.Rank
	FROM   	#SearchResultsGrouped S 
		INNER JOIN dbo.BLOGDEV_Blog_Entries E ON S.EntryID = E.EntryID
		INNER JOIN dbo.BLOGDEV_Blog_Blogs B ON B.[BlogID] = E.[BlogID]  
		INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
Order by S.Rank DESC, E.AddedDate DESC, E.PermaLink DESC
set nocount off
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_UpdateBlog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_UpdateBlog]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_UpdateBlog
	@PortalID INT ,
	@BlogID INT ,
	@ParentBlogID INT = -1 ,
	@UserID INT ,
	@Title NVARCHAR(512) ,
	@Description NVARCHAR(1024) ,
	@Public BIT ,
	@AllowComments BIT ,
	@AllowAnonymous BIT ,
	@ShowFullName BIT ,
	@Syndicated BIT ,
	@SyndicateIndependant BIT ,
	@SyndicationURL NVARCHAR(1024) ,
	@SyndicationEmail NVARCHAR(255) ,
	@EmailNotification BIT ,
	@AllowTrackbacks BIT ,
	@AutoTrackback BIT ,
	@MustApproveComments BIT ,
	@MustApproveAnonymous BIT ,
	@MustApproveTrackbacks BIT ,
	@UseCaptcha BIT ,
	@AuthorMode INT
AS 
	UPDATE  dbo.BLOGDEV_Blog_Blogs
	SET     [PortalID] = @PortalID ,
			[ParentBlogID] = @ParentBlogID ,
			[UserID] = @UserID ,
			[Title] = @Title ,
			[Description] = @Description ,
			[Public] = @Public ,
			[AllowComments] = @AllowComments ,
			[AllowAnonymous] = @AllowAnonymous ,
			[ShowFullName] = @ShowFullName ,
			[Syndicated] = @Syndicated ,
			[SyndicateIndependant] = @SyndicateIndependant ,
			[SyndicationURL] = @SyndicationURL ,
			[SyndicationEmail] = @SyndicationEmail ,
			[EmailNotification] = @EmailNotification ,
			[AllowTrackbacks] = @AllowTrackbacks ,
			[AutoTrackback] = @AutoTrackback ,
			[MustApproveComments] = @MustApproveComments ,
			[MustApproveAnonymous] = @MustApproveAnonymous ,
			[MustApproveTrackbacks] = @MustApproveTrackbacks ,
			[UseCaptcha] = @UseCaptcha ,
			[AuthorMode] = @AuthorMode
	WHERE   [BlogID] = @BlogID

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_UpdateComment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_UpdateComment]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_UpdateComment
	@CommentID int, 
	@EntryID int, 
	@UserID int, 
	@Title nvarchar(255),
	@Comment ntext,
	@Author nvarchar(50),
	@Approved bit,
	@Website nvarchar(255),
	@Email nvarchar(255),
@AddedDate datetime
AS
UPDATE dbo.BLOGDEV_Blog_Comments SET
	[EntryID] = @EntryID,
	[UserID] = @UserID,
	[Title] = @Title,
	[Comment] = @Comment,
	[Author] = @Author,
	[Approved] = @Approved,
	[AddedDate] = COALESCE(@AddedDate, GetUTCDate()),
	[Website] = @Website,
	[Email] = @Email
WHERE
	[CommentID] = @CommentID
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_UpdateEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_UpdateEntry]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_UpdateEntry
	@BlogID INT ,
	@EntryID INT ,
	@Title NVARCHAR(255) ,
	@Description NTEXT ,
	@Entry NTEXT ,
	@Published BIT ,
	@AllowComments BIT ,
	@AddedDate DATETIME ,
	@DisplayCopyright BIT ,
	@Copyright NVARCHAR(256) ,
	@PermaLink NVARCHAR(1024) ,
	@ContentItemId INT
AS 
	UPDATE  dbo.BLOGDEV_Blog_Entries
	SET     [BlogID] = @BlogID ,
			[Title] = @Title ,
			[Description] = @Description ,
			[Entry] = @Entry ,
			[Published] = @Published ,
			[AllowComments] = @AllowComments ,
			[AddedDate] = @AddedDate ,
			[DisplayCopyright] = @DisplayCopyright ,
			[Copyright] = @Copyright ,
			[PermaLink] = @PermaLink ,
			[ContentItemId] = @ContentItemId
	WHERE   [EntryID] = @EntryID
	IF @Published = 1 
		UPDATE  dbo.BLOGDEV_Blog_Blogs
		SET     [LastEntry] = GETUTCDATE()
		WHERE   [BlogID] = @BlogID

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_UpdateEntryViewCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_UpdateEntryViewCount]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.[BLOGDEV_Blog_UpdateEntryViewCount] @EntryID INT
AS 
	UPDATE  dbo.BLOGDEV_Blog_Entries
	SET     ViewCount = ViewCount + 1
	WHERE   EntryID = @EntryID
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_UpdateSetting]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_UpdateSetting]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_UpdateSetting
	@PortalID int,
	@TabID int,
	@Key nvarchar(50),
	@Value nvarchar(1024)
AS
set nocount on
If EXISTS (SELECT [Key] FROM dbo.BLOGDEV_Blog_Settings WHERE PortalID=@PortalID AND TabID=@TabID AND [Key]=@Key)
	UPDATE dbo.BLOGDEV_Blog_Settings SET
		[Value] = @Value
	WHERE PortalID=@PortalID AND TabID=@TabID AND [Key]=@Key
ELSE
	INSERT INTO dbo.BLOGDEV_Blog_Settings (PortalID, TabID,[Key], [Value])
	VALUES (@PortalID, @TabID,@Key, @Value)
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Upgrade_CategoriesGet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_Upgrade_CategoriesGet]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_Upgrade_CategoriesGet
AS 
	BEGIN
		SELECT  *
		FROM    dbo.BLOGDEV_Blog_Categories
		ORDER BY PortalID ASC ,
				ParentID ASC ,
				Category ASC
	END

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Upgrade_GetCategoriesByEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_Upgrade_GetCategoriesByEntry]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_Upgrade_GetCategoriesByEntry 
	@entryid int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT c.* 
	FROM dbo.BLOGDEV_Blog_Entry_Categories ec
	INNER JOIN dbo.BLOGDEV_Blog_Categories c on ec.CatID = c.CatID
	WHERE ec.EntryID = @entryid
	
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Upgrade_GetTagsByEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_Upgrade_GetTagsByEntry]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_Upgrade_GetTagsByEntry 
	@entryid int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT	t.*
	FROM	dbo.BLOGDEV_Blog_Tags t
	INNER JOIN dbo.BLOGDEV_Blog_Entry_Tags e ON t.TagID = e.TagID
	WHERE	EntryID = @entryid
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Upgrade_RetrieveTaxonomyEntries]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_Upgrade_RetrieveTaxonomyEntries]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE dbo.BLOGDEV_Blog_Upgrade_RetrieveTaxonomyEntries
AS 
	BEGIN
		SELECT  E.* ,
				B.[UserID] ,
				B.[SyndicationEmail] ,
				U.[UserName] ,
				U.[DisplayName] AS UserFullName ,
				( SELECT    1
				) AS CommentCount,
				CI.CreatedByUserID ,
				CI.CreatedOnDate ,
				CI.ContentKey ,
				CI.Indexed ,
				CI.Content ,
				CI.LastModifiedByUserID ,
				CI.LastModifiedOnDate ,
				CI.ModuleID ,
				CI.TabID ,
				CI.ContentTypeID ,
				( SELECT    1
				) AS TotalRecords
		FROM    dbo.BLOGDEV_Blog_Blogs B
				INNER JOIN dbo.BLOGDEV_Blog_Entries E ON B.[BlogID] = E.[BlogID]
				INNER JOIN dbo.BLOGDEV_Users U ON B.[UserID] = U.[UserID]
				LEFT OUTER JOIN dbo.BLOGDEV_ContentItems CI ON E.ContentItemId = CI.ContentItemID
		WHERE   EntryID IN ( SELECT EntryID
							 FROM   dbo.BLOGDEV_Blog_Entry_Categories C
							 WHERE  C.EntryID = EntryID )
				OR EntryID IN ( SELECT  EntryID
								FROM    dbo.BLOGDEV_Blog_Entry_Tags T
								WHERE   T.EntryID = EntryID )
	END

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BLOGDEV_Blog_Upgrade_TagsGet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[BLOGDEV_Blog_Upgrade_TagsGet]
GO



SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE dbo.BLOGDEV_Blog_Upgrade_TagsGet
AS 
 BEGIN
  SELECT  *
  FROM    dbo.BLOGDEV_Blog_Tags
  ORDER BY PortalID ASC ,
    Tag ASC
 END
GO



