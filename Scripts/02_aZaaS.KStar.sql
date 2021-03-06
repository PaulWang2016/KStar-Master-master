USE [aZaaS.KStar]
GO
/****** Object:  StoredProcedure [dbo].[CheckAuthorizationAccessProcess]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CheckAuthorizationAccessProcess]
(
	@ProcessFullName nvarchar(500),
	@UserName nvarchar(max)	
)	
AS
BEGIN
	declare @UserSysId uniqueidentifier,@ProcessSetId int,@Result int=0
	select @UserSysId=SysId from [aZaaS.Framework].dbo.[User] where UserName=@UserName
	select @ProcessSetId=ID from [dbo].[Configuration_ProcessSet] where ProcessFullName=@ProcessFullName


	select @Result=COUNT(1) from Configuration_User  where [Key]=@UserSysId and  RefType='ProcessSet' and OperateType='StartProcess'  and RefID=@ProcessSetId and UserType='User'
	if(@Result>0)
	  select '1'
    else
	begin
	    select @Result=COUNT(1) from Configuration_User a inner join [aZaaS.Framework].dbo.PositionUsers b on a.[Key]=CONVERT(char(255),b.Position_SysId) where User_SysId=@UserSysId and RefType='ProcessSet' and OperateType='StartProcess' and RefID=@ProcessSetId and UserType='Position'
		if(@Result>0)
	       select '1'
		else
		begin
		   select @Result=COUNT(1) from Configuration_User a inner join [aZaaS.Framework].dbo.[RoleUsers] b on a.[Key]=CONVERT(char(255), b.Role_SysId)   where User_SysId=@UserSysId and RefType='ProcessSet' and OperateType='StartProcess' and RefID=@ProcessSetId and UserType='Role'
			if(@Result>0)
			   select '1'
			else
			begin
				select @Result=COUNT(1) from Configuration_User a inner join [aZaaS.Framework].dbo.UserOrgNodes b on a.[Key]=CONVERT(char(255), b.OrgNode_SysId)   where User_SysId=@UserSysId and RefType='ProcessSet' and OperateType='StartProcess' and RefID=@ProcessSetId and UserType='OrgNode'
				if(@Result>0)
				   select '1'
				else
				begin
				   select '0'
				end
			end
		end
	end


END


GO
/****** Object:  StoredProcedure [dbo].[GetActivityConfiguredGeneralParticipants]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetActivityConfiguredGeneralParticipants]
@ProcessFullName nvarchar(500),
@ActivityName    nvarchar(500)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @ActivityId int	
    --创建临时表
	CREATE TABLE #User(
		[SysId] [uniqueidentifier] NOT NULL,
		[UserName] [nvarchar](max) NULL,
		[FirstName] [nvarchar](max) NULL,
		[LastName] [nvarchar](max) NULL,
		[Sex] [nvarchar](max) NULL,
		[Email] [nvarchar](max) NULL,
		[Address] [nvarchar](max) NULL,
		[Phone] [nvarchar](max) NULL,
		[Status] [nvarchar](max) NULL,
		[Remark] [nvarchar](max) NULL,
		[UserId] [nvarchar](50) NULL,
		[CreateDate] [datetime] NULL
	)
		
	SELECT @ActivityId=a.[ID] FROM [K2].[Server].[Act] a inner join [K2].[Server].[ProcSet] b on a.[ProcID]=b.ProcVerID  
	WHERE b.FullName=@ProcessFullName  AND a.[Name] = @ActivityName
	
	select b.* into #ConfigurationUser from Configuration_Activity a inner join 
	Configuration_User b on a.ID=b.RefID and b.RefType='Activity' where a.ActivityID=@ActivityId
	
	
	insert into #User select b.* from #ConfigurationUser a inner join [aZaaS.Framework].dbo.[User] b on a.[Key]=CONVERT(char(255),b.SysId) where  a.UserType='User'	
	insert into #User select c.* from #ConfigurationUser a inner join [aZaaS.Framework].dbo.PositionUsers b on a.[Key]=CONVERT(char(255),b.Position_SysId) inner join [aZaaS.Framework].dbo.[User] c on b.User_SysId=c.SysId  where  a.UserType='Position'
	insert into #User select c.* from #ConfigurationUser a inner join [aZaaS.Framework].dbo.RoleUsers b on a.[Key]=CONVERT(char(255),b.Role_SysId) inner join [aZaaS.Framework].dbo.[User] c on b.User_SysId=c.SysId  where  a.UserType='Role'
	insert into #User select c.* from #ConfigurationUser a inner join [aZaaS.Framework].dbo.UserOrgNodes b on a.[Key]=CONVERT(char(255),b.OrgNode_SysId) inner join [aZaaS.Framework].dbo.[User] c on b.User_SysId=c.SysId  where  a.UserType='OrgNode'
		
	select * from #User
END

GO
/****** Object:  StoredProcedure [dbo].[GetRelatedProcessSetByUserName]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetRelatedProcessSetByUserName]
(@UserName varchar(max))
AS
BEGIN
	declare @UserSysId uniqueidentifier,@PositionSysIds nvarchar(500)='';
	create table #TempIds(SysId nvarchar(50),[Type] nvarchar(20))
    select @UserSysId=SysId from [aZaaS.Framework].dbo.[User] where UserName=@UserName
	insert into #TempIds  select Position_SysId,'Position' from [aZaaS.Framework].dbo.PositionUsers where User_SysId=@UserSysId
    select @PositionSysIds=@PositionSysIds+','+SysId  from #TempIds		
	
	insert into #TempIds select @UserSysId,'User'
	insert into #TempIds  select OrgNode_SysId,'Department' from [aZaaS.Framework].dbo.UserOrgNodes where User_SysId=@UserSysId	
	insert into #TempIds  select Role_SysId,'Role' from [aZaaS.Framework].dbo.RoleUsers where User_SysId=@UserSysId

	select distinct @UserName as UserName,b.ProcessSetID,b.ProcessFullName,b.ProcessName,0 ProcessVersionID,0 ActivityID,'' as ActivityName ,SUBSTRING(@PositionSysIds,2,len(@PositionSysIds)) PositionSysId
	  from Configuration_User a  inner join Configuration_ProcessSet b on a.RefID=b.ID 
	and a.RefType='ProcessSet'
	 where a.[Key] in(select SysId from #TempIds)
	 union
	 	select distinct @UserName as UserName,d.ProcessSetID,d.ProcessFullName,d.ProcessName,c.ProcessVersionID,b.ActivityID,e.Name as ActivityName,SUBSTRING(@PositionSysIds,2,len(@PositionSysIds))  PositionSysId 
		from Configuration_User a  inner join Configuration_Activity b on a.RefID=b.ID 
		inner join Configuration_ProcessVersion c on b.Configuration_ProcessVersionID=c.ID
		inner join Configuration_ProcessSet d on c.Configuration_ProcessSetID=d.ID
		inner join [K2].[Server].[Act] e on b.ActivityID=e.ID
	 where a.[Key] in(select SysId from #TempIds) and a.RefType='Activity'
	 

END

GO
/****** Object:  StoredProcedure [dbo].[SP_GetCandidateUsers]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetCandidateUsers]	
(@SysId uniqueidentifier)
AS
BEGIN
	 create table #orgnodetree(SysId uniqueidentifier,OrgNode_SysId uniqueidentifier)
	 select identity(int,1,1) as id,a.SysId,b.OrgNode_SysId into #orgnode from dbo.Candidate a INNER JOIN
				   [aZaaS.Framework].dbo.[UserOrgNodes] b ON a.PersonId = b.OrgNode_SysId and a.Type = 'Department' and a.SysId=@SysId
	 declare @count int,@index int=1,@tempid uniqueidentifier,@tempsysid uniqueidentifier
	 select @count=count(1) from #orgnode

	 while(@index<=@count)
	 begin
		select @tempid=OrgNode_SysId,@tempsysid=SysId from #orgnode where id=@index;
		WITH tree
		AS
		(    
		 SELECT * FROM [aZaaS.Framework].dbo.OrgNode where SysId=@tempid
		 UNION ALL   
		 SELECT a.* FROM [aZaaS.Framework].dbo.OrgNode a, tree b  WHERE a.Parent_SysId = b.SysId
		) 
		insert into #orgnodetree
		SELECT distinct @tempsysid,SysId FROM tree 
		select @index=@index+1
	 end


	 SELECT   a.SysId, b.SysId AS PersonId, b.UserName, b.FirstName, b.LastName
	 FROM      dbo.Candidate a INNER JOIN
					[aZaaS.Framework].dbo.[User] b ON a.PersonId = b.SysId AND a.Type = 'Person' and a.SysId=@SysId
	 UNION
	 SELECT   a.SysId, c.SysId AS PersonId, c.UserName, c.FirstName, c.LastName
	 FROM      dbo.Candidate a INNER JOIN
					[aZaaS.Framework].dbo.[PositionUsers] b ON a.PersonId = b.Position_SysId AND a.Type = 'Position' and a.SysId=@SysId INNER JOIN
					[aZaaS.Framework].dbo.[User] c ON b.User_SysId = c.SysId
     UNION
	 SELECT   a.SysId, c.SysId AS PersonId, c.UserName, c.FirstName, c.LastName
	 FROM      dbo.Candidate a INNER JOIN
					[aZaaS.Framework].dbo.[RoleUsers] b ON a.PersonId = b.Role_SysId AND a.Type = 'Role' and a.SysId=@SysId INNER JOIN
					[aZaaS.Framework].dbo.[User] c ON b.User_SysId = c.SysId
	 UNION
	 SELECT b.SysId, c.SysId AS PersonId, c.UserName, c.FirstName, c.LastName 
	 FROM  [aZaaS.Framework].dbo.[UserOrgNodes] a
		   inner join #orgnodetree b on a.OrgNode_SysId=b.OrgNode_SysId
		   inner join [aZaaS.Framework].dbo.[User] c ON a.User_SysId = c.SysId
END




GO
/****** Object:  StoredProcedure [dbo].[SP_NW_ProcInstUrge]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[SP_NW_ProcInstUrge]
@FullName	nvarchar(100),
@ActName	nvarchar(100)
as
SELECT 
	cast(wl.procinstid as nvarchar(10)) + '_' + cast(wl.ActInstDestID as nvarchar(10)) as SN,
	wl.Destination,
	wl.StartDate,
	wlh.Data as url
  FROM [K2].[ServerLog].[Worklist] wl
	inner join [K2].[Server].[WorklistHeader] wlh
		on wl.ProcInstID = wlh.ProcInstID
			and wl.ActInstDestID = wlh.ActInstDestID
	inner join [K2].[Server].[Act] act
		on wlh.ActID = act.ID
	inner join [K2].[Server].[ProcInst] pinst
		on wlh.ProcInstID = pinst.ID
	inner join [K2].[Server].[Proc] pc
		on pinst.ProcID = pc.ID
	inner join [K2].[Server].[ProcSet] ps
		on pc.ProcSetID = ps.ID
  where ps.FullName = @FullName
  and act.Name = @ActName
  and datediff(d,wl.StartDate,getdate()) > 1 

GO
/****** Object:  StoredProcedure [dbo].[UpdateProcessSetStartPerson]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateProcessSetStartPerson]
(
@UserName varchar(max),
@ToUserName varchar(max)
)
AS
BEGIN
	declare @UserSysId uniqueidentifier,@ToUserSysId uniqueidentifier,@FirstName nvarchar(max),@LastName nvarchar(max);
	
    select @UserSysId=SysId from [aZaaS.Framework].dbo.[User] where UserName=@UserName
	select @ToUserSysId=SysId,@FirstName=FirstName,@LastName=LastName from [aZaaS.Framework].dbo.[User] where UserName=@ToUserName
	
	
	delete from [aZaaS.Framework].dbo.PositionUsers  where User_SysId=@UserSysId
		and Position_SysId in(select a.Position_SysId  from (
	(select * from [aZaaS.Framework].dbo.PositionUsers where User_SysId=@UserSysId) a inner join
	(select * from [aZaaS.Framework].dbo.PositionUsers where User_SysId=@ToUserSysId) b
	 on a.Position_SysId=b.Position_SysId)) 

	update [aZaaS.Framework].dbo.PositionUsers set User_SysId=@ToUserSysId where User_SysId=@UserSysId
 	

	delete from [aZaaS.Framework].dbo.RoleUsers  where User_SysId=@UserSysId
		and Role_SysId in(select a.Role_SysId  from (
	(select * from [aZaaS.Framework].dbo.RoleUsers where User_SysId=@UserSysId) a inner join
	(select * from [aZaaS.Framework].dbo.RoleUsers where User_SysId=@ToUserSysId) b
	 on a.Role_SysId=b.Role_SysId)) 

	update [aZaaS.Framework].dbo.RoleUsers set User_SysId=@ToUserSysId where User_SysId=@UserSysId	

	
	delete from Configuration_User where ID in (select a.ID from (select * from Configuration_User  where UserType='User' 
	and [key]=CONVERT(char(255), @UserSysId)) a inner join
	(select * from Configuration_User where UserType='User' and [key]=CONVERT(char(255), @ToUserSysId)	) b
	on a.RefID=b.RefID and a.UserType=b.UserType and a.RefType=b.RefType and a.OperateType=b.OperateType)


	update  Configuration_User set  Value=@LastName+@FirstName,[Key]=@ToUserSysId
	 where UserType='User' and [key]=CONVERT(char(255), @UserSysId)		
END


GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AppDelegate]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AppDelegate](
	[Id] [uniqueidentifier] NOT NULL,
	[AppId] [uniqueidentifier] NOT NULL,
	[ProcessFullName] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AppRole]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppRole](
	[MenuId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AttachmentInfo]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachmentInfo](
	[ID] [uniqueidentifier] NOT NULL,
	[FileName] [nvarchar](50) NULL,
	[FileID] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[Creator] [nvarchar](50) NULL,
	[Remark] [nvarchar](500) NULL,
	[Form_ID] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AuthorizationMatrices]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthorizationMatrices](
	[SysId] [uniqueidentifier] NOT NULL,
	[AuthorityId] [uniqueidentifier] NOT NULL,
	[ResourcePermissionSysId] [uniqueidentifier] NOT NULL,
	[AuthorityType] [int] NOT NULL,
	[Granted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Configuration_Activity]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration_Activity](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityID] [int] NOT NULL,
	[Configuration_ProcessVersionID] [int] NOT NULL,
	[ProcessTime] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Configuration_Category]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration_Category](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Configuration_ProcCommon]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration_ProcCommon](
	[UserName] [nvarchar](256) NOT NULL,
	[ConfigProcSetID] [int] NOT NULL,
 CONSTRAINT [PK_Configuration_ProcCommon] PRIMARY KEY CLUSTERED 
(
	[UserName] ASC,
	[ConfigProcSetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Configuration_ProcessSet]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration_ProcessSet](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessSetID] [int] NOT NULL,
	[Configuration_CategoryID] [int] NULL,
	[ProcessSetNo] [nvarchar](500) NULL,
	[ProcessFullName] [nvarchar](500) NULL,
	[ProcessName] [nvarchar](500) NULL,
	[OrderNo] [int] NULL,
	[StartUrl] [nvarchar](500) NULL,
	[ViewUrl] [nvarchar](500) NULL,
	[ApproveUrl] [nvarchar](500) NULL,
	[NotAssignIfApproved] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Configuration_ProcessVersion]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration_ProcessVersion](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Configuration_ProcessSetID] [int] NOT NULL,
	[ProcessVersionID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Configuration_RefActivity]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration_RefActivity](
	[Configuration_ActivityID] [int] NOT NULL,
	[ActivityID] [int] NOT NULL,
 CONSTRAINT [PK_Configuration_RefActivity] PRIMARY KEY CLUSTERED 
(
	[Configuration_ActivityID] ASC,
	[ActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Configuration_User]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration_User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RefID] [int] NOT NULL,
	[UserType] [nvarchar](50) NULL,
	[RefType] [nvarchar](50) NULL,
	[OperateType] [nvarchar](50) NULL,
	[Value] [nvarchar](max) NULL,
	[Key] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CustomRoleCategory]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomRoleCategory](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Parent_SysId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CustomRoleClassify]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomRoleClassify](
	[SysId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](100) NULL,
	[ClassName] [varchar](100) NOT NULL,
	[AssembleName] [varchar](100) NOT NULL,
	[Status] [varchar](1) NULL,
	[Category_SysId] [uniqueidentifier] NOT NULL,
	[RoleKey] [uniqueidentifier] NULL,
 CONSTRAINT [PK__CustomRo__EB33B1C237C11E7A] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DataDictionary]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataDictionary](
	[Id] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](80) NULL,
	[Value] [nvarchar](200) NULL,
	[Type] [int] NOT NULL,
	[Order] [int] NULL,
	[Remark] [nvarchar](200) NULL,
 CONSTRAINT [PK_DataDictionary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentItem]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentItem](
	[ID] [uniqueidentifier] NOT NULL,
	[DisplayName] [nvarchar](max) NULL,
	[FileName] [nvarchar](max) NULL,
	[IconPath] [nvarchar](max) NULL,
	[FileExtension] [nvarchar](max) NULL,
	[Creator] [nvarchar](max) NULL,
	[CreateTime] [datetime] NOT NULL,
	[EnabledRoles] [nvarchar](max) NULL,
	[StorageUri] [nvarchar](max) NULL,
	[DocumentLibraryID] [uniqueidentifier] NOT NULL,
	[DocumentItemOrder] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentLibrary]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentLibrary](
	[ID] [uniqueidentifier] NOT NULL,
	[Key] [nvarchar](128) NOT NULL,
	[DisplayName] [nvarchar](max) NULL,
	[IconPath] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[EnabledRoles] [nvarchar](max) NULL,
	[MenuID] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DynamicWidget]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DynamicWidget](
	[ID] [uniqueidentifier] NOT NULL,
	[Key] [nvarchar](128) NOT NULL,
	[DisplayName] [nvarchar](max) NULL,
	[RazorContent] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[EnabledRoles] [nvarchar](max) NULL,
	[MenuID] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fx_User]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fx_User](
	[UserName] [nvarchar](128) NOT NULL,
	[Password] [nvarchar](max) NULL,
	[PasswordSalt] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Fx_User] PRIMARY KEY CLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LocalizationResource_ENUS]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LocalizationResource_ENUS](
	[ID] [uniqueidentifier] NOT NULL,
	[DataBaseName] [varchar](50) NOT NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[ColumnName] [nvarchar](100) NOT NULL,
	[ResxKey] [nvarchar](100) NOT NULL,
	[ResxValue] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_LocalizationResource_ENUS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LocalizationResource_ZHCN]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LocalizationResource_ZHCN](
	[ID] [uniqueidentifier] NOT NULL,
	[DataBaseName] [varchar](50) NOT NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[ColumnName] [nvarchar](100) NOT NULL,
	[ResxKey] [nvarchar](100) NOT NULL,
	[ResxValue] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LocalizationResource_ZHTW]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LocalizationResource_ZHTW](
	[ID] [uniqueidentifier] NOT NULL,
	[DataBaseName] [varchar](50) NOT NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[ColumnName] [nvarchar](100) NOT NULL,
	[ResxKey] [nvarchar](100) NOT NULL,
	[ResxValue] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[ID] [uniqueidentifier] NOT NULL,
	[Key] [nvarchar](128) NOT NULL,
	[DisplayName] [nvarchar](max) NOT NULL,
	[DefaultPage] [nvarchar](max) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[MenuOrder] [nvarchar](max) NULL,
	[EnabledRoles] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MenuItem]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MenuItem](
	[ID] [uniqueidentifier] NOT NULL,
	[DisplayName] [nvarchar](max) NOT NULL,
	[Hyperlink] [nvarchar](max) NULL,
	[IconKey] [nvarchar](max) NULL,
	[EnabledRoles] [nvarchar](max) NULL,
	[ParentId] [uniqueidentifier] NULL,
	[KindValue] [int] NOT NULL,
	[TargetValue] [int] NOT NULL,
	[MenuID] [uniqueidentifier] NOT NULL,
	[MenuItemOrder] [nvarchar](max) NULL,
	[Scope] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[SysId] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PortalEnvironment]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PortalEnvironment](
	[ID] [uniqueidentifier] NOT NULL,
	[Key] [nvarchar](256) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportCategory]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportCategory](
	[ID] [uniqueidentifier] NOT NULL,
	[ParnentID] [uniqueidentifier] NOT NULL,
	[Category] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportFavourite]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportFavourite](
	[ID] [uniqueidentifier] NOT NULL,
	[ParnentID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportInfo]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReportInfo](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Department] [nvarchar](100) NOT NULL,
	[PublishedDate] [datetime] NOT NULL,
	[Level] [nvarchar](100) NOT NULL,
	[Category] [nvarchar](100) NOT NULL,
	[ReportCode] [nvarchar](100) NOT NULL,
	[Status] [nvarchar](100) NOT NULL,
	[Rate] [nvarchar](100) NOT NULL,
	[ReportUrl] [varchar](200) NULL,
	[ImageThumbPath] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](200) NULL,
	[IsFavourite] [bit] NOT NULL,
	[ParnentID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK__ReportIn__3214EC27AF19182D] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReportInfo_Favourite]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportInfo_Favourite](
	[ID] [uniqueidentifier] NOT NULL,
	[FavouriteID] [uniqueidentifier] NOT NULL,
	[ReportInfoID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[FavouriteDate] [datetime] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ReportInfo_Favourite] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportInfo_Feedback]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportInfo_Feedback](
	[ID] [uniqueidentifier] NOT NULL,
	[Rate] [int] NOT NULL,
	[Comment] [nvarchar](2000) NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[CommitDate] [datetime] NOT NULL,
	[ReportInfoID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_FeedbackInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RequestMainInfo]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestMainInfo](
	[ID] [uniqueidentifier] NOT NULL,
	[FormNo] [nvarchar](max) NULL,
	[ProcInstID] [int] NOT NULL,
	[FormType] [nvarchar](max) NULL,
	[ApplicantName] [nvarchar](max) NULL,
	[ApplicantAccount] [nvarchar](max) NULL,
	[ApplicantEmail] [nvarchar](max) NULL,
	[ApplicantDepartment] [nvarchar](max) NULL,
	[ApplicantJobTitle] [nvarchar](max) NULL,
	[RequesterAccount] [nvarchar](max) NULL,
	[RequesterName] [nvarchar](max) NULL,
	[RequesterJobTitle] [nvarchar](max) NULL,
	[RequesterEmail] [nvarchar](max) NULL,
	[RequesterDepartment] [nvarchar](max) NULL,
	[FormStatus] [nvarchar](max) NULL,
	[ApplicationDate] [datetime] NULL,
	[RecordLevelType] [nvarchar](max) NULL,
	[TradeMix] [nvarchar](max) NULL,
	[CustomerType] [nvarchar](max) NULL,
	[UnitType] [nvarchar](max) NULL,
	[CustomerCode] [nvarchar](max) NULL,
	[PropertyCode] [nvarchar](max) NULL,
	[UnitCode] [nvarchar](max) NULL,
	[RecordContent] [nvarchar](max) NULL,
	[RefFormID] [nvarchar](max) NULL,
	[ChangeReason] [nvarchar](max) NULL,
	[Comment] [nvarchar](max) NULL,
	[ReviewerAccount] [nvarchar](50) NULL,
	[ReviewerName] [nvarchar](50) NULL,
	[ReviewerJobTitle] [nvarchar](50) NULL,
	[ReviewerEmail] [nvarchar](50) NULL,
	[ReviewerDepartment] [nvarchar](50) NULL,
	[RecipReviewerAccount] [nvarchar](50) NULL,
	[RecipReviewerName] [nvarchar](50) NULL,
	[RecipReviewerJobTitle] [nvarchar](50) NULL,
	[RecipReviewerDepartment] [nvarchar](50) NULL,
	[RecipReviewerEmail] [nvarchar](50) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[ModifyDate] [datetime] NULL,
	[ActivityState] [nvarchar](max) NULL,
	[ActivityDate] [datetime] NULL,
 CONSTRAINT [PK_dbo.RequestMainInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ResourcePermissions]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResourcePermissions](
	[SysId] [uniqueidentifier] NOT NULL,
	[PermissionSysId] [uniqueidentifier] NOT NULL,
	[ResourceId] [nvarchar](max) NULL,
	[ResourceType] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Role_ProcessSet]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role_ProcessSet](
	[Role_SysId] [uniqueidentifier] NOT NULL,
	[ProcessFullName] [nvarchar](500) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SuperAD]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuperAD](
	[UserID] [uniqueidentifier] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Task]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Task](
	[TaskName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](128) NULL,
	[PrivateBinPath] [nvarchar](128) NULL,
	[AssemblyName] [nvarchar](128) NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[LastRunTime] [datetime] NULL,
	[Status] [int] NOT NULL,
	[RunCount] [int] NOT NULL,
	[OnErrorNotification] [bit] NOT NULL,
	[OnExecNotification] [bit] NOT NULL,
	[NotificationReceiver] [nvarchar](256) NULL,
PRIMARY KEY CLUSTERED 
(
	[TaskName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TaskExtraData]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskExtraData](
	[TaskName] [nvarchar](50) NOT NULL,
	[DataKey] [nvarchar](50) NOT NULL,
	[DataValue] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[TaskName] ASC,
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TaskRunHistory]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskRunHistory](
	[LogId] [int] NOT NULL,
	[TaskName] [nvarchar](50) NOT NULL,
	[RunStatus] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[RunTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TaskTriggerByInterval]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskTriggerByInterval](
	[TriggerName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[StartTime] [datetime] NOT NULL,
	[Interval] [float] NULL,
	[IntervalType] [int] NOT NULL,
	[ExitOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TriggerName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[UserId] [uniqueidentifier] NOT NULL,
	[Key] [nvarchar](200) NOT NULL,
	[Value] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ViewFlowArgs]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ViewFlowArgs](
	[ProcInstID] [nvarchar](50) NOT NULL,
	[FlowArgs] [nvarchar](max) NULL,
 CONSTRAINT [PK_ProcParameter] PRIMARY KEY CLUSTERED 
(
	[ProcInstID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Widget]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Widget](
	[ID] [uniqueidentifier] NOT NULL,
	[Key] [nvarchar](128) NOT NULL,
	[Title] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Url] [nvarchar](max) NOT NULL,
	[EnabledRoles] [nvarchar](max) NULL,
	[RenderModeValue] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  View [dbo].[VW_DictionaryList]    Script Date: 7/8/2015 10:00:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VW_DictionaryList]
AS
SELECT     TOP (100) PERCENT DC.Code AS Category, DD.Code, DD.Name, DD.Value, DD.Remark
FROM         dbo.DataDictionary AS DD INNER JOIN
                      dbo.DataDictionary AS DC ON DD.ParentId = DC.Id
WHERE     (DD.Type = 2)
ORDER BY DD.[Order]



GO
ALTER TABLE [dbo].[Configuration_ProcessSet] ADD  DEFAULT ((0)) FOR [NotAssignIfApproved]
GO
ALTER TABLE [dbo].[DataDictionary] ADD  CONSTRAINT [DF_DataDictionary_Order]  DEFAULT ((0)) FOR [Order]
GO
ALTER TABLE [dbo].[ReportCategory] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[ReportFavourite] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[ReportInfo] ADD  CONSTRAINT [DF__ReportInfo__ID__5535A963]  DEFAULT (newid()) FOR [ID]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0=文件夹，1=字典类，2=字典' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DataDictionary', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "DD"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 189
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DC"
            Begin Extent = 
               Top = 6
               Left = 227
               Bottom = 114
               Right = 378
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_DictionaryList'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_DictionaryList'
GO

INSERT INTO [dbo].[MenuItem]([ID],[DisplayName] ,[Hyperlink],[IconKey] ,[EnabledRoles] ,[ParentId],[KindValue] ,[TargetValue],[MenuID],[MenuItemOrder] ,[Scope])
     VALUES('B7A20D69-6430-47C8-836E-8A88C3BF061A',	N'加签管理',	'/Maintenance/ProcessInstanceSign',	NULL,	NULL,	'FDE858AD-0E9E-4058-B910-055313264BA9',	0,	2,	'277D2019-1393-4FE2-8FCF-4D9B1F6D229C',	9,	NULL)
GO
