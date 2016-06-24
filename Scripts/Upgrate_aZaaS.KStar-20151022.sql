USE [aZaaS.KStar]
GO

/****** Object:  Table [dbo].[ReportPermission]    Script Date: 10/22/2015 1:27:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReportPermission](
	[ID] [uniqueidentifier] NOT NULL,
	[ReportID] [uniqueidentifier] NULL,
	[RoleID] [uniqueidentifier] NULL,
	[RoleName] [nvarchar](100) NULL,
 CONSTRAINT [PK_ReportPermission] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- 报表管理 菜单 & 多语言

INSERT [dbo].[LocalizationResource_ENUS] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'1a4584f8-0884-4f01-b376-dc6a0bc0bfd9', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'报表管理', N'Report Management')
INSERT [dbo].[LocalizationResource_ZHCN] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'c8978850-9b04-4b4f-9e65-e5ab154da7ac', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'报表管理', N'报表管理')
INSERT [dbo].[LocalizationResource_ZHTW] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'91bf86ee-f00c-4fe0-9951-3014beab3c43', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'报表管理', N'報表管理')

INSERT [dbo].[MenuItem] ([ID], [DisplayName], [Hyperlink], [IconKey], [EnabledRoles], [ParentId], [KindValue], [TargetValue], [MenuID], [MenuItemOrder], [Scope]) VALUES (N'9ce2405f-ff97-4f65-9ecc-d6e5757e25a3', N'报表管理', N'/Report/ReportCenter/Manage', NULL, NULL, N'fde858ad-0e9e-4058-b910-055313264ba9', 0, 2, N'277d2019-1393-4fe2-8fcf-4d9b1f6d229c', N'12', NULL)


USE [aZaaS.KStar]
GO

/****** Object:  Table [dbo].[ReportStatistics]    Script Date: 2015/10/28 10:19:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReportStatistics](
	[ID] [uniqueidentifier] NOT NULL,
	[Browser] [nvarchar](500) NULL,
	[UserHostAddress] [nvarchar](500) NULL,
	[ReportID] [uniqueidentifier] NULL,
	[SysID] [uniqueidentifier] NULL,
	[UserId] [nvarchar](500) NULL,
	[FirstName] [nvarchar](500) NULL,
	[CreateTime] [datetime] NULL,
 CONSTRAINT [PK_ReportStatistics] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


USE [aZaaS.Framework]
GO

/****** Object:  StoredProcedure [dbo].[SP_QuartzMailService]    Script Date: 10/28/2015 11:56:17 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_QuartzMailService]
	@Title NVARCHAR(2000),
	@Body  NVARCHAR(MAX),
	@Recipients NVARCHAR(500)
AS
BEGIN


INSERT INTO [QuartzCacheSendMails]
           ([Title]
           ,[Body]
           ,[ReplyTo]
           ,[Forward]
           ,[ReplyDate]
           ,[CreateDate])
     VALUES (@Title,@Body,@Recipients,0,GETDATE(),GETDATE())


END

GO


--通用报表管理 菜单

USE [aZaaS.KStar]
GO

INSERT [dbo].[LocalizationResource_ENUS] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'af173bfb-9cc9-498a-843a-39948b09c0e1', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'通用报表配置-查询', N'Common Report Config-Search')
INSERT [dbo].[LocalizationResource_ENUS] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'60c8e8a5-b0e2-4ea3-bbd1-60140fb65371', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'通用报表配置', N'Common Report Config')
INSERT [dbo].[LocalizationResource_ZHCN] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'b8671109-7f3d-46ac-a8fa-4341203b9afa', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'通用报表配置-查询', N'通用报表配置-查询')
INSERT [dbo].[LocalizationResource_ZHCN] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'6c08658e-a7d0-483f-8c83-cb86cd0904ff', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'通用报表配置', N'通用报表配置')
INSERT [dbo].[LocalizationResource_ZHTW] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'befa266d-f34b-4cd2-aca4-4123d813520b', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'通用报表配置-查询', N'通用报表配置-查询')
INSERT [dbo].[LocalizationResource_ZHTW] ([ID], [DataBaseName], [TableName], [ColumnName], [ResxKey], [ResxValue]) VALUES (N'67dc1447-229e-4441-9f72-ad4310106ed3', N'aZaaS.KStar', N'MenuItem', N'DisplayName', N'通用报表配置', N'通用报表配置')

INSERT [dbo].[MenuItem] ([ID], [DisplayName], [Hyperlink], [IconKey], [EnabledRoles], [ParentId], [KindValue], [TargetValue], [MenuID], [MenuItemOrder], [Scope]) VALUES (N'd17da322-f0f7-4af9-8f6f-189f6155d897', N'通用报表配置-查询', N'/Maintenance/CommonReportSearchConfig', NULL, NULL, N'fde858ad-0e9e-4058-b910-055313264ba9', 0, 2, N'277d2019-1393-4fe2-8fcf-4d9b1f6d229c', N'99', NULL)
INSERT [dbo].[MenuItem] ([ID], [DisplayName], [Hyperlink], [IconKey], [EnabledRoles], [ParentId], [KindValue], [TargetValue], [MenuID], [MenuItemOrder], [Scope]) VALUES (N'24ce0abb-f7ab-43d0-85fe-d578a747ca28', N'通用报表配置', N'/Maintenance/CommonReportMaitance', NULL, NULL, N'fde858ad-0e9e-4058-b910-055313264ba9', 0, 2, N'277d2019-1393-4fe2-8fcf-4d9b1f6d229c', N'99', NULL)

-- 添加[报表权限]角色类型

USE [aZaaS.KStar]

ALTER TABLE [ReportPermission]
ADD [RoleType] NVARCHAR(100)  NULL
