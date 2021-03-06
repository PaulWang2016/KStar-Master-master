USE [KSTARService]
GO
/****** Object:  Table [dbo].[GroupDefinition]    Script Date: 7/8/2015 10:05:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupDefinition](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[LabelID] [uniqueidentifier] NULL,
	[Type] [nvarchar](50) NULL,
	[Collapsed] [bit] NOT NULL,
 CONSTRAINT [PK_GroupDefinition1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemDefinition]    Script Date: 7/8/2015 10:05:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemDefinition](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[LabelID] [uniqueidentifier] NULL,
	[Visible] [bit] NOT NULL,
	[Editable] [bit] NOT NULL,
	[Format] [nvarchar](50) NULL,
 CONSTRAINT [PK_Item1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LabelContent]    Script Date: 7/8/2015 10:05:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LabelContent](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LabelID] [uniqueidentifier] NOT NULL,
	[Language] [varchar](50) NOT NULL,
	[Content] [nvarchar](500) NULL,
 CONSTRAINT [PK_LabelContent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LogEntity]    Script Date: 7/8/2015 10:05:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogEntity](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[RequestUrl] [nvarchar](500) NULL,
	[Parameters] [nvarchar](500) NULL,
	[Message] [nvarchar](500) NULL,
	[Details] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_LogEntity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessDefinition]    Script Date: 7/8/2015 10:05:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessDefinition](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessFullName] [nvarchar](500) NOT NULL,
	[ParentID] [int] NULL,
	[ChildType] [nvarchar](50) NOT NULL,
	[ChildID] [int] NOT NULL,
	[OrderNo] [int] NOT NULL,
	[ConnectionString] [nvarchar](200) NULL,
	[Mapping] [nvarchar](200) NULL,
	[WhereString] [nvarchar](500) NULL,
 CONSTRAINT [PK_ProcessDefinition] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessExtend]    Script Date: 7/8/2015 10:05:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessExtend](
	[ProcessFullName] [nvarchar](450) NOT NULL,
	[ControllerFullName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ProcessExtend] PRIMARY KEY CLUSTERED 
(
	[ProcessFullName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessPermission]    Script Date: 7/8/2015 10:05:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessPermission](
	[ProcessFullName] [nvarchar](450) NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityName] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_ProcessPermission] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
