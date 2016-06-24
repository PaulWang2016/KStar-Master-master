USE [aZaaS.Framework]
GO

/****** Object:  Table [dbo].[RoleCategory]    Script Date: 2015/8/24 15:07:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
--创建角色分类表
CREATE TABLE [dbo].[RoleCategory](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Parent_SysId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_RoleCategory] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

--增加角色与分类外键
ALTER TABLE  [dbo].[Role] ADD Category_SysId  uniqueidentifier
GO

--初始化角色分类默认分类数据 以及与角色的关联
declare @category_sysid uniqueidentifier
set @category_sysid=NEWID()
insert into [dbo].[RoleCategory](SysId,Name,Parent_SysId)
values(@category_sysid,N'默认分类',null)

update [dbo].[Role] set Category_SysId=@category_sysid



 
GO

/****** Object:  Table [dbo].[ProcessActivityParticipantSet]    Script Date: 2016/3/4 15:51:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProcessActivityParticipantSet](
	[SetID] [uniqueidentifier] NOT NULL,
	[Assigner] [nvarchar](50) NOT NULL,
	[AssignerName] [nvarchar](100) NULL,
	[Setter] [nvarchar](50) NULL,
	[SetterName] [nvarchar](100) NULL,
	[Priority] [int] NULL,
	[ProcInstID] [int] NULL,
	[ProcessFullName] [nvarchar](100) NULL,
	[ActivityID] [int] NULL,
	[ActivityName] [nvarchar](100) NULL,
	[IsPeeked] [bit] NULL,
	[IsOriginal] [bit] NULL,
	[SkipAssigner] [bit] NULL,
	[SkipSet] [bit] NULL,
	[DateAssigned] [datetime] NULL,
	[Remark] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[SetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
 
GO

/****** Object:  Table [dbo].[ProcessActivityParticipantSetEntry]    Script Date: 2016/3/4 15:51:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProcessActivityParticipantSetEntry](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SetID] [uniqueidentifier] NULL,
	[EntryType] [nvarchar](20) NULL,
	[EntryID] [uniqueidentifier] NULL,
	[EntryName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


