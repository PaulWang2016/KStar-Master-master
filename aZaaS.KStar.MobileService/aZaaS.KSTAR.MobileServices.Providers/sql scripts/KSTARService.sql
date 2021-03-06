USE [KSTARService]
GO
/****** Object:  Table [dbo].[GroupDefinition]    Script Date: 4/15/2014 6:14:57 PM ******/
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
/****** Object:  Table [dbo].[ItemDefinition]    Script Date: 4/15/2014 6:14:57 PM ******/
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
 CONSTRAINT [PK_Item1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LabelContent]    Script Date: 4/15/2014 6:14:57 PM ******/
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
/****** Object:  Table [dbo].[LogEntity]    Script Date: 4/15/2014 6:14:57 PM ******/
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
/****** Object:  Table [dbo].[ProcessDefinition]    Script Date: 4/15/2014 6:14:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessDefinition](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessFullName] [nvarchar](500) NOT NULL,
	[ParentID] [int] NOT NULL,
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
SET IDENTITY_INSERT [dbo].[GroupDefinition] ON 

INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (1, N'Task', NULL, NULL, 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (2, N'TaskInfo', NULL, NULL, 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (3, N'BaseInfo', NULL, N'Single', 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (4, N'ExtendInfo', NULL, N'Single', 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (5, N'ProcBaseInfo', N'1a8d7662-4eeb-441a-88b8-1ec3bf176f8f', N'Single', 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (6, N'BizInfo', NULL, NULL, 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (7, N'ProcLogInfo', N'f975b907-412a-4f56-b408-06c0823304d0', N'Table', 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (8, N'Header', NULL, NULL, 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (9, N'Row', NULL, NULL, 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (10, N'Data', NULL, NULL, 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (11, N'More', NULL, NULL, 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (12, N'TravelInfo', N'0cf1775d-4654-4612-ba0a-4d537647659b', N'Single', 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (13, N'FinanceInfo', N'2f9ee9a0-98d5-48c4-acc5-ede3570fe51d', N'Single', 0)
INSERT [dbo].[GroupDefinition] ([ID], [Name], [LabelID], [Type], [Collapsed]) VALUES (14, N'CostDetails', N'19ee4ab5-554b-449c-8f9e-d121cd476926', N'Table', 0)
SET IDENTITY_INSERT [dbo].[GroupDefinition] OFF
SET IDENTITY_INSERT [dbo].[ItemDefinition] ON 

INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (1, N'ActivityName', N'509a768c-a230-4637-bcba-cb4e7545acb6', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (2, N'Folio', N'a84984ce-5a98-47aa-b030-b0c3355317eb', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (3, N'Originator', NULL, 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (4, N'ProcessName', NULL, 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (5, N'ProcInstID', NULL, 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (6, N'SN', N'19631f7f-17ac-4042-b476-19232e212c8e', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (7, N'StartDate', N'e9ca2059-f0eb-4796-95a4-b5e5b35a92c6', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (8, N'Summary', NULL, 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (9, N'RequesterName', N'9e1fb16a-6a68-4567-a939-8e730e25f0c8', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (10, N'ID', N'1de0280f-3ed0-434f-8206-71c7e96782a5', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (11, N'OperatorName', N'0033c8c6-2b1c-4e52-b542-68e260b62b67', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (12, N'Action', N'a3cdfa33-e613-419b-935f-9a88ba7a2912', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (13, N'EndDate', N'9d8dbb0d-56d9-4920-a4f9-31eb3e8eeaa5', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (14, N'Opinion', N'e172c974-6007-4369-9f02-3d9d99450a21', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (15, N'Destination', N'871ad380-e94e-4df1-995c-ff56baff8d6a', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (16, N'CostCenter', N'b307a002-99a6-423f-a520-dd23edb0783c', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (17, N'TotalAmount', N'ae615a33-d1d8-4f5e-a475-6596ba35c904', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (18, N'CostType', N'3a657839-d726-4f44-91aa-95a10a968fec', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (19, N'Amount', N'0a674535-04f8-4b7c-a2b5-3aa89e904c7c', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (20, N'Remark', N'efa63998-53c9-40d8-b0cd-5a78de3999d5', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (21, N'StartDate', N'e9ca2059-f0eb-4796-95a4-b5e5b35a92c6', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (22, N'EndDate', N'9d8dbb0d-56d9-4920-a4f9-31eb3e8eeaa5', 1, 0)
INSERT [dbo].[ItemDefinition] ([ID], [Name], [LabelID], [Visible], [Editable]) VALUES (23, N'ApplicationDate', N'361bd3db-075a-47b4-805b-c86ad87936e1', 1, 0)
SET IDENTITY_INSERT [dbo].[ItemDefinition] OFF
SET IDENTITY_INSERT [dbo].[LabelContent] ON 

INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (1, N'771f7b6c-936c-48cb-b57e-e2d40bac89f4', N'zh-cn', N'登录成功')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (2, N'5d45ef67-4387-4d49-ba81-0b20e0bb21ae', N'zh-cn', N'用户帐号被禁用')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (3, N'492e7507-d08d-458a-8992-9932cedd93a6', N'zh-cn', N'用户不存在')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (4, N'30d2b74c-8662-4bc8-893e-81e512e068b3', N'zh-cn', N'用户密码不正确')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (5, N'f569a981-28c7-4853-bebd-e00a50cbaa2c', N'zh-cn', N'提交成功')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (6, N'1a8d7662-4eeb-441a-88b8-1ec3bf176f8f', N'zh-cn', N'流程基础信息')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (7, N'19631f7f-17ac-4042-b476-19232e212c8e', N'zh-cn', N'SN')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (8, N'a84984ce-5a98-47aa-b030-b0c3355317eb', N'zh-cn', N'流程主题')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (9, N'9e1fb16a-6a68-4567-a939-8e730e25f0c8', N'zh-cn', N'申请人')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (10, N'509a768c-a230-4637-bcba-cb4e7545acb6', N'zh-cn', N'环节名称')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (11, N'f975b907-412a-4f56-b408-06c0823304d0', N'zh-cn', N'流程审批历史记录')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (12, N'1de0280f-3ed0-434f-8206-71c7e96782a5', N'zh-cn', N'序号')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (13, N'0033c8c6-2b1c-4e52-b542-68e260b62b67', N'zh-cn', N'审批人')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (14, N'a3cdfa33-e613-419b-935f-9a88ba7a2912', N'zh-cn', N'操作')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (15, N'e9ca2059-f0eb-4796-95a4-b5e5b35a92c6', N'zh-cn', N'开始时间')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (16, N'9d8dbb0d-56d9-4920-a4f9-31eb3e8eeaa5', N'zh-cn', N'结束时间')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (17, N'e172c974-6007-4369-9f02-3d9d99450a21', N'zh-cn', N'审批意见')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (18, N'0cf1775d-4654-4612-ba0a-4d537647659b', N'zh-cn', N'出差信息')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (19, N'871ad380-e94e-4df1-995c-ff56baff8d6a', N'zh-cn', N'目的地')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (20, N'2f9ee9a0-98d5-48c4-acc5-ede3570fe51d', N'zh-cn', N'财务信息')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (21, N'b307a002-99a6-423f-a520-dd23edb0783c', N'zh-cn', N'成本中心')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (22, N'ae615a33-d1d8-4f5e-a475-6596ba35c904', N'zh-cn', N'费用合计')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (23, N'19ee4ab5-554b-449c-8f9e-d121cd476926', N'zh-cn', N'费用明细')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (24, N'3a657839-d726-4f44-91aa-95a10a968fec', N'zh-cn', N'费用类型')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (25, N'0a674535-04f8-4b7c-a2b5-3aa89e904c7c', N'zh-cn', N'费用')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (26, N'efa63998-53c9-40d8-b0cd-5a78de3999d5', N'zh-cn', N'说明')
INSERT [dbo].[LabelContent] ([ID], [LabelID], [Language], [Content]) VALUES (27, N'361bd3db-075a-47b4-805b-c86ad87936e1', N'zh-cn', N'申请时间')
SET IDENTITY_INSERT [dbo].[LabelContent] OFF
SET IDENTITY_INSERT [dbo].[ProcessDefinition] ON 

INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (1, N'ProcessSample\ProcessOne', 0, N'Group', 1, 0, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (2, N'ProcessSample\ProcessOne', 1, N'Group', 3, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (3, N'ProcessSample\ProcessOne', 1, N'Group', 4, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (4, N'ProcessSample\ProcessOne', 2, N'Item', 1, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (5, N'ProcessSample\ProcessOne', 2, N'Item', 2, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (6, N'ProcessSample\ProcessOne', 2, N'Item', 3, 3, NULL, N'ProcessInstance.Originator', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (7, N'ProcessSample\ProcessOne', 2, N'Item', 4, 4, NULL, N'ProcDispName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (8, N'ProcessSample\ProcessOne', 2, N'Item', 5, 5, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (9, N'ProcessSample\ProcessOne', 2, N'Item', 6, 6, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (10, N'ProcessSample\ProcessOne', 2, N'Item', 7, 7, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (11, N'ProcessSample\ProcessOne', 2, N'Item', 8, 8, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (12, N'ProcessSample\ProcessOne', 0, N'Group', 2, 0, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (13, N'ProcessSample\ProcessOne', 12, N'Group', 5, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (14, N'ProcessSample\ProcessOne', 12, N'Group', 6, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (15, N'ProcessSample\ProcessOne', 12, N'Group', 7, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (16, N'ProcessSample\ProcessOne', 13, N'Item', 6, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (17, N'ProcessSample\ProcessOne', 13, N'Item', 2, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (18, N'ProcessSample\ProcessOne', 13, N'Item', 1, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (19, N'ProcessSample\ProcessOne', 14, N'Group', 12, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (20, N'ProcessSample\ProcessOne', 14, N'Group', 13, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (21, N'ProcessSample\ProcessOne', 14, N'Group', 14, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (22, N'ProcessSample\ProcessOne', 19, N'Item', 7, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (23, N'ProcessSample\ProcessOne', 19, N'Item', 13, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (24, N'ProcessSample\ProcessOne', 19, N'Item', 15, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (25, N'ProcessSample\ProcessOne', 20, N'Item', 16, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (26, N'ProcessSample\ProcessOne', 20, N'Item', 17, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (27, N'ProcessSample\ProcessOne', 21, N'Group', 8, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (28, N'ProcessSample\ProcessOne', 21, N'Group', 9, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (29, N'ProcessSample\ProcessOne', 27, N'Item', 10, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (30, N'ProcessSample\ProcessOne', 27, N'Item', 18, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (31, N'ProcessSample\ProcessOne', 27, N'Item', 19, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (32, N'ProcessSample\ProcessOne', 28, N'Group', 11, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (33, N'ProcessSample\ProcessOne', 32, N'Item', 7, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (34, N'ProcessSample\ProcessOne', 32, N'Item', 13, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (35, N'ProcessSample\ProcessOne', 32, N'Item', 20, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (36, N'ProcessSample\ProcessOne', 15, N'Group', 8, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (37, N'ProcessSample\ProcessOne', 15, N'Group', 9, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (38, N'ProcessSample\ProcessOne', 36, N'Item', 10, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (39, N'ProcessSample\ProcessOne', 36, N'Item', 1, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (40, N'ProcessSample\ProcessOne', 36, N'Item', 11, 3, NULL, N'UserName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (41, N'ProcessSample\ProcessOne', 36, N'Item', 12, 4, NULL, N'ActionName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (42, N'ProcessSample\ProcessOne', 37, N'Group', 11, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (43, N'ProcessSample\ProcessOne', 42, N'Item', 7, 1, NULL, N'CommentDate', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (44, N'ProcessSample\ProcessOne', 42, N'Item', 13, 2, NULL, N'CommentDate', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (45, N'ProcessSample\ProcessOne', 42, N'Item', 14, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (46, N'ProcessSample\ProcessTwo', 0, N'Group', 1, 0, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (47, N'ProcessSample\ProcessTwo', 46, N'Group', 3, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (48, N'ProcessSample\ProcessTwo', 46, N'Group', 4, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (49, N'ProcessSample\ProcessTwo', 47, N'Item', 1, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (50, N'ProcessSample\ProcessTwo', 47, N'Item', 2, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (51, N'ProcessSample\ProcessTwo', 47, N'Item', 3, 3, NULL, N'ProcessInstance.Originator', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (52, N'ProcessSample\ProcessTwo', 47, N'Item', 4, 4, NULL, N'ProcDispName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (53, N'ProcessSample\ProcessTwo', 47, N'Item', 5, 5, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (54, N'ProcessSample\ProcessTwo', 47, N'Item', 6, 6, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (55, N'ProcessSample\ProcessTwo', 47, N'Item', 7, 7, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (56, N'ProcessSample\ProcessTwo', 47, N'Item', 8, 8, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (57, N'ProcessSample\ProcessTwo', 0, N'Group', 2, 0, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (58, N'ProcessSample\ProcessTwo', 57, N'Group', 5, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (59, N'ProcessSample\ProcessTwo', 57, N'Group', 6, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (60, N'ProcessSample\ProcessTwo', 57, N'Group', 7, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (61, N'ProcessSample\ProcessTwo', 58, N'Item', 6, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (62, N'ProcessSample\ProcessTwo', 58, N'Item', 2, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (63, N'ProcessSample\ProcessTwo', 58, N'Item', 1, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (64, N'ProcessSample\ProcessTwo', 59, N'Group', 12, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (65, N'ProcessSample\ProcessTwo', 59, N'Group', 13, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (66, N'ProcessSample\ProcessTwo', 59, N'Group', 14, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (67, N'ProcessSample\ProcessTwo', 64, N'Item', 7, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (68, N'ProcessSample\ProcessTwo', 64, N'Item', 13, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (69, N'ProcessSample\ProcessTwo', 64, N'Item', 15, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (70, N'ProcessSample\ProcessTwo', 65, N'Item', 16, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (71, N'ProcessSample\ProcessTwo', 65, N'Item', 17, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (72, N'ProcessSample\ProcessTwo', 66, N'Group', 8, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (73, N'ProcessSample\ProcessTwo', 66, N'Group', 9, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (74, N'ProcessSample\ProcessTwo', 72, N'Item', 10, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (75, N'ProcessSample\ProcessTwo', 72, N'Item', 18, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (76, N'ProcessSample\ProcessTwo', 72, N'Item', 19, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (77, N'ProcessSample\ProcessTwo', 73, N'Group', 11, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (78, N'ProcessSample\ProcessTwo', 77, N'Item', 7, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (79, N'ProcessSample\ProcessTwo', 77, N'Item', 13, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (80, N'ProcessSample\ProcessTwo', 77, N'Item', 20, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (81, N'ProcessSample\ProcessTwo', 60, N'Group', 8, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (82, N'ProcessSample\ProcessTwo', 60, N'Group', 9, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (83, N'ProcessSample\ProcessTwo', 81, N'Item', 10, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (84, N'ProcessSample\ProcessTwo', 81, N'Item', 1, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (85, N'ProcessSample\ProcessTwo', 81, N'Item', 11, 3, NULL, N'UserName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (86, N'ProcessSample\ProcessTwo', 81, N'Item', 12, 4, NULL, N'ActionName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (87, N'ProcessSample\ProcessTwo', 82, N'Group', 11, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (88, N'ProcessSample\ProcessTwo', 87, N'Item', 7, 1, NULL, N'CommentDate', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (89, N'ProcessSample\ProcessTwo', 87, N'Item', 13, 2, NULL, N'CommentDate', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (90, N'ProcessSample\ProcessTwo', 87, N'Item', 14, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (91, N'TheLink\TDCW', 0, N'Group', 1, 0, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (92, N'TheLink\TDCW', 91, N'Group', 3, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (93, N'TheLink\TDCW', 91, N'Group', 4, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (94, N'TheLink\TDCW', 92, N'Item', 1, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (95, N'TheLink\TDCW', 92, N'Item', 2, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (96, N'TheLink\TDCW', 92, N'Item', 3, 3, NULL, N'ProcessInstance.Originator', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (97, N'TheLink\TDCW', 92, N'Item', 4, 4, NULL, N'ProcDispName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (98, N'TheLink\TDCW', 92, N'Item', 5, 5, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (99, N'TheLink\TDCW', 92, N'Item', 6, 6, NULL, NULL, NULL)
GO
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (100, N'TheLink\TDCW', 92, N'Item', 7, 7, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (101, N'TheLink\TDCW', 92, N'Item', 8, 8, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (102, N'TheLink\TDCW', 0, N'Group', 2, 0, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (103, N'TheLink\TDCW', 102, N'Group', 5, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (104, N'TheLink\TDCW', 102, N'Group', 6, 2, N'Data Source=192.168.1.35;Initial Catalog=TenantDatabase;Persist Security Info=True;User ID=sa;Password=K2pass!;', NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (105, N'TheLink\TDCW', 102, N'Group', 7, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (106, N'TheLink\TDCW', 103, N'Item', 6, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (107, N'TheLink\TDCW', 103, N'Item', 2, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (108, N'TheLink\TDCW', 103, N'Item', 1, 3, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (109, N'TheLink\TDCW', 104, N'Group', 12, 1, NULL, N'RequestMainInfo', N'FormNo=@Folio')
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (110, N'TheLink\TDCW', 104, N'Group', 13, 2, NULL, N'RequestMainInfo', N'where [ProcInstID]=@ProcessInstance.ID')
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (111, N'TheLink\TDCW', 104, N'Group', 14, 3, NULL, N'ActivistsDetails', N'FK_Form_ID=(SELECT [ID] FROM [RequestMainInfo] WHERE ProcInstID=@ProcInstID  And FormNo=@Folio)')
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (112, N'TheLink\TDCW', 109, N'Item', 9, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (113, N'TheLink\TDCW', 109, N'Item', 23, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (114, N'TheLink\TDCW', 109, N'Item', 15, 3, NULL, N'ChangeReason', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (115, N'TheLink\TDCW', 110, N'Item', 23, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (116, N'TheLink\TDCW', 110, N'Item', 9, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (117, N'TheLink\TDCW', 111, N'Group', 8, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (118, N'TheLink\TDCW', 111, N'Group', 9, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (119, N'TheLink\TDCW', 117, N'Item', 9, 1, NULL, N'TermFrom', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (120, N'TheLink\TDCW', 117, N'Item', 23, 2, NULL, N'TermTo', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (121, N'TheLink\TDCW', 117, N'Item', 19, 3, NULL, N'Remarks', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (122, N'TheLink\TDCW', 118, N'Group', 11, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (123, N'TheLink\TDCW', 122, N'Item', 9, 1, NULL, N'ID', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (124, N'TheLink\TDCW', 122, N'Item', 23, 2, NULL, N'Association', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (125, N'TheLink\TDCW', 122, N'Item', 20, 3, NULL, N'Post', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (126, N'TheLink\TDCW', 105, N'Group', 8, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (127, N'TheLink\TDCW', 105, N'Group', 9, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (128, N'TheLink\TDCW', 126, N'Item', 10, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (129, N'TheLink\TDCW', 126, N'Item', 1, 2, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (130, N'TheLink\TDCW', 126, N'Item', 11, 3, NULL, N'UserName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (131, N'TheLink\TDCW', 126, N'Item', 12, 4, NULL, N'ActionName', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (132, N'TheLink\TDCW', 127, N'Group', 11, 1, NULL, NULL, NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (133, N'TheLink\TDCW', 132, N'Item', 7, 1, NULL, N'CommentDate', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (134, N'TheLink\TDCW', 132, N'Item', 13, 2, NULL, N'CommentDate', NULL)
INSERT [dbo].[ProcessDefinition] ([ID], [ProcessFullName], [ParentID], [ChildType], [ChildID], [OrderNo], [ConnectionString], [Mapping], [WhereString]) VALUES (135, N'TheLink\TDCW', 132, N'Item', 14, 3, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ProcessDefinition] OFF
