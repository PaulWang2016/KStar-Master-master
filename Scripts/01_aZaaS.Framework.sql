USE [aZaaS.Framework]
GO
/****** Object:  StoredProcedure [dbo].[GetAllProcessSet]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetAllProcessSet] 
AS
BEGIN
	SELECT * FROM [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps
END

GO
/****** Object:  StoredProcedure [dbo].[GetDepartment]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetDepartment]
(
	@orgnode_sysid uniqueidentifier,
	@type NVARCHAR(20 )
)
AS
BEGIN


declare @result_sysid uniqueidentifier
declare @resultp_sysid uniqueidentifier
declare @result_type nvarchar(20)


declare @break_flag bit

 select @resultp_sysid = [Parent_SysId],@result_type=[type] ,@result_sysid = [SysId]
 from[aZaaS.Framework].[dbo].[OrgNode]
 where [SysId] = @orgnode_sysid

  print @result_sysid
  print @result_type
  IF @result_type=@type
  BEGIN
  	SELECT  t.*,one.[Value],u.FirstName,u.Email,u.UserName
  	  from orgNode t INNER JOIN OrgNodeExtends AS one ON t.SysId=one.SysId 
  	  INNER JOIN [User] AS u ON u.UserId  COLLATE Chinese_PRC_CI_AS=one.[Value] 
  	where t.SysId=@result_sysid AND one.Name='DeptOwner'
  END
     
  ELSE 
  	BEGIN
	  set @break_flag = 0
	 while(ltrim(@result_type)!=ltrim(@type) and @break_flag = 0 )
	 begin
		if  Exists( select * 
			from[aZaaS.Framework].[dbo].[OrgNode]
			where [SysId] = @resultp_sysid )
		begin
			 select @resultp_sysid = [Parent_SysId],@result_type=[type] ,@result_sysid = [SysId]
			 from[aZaaS.Framework].[dbo].[OrgNode]
			 where [SysId] = @resultp_sysid
		 
			 print @result_sysid
			print @result_type
			 IF ltrim(@result_type)=ltrim(@type)
			 BEGIN
		 		SET @break_flag=1
			 END
		 end
		 else
		 begin
			set @break_flag = 1
			SET @result_sysid =NULL
		 end
	 end
 
	SELECT  t.*,one.[Value],u.FirstName,u.Email,u.UserName
  	  from orgNode t INNER JOIN OrgNodeExtends AS one ON t.SysId=one.SysId 
  	  INNER JOIN [User] AS u ON u.UserId  COLLATE Chinese_PRC_CI_AS=one.[Value] 
  	where t.SysId=@result_sysid AND one.Name='DeptOwner'
	END
END


--EXEC 
--GetDepartment 'F2C3EB40-5A45-41F8-B908-3CF3A9A24003','Division'


GO
/****** Object:  StoredProcedure [dbo].[LoanPage]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create proc [dbo].[LoanPage]
@pageIndex int=1,--页索引
@pageSize int=10,--页大小
@tableName nvarchar(20),--表名
@orderField nvarchar(50),--排序字段
@orderType nvarchar(10),--排序类型
@returnField nvarchar(500)='*',--返回的字段
@strWhere nvarchar(500)='',--条件
@tolRecord int output,--总记录数
@tolPage int output--总页数
as
declare @firstSQL nvarchar(1000)
if(@strWhere='')
	set @firstSQL='select @tolRecord=COUNT(*) from '+ @tableName
else
	set @firstSQL='select @tolRecord=COUNT(*) from '+ @tableName +' where '+@strWhere
exec sp_executesql @firstSQL,N'@tolRecord int output',@tolRecord output
set @tolPage=ceiling(@tolRecord*1.0/@pageSize)
declare @ssql nvarchar(1000)
if(@strWhere!='')
	set @ssql='select top '+convert(varchar(10),@pageSize)+
	' '+@returnField+' from '+@tableName+' where '+@strWhere+' and ('+@orderField+
	' not in(select top '+convert(varchar(10),(@pageIndex-1)*@pageSize)+
	' '+@orderField+' from '+@tableName+' where '+@strWhere+' order by '+@orderField+
	' '+@orderType+')) order by '+@orderField+' '+@orderType
else
	set @ssql='select top '+convert(varchar(10),@pageSize)+
	' '+@returnField+' from '+@tableName+' where '+@orderField+
	' not in(select top '+convert(varchar(10),(@pageIndex-1)*@pageSize)+
	' '+@orderField+' from '+@tableName+' order by '+@orderField+
	' '+@orderType+') order by '+@orderField+' '+@orderType
print @ssql
exec(@ssql)


GO
/****** Object:  StoredProcedure [dbo].[NW_Common_CustomerRole]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[NW_Common_CustomerRole] 
(
@ProcessFullName NVARCHAR(300),
@ActivityName NVARCHAR(300),
@FormID INT
)
AS 
BEGIN
	DECLARE @XmlData NVARCHAR(MAX)
	DECLARE @FieldID VARCHAR(100)
	DECLARE @xmlDataSql VARCHAR(max) 
	SELECT @FieldID=FieldID FROM CustomerRoleConfig AS c WHERE c.ProcessFullName=@ProcessFullName AND c.ActivityName=@ActivityName
    set @xmlDataSql='select [XmlData].value(''(/ContentData/'+ @FieldID +  ')[1]'' , ''nvarchar(max)'')  FROM ProcessFormContent AS pfc WHERE pfc.FormID='+convert(nvarchar,@FormID)
	exec(@xmlDataSql)
END
 

GO
/****** Object:  StoredProcedure [dbo].[PROC_GET_UserExFields]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[PROC_GET_UserExFields]
@UserName varchar(max)
	AS
BEGIN	

declare @sql varchar(8000) 
select @sql= isnull(@sql + ',' , '') + '[' + a.[PropertyName] +']' from [dbo].[UserExField] a
inner join [dbo].[User] b on a.[User_Id]=b.[Id]
where b.[UserName]=@UserName

exec ('
			
			select * from
			(
Select b.[UserName],isnull(b.[FirstName],'''')+'' ''+isnull(b.[LastName],'''') as [DisplayName],a.[User_Id],a.[PropertyName],[PropertyValue]=case 
	when a.[TypeCode]=1 then [ValueString]
	when a.[TypeCode]=2 then	CONVERT(nvarchar(max),a.[ValueDateTime])
	when a.[TypeCode]=4 then  CONVERT(nvarchar(max),a.[ValueNumber])
	else ''''
	end 
	from [dbo].[UserExField] a
	inner join [dbo].[User] b on a.[User_Id]=b.[ID]
	where b.[UserName]='''+@UserName+'''
			) t
			PIVOT  (
			Max([PropertyValue])	FOR [PropertyName] in ('+@sql+')
		) ts	
	') 
end






GO
/****** Object:  StoredProcedure [dbo].[sp_CommonReport_GetAllProcessList]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_CommonReport_GetAllProcessList]
AS 
BEGIN
	SELECT * FROM [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps
END

GO
/****** Object:  StoredProcedure [dbo].[sp_CommonReportConfig_GetConfigByProcSetID]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_CommonReportConfig_GetConfigByProcSetID]
(
	@ProcSetID VARCHAR(10)
)
AS 
BEGIN
	SELECT * FROM CommonReportConfig_SearchArea AS crcsa WHERE crcsa.ProcSetID=@ProcSetID
	SELECT * FROM CommonReportConfig_DisplayArea AS crcda WHERE crcda.ProcSetID=@ProcSetID
END

GO
/****** Object:  StoredProcedure [dbo].[SP_GenerateFormFolio]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GenerateFormFolio]
	-- Add the parameters for the stored procedure here
	 @FormID int,@Folio nvarchar(50) OUTPUT 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @Code nvarchar(50)
	DECLARE @Now datetime =  GETDATE()
	DECLARE @AutoNo nvarchar(50)
	
	SELECT @Code = [ProcessFolio] FROM [ProcessFormHeader] WHERE [FormID] = @FormID

    -- Insert statements for procedure here
	EXEC [SP_GetSerialNumber] @code = @Code, @now = @Now, @result = @AutoNo OUTPUT

	SET @Folio = @Code+@AutoNo
END




GO
/****** Object:  StoredProcedure [dbo].[SP_GenerateProcessFolio]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GenerateProcessFolio]
	-- Add the parameters for the stored procedure here
	@Prefix nvarchar(50),@Code nvarchar(50),@Folio nvarchar(100) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--DECLARE @SNCode nvarchar(50)
	DECLARE @Now datetime =  GETDATE()
	DECLARE @No nvarchar(50)
	--SELECT @SNCode = [ProcessFolio] FROM [ProcessFormHeader] WHERE [FormID] = @FormID

    -- Insert statements for procedure here
	EXEC [SP_GetSerialNumber] @code = @Code, @now = @Now, @result = @No OUTPUT

	SET @Folio = @Prefix+@No
END




GO
/****** Object:  StoredProcedure [dbo].[sp_GetDelegateInfo_byUserName]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_GetDelegateInfo_byUserName]
(
	@userName VARCHAR(50),
	@ProcessFullName  VARCHAR(200)
)
AS
BEGIN

SELECT [DeleType],[ActionType] ,[FromUser] ,[ToUser],[StartDate],[EndDate] 
  ,(SELECT u.Email FROM [User] AS u WHERE u.UserName COLLATE Chinese_PRC_CI_AS =ToUser) AS toEmail, 
  (SELECT u.FirstName FROM [User] AS u WHERE u.UserName COLLATE Chinese_PRC_CI_AS=ToUser) AS FirstName
  FROM [aZaaS.Framework].[dbo].[Delegation] WHERE FromUser=@userName  AND FullName=@ProcessFullName
  AND StartDate<GETDATE() AND EndDate>GETDATE() AND DeleType='Process' AND IsEnable=1
  
  END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetDepartmentAndUsers]    Script Date: 7/16/2015 2:48:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE proc [dbo].[sp_GetDepartmentAndUsers]
@UserId	nvarchar(20)
as
Begin
	Declare @Depts	nvarchar(200)=''
	Declare @UserIds nvarchar(max)
	Declare @UserNames nvarchar(max)
	Declare @Dept nvarchar(50)
	Declare @Parent nvarchar(50)
	if(CHARINDEX('\',@UserId,0)<=0)
		set @UserId='neowaydc\'+@UserId

	select @Parent=DepartmentId,@Dept=DepartmentId FROM VW_UserDepartmentList WHERE UserName=@UserId
	if exists(select * from OrgNodeExtends where 'neowaydc\'+Value=@UserId and sysid = @Dept)
		select @UserIds=stuff((select ','+ substring(UserName,CHARINDEX('\',UserName,0)+1,len(UserName)-CHARINDEX('\',UserName,0)) from VW_UserDepartmentList where DepartmentID=@Dept and UserName<>@UserId for xml path('')),1,1,''),
		@UserNames=stuff((select ','+ [Name] from VW_UserDepartmentList where DepartmentID=@Dept and UserName<>@UserId for xml path('')),1,1,'')

	while len(@Parent)>0
	Begin
		select @Parent = Parent_SysId,@Dept=Name from OrgNode where SysId = @Parent
		set @Depts=@Depts+','+@Dept
	end
	select top 1 @Depts Depts,@UserIds UserIds,@UserNames UserNames from OrgNode
End


--[sp_GetDepartmentAndUsers] '20080818918'

GO
/****** Object:  StoredProcedure [dbo].[SP_GetDepOrOff]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetDepOrOff]
@UserName varchar(100)
as
declare @nodeType varchar(100)
declare @OrgID varchar(100)
declare @tempOrgID VARCHAR(100)
select  @nodeType=n.[Type],@OrgID=n.SysId from [User] u inner join [UserOrgNodes] un on u.SysId=un.User_SysId
inner join OrgNode n on un.OrgNode_SysId= n.sysid where UserName=@UserName
if @nodeType='Property'

BEGIN
   IF EXISTs(select n.SysId from OrgNode n WHERE SysId =(
	select TOP 1 n.Parent_SysId from OrgNode n where N.SysId=@OrgID) and [type]='Cluster')
	BEGIN
	 select @OrgID=n.SysId from OrgNode n WHERE SysId =(
	 select TOP 1 n.Parent_SysId from OrgNode n where N.SysId=@OrgID)
	END
 END
	select 
		FirstName,org.Name,org.SysId
	from [User] u 
		inner join OrgNodeExtends o
		   on u.UserId COLLATE Chinese_PRC_CI_AS=o.Value and o.SysId=@OrgID
		join OrgNode org on o.SysId=org.SysId

GO
/****** Object:  StoredProcedure [dbo].[SP_GetFormProcessLogs]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetFormProcessLogs]
	-- Add the parameters for the stored procedure here
	@FormId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT [ID]
      ,P.[ProcInstID]
      ,[ActInstID]
      ,[SN]
      ,[ProcessName]
      ,[ActivityName]
      ,[OrigUserName]
      ,[OrigUserAccount]
      ,[UserName]
      ,[UserAccount]
      ,[ProfileID]
      ,[OpType]
      ,[ActionName]
      ,[Comment]
      ,[CommentDate]
      ,[TenantID]
  FROM [ProcessLog] AS [P] INNER JOIN [ProcessFormHeader] AS [F] ON P.ProcInstID = F.ProcInstID
  WHERE F.FormID = @FormId 



END



GO
/****** Object:  StoredProcedure [dbo].[sp_GetHardwareAudit]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_GetHardwareAudit]
(
	@procInstID INT 
)
AS
DECLARE @TestProject NVARCHAR(100)
SELECT @TestProject=TestItem FROM HardTestInfo AS hti WHERE hti.ProcInstID=@procInstID
SELECT
	nwbdahu.ID,
	nwbdahu.TestType,
	nwbdahu.TestChildType,
	nwbdahu.EntryAudit,
	nwbdahu.EntryAuditName,
	nwbdahu.TestPlanCharge,
	nwbdahu.TestPlanChargeName,
	nwbdahu.TestEngeer,
	nwbdahu.TestEngeerName,
	nwbdahu.TestProject,
	nwbdahu.ReportAudit,
	nwbdahu.ReportAuditName
FROM
	NeoWay_BaseData_ActivityHandlUser AS nwbdahu WHERE nwbdahu.TestProject=@TestProject

GO
/****** Object:  StoredProcedure [dbo].[sp_GetProcessInstanceDestinationByUserName]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_GetProcessInstanceDestinationByUserName] --neowaydc\oafordevelop
(
	@UserName VARCHAR(50),
	@pageIndex INT,
	@pageSize INT 
	
)
AS
BEGIN
	select * from (SELECT ROW_NUMBER() OVER (ORDER BY t.ProcInstID DESC) AS ROWID,t.ProcInstID,t.TaskStartDate,t.Destination,t.StartDate,t.Originator,t.StartName,t.Folio,t.FinishDate,t.ActName,t.SN,
	(SELECT ps.StatusName FROM ProcessStatus AS ps WHERE ps.StatusID=t.[Status]) AS [STATUS],cps.ProcessSetID,cps.ProcessName,
	(SELECT FirstName FROM [User] AS u WHERE 'K2:'+u.UserName COLLATE Chinese_PRC_CI_AS =T.Destination) AS HandlerUser,cps.ViewUrl,cps.ApproveUrl,pfh.FormID
	 FROM view_ProcinstList AS t INNER JOIN k2.ServerLog.[Proc] AS p ON t.ProcID=p.ID INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
     INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON  ps.ID= cps.ProcessSetID INNER JOIN ProcessFormHeader AS pfh ON t.ProcInstID=pfh.ProcInstID
	WHERE t.Destination='K2:'+@UserName AND t.[Status]<>3) AS temp WHERE temp.ROWID BETWEEN
	(@pageIndex-1)*@PageSize+1 AND @pageIndex*@PageSize 
	
	SELECT COUNT(*)
	 FROM view_ProcinstList AS t 
	WHERE t.Destination='K2:'+@UserName AND t.[Status]<>0
END

GO
/****** Object:  StoredProcedure [dbo].[sp_GetPropotyLend_ExpireList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE proc [dbo].[sp_GetPropotyLend_ExpireList]
  as
  select[ProcInstID]  from view_ProcinstList where [ProcInstID] in (
  SELECT pi1.ID
  FROM k2.ServerLog.ProcSet AS ps INNER JOIN k2.ServerLog.[Proc] AS p ON ps.ID=p.ProcSetID
INNER JOIN k2.ServerLog.ProcInst AS pi1 ON pi1.ProcID=p.ID INNER JOIN [aZaaS.Framework].dbo.ProcessFormHeader AS pfh  ON pi1.ID=pfh.ProcInstID
INNER JOIN ProcessFormContent AS pfc ON pfh.FormID=pfc.FormID

WHERE ps.ID in
(SELECT  cps.ProcessSetID FROM [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps
WHERE cps.ProcessFullName='Innos.KStar.Workflow\PrototypeLend' or cps.ProcessFullName='Innos.KStar.Workflow\prototypeApply' )
 AND pi1.[Status]=3 AND pi1.[Status]<>5 and pi1.ID not in (select ProcInstID from Neoway_Business_PropotyRemindRecord )
 AND (pfc.XmlData.value('(/ContentData/RenewDate)[1]','datetime') <=GETDATE() 
 or pfc.XmlData.value('(/ContentData/ReturnTime)[1]','datetime') <=GETDATE()))

GO
/****** Object:  StoredProcedure [dbo].[SP_GetPrototypeApply]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_GetPrototypeApply]
@flag nvarchar(20),
@tablename nvarchar(50),
@ziduan nvarchar(50),
@result nvarchar(50) output
AS
declare @nowTime nvarchar(8)
declare @sql nvarchar(500)
declare @sql1 nvarchar(500)
declare @count int
set @nowTime=convert(nvarchar(8),getdate(),112)
set @sql='select @count=count(1) from '+@tablename+' where '+@ziduan+' like ''%'+@nowTime+'%'''
print @sql
exec sp_executesql @sql,N'@count int output',@count output

if (@count>0)
BEGIN
declare @maxNum nvarchar(20),@tempNum nvarchar(4)
		set @sql1='select @maxNum=max('+@ziduan+') from '+@tablename
		exec sp_executesql @sql1,N'@maxNum nvarchar(20) output',@maxNum output
		if(@maxNum!='')
			begin
				print @maxNum
				set @tempNum= right('00'+convert(nvarchar(3),right(@maxNum,3)+1),3)
				print @tempNum
				set @result=@flag+@nowTime+@tempNum
			end
end
else	
		set @result=@flag+@nowTime+'001'




GO
/****** Object:  StoredProcedure [dbo].[SP_GetSerialNumber]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_GetSerialNumber]
	@code varchar(50),
	@now datetime,
    @result varchar(50) = '' OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	declare @StartValue int;
	declare @StepValue int;
	declare @CurrentValue int;
	declare @CurrentValueTime datetime;
	declare @NeedFill bit;
	declare @FillLenth int;
	declare @FillChar char(1);
	declare @ZeroMode varchar(50);
	declare @ZeroTime datetime;
	declare @ZeroTimes int;

	if(isnull(@now,'2011-4-1')='2011-4-1')
	begin
		set @now=getdate();
	end	
	
	BEGIN TRANSACTION tranNumbering
	
	update ProcessSerialNumber set CurrentValueTime=@now 
	where code=@code;

	if(@@Rowcount=0)
	begin
		RAISERROR(N'指定代码的编号模板不存在',11,1)
	end	
	
	select @StartValue=StartValue,
		@StepValue=StepValue,
		@CurrentValue=CurrentValue,
		@CurrentValueTime=CurrentValueTime,
		@NeedFill= NeedFill,
		@FillLenth=FillLenth,
		@FillChar=FillChar,
		@ZeroMode=ZeroMode,
		@ZeroTime=ZeroTime,
		@ZeroTimes=ZeroTimes
	from ProcessSerialNumber
	where code=@code;
	
	if(@now > @ZeroTime and lower(@ZeroMode) != 'ever')
	begin
		--用while解决多次时间间隔期间未生成序号导致@ZeroTime小几个时间间隔的问题
		while(@now > @ZeroTime)
		begin
			select @ZeroTime =CASE lower(@ZeroMode)
				WHEN 'year' THEN Dateadd(Year,1,@ZeroTime)
				WHEN 'month' THEN Dateadd(Month,1,@ZeroTime)
				WHEN 'day' THEN Dateadd(Day,1,@ZeroTime)
				WHEN 'hour' THEN Dateadd(Hour,1,@ZeroTime)
				WHEN 'minute' THEN Dateadd(Minute,1,@ZeroTime)
				WHEN 'second' THEN Dateadd(Second,1,@ZeroTime)
				WHEN 'quarter' THEN Dateadd(Quarter,1,@ZeroTime)
				WHEN 'week' THEN Dateadd(Week,1,@ZeroTime)
				ELSE Dateadd(yyyy,1000,@ZeroTime)
			END;
		end	
		set @CurrentValue=@StartValue;
		set @ZeroTimes=@ZeroTimes+1;
	end	
	else
	begin
		set @CurrentValue=@CurrentValue+@StepValue;
	end	
	
	update ProcessSerialNumber 
	set CurrentValue=@CurrentValue,ZeroTime=@ZeroTime,ZeroTimes=@ZeroTimes 
	where code=@code;
	
	COMMIT TRANSACTION tranNumbering
	
	set @result=cast(@CurrentValue as varchar(50));

	if(@NeedFill=1)
	begin		
		set @result=REPLICATE(@FillChar, @FillLenth - LEN(@result)) + @result;

	end	
END

GO
/****** Object:  StoredProcedure [dbo].[SP_GetUserByUserName]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetUserByUserName] 
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(50) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
   SELECT TOP 1
	  [SysId]
      ,[UserName]
      ,[FirstName]
      ,[LastName]
	  ,[FirstName] +' '+[LastName] AS [DisplayName]
      ,[Sex]
      ,[Email]
      ,[Address]
      ,[Phone]
      ,[Status]
      ,[Remark]
  FROM [dbo].[User] WHERE [UserName] = @UserName

END




GO
/****** Object:  StoredProcedure [dbo].[SP_GetUserDepartmentList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetUserDepartmentList] 
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     u.UserName,  o.SysId AS DepartmentID, o.Name AS DepartmentName
	FROM       dbo.[User] AS u INNER JOIN
                      dbo.UserOrgNodes AS uo ON u.SysId = uo.User_SysId INNER JOIN
                      dbo.OrgNode AS o ON uo.OrgNode_SysId = o.SysId	
    WHERE u.UserName = @UserName

END




GO
/****** Object:  StoredProcedure [dbo].[SP_GetUserPositionList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetUserPositionList]
	-- Add the parameters for the stored procedure here
	@UserName nvarchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	select 
		u.UserName as [UserName],
		p.SysId as PositionID,
		p.Name as PositionName
	from [aZaaS.Framework].[dbo].[User] u
		inner join 	[aZaaS.Framework].[dbo].[PositionUsers] pu
			on u.SysId = pu.User_SysId
		inner join [aZaaS.Framework].[dbo].[Position] p
			on pu.Position_SysId = p.SysId
             
			 where u.UserName  = @UserName         

END



GO
/****** Object:  StoredProcedure [dbo].[SP_InsertApprovalHistory]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_InsertApprovalHistory]
	-- Add the parameters for the stored procedure here
	@ProcInstID int,@ProcessName nvarchar(50),@TaskOwner nvarchar(50),
	@ActionTaker nvarchar(50),@ActivityName nvarchar(50),@ActionName nvarchar(50),
	@Comment nvarchar(500) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @TaskOwnerDisplayName nvarchar(50)
	DECLARE @ActionTakerDisplayName nvarchar(50)

	SELECT @TaskOwnerDisplayName = [FirstName]+' '+[LastName]  FROM [User] WHERE UserName = @TaskOwner
	SELECT @ActionTakerDisplayName = [FirstName]+' '+[LastName] FROM [User] WHERE UserName = @ActionTaker

	SET @TaskOwnerDisplayName = ISNULL(@TaskOwnerDisplayName,'Unknown')
	SET @ActionTakerDisplayName = ISNULL(@ActionTakerDisplayName,'Unknown')

	INSERT INTO [dbo].[ProcessLog]
           ([ProcInstID]
           ,[ActInstID]
           ,[SN]
           ,[ProcessName]
           ,[ActivityName]
           ,[OrigUserName]
           ,[OrigUserAccount]
           ,[UserName]
           ,[UserAccount]
           ,[ProfileID]
           ,[OpType]
           ,[ActionName]
           ,[Comment]
           ,[CommentDate]
           ,[TenantID])
     VALUES
           (@ProcInstID
           ,0
           ,NULL
           ,@ProcessName
           ,@ActivityName
           ,@TaskOwnerDisplayName
           ,@TaskOwner
           ,@ActionTakerDisplayName
           ,@ActionTaker
           ,NULL
           ,NULL
           ,@ActionName
           ,@Comment
           ,GETDATE()
           ,NULL)

END




GO
/****** Object:  StoredProcedure [dbo].[SP_InsertProcessFormContent]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		chris.hui
-- Create date: 2014-12-05
-- Description:	表单头
-- =============================================
CREATE PROCEDURE [dbo].[SP_InsertProcessFormContent]
	@FormId	int,
	@JsonData nvarchar(max),
	@SysId int output
AS
BEGIN
	INSERT INTO [dbo].[ProcessFormContent]
           ([FormID],[JsonData])
     VALUES
           (@FormId,@JsonData)

			select @SysId=@@IDENTITY
END




GO
/****** Object:  StoredProcedure [dbo].[SP_InsertProcessFormContentExt]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[SP_InsertProcessFormContentExt]
	@FormId	int,
	@JsonData nvarchar(max),
	@xmlData NVARCHAR(MAX),
	@SysId int output
AS
BEGIN
	INSERT INTO [dbo].[ProcessFormContent]
           ([FormID],[JsonData],[XmlData])
     VALUES
           (@FormId,@JsonData,@xmlData)

			select @SysId=@@IDENTITY
END

GO
/****** Object:  StoredProcedure [dbo].[SP_InsertProcessFormHeader]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_InsertProcessFormHeader]
	@FormSubject	nvarchar(100)=''
	,@ProcInstID	int=0
	,@ProcessFolio	nvarchar(30)=''
	,@Priority	int=0
	,@SubmitterAccount	nvarchar(30)=''
	,@SubmitterDisplayName	nvarchar(30)=''
	,@SubmitDate	datetime='2014-12-12'
	,@ApplicantAccount	nvarchar(30)=''
	,@ApplicantDisplayName	nvarchar(30)=''
	,@ApplicantTelNo	nvarchar(20)=''
	,@ApplicantEmail	nvarchar(20)=''
	,@ApplicantPositionID	nvarchar(20)=''
	,@ApplicantPositionName	nvarchar(30)=''
	,@ApplicantOrgNodeID	nvarchar(20)=''
	,@ApplicantOrgNodeName	nvarchar(30)=''
	,@SubmitComment	nvarchar(100)=''
	,@IsDraft	bit=1
	,@DraftUrl	nvarchar(10)=''
	,@FormId	int	output
AS
BEGIN
	INSERT INTO [dbo].[ProcessFormHeader]
           ([FormSubject]
           ,[ProcInstID]
           ,[ProcessFolio]
           ,[Priority]
           ,[SubmitterAccount]
           ,[SubmitterDisplayName]
           ,[SubmitDate]
           ,[ApplicantAccount]
           ,[ApplicantDisplayName]
           ,[ApplicantTelNo]
           ,[ApplicantEmail]
           ,[ApplicantPositionID]
           ,[ApplicantPositionName]
           ,[ApplicantOrgNodeID]
           ,[ApplicantOrgNodeName]
           ,[SubmitComment]
           ,[IsDraft]
           ,[DraftUrl])
     VALUES
           (@FormSubject
			,@ProcInstID
			,@ProcessFolio
			,@Priority
			,@SubmitterAccount
			,@SubmitterDisplayName
			,@SubmitDate
			,@ApplicantAccount
			,@ApplicantDisplayName
			,@ApplicantTelNo
			,@ApplicantEmail
			,@ApplicantPositionID
			,@ApplicantPositionName
			,@ApplicantOrgNodeID
			,@ApplicantOrgNodeName
			,@SubmitComment
			,@IsDraft
			,@DraftUrl)

			select @FormId=@@IDENTITY
END





GO
/****** Object:  StoredProcedure [dbo].[sp_JSRSellDeliveryFlowExpire_GetList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_JSRSellDeliveryFlowExpire_GetList]
AS
SELECT pi1.ID AS ProcInstID
  FROM k2.ServerLog.ProcSet AS ps INNER JOIN k2.ServerLog.[Proc] AS p ON ps.ID=p.ProcSetID
INNER JOIN k2.ServerLog.ProcInst AS pi1 ON pi1.ProcID=p.ID INNER JOIN [aZaaS.Framework].dbo.ProcessFormHeader AS pfh  ON pi1.ID=pfh.ProcInstID
INNER JOIN ProcessFormContent AS pfc ON pfh.FormID=pfc.FormID
WHERE ps.ID= 
(SELECT TOP 1 cps.ProcessSetID FROM [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps
WHERE cps.ProcessFullName='Innos.KStar.Workflow\JSROverseasSell')
 AND pi1.[Status]<>3 AND pi1.[Status]<>5 AND pi1.ID NOT IN (SELECT ProcInstID  FROM Neoway_Business_JSRSellDeliveryFlowExpireRemind )
 AND pfc.XmlData.value('(/ContentData/OverDate)[1]','datetime') <=GETDATE()

GO
/****** Object:  StoredProcedure [dbo].[sp_Neoway_GetRoleProcessByRoleID]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Neoway_GetRoleProcessByRoleID]
(
	@RoleID VARCHAR(100)
)
AS
SELECT * FROM RoleProcess AS rp WHERE rp.RoleSysId=@RoleID


GO
/****** Object:  StoredProcedure [dbo].[SP_NW__SIManagement_Add]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[SP_NW__SIManagement_Add]
@FormId							nvarchar(20),		--id
@LongNumber						nvarchar(20),		--长号
@ShortNumber					nvarchar(20)=null,		--短号
@Suite							nvarchar(20)=null,		--序列
@MonthlyCost					nvarchar(20)=null,		--月费用
@Operators						nvarchar(20),		--运营商
@Type							nvarchar(20),		--类型
@CCID							nvarchar(20)=null,		--CCID
@PUK							nvarchar(20)=null,		--PUK
@Password						nvarchar(20)=null,		--密码
@ServiceKey						nvarchar(20)=null,		--服务密码
@CompositeCard					nvarchar(20),		--子母卡
@ServiceState					nvarchar(20),		--网络制式
@SeriesNo						nvarchar(20)=null,		--序列号
@CardType						nvarchar(20)=null,		--大小卡
@Purpose						nvarchar(20)=null,		--用途
@Remark							nvarchar(20)=null,		--备注
@SIMStatus						nvarchar(20),		--状态
@NUM							nvarchar(20)=0,		--充值次数
@CreatedBy						nvarchar(20)=null,		--状态
@CreatedDate					nvarchar(20)=null,		--状态
@UpdatedBy						nvarchar(20)=null,		--状态
@UpdatedDate					nvarchar(20)=null,		--状态
@Borrower						nvarchar(20)=null,		--借用人
@BorrowDate						nvarchar(20)=null,		--借出日期
@BorrowDept						nvarchar(20)=null,		--借用人部门/科室
@BorrowerUserName				nvarchar(50)=null,		--借用人部门/科室
@Validate						nvarchar(50)=null,		--借用人部门/科室
@_FormId						nvarchar(50)=null		--借用人部门/科室
as
Begin
	if(@FormId<>0 and exists(select NUM from [NW_SIMManagement] where FormId=@FormId))
		update [NW_SIMManagement] set	LongNumber		=isnull(@LongNumber,LongNumber),		--长号
										ShortNumber		=isnull(@ShortNumber		,ShortNumber),		--短号
										Suite			=isnull(@Suite	,Suite),		--序列
										MonthlyCost		=isnull(@MonthlyCost		,MonthlyCost),		--月费用
										Operators		=isnull(@Operators,Operators),		--运营商
										[Type]			=isnull(@Type,[Type]),		--类型
										CCID			=isnull(@CCID,CCID),		--CCID
										PUK				=isnull(@PUK	,PUK),		--PUK
										[Password]		=isnull(@Password	,[Password]),		--密码
										ServiceKey		=isnull(@ServiceKey	,ServiceKey),		--服务密码
										CompositeCard	=isnull(@CompositeCard	,CompositeCard),		--子母卡
										ServiceState	=isnull(@ServiceState	,ServiceState),		--网络制式
										SeriesNo		=isnull(@SeriesNo	,SeriesNo),		--序列号
										CardType		=isnull(@CardType	,CardType),		--大小卡
										Purpose			=isnull(@Purpose,Purpose),		--用途
										Remark			=isnull(@Remark,Remark)		,		--备注
										SIMStatus		=isnull(@SIMStatus	,SIMStatus),		--状态
										NUM				=isnull(@NUM,NUM),		--充值次数
										--CreatedBy		=isnull(@CreatedBy,CreatedBy),		--状态
										--CreatedDate		=isnull(@CreatedDate,CreatedDate),		--状态
										UpdatedBy		=isnull(@UpdatedBy,UpdatedBy),		--状态
										UpdatedDate		=isnull(@UpdatedDate,UpdatedDate),		--状态
										Borrower		=isnull(@Borrower	,Borrower),		--借用人
										BorrowDate		=isnull(@BorrowDate	,BorrowDate),		--借出日期
										BorrowDept		=isnull(@BorrowDept,BorrowDept),						--借用人部门/科室
										BorrowerUserName=isnull(@BorrowerUserName,BorrowerUserName),
										Validate		=isnull(@Validate,Validate)
			where FormId = @FormId
		else
			insert into [NW_SIMManagement](LongNumber,ShortNumber,Suite,MonthlyCost,Operators,Type,CCID,PUK,Password,ServiceKey,CompositeCard,ServiceState,SeriesNo,CardType,Purpose,Remark,SIMStatus,NUM,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,Borrower,BorrowDate,BorrowDept,BorrowerUserName,Validate)
			values(@LongNumber,		--长号
					@ShortNumber		,		--短号
					@Suite		,		--序列
					@MonthlyCost		,		--月费用
					@Operators	,		--运营商
					@Type		,		--类型
					@CCID		,		--CCID
					@PUK			,		--PUK
					@Password		,		--密码
					@ServiceKey		,		--服务密码
					@CompositeCard	,		--子母卡
					@ServiceState	,		--网络制式
					@SeriesNo		,		--序列号
					@CardType		,		--大小卡
					@Purpose		,		--用途
					@Remark		,		--备注
					@SIMStatus		,		--状态
					@NUM			,		--充值次数
					@CreatedBy		,		--状态
					@CreatedDate	,		--状态
					@UpdatedBy		,		--状态
					@UpdatedDate	,		--状态
					@Borrower		,		--借用人
					@BorrowDate		,		--借出日期
					@BorrowDept	,							--借用人部门/科室
					@BorrowerUserName,
					@Validate
					)

	select @@IdENTITY
End



GO
/****** Object:  StoredProcedure [dbo].[SP_NW_Common_ProcInstManager]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[dbo].[SP_NW_Common_ProcInstManager] '10,12','','','','',0,10,1,'69785475-997c-4d25-ac99-6ef75da2ea92'
CREATE PROC [dbo].[SP_NW_Common_ProcInstManager] --'','','2015-01-30','','',0,10,1,'69785475-997c-4d25-ac99-6ef75da2ea92'
  (
  		@ProcessName NVARCHAR(200),
  		@Folio NVARCHAR(200),
  		@StartTime VARCHAR(50),
  		@FinishTime VARCHAR(50),
  		@StartUser NVARCHAR(100),
  		@Status int	,
  		@PageSize INT ,
  		@pageIndex INT,
  		@SysId VARCHAR(100)
  )
  AS 
  DECLARE @SqlText NVARCHAR(MAX)
  DECLARE @sqlWhere NVARCHAR(1000)
  DECLARE @CurrentUser varchar(1050)
  DECLARE @straa VARCHAR(4000)
   set @straa=''''+','+'''';
  set @CurrentUser=''

  SET @sqlWhere = 'Folio like N''%'+@Folio+'%'''
	IF @ProcessName<>'' AND LEN( @ProcessName)<>0
	BEGIN
		set @ProcessName = replace(@ProcessName,',',@straa)
		SET @sqlWhere +=' and cps.[ProcessSetID] in ('''+@ProcessName+''')'
	END
		
		
    IF @StartTime<>''
	SET @sqlWhere += ' and StartDate>='''+@StartTime+''''
  IF @FinishTime<>''
	SET @sqlWhere +=' and StartDate<='''+@FinishTime+''''
  IF @Status<>0
	SET @sqlWhere +=' and Status='+@Status+''
  IF @StartUser<>''
	SET @sqlWhere +=' AND t.StartName like ''%'+@StartUser+'%'''
  IF @SysId<>''
     Set @CurrentUser='where ru.User_SysId='''+@SysId+''''

  set @SqlText =N'SELECT * FROM 
  ( SELECT  ROW_NUMBER() OVER (ORDER BY t.TaskStartDate DESC) AS ROWID,
   t.ProcInstID as ID,
   t.ProcID,
   t.StartDate,
   t.TaskStartDate,
   t.FinishDate,
   t.Originator,
   t.StartName,
   t.Folio,
   t.ActName,
   t.Destination,
   isnull(replace(cast(pfc2.[Status] AS NVARCHAR(20)),''5'',''作废''),(SELECT ps2.StatusName
                           FROM ProcessStatus AS ps2 WHERE ps2.StatusID=t.[Status])) AS STATUS,
  cps.ProcessFullName,
  cps.ProcessName,
  cps.ProcessSetID,
  cps.ViewUrl,
  pfh.[FormID],
  (SELECT FirstName FROM [User] AS u WHERE u.UserName COLLATE Chinese_PRC_CI_AS =RIGHT(destination,len(destination)-CHARINDEX('':'',destination))) as HandlerUser 
 FROM view_ProcinstList AS t INNER JOIN ProcessFormHeader AS pfh ON pfh.ProcInstID=t.ProcInstID
 INNER JOIN ProcessFormContent AS pfc ON pfc.FormID=pfh.FormID
 INNER JOIN k2.ServerLog.[Proc] AS p ON p.ID=t.ProcID
 INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
 INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON ps.ID=cps.ProcessSetID
 LEFT JOIN ProcessFormCancel AS pfc2 ON pfc2.ProcInstId=t.ProcInstID
                  where '+@sqlWhere+' and ps.ID IN (SELECT cps.ProcessSetID
  FROM  [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps INNER JOIN [aZaaS.KStar].dbo.Role_ProcessSet AS rps ON cps.ProcessFullName
=rps.ProcessFullName 
INNER JOIN [Role] AS r ON  r.SysId= rps.Role_SysId 
INNER JOIN RoleUsers AS ru ON ru.Role_SysId = r.SysId '+@CurrentUser+')  ) AS tp
                
  WHERE tp.ROWID BETWEEN '+CAST(((@pageIndex-1)*@PageSize+1) AS VARCHAR(20))+' AND '+cast(@pageIndex*@PageSize as varchar(20))+'
  select count(*)  FROM view_ProcinstList AS t INNER JOIN ProcessFormHeader AS pfh ON pfh.ProcInstID=t.ProcInstID
 INNER JOIN ProcessFormContent AS pfc ON pfc.FormID=pfh.FormID
 INNER JOIN k2.ServerLog.[Proc] AS p ON p.ID=t.ProcID
 INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
 INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON ps.ID=cps.ProcessSetID
 LEFT JOIN ProcessFormCancel AS pfc2 ON pfc2.ProcInstId=t.ProcInstID
                  where '+@sqlWhere+' and ps.ID IN (SELECT cps.ProcessSetID
  FROM  [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps INNER JOIN [aZaaS.KStar].dbo.Role_ProcessSet AS rps ON cps.ProcessFullName
=rps.ProcessFullName 
INNER JOIN [Role] AS r ON  r.SysId= rps.Role_SysId 
INNER JOIN RoleUsers AS ru ON ru.Role_SysId = r.SysId '+@CurrentUser+') '
 PRINT @SqlText

 exec sp_executesql   @SqlText

GO
/****** Object:  StoredProcedure [dbo].[sp_NW_DELIVERYGOODS_GETLIST]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_NW_DELIVERYGOODS_GETLIST]
(
	@ContractNo VARCHAR(100)
)
AS
BEGIN
	SELECT TOP 1 * FROM NW_DELIVERYRECORDS AS nd WHERE nd.ClientName=@ContractNo
	ORDER BY nd.CreateDate DESC
END

GO
/****** Object:  StoredProcedure [dbo].[sp_NW_DELIVERYGOODS_GETLISTByClientName]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROC [dbo].[sp_NW_DELIVERYGOODS_GETLISTByClientName]
(
	@ClientName VARCHAR(100),
	@ModelType VARCHAR(100)
)
AS
BEGIN
	SELECT TOP 1 * FROM NW_DELIVERYRECORDS AS nd WHERE nd.ClientName=@ClientName
	ORDER BY nd.CreateDate DESC
END

GO
/****** Object:  StoredProcedure [dbo].[sp_NW_DELIVERYRECORDS_INSERT]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  CREATE  PROC [dbo].[sp_NW_DELIVERYRECORDS_INSERT]
(
	@ContractNo VARCHAR(100)='0',
	@jsonData XML,
	@ClientName VARCHAR(100),
	@TransportType NVARCHAR(100),
	@ContactUser NVARCHAR(100),
	@RecieverUnit NVARCHAR(1000),
	@RecieverAddress NVARCHAR(200),
	@PackageRequest NVARCHAR(1000),
	@DeliveryContent XML	
)
AS
DECLARE @Flag INT 
IF not EXISTS(SELECT * FROM NW_DELIVERYRECORDS AS nd WHERE nd.ContractNo=@ContractNo)
 SET @Flag=1
ELSE
	BEGIN
		SELECT @Flag=Cast(flag AS INT) FROM 
		NW_DELIVERYRECORDS AS nd WHERE nd.ContractNo=@ContractNo
		SET @Flag=@Flag+1
	END
	
	
INSERT INTO NW_DELIVERYRECORDS(ContractNo,jsonData,flag,ClientName,TransportType,ContactUser,RecieverUnit,RecieverAddress,PackageRequest,DeliveryContent,CreateDate)
VALUES
(
	@ContractNo,@jsonData ,@Flag,@ClientName,@TransportType,@ContactUser,@RecieverUnit,@RecieverAddress,@PackageRequest,@DeliveryContent,GETDATE()
)

GO
/****** Object:  StoredProcedure [dbo].[SP_NW_FormAttachment_Add]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[SP_NW_FormAttachment_Add]
	@FileGuid uniqueidentifier,
	@FormId nvarchar(100),
	@ProcInstId int,
	@ProcessName nvarchar(100),
	@ActivityName nvarchar(100),
	@FileBytes bigint,
	@FileType nvarchar(50),
	@FileExtension nvarchar(50),
	@OldFileName nvarchar(100),
	@NewFileName nvarchar(100),
	@StoragePath nvarchar(200),
	@DownloadUrl nvarchar(200),
	@Uploader nvarchar(50),
	@UploaderName nvarchar(50)='',
	@UploadedDate datetime,
	@FileComment nvarchar(500)
AS
INSERT INTO [dbo].[FormAttachment]
           ([FileGuid]
           ,[FormId]
           ,[ProcInstId]
           ,[ProcessName]
           ,[ActivityName]
           ,[FileBytes]
           ,[FileType]
           ,[FileExtension]
           ,[OldFileName]
           ,[NewFileName]
           ,[StoragePath]
           ,[DownloadUrl]
           ,[Uploader]
           ,[UploaderName]
           ,[UploadedDate]
           ,[FileComment])
     VALUES
           (@FileGuid,
		   @FormId ,
           @ProcInstId, 
           @ProcessName ,
           @ActivityName ,
           @FileBytes, 
           @FileType ,
           @FileExtension ,
           @OldFileName ,
           @NewFileName ,
           @StoragePath ,
           @DownloadUrl ,
           @Uploader ,
           @UploaderName ,
           @UploadedDate, 
           @FileComment )


GO
/****** Object:  StoredProcedure [dbo].[SP_NW_FormData_Add]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[SP_NW_FormData_Add]
@Fromid									nvarchar(20),		--id
@JsonData								nvarchar(max),		--@JsonData
@XmlData								nvarchar(max)		--@XmlData
as
Begin
	if exists(select Formid from [NW_FormData] where FormID=@Fromid)
		update [NW_FormData] set JsonData=@JsonData,XmlData=@XmlData where FormID=@Fromid
	else
		INSERT INTO [NW_FormData]
			   ([FormID]
			   ,[JsonData]
			   ,[XmlData])
		 VALUES
			   (@Fromid
			   ,@JsonData
			   ,@XmlData)

	select @@IDENTITY
End


GO
/****** Object:  StoredProcedure [dbo].[SP_NW_GetData]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[SP_NW_GetData]
@Formid								nvarchar(20)		--id
as
Begin
	if(len(@Formid)>0)
		SELECT [JsonData]
		  FROM [dbo].[NW_Formdata]
		  where [FormID]=@Formid
End


GO
/****** Object:  StoredProcedure [dbo].[SP_NW_GetDpetOwner]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[SP_NW_GetDpetOwner]
@UserName								nvarchar(20)		--id
as
Begin
	declare @OrgNode_SysId nvarchar(80)
	declare @OrgNode_SysId1 nvarchar(80)
	declare @Type nvarchar(30)
	declare @Type1 nvarchar(30)

	select @OrgNode_SysId=OrgNode_SysId from [UserOrgNodes]
    where User_SysId=(select sysid from [user] where username=@UserName)
	select @Type=[type] FROM [aZaaS.Framework].[dbo].[OrgNode] where SysId=@OrgNode_SysId
	--print @Type
	if(@Type = 'Property')
	begin
		select @Type1=[type],@OrgNode_SysId1=SysId FROM [aZaaS.Framework].[dbo].[OrgNode]
		where SysId=(select Parent_SysId FROM [aZaaS.Framework].[dbo].[OrgNode] where SysId=@OrgNode_SysId)
		--print @Type1
		if(@Type1='Cluster')
		begin
			set @Type=@Type1
			set @OrgNode_SysId=@OrgNode_SysId1
		end
	end
	select Value from OrgNodeExtends where SysId=@OrgNode_SysId
End


GO
/****** Object:  StoredProcedure [dbo].[SP_NW_GetPositionUsers]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[SP_NW_GetPositionUsers]
@Position nvarchar(30)
as
select stuff((select ','+UserName from (
	SELECT u.UserName from [Position] p join [PositionUsers] ps
	on p.SysId = ps.Position_SysId
	join [User] u on u.SysId = ps.User_SysId
	and p.Name=@Position
) a where 1=1 for xml path('')),1,1,'')



GO
/****** Object:  StoredProcedure [dbo].[SP_NW_PersonPlan]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[SP_NW_PersonPlan]
@UserName		NVARCHAR(30),
@StartDate		NVARCHAR(30),
@EndDate		NVARCHAR(30)=null,
@Status			NVARCHAR(20),
@CreatedBy		NVARCHAR(30),
@CreatedDate	NVARCHAR(30),
@UpdatedBy		NVARCHAR(30),
@UpdatedDate	NVARCHAR(30)
AS
BEGIN
	IF(charindex('\\',@UserName)>=0)
		set @UserName=replace(@UserName,'\\','\')

	IF EXISTS(SELECT TOP 1 UserName FROM [aZaaS.Framework].[dbo].[NW_PersonPlan] WHERE UserName=@UserName)
		UPDATE [aZaaS.Framework].[dbo].[NW_PersonPlan] SET StartDate=@StartDate,EndDate=@EndDate,[Status]=@Status,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate Where UserName=@UserName
	ELSE
		INSERT INTO [aZaaS.Framework].[dbo].[NW_PersonPlan](UserName,StartDate,EndDate,[Status],CreatedBy,CreatedDate) VALUES(@UserName,@StartDate,@EndDate,@Status,@CreatedBy,@CreatedDate)
END


GO
/****** Object:  StoredProcedure [dbo].[SP_NW_ProcInstUrge]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[SP_NW_ProcInstUrge]
@FullName	nvarchar(100),
@ActName	nvarchar(100)
as
SELECT 
	cast(wl.procinstid as nvarchar(10)) + '_' + cast(wl.ActInstDestID as nvarchar(10)) as SN,
	wl.Destination,
	wl.StartDate,
	datediff(hour,wl.StartDate,getdate()),
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
   and datediff(hour,wl.StartDate,getdate()) > 12


GO
/****** Object:  StoredProcedure [dbo].[SP_NW_SIManagement_Update]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[SP_NW_SIManagement_Update]
@LongNumber						nvarchar(20),		--长号
@SIMStatus						nvarchar(20)=null,		--状态
@Borrower						nvarchar(20)=null,		--借用人
@BorrowDate						nvarchar(20)=null,		--借用人部门/科室
@BorrowDept						nvarchar(20)=null,		--借用人部门/科室
@BorrowerUserName				nvarchar(50)=null,		--借用人部门/科室
@Validate						nvarchar(20)=null,		--借用人部门/科室
@_FormId						nvarchar(20)=null		--借用人部门/科室
as
Begin
		update [NW_SIMManagement] set	LongNumber		=isnull(@LongNumber,LongNumber),		--长号
										SIMStatus		=isnull(@SIMStatus	,SIMStatus),		--状态
										Borrower		=isnull(@Borrower	,Borrower),		--借用人
										BorrowDate		=isnull(@BorrowDate, BorrowDate),		--借出日期
										BorrowDept		=isnull(@BorrowDept,BorrowDept),						--借用人部门/科室
										BorrowerUserName=isnull(@BorrowerUserName,BorrowerUserName),
										Validate		=isnull(@Validate,Validate),
										_FormId		=isnull(@_FormId,_FormId)
			where LongNumber = @LongNumber

End



GO
/****** Object:  StoredProcedure [dbo].[SP_NW_SoftwareTest_Add]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[SP_NW_SoftwareTest_Add]
@Model nvarchar(20),
@SoftwareVersion nvarchar(20),
@A int=0,@B int=0,@C int=0,@D int=0,
@CreateDate nvarchar(30)=null
as
Begin
	if exists(select [Model] from [NW_SoftwareTest] where [Model]=@Model and SoftwareVersion=@SoftwareVersion)
		UPDATE [dbo].[NW_SoftwareTest]
		   SET [A] = @A
			  ,[B] = @B
			  ,[C] = @C
			  ,[D] = @D
		 WHERE [Model]=@Model and SoftwareVersion=@SoftwareVersion
	else
		INSERT INTO [dbo].[NW_SoftwareTest]
				   ([Model] ,[SoftwareVersion],[A],[B],[C],[D],[CreateDate])
			 VALUES
				   (@Model,@SoftwareVersion,@A,@B,@C,@D,isnull(@CreateDate,GETDATE()))
End



GO
/****** Object:  StoredProcedure [dbo].[SP_NW_SoftwareTest_Get]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[SP_NW_SoftwareTest_Get]
@Model nvarchar(20),
@SoftwareVersion nvarchar(20)
as
Begin
	IF EXISTS(select [CreateDate] from NW_SoftwareTest where [Model]=@Model and SoftwareVersion=@SoftwareVersion)
	select [Model] ,[SoftwareVersion],[A],[B],[C],[D],[CreateDate] from NW_SoftwareTest
	where [Model]=@Model and [CreateDate]<(select [CreateDate] from NW_SoftwareTest where [Model]=@Model and SoftwareVersion=@SoftwareVersion)
	order by CreateDate desc
	else
	select [Model] ,[SoftwareVersion],[A],[B],[C],[D],[CreateDate] from NW_SoftwareTest
	where [Model]=@Model
End



/*
select * from NW_SoftwareTest

select [Model] ,[SoftwareVersion],[A],[B],[C],[D],[CreateDate] from NW_SoftwareTest
	where [Model]='E40B' and [CreateDate]<(select [CreateDate] from NW_SoftwareTest where [Model]='E40B' and SoftwareVersion='V1.5')


	*/

GO
/****** Object:  StoredProcedure [dbo].[SP_NWHelpDesk_Add]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------------------
--用途：    增加一条数据 
--项目名称：aZaaS.KStarForm
--说明：    在表NW_HelpDesk中插入一条记录
--时间：    2014-12-06 14:05:16
-------------------------------------------
CREATE PROCEDURE [dbo].[SP_NWHelpDesk_Add]
@FormId int,
@FeedbackNo varchar(20),
@CustomerName nvarchar(40),
@Industry nvarchar(40),
@Remark nvarchar(200),
@RequestType varchar(20),
@Attachment varchar(20),
@ProductsModel nvarchar(40),
@HardwareVersion nvarchar(40),
@SoftwareVersion nvarchar(40),
@DemandType nvarchar(40),
@ComponentType nvarchar(40),
@Project nvarchar(40),
@ProjectManager nvarchar(40),
@ClamentStatus varchar(20),
@PreProcessDate datetime,
@PreRemark nvarchar(200),
@PreAttachment varchar(20),
@PreProcess varchar(20),
@PreContent varchar(20),
@FAE varchar(20),
@PreCC varchar(20),
@PreProcessContent nvarchar(200),
@BackContent nvarchar(200),
@TaskComplateDate nvarchar(40),
@isFinalFixed nvarchar(40),
@IsTransfer nvarchar(40),
@IsDescCocrret nvarchar(40),
@FAEProcessContent varchar(100),
@FAEBackContent varchar(100),
@FAEFinalFixed nvarchar(20),
@FAEExact varchar(20),
@FAERemark varchar(20),
@FAENeedDev varchar(20),
@FAEProvince varchar(20),
@FAECity varchar(20),
@FAEArea varchar(20),
@FAEQuestionType varchar(20),
@FAEProblemType varchar(20),
@ManagerSolution varchar(20),
@ManagerComplateDate varchar(20),
@ManagerContent varchar(20),
@ManagerAttachment varchar(20),
@Developer varchar(20),
@ManagerCC varchar(20),
@DevResult varchar(20),
@DevCC varchar(20),
@DevAttachment varchar(20),
@TestManager varchar(20),
@NeedTest varchar(20),
@DevFixed varchar(20),
@DevContent varchar(20),
@DevComplateDate varchar(20),
@TestResult varchar(20),
@TestCC varchar(20),
@TestAttachment varchar(20),
@Tester varchar(20),
@TestContent varchar(20),
@TestFixed varchar(20),
@ProdectBackContent varchar(20),
@ProdectContent varchar(20),
@ProdectFixed varchar(20),
@FAEBackContent1 varchar(20),
@FAEContent1 varchar(20),
@InterfaceContent varchar(20),
@CustomerContent varchar(20),
@CustomerSatisfaction varchar(20),
@CustomerFixed varchar(20)
AS

BEGIN

SET NOCOUNT ON


begin try

    begin tran
	if exists(select formid from [NW_HelpDesk] where FormId=@FormId)
	UPDATE [dbo].[NW_HelpDesk] SET
        [FeedbackNo] = isnull(@FeedbackNo,[FeedbackNo]),
        [CustomerName] = isnull(@CustomerName,[CustomerName]),
        [Industry] = isnull(@Industry,[Industry]),
        [Remark] = isnull(@Remark,[Remark]),
        [RequestType] = isnull(@RequestType,[RequestType]),
        [Attachment] = isnull(@Attachment,[Attachment]),
        [ProductsModel] = isnull(@ProductsModel,[ProductsModel]),
        [HardwareVersion] = isnull(@HardwareVersion,[HardwareVersion]),
        [SoftwareVersion] = isnull(@SoftwareVersion,[SoftwareVersion]),
        [DemandType] = isnull(@DemandType,[DemandType]),
        [ComponentType] = isnull(@ComponentType,[ComponentType]),
        [Project] = isnull(@Project,[Project]),
        [ProjectManager] = isnull(@ProjectManager,[ProjectManager]),
        [ClamentStatus] = isnull(@ClamentStatus,[ClamentStatus]),
        [PreProcessDate] = isnull(@PreProcessDate,[PreProcessDate]),
        [PreRemark] = isnull(@PreRemark,[PreRemark]),
        [PreAttachment] = isnull(@PreAttachment,[PreAttachment]),
        [PreProcess] = isnull(@PreProcess,[PreProcess]),
        [PreContent] = isnull(@PreContent,[PreContent]),
        [FAE] = isnull(@FAE,[FAE]),
        [PreCC] = isnull(@PreCC,[PreCC]),
        [PreProcessContent] = isnull(@PreProcessContent,[PreProcessContent]),
        [BackContent] = isnull(@BackContent,[BackContent]),
        [TaskComplateDate] = isnull(@TaskComplateDate,[TaskComplateDate]),
        [isFinalFixed] = isnull(@isFinalFixed,[isFinalFixed]),
        [IsTransfer] = isnull(@IsTransfer,[IsTransfer]),
        [IsDescCocrret] = isnull(@IsDescCocrret,[IsDescCocrret]),
        [FAEProcessContent] = isnull(@FAEProcessContent,[FAEProcessContent]),
        [FAEBackContent] = isnull(@FAEBackContent,[FAEBackContent]),
        [FAEFinalFixed] = isnull(@FAEFinalFixed,[FAEFinalFixed]),
        [FAEExact] = isnull(@FAEExact,[FAEExact]),
        [FAERemark] = isnull(@FAERemark,[FAERemark]),
        [FAENeedDev] = isnull(@FAENeedDev,[FAENeedDev]),
        [FAEProvince] = isnull(@FAEProvince,[FAEProvince]),
        [FAECity] = isnull(@FAECity,[FAECity]),
        [FAEArea] = isnull(@FAEArea,[FAEArea]),
        [FAEQuestionType] = isnull(@FAEQuestionType,[FAEQuestionType]),
        [FAEProblemType] = isnull(@FAEProblemType,[FAEProblemType]),
        [ManagerSolution] = isnull(@ManagerSolution,[ManagerSolution]),
        [ManagerComplateDate] = isnull(@ManagerComplateDate,[ManagerComplateDate]),
        [ManagerContent] = isnull(@ManagerContent,[ManagerContent]),
        [ManagerAttachment] = isnull(@ManagerAttachment,[ManagerAttachment]),
        [Developer] = isnull(@Developer,[Developer]),
        [ManagerCC] = isnull(@ManagerCC,[ManagerCC]),
        [DevResult] = isnull(@DevResult,[DevResult]),
        [DevCC] = isnull(@DevCC,[DevCC]),
        [DevAttachment] = isnull(@DevAttachment,[DevAttachment]),
        [TestManager] = isnull(@TestManager,[TestManager]),
        [NeedTest] = isnull(@NeedTest,[NeedTest]),
        [DevFixed] = isnull(@DevFixed,[DevFixed]),
        [DevContent] = isnull(@DevContent,[DevContent]),
        [DevComplateDate] = isnull(@DevComplateDate,[DevComplateDate]),
        [TestResult] = isnull(@TestResult,[TestResult]),
        [TestCC] = isnull(@TestCC,[TestCC]),
        [TestAttachment] = isnull(@TestAttachment,[TestAttachment]),
        [Tester] = isnull(@Tester,[Tester]),
        [TestContent] = isnull(@TestContent,[TestContent]),
        [TestFixed] = isnull(@TestFixed,[TestFixed]),
        [ProdectBackContent] = isnull(@ProdectBackContent,[ProdectBackContent]),
        [ProdectContent] = isnull(@ProdectContent,[ProdectContent]),
        [ProdectFixed] = isnull(@ProdectFixed,[ProdectFixed]),
        [FAEBackContent1] = isnull(@FAEBackContent1,[FAEBackContent1]),
        [FAEContent1] = isnull(@FAEContent1,[FAEContent1]),
        [InterfaceContent] = isnull(@InterfaceContent,[InterfaceContent]),
        [CustomerContent] = isnull(@CustomerContent,[CustomerContent]),
        [CustomerSatisfaction] = isnull(@CustomerSatisfaction,[CustomerSatisfaction]),
        [CustomerFixed] = isnull(@CustomerFixed,[CustomerFixed])
    WHERE
        [FormId] = @FormId
		else
    INSERT INTO [dbo].[NW_HelpDesk](
        [FormId],
        [FeedbackNo],
        [CustomerName],
        [Industry],
        [Remark],
        [RequestType],
        [Attachment],
        [ProductsModel],
        [HardwareVersion],
        [SoftwareVersion],
        [DemandType],
        [ComponentType],
        [Project],
        [ProjectManager],
        [ClamentStatus],
        [PreProcessDate],
        [PreRemark],
        [PreAttachment],
        [PreProcess],
        [PreContent],
        [FAE],
        [PreCC],
        [PreProcessContent],
        [BackContent],
        [TaskComplateDate],
        [isFinalFixed],
        [IsTransfer],
        [IsDescCocrret],
        [FAEProcessContent],
        [FAEBackContent],
        [FAEFinalFixed],
        [FAEExact],
        [FAERemark],
        [FAENeedDev],
        [FAEProvince],
        [FAECity],
        [FAEArea],
        [FAEQuestionType],
        [FAEProblemType],
        [ManagerSolution],
        [ManagerComplateDate],
        [ManagerContent],
        [ManagerAttachment],
        [Developer],
        [ManagerCC],
        [DevResult],
        [DevCC],
        [DevAttachment],
        [TestManager],
        [NeedTest],
        [DevFixed],
        [DevContent],
        [DevComplateDate],
        [TestResult],
        [TestCC],
        [TestAttachment],
        [Tester],
        [TestContent],
        [TestFixed],
        [ProdectBackContent],
        [ProdectContent],
        [ProdectFixed],
        [FAEBackContent1],
        [FAEContent1],
        [InterfaceContent],
        [CustomerContent],
        [CustomerSatisfaction],
        [CustomerFixed])
    VALUES(
        @FormId,
        @FeedbackNo,
        @CustomerName,
        @Industry,
        @Remark,
        @RequestType,
        @Attachment,
        @ProductsModel,
        @HardwareVersion,
        @SoftwareVersion,
        @DemandType,
        @ComponentType,
        @Project,
        @ProjectManager,
        @ClamentStatus,
        @PreProcessDate,
        @PreRemark,
        @PreAttachment,
        @PreProcess,
        @PreContent,
        @FAE,
        @PreCC,
        @PreProcessContent,
        @BackContent,
        @TaskComplateDate,
        @isFinalFixed,
        @IsTransfer,
        @IsDescCocrret,
        @FAEProcessContent,
        @FAEBackContent,
        @FAEFinalFixed,
        @FAEExact,
        @FAERemark,
        @FAENeedDev,
        @FAEProvince,
        @FAECity,
        @FAEArea,
        @FAEQuestionType,
        @FAEProblemType,
        @ManagerSolution,
        @ManagerComplateDate,
        @ManagerContent,
        @ManagerAttachment,
        @Developer,
        @ManagerCC,
        @DevResult,
        @DevCC,
        @DevAttachment,
        @TestManager,
        @NeedTest,
        @DevFixed,
        @DevContent,
        @DevComplateDate,
        @TestResult,
        @TestCC,
        @TestAttachment,
        @Tester,
        @TestContent,
        @TestFixed,
        @ProdectBackContent,
        @ProdectContent,
        @ProdectFixed,
        @FAEBackContent1,
        @FAEContent1,
        @InterfaceContent,
        @CustomerContent,
        @CustomerSatisfaction,
        @CustomerFixed)

    commit tran

end try

begin catch
    if @@trancount > 0 rollback tran;
    declare @error_message    nvarchar(1024)
    declare @error_serverity  int
    declare @error_state      int

    set @error_message   = error_message()
    set @error_serverity = error_severity()
    set @error_state     = error_state()

    raiserror(@error_message, @error_serverity, @error_state);
end catch

END





GO
/****** Object:  StoredProcedure [dbo].[SP_NWHelpDesk_Update]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------------------
--用途：    更新一条数据 
--项目名称：aZaaS.KStarForm
--说明：    在表NW_HelpDesk中更新一条记录
--时间：    2014-12-06 14:05:16
-------------------------------------------
CREATE PROCEDURE [dbo].[SP_NWHelpDesk_Update]
@FormId int,
@FeedbackNo varchar(20),
@CustomerName nvarchar(40),
@Industry nvarchar(40),
@Remark nvarchar(200),
@RequestType varchar(20),
@Attachment varchar(20),
@ProductsModel nvarchar(40),
@HardwareVersion nvarchar(40),
@SoftwareVersion nvarchar(40),
@DemandType nvarchar(40),
@ComponentType nvarchar(40),
@Project nvarchar(40),
@ProjectManager nvarchar(40),
@ClamentStatus varchar(20),
@PreProcessDate datetime,
@PreRemark nvarchar(200),
@PreAttachment varchar(20),
@PreProcess varchar(20),
@PreContent varchar(20),
@FAE varchar(20),
@PreCC varchar(20),
@PreProcessContent nvarchar(200),
@BackContent nvarchar(200),
@TaskComplateDate nvarchar(40),
@isFinalFixed nvarchar(40),
@IsTransfer nvarchar(40),
@IsDescCocrret nvarchar(40),
@FAEProcessContent varchar(100),
@FAEBackContent varchar(100),
@FAEFinalFixed nvarchar(20),
@FAEExact varchar(20),
@FAERemark varchar(20),
@FAENeedDev varchar(20),
@FAEProvince varchar(20),
@FAECity varchar(20),
@FAEArea varchar(20),
@FAEQuestionType varchar(20),
@FAEProblemType varchar(20),
@ManagerSolution varchar(20),
@ManagerComplateDate varchar(20),
@ManagerContent varchar(20),
@ManagerAttachment varchar(20),
@Developer varchar(20),
@ManagerCC varchar(20),
@DevResult varchar(20),
@DevCC varchar(20),
@DevAttachment varchar(20),
@TestManager varchar(20),
@NeedTest varchar(20),
@DevFixed varchar(20),
@DevContent varchar(20),
@DevComplateDate varchar(20),
@TestResult varchar(20),
@TestCC varchar(20),
@TestAttachment varchar(20),
@Tester varchar(20),
@TestContent varchar(20),
@TestFixed varchar(20),
@ProdectBackContent varchar(20),
@ProdectContent varchar(20),
@ProdectFixed varchar(20),
@FAEBackContent1 varchar(20),
@FAEContent1 varchar(20),
@InterfaceContent varchar(20),
@CustomerContent varchar(20),
@CustomerSatisfaction varchar(20),
@CustomerFixed varchar(20)
AS

BEGIN

SET NOCOUNT ON


begin try

    begin tran
	if exists(select formid from [NW_HelpDesk] where FormId=@FormId)
	UPDATE [dbo].[NW_HelpDesk] SET
        [FeedbackNo] = isnull(@FeedbackNo,[FeedbackNo]),
        [CustomerName] = isnull(@CustomerName,[CustomerName]),
        [Industry] = isnull(@Industry,[Industry]),
        [Remark] = isnull(@Remark,[Remark]),
        [RequestType] = isnull(@RequestType,[RequestType]),
        [Attachment] = isnull(@Attachment,[Attachment]),
        [ProductsModel] = isnull(@ProductsModel,[ProductsModel]),
        [HardwareVersion] = isnull(@HardwareVersion,[HardwareVersion]),
        [SoftwareVersion] = isnull(@SoftwareVersion,[SoftwareVersion]),
        [DemandType] = isnull(@DemandType,[DemandType]),
        [ComponentType] = isnull(@ComponentType,[ComponentType]),
        [Project] = isnull(@Project,[Project]),
        [ProjectManager] = isnull(@ProjectManager,[ProjectManager]),
        [ClamentStatus] = isnull(@ClamentStatus,[ClamentStatus]),
        [PreProcessDate] = isnull(@PreProcessDate,[PreProcessDate]),
        [PreRemark] = isnull(@PreRemark,[PreRemark]),
        [PreAttachment] = isnull(@PreAttachment,[PreAttachment]),
        [PreProcess] = isnull(@PreProcess,[PreProcess]),
        [PreContent] = isnull(@PreContent,[PreContent]),
        [FAE] = isnull(@FAE,[FAE]),
        [PreCC] = isnull(@PreCC,[PreCC]),
        [PreProcessContent] = isnull(@PreProcessContent,[PreProcessContent]),
        [BackContent] = isnull(@BackContent,[BackContent]),
        [TaskComplateDate] = isnull(@TaskComplateDate,[TaskComplateDate]),
        [isFinalFixed] = isnull(@isFinalFixed,[isFinalFixed]),
        [IsTransfer] = isnull(@IsTransfer,[IsTransfer]),
        [IsDescCocrret] = isnull(@IsDescCocrret,[IsDescCocrret]),
        [FAEProcessContent] = isnull(@FAEProcessContent,[FAEProcessContent]),
        [FAEBackContent] = isnull(@FAEBackContent,[FAEBackContent]),
        [FAEFinalFixed] = isnull(@FAEFinalFixed,[FAEFinalFixed]),
        [FAEExact] = isnull(@FAEExact,[FAEExact]),
        [FAERemark] = isnull(@FAERemark,[FAERemark]),
        [FAENeedDev] = isnull(@FAENeedDev,[FAENeedDev]),
        [FAEProvince] = isnull(@FAEProvince,[FAEProvince]),
        [FAECity] = isnull(@FAECity,[FAECity]),
        [FAEArea] = isnull(@FAEArea,[FAEArea]),
        [FAEQuestionType] = isnull(@FAEQuestionType,[FAEQuestionType]),
        [FAEProblemType] = isnull(@FAEProblemType,[FAEProblemType]),
        [ManagerSolution] = isnull(@ManagerSolution,[ManagerSolution]),
        [ManagerComplateDate] = isnull(@ManagerComplateDate,[ManagerComplateDate]),
        [ManagerContent] = isnull(@ManagerContent,[ManagerContent]),
        [ManagerAttachment] = isnull(@ManagerAttachment,[ManagerAttachment]),
        [Developer] = isnull(@Developer,[Developer]),
        [ManagerCC] = isnull(@ManagerCC,[ManagerCC]),
        [DevResult] = isnull(@DevResult,[DevResult]),
        [DevCC] = isnull(@DevCC,[DevCC]),
        [DevAttachment] = isnull(@DevAttachment,[DevAttachment]),
        [TestManager] = isnull(@TestManager,[TestManager]),
        [NeedTest] = isnull(@NeedTest,[NeedTest]),
        [DevFixed] = isnull(@DevFixed,[DevFixed]),
        [DevContent] = isnull(@DevContent,[DevContent]),
        [DevComplateDate] = isnull(@DevComplateDate,[DevComplateDate]),
        [TestResult] = isnull(@TestResult,[TestResult]),
        [TestCC] = isnull(@TestCC,[TestCC]),
        [TestAttachment] = isnull(@TestAttachment,[TestAttachment]),
        [Tester] = isnull(@Tester,[Tester]),
        [TestContent] = isnull(@TestContent,[TestContent]),
        [TestFixed] = isnull(@TestFixed,[TestFixed]),
        [ProdectBackContent] = isnull(@ProdectBackContent,[ProdectBackContent]),
        [ProdectContent] = isnull(@ProdectContent,[ProdectContent]),
        [ProdectFixed] = isnull(@ProdectFixed,[ProdectFixed]),
        [FAEBackContent1] = isnull(@FAEBackContent1,[FAEBackContent1]),
        [FAEContent1] = isnull(@FAEContent1,[FAEContent1]),
        [InterfaceContent] = isnull(@InterfaceContent,[InterfaceContent]),
        [CustomerContent] = isnull(@CustomerContent,[CustomerContent]),
        [CustomerSatisfaction] = isnull(@CustomerSatisfaction,[CustomerSatisfaction]),
        [CustomerFixed] = isnull(@CustomerFixed,[CustomerFixed])
    WHERE
        [FormId] = @FormId
		else
    INSERT INTO [dbo].[NW_HelpDesk](
        [FormId],
        [FeedbackNo],
        [CustomerName],
        [Industry],
        [Remark],
        [RequestType],
        [Attachment],
        [ProductsModel],
        [HardwareVersion],
        [SoftwareVersion],
        [DemandType],
        [ComponentType],
        [Project],
        [ProjectManager],
        [ClamentStatus],
        [PreProcessDate],
        [PreRemark],
        [PreAttachment],
        [PreProcess],
        [PreContent],
        [FAE],
        [PreCC],
        [PreProcessContent],
        [BackContent],
        [TaskComplateDate],
        [isFinalFixed],
        [IsTransfer],
        [IsDescCocrret],
        [FAEProcessContent],
        [FAEBackContent],
        [FAEFinalFixed],
        [FAEExact],
        [FAERemark],
        [FAENeedDev],
        [FAEProvince],
        [FAECity],
        [FAEArea],
        [FAEQuestionType],
        [FAEProblemType],
        [ManagerSolution],
        [ManagerComplateDate],
        [ManagerContent],
        [ManagerAttachment],
        [Developer],
        [ManagerCC],
        [DevResult],
        [DevCC],
        [DevAttachment],
        [TestManager],
        [NeedTest],
        [DevFixed],
        [DevContent],
        [DevComplateDate],
        [TestResult],
        [TestCC],
        [TestAttachment],
        [Tester],
        [TestContent],
        [TestFixed],
        [ProdectBackContent],
        [ProdectContent],
        [ProdectFixed],
        [FAEBackContent1],
        [FAEContent1],
        [InterfaceContent],
        [CustomerContent],
        [CustomerSatisfaction],
        [CustomerFixed])
    VALUES(
        @FormId,
        @FeedbackNo,
        @CustomerName,
        @Industry,
        @Remark,
        @RequestType,
        @Attachment,
        @ProductsModel,
        @HardwareVersion,
        @SoftwareVersion,
        @DemandType,
        @ComponentType,
        @Project,
        @ProjectManager,
        @ClamentStatus,
        @PreProcessDate,
        @PreRemark,
        @PreAttachment,
        @PreProcess,
        @PreContent,
        @FAE,
        @PreCC,
        @PreProcessContent,
        @BackContent,
        @TaskComplateDate,
        @isFinalFixed,
        @IsTransfer,
        @IsDescCocrret,
        @FAEProcessContent,
        @FAEBackContent,
        @FAEFinalFixed,
        @FAEExact,
        @FAERemark,
        @FAENeedDev,
        @FAEProvince,
        @FAECity,
        @FAEArea,
        @FAEQuestionType,
        @FAEProblemType,
        @ManagerSolution,
        @ManagerComplateDate,
        @ManagerContent,
        @ManagerAttachment,
        @Developer,
        @ManagerCC,
        @DevResult,
        @DevCC,
        @DevAttachment,
        @TestManager,
        @NeedTest,
        @DevFixed,
        @DevContent,
        @DevComplateDate,
        @TestResult,
        @TestCC,
        @TestAttachment,
        @Tester,
        @TestContent,
        @TestFixed,
        @ProdectBackContent,
        @ProdectContent,
        @ProdectFixed,
        @FAEBackContent1,
        @FAEContent1,
        @InterfaceContent,
        @CustomerContent,
        @CustomerSatisfaction,
        @CustomerFixed)

    commit tran

end try

begin catch
    if @@trancount > 0 rollback tran;
    declare @error_message    nvarchar(1024)
    declare @error_serverity  int
    declare @error_state      int

    set @error_message   = error_message()
    set @error_serverity = error_severity()
    set @error_state     = error_state()

    raiserror(@error_message, @error_serverity, @error_state);
end catch

END

GO
/****** Object:  StoredProcedure [dbo].[sp_Organazation_GetDeptHeadByDeptID]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Organazation_GetDeptHeadByDeptID] --'53c38aa4-f6a8-4247-87b8-02e90d45a9c5'
(
	@SysID VARCHAR(50)
)
AS
BEGIN
	SELECT u.*,on1.Name AS OrgName,on1.[Type] FROM [User] AS u 
	INNER JOIN UserOrgNodes AS uon ON u.SysId=uon.User_SysId 
	INNER JOIN OrgNode AS on1 ON on1.SysId=uon.OrgNode_SysId
	 WHERE u.UserId COLLATE Chinese_PRC_CI_AS =(
	SELECT TOP 1 one.[Value]
	  FROM OrgNodeExtends AS one WHERE one.SysId=@SysID AND one.Name='DeptOwner')
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Organazation_GetUpUserAllInfo]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Organazation_GetUpUserAllInfo]
(
	@UserID VARCHAR(50),
	@type NVARCHAR(20)
)
AS 
BEGIN
declare @result_sysid uniqueidentifier
declare @resultp_sysid uniqueidentifier
declare @result_type nvarchar(20)
declare @break_flag BIT
	SELECT @result_sysid=on1.SysId,@resultp_sysid=on1.Parent_SysId,@result_type=on1.[Type]
	  FROM OrgNode AS on1 INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId INNER JOIN 
	[User] AS u ON U.SysId=uon.User_SysId 
	WHERE U.UserId=@UserID OR U.UserName=@UserID
 IF @result_type=@type
  BEGIN
  	SELECT  t.*,one.[Value],u.FirstName,u.Email,u.UserName
  	  from orgNode t INNER JOIN OrgNodeExtends AS one ON t.SysId=one.SysId 
  	  INNER JOIN [User] AS u ON u.UserId  COLLATE Chinese_PRC_CI_AS=one.[Value] 
  	where t.SysId=@result_sysid AND one.Name='DeptOwner'
  END
  ELSE 
  	BEGIN
	  set @break_flag = 0
	 while(ltrim(@result_type)!=ltrim(@type) and @break_flag = 0 )
	 begin
		if  Exists( select * 
			from[aZaaS.Framework].[dbo].[OrgNode]
			where [SysId] = @resultp_sysid )
			begin
				 select @resultp_sysid = [Parent_SysId],@result_type=[type] ,@result_sysid = [SysId]
				 from[aZaaS.Framework].[dbo].[OrgNode]
				 where [SysId] = @resultp_sysid
		 
				 print @result_sysid
				print @result_type
				 IF ltrim(@result_type)=ltrim(@type)
				 BEGIN
		 			SET @break_flag=1
				 END
			 end
		 else
			 begin
				set @break_flag = 1
				SET @result_sysid =NULL
			 end
	 end
 
		SELECT  t.*,one.[Value],u.FirstName,u.Email,u.UserName
  		  from orgNode t INNER JOIN OrgNodeExtends AS one ON t.SysId=one.SysId 
  		  INNER JOIN [User] AS u ON u.UserId  COLLATE Chinese_PRC_CI_AS=one.[Value] 
  		where t.SysId=@result_sysid AND one.Name='DeptOwner'
	END
END



GO
/****** Object:  StoredProcedure [dbo].[sp_Organazation_GetUserAllInfo]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Organazation_GetUserAllInfo]
(
	@UserID VARCHAR(50),
	@type NVARCHAR(20)
)
AS 
BEGIN
declare @result_sysid uniqueidentifier
declare @resultp_sysid uniqueidentifier
declare @result_type nvarchar(20)
declare @break_flag BIT
	SELECT @result_sysid=on1.SysId,@resultp_sysid=on1.Parent_SysId,@result_type=on1.[Type]
	  FROM OrgNode AS on1 INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId INNER JOIN 
	[User] AS u ON U.SysId=uon.User_SysId 
	WHERE U.UserId=@UserID OR U.UserName=@UserID
 IF @result_type=@type
  BEGIN
  	SELECT  t.*,one.[Value],u.FirstName,u.Email,u.UserName
  	  from orgNode t INNER JOIN OrgNodeExtends AS one ON t.SysId=one.SysId 
  	  INNER JOIN [User] AS u ON u.UserId  COLLATE Chinese_PRC_CI_AS=one.[Value] 
  	where t.SysId=@result_sysid AND one.Name='DeptOwner'
  END
  ELSE 
  	BEGIN
	  set @break_flag = 0
	 while(ltrim(@result_type)!=ltrim(@type) and @break_flag = 0 )
	 begin
		if  Exists( select * 
			from[aZaaS.Framework].[dbo].[OrgNode]
			where [SysId] = @resultp_sysid )
			begin
				 select @resultp_sysid = [Parent_SysId],@result_type=[type] ,@result_sysid = [SysId]
				 from[aZaaS.Framework].[dbo].[OrgNode]
				 where [SysId] = @resultp_sysid
		 
				 print @result_sysid
				print @result_type
				 IF ltrim(@result_type)=ltrim(@type)
				 BEGIN
		 			SET @break_flag=1
				 END
			 end
		 else
			 begin
				set @break_flag = 1
				SET @result_sysid =NULL
			 end
	 end
 
		SELECT  t.*,one.[Value],u.FirstName,u.Email,u.UserName
  		  from orgNode t INNER JOIN OrgNodeExtends AS one ON t.SysId=one.SysId 
  		  INNER JOIN [User] AS u ON u.UserId  COLLATE Chinese_PRC_CI_AS=one.[Value] 
  		where t.SysId=@result_sysid AND one.Name='DeptOwner'
	END
END



GO
/****** Object:  StoredProcedure [dbo].[sp_Organization_GetAllChildUser_ByUserID]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Organization_GetAllChildUser_ByUserID] --'20130704337'
(
	@UserID VARCHAR(30)
)
AS
BEGIN
	  DECLARE @OrgNodeID VARCHAR(500)
      SELECT @OrgNodeID=on1.SysId
	  FROM OrgNode AS on1 
	  INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId 
	  INNER JOIN [User] AS u ON U.SysId=uon.User_SysId
	  INNER JOIN OrgNodeExtends AS one ON one.SysId=on1.SysId
	  WHERE U.UserId=@UserID AND one.[Value] =@UserID
	  IF @OrgNodeID<>''
		  BEGIN
		  	SELECT ON1.Name,ON1.SysId AS OrgNodeID,U.UserName,U.FirstName,U.SysId,u.Email,u.UserId
	  		FROM OrgNode AS on1 
	  INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId 
	  INNER JOIN [User] AS u ON U.SysId=uon.User_SysId
      WHERE ON1.SysId IN (select id
				from dbo.UF_GetChildDept(@OrgNodeID)
      UNION SELECT @OrgNodeID AS  id )	
		  END
	  ELSE
	  	BEGIN
	  		SELECT ON1.Name,ON1.SysId AS OrgNodeID,U.UserName,U.FirstName,U.SysId,u.Email,u.UserId
	  		FROM OrgNode AS on1 
	  INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId 
	  INNER JOIN [User] AS u ON U.SysId=uon.User_SysId
	  		WHERE u.UserId=@UserID
	  	END
	
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Organization_GetUpOrgNode]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--sp_Organization_GetUpOrgNode '20140613001'
CREATE PROC [dbo].[sp_Organization_GetUpOrgNode] --'20140613001'
(
	@UserID VARCHAR(50)
)
AS
BEGIN
	DECLARE @Parent_SysID VARCHAR(50)
	SELECT  @Parent_SysID =on1.Parent_SysId
	  FROM OrgNode AS on1 INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId INNER JOIN 
	[User] AS u ON U.SysId=uon.User_SysId  
	WHERE U.UserId=@UserID OR U.UserName=@UserID
	
	SELECT on1.Name AS OrgName,on1.[Type] ,on1.SysId AS OrgSysID,
	(SELECT t.[Type] FROM orgNode t WHERE t.SysId=on1.Parent_SysId) AS UpType
		  FROM OrgNode AS on1 
		  
		WHERE on1.SysId =@Parent_SysID 
		
END

GO
/****** Object:  StoredProcedure [dbo].[sp_Organization_GetUserHead]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 --[dbo].[sp_Organization_GetUserHead] 'neowaydc\20080718126'
CREATE PROC [dbo].[sp_Organization_GetUserHead] --'20140613001'
(
	@UserID VARCHAR(50)
)
AS 
BEGIN
	DECLARE @Result_SysID VARCHAR(50)
	DECLARE @Result_ParentSysID VARCHAR(50)
	DECLARE @UpUserId VARCHAR(20)
	DECLARE @UpUserNAME VARCHAR(100)
	SELECT  @Result_SysID =on1.SysId
	  FROM OrgNode AS on1 INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId INNER JOIN 
	[User] AS u ON U.SysId=uon.User_SysId  
	WHERE (U.UserId=@UserID OR U.UserName=@UserID)
	
	SELECT @UpUserId=u.UserID,@Result_ParentSysID=on1.Parent_SysId ,@UpUserNAME=U.UserName
	  FROM OrgNode AS on1 INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId INNER JOIN 
	[User] AS u ON U.SysId=uon.User_SysId  WHERE u.UserId=
	(SELECT TOP 1 one.[Value] COLLATE chinese_prc_ci_as FROM OrgNodeExtends AS one WHERE one.SysId=@Result_SysID)
	IF(@UpUserId=@UserID OR @UpUserNAME=@UserID )
	BEGIN
			SELECT u.*,on1.Name AS OrgName,on1.[Type] ,on1.SysId AS OrgSysID
		  FROM OrgNode AS on1 INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId INNER JOIN 
	[User] AS u ON U.SysId=uon.User_SysId  WHERE u.UserId=
	(SELECT TOP 1 one.[Value] COLLATE chinese_prc_ci_as FROM OrgNodeExtends AS one WHERE one.SysId=@Result_ParentSysID)
	END
	ELSE
		BEGIN
			SELECT u.*,on1.Name AS OrgName,on1.[Type] ,on1.SysId AS OrgSysID
		   FROM OrgNode AS on1 INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId INNER JOIN 
	[User] AS u ON U.SysId=uon.User_SysId  WHERE u.UserId=
	(SELECT TOP 1 one.[Value] COLLATE chinese_prc_ci_as FROM OrgNodeExtends AS one WHERE one.SysId=@Result_SysID)
		END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Pager]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Pager]
@TotalPage int output,    --总页数输出
@TotalCount int output,    --总记录数输出
@TableName nvarchar(100),    --查询表名
@PrimaryKy varchar(50),        --主键
@Fields nvarchar(500),    --查询字段
@Where nvarchar(3000),    --查询条件
@Order nvarchar(100),    --排序字段
@PageIndex int,        --当前页数
@PageSize int        --页大小
AS
	Begin
		DECLARE @SqlText NVARCHAR(MAX)
		DECLARE @SqlCount NVARCHAR(MAX)
		  DECLARE @CurrentUser varchar(1050)
		  DECLARE @straa VARCHAR(4000)
		   set @straa=''''+','+'''';
		  set @CurrentUser=''

		  set @SqlText =N'SELECT * FROM 
		  ( SELECT ROW_NUMBER() OVER (ORDER BY '+@PrimaryKy+' DESC) AS ROWID, '+@Fields+' from '+@TableName+' where 1=1 '+@Where+') t'+
		  ' where ROWID BETWEEN '+CAST(((@pageIndex-1)*@PageSize+1) AS VARCHAR(20))+' AND '+cast(@pageIndex*@PageSize as varchar(20)) 


		  SET @SqlCount = 'SELECT @TotalCount=COUNT(1),@TotalPage=CEILING((COUNT(1)+0.0)/'
            + CAST(@PageSize AS NVARCHAR)+') FROM ' + @TableName +' where 1=1 '+@Where

--print @SqlCount
			EXEC SP_EXECUTESQL @SqlCount,N'@TotalCount INT OUTPUT,@TotalPage INT OUTPUT',
               @TotalCount OUTPUT,@TotalPage OUTPUT
		--PRINT @TotalCount
			set @TotalPage= @TotalCount / @PageSize +1

		 exec sp_executesql   @SqlText
	End


GO
/****** Object:  StoredProcedure [dbo].[SP_Pager1]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Pager1]
@TotalPage int output,    --总页数输出
@TotalCount int output,    --总记录数输出
@TableName nvarchar(100),    --查询表名
@PrimaryKy varchar(50),        --主键
@Fields nvarchar(500),    --查询字段
@Where nvarchar(3000),    --查询条件
@Order nvarchar(100),    --排序字段
@PageIndex int,        --当前页数
@PageSize int        --页大小
AS
	Begin
		DECLARE @SqlText NVARCHAR(MAX)
		  DECLARE @CurrentUser varchar(1050)
		  DECLARE @straa VARCHAR(4000)
		   set @straa=''''+','+'''';
		  set @CurrentUser=''

		  set @SqlText =N'SELECT * FROM 
		  ( SELECT ROW_NUMBER() OVER (ORDER BY '+@PrimaryKy+' DESC) AS ROWID, '+@Fields+' from '+@TableName+' where 1=1 '+@Where+') t'+
		  ' where ROWID BETWEEN '+CAST(((@pageIndex-1)*@PageSize+1) AS VARCHAR(20))+' AND '+cast(@pageIndex*@PageSize as varchar(20))
		 PRINT @SqlText

		 exec sp_executesql   @SqlText
	End


GO
/****** Object:  StoredProcedure [dbo].[SP_Pager2]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE [dbo].[SP_Pager2]
@TotalPage int output,    --总页数输出
@TotalCount int output,    --总记录数输出
@TableName nvarchar(100),    --查询表名
@PrimaryKy varchar(50),        --主键
@Fields nvarchar(500),    --查询字段
@Where nvarchar(3000),    --查询条件
@Order nvarchar(100),    --排序字段
@PageIndex int,        --当前页数
@PageSize int        --页大小
AS

SET NOCOUNT ON
SET ANSI_WARNINGS ON

IF @PageSize < 0 OR @PageIndex < 0
BEGIN        
RETURN
END

DECLARE @new_where1 NVARCHAR(3000)
DECLARE @new_order1 NVARCHAR(100)
DECLARE @new_order2 NVARCHAR(100)
DECLARE @Sql NVARCHAR(4000)
DECLARE @SqlCount NVARCHAR(4000)
DECLARE @sys_Begin INT

DECLARE @Top int

if(@PageIndex <=1)
    set @sys_Begin=0
else
    set @sys_Begin=(@PageIndex-1)*@PageSize+1

IF ISNULL(@Where,'') = ''
    SET @new_where1 = ' '
ELSE
    SET @new_where1 = ' WHERE 1=1 ' + @Where 

IF ISNULL(@Order,'') <> '' 
BEGIN
    SET @new_order1 = ' ORDER BY ' + Replace(@Order,'desc','')
    SET @new_order1 = Replace(@new_order1,'asc','desc')

    SET @new_order2 = ' ORDER BY ' + @Order
END
ELSE
BEGIN
    SET @new_order1 = ' ORDER BY ID DESC'
    SET @new_order2 = ' ORDER BY ID ASC'
END

SET @SqlCount = 'SELECT @TotalCount=COUNT(1),@TotalPage=CEILING((COUNT(1)+0.0)/'
            + CAST(@PageSize AS NVARCHAR)+') FROM ' + @TableName + @new_where1
print @SqlCount
EXEC SP_EXECUTESQL @SqlCount,N'@TotalCount INT OUTPUT,@TotalPage INT OUTPUT',
               @TotalCount OUTPUT,@TotalPage OUTPUT

IF @PageIndex > CEILING((@TotalCount+0.0)/@PageSize)    --如果输入的当前页数大于实际总页数,则把实际总页数赋值给当前页数
BEGIN
    SET @PageIndex =  CEILING((@TotalCount+0.0)/@PageSize)
END

set @sql = 'select '+ @Fields +' from ' + @TableName + ' w1 '
    + ' where '+ @PrimaryKy +' in ('
        +'select top '+ ltrim(str(@PageSize)) +' ' + @PrimaryKy + ' from '
        +'('
            +'select top ' + ltrim(STR(@PageSize * @PageIndex + @sys_Begin)) + ' ' + @PrimaryKy + ' FROM ' 
        + @TableName + @new_where1 + @new_order2 
        +') w ' + @new_order1
    +') ' + @new_order2

print(@sql)

Exec(@sql)



GO
/****** Object:  StoredProcedure [dbo].[sp_ProcessReminder]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_ProcessReminder] --'10897'
(
	@processInstIDS VARCHAR(100)
)
AS
DECLARE @strSql NVARCHAR(4000)
DECLARE @InstIDs VARCHAR(1000)
DECLARE @straa VARCHAR(4000)
   set @straa=''''+','+'''';
 set @InstIDs = replace(@processInstIDS,',',@straa)
 SET @strSql='SELECT distinct  t.ProcInstID as ID,t.ProcID,t.StartDate,t.TaskStartDate,t.FinishDate,t.Originator,t.ActID,
 (SELECT FirstName FROM [User] AS u WHERE u.UserName COLLATE Chinese_PRC_CI_AS =RIGHT(t.destination,len(t.destination)-CHARINDEX('':'',t.destination))) as HandlerUser,t.StartName,t.Folio,t.ActName,t.Destination,t.SN,t.Status,
(SELECT u.Email FROM [User] AS u WHERE CHARINDEX(u.UserName collate Chinese_PRC_CI_AS,t.Destination)>0) AS Email
   ,cp.ProcessFullName,cp.ProcessName,cp.ProcessSetID,cp.ApproveUrl,pf.[FormID],e.subject
                FROM view_ProcinstList AS t left join [dbo].[ProcessFormHeader] pf on t.ProcInstID=pf.[ProcInstID]  LEFT  join K2.ServerLog.[Proc] AS p ON t.ProcID=p.ID INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet cp ON p.ProcSetID=cp.ProcessSetID
                left join [dbo].[CustomEmailSubject] e on t.ProcInstID=e.[ProcessInstID]
                   WHERE Status<>5 AND t.ProcInstID IN ('''+@InstIDs+''')
   SELECT  TypeValue from EmailTransferConfig WHERE [Type]=''Email'' and TypeKey=''Url'''
 PRINT @strSql
   exec sp_executesql   @strSql

GO
/****** Object:  StoredProcedure [dbo].[sp_ProcessSuperviser_InstList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 --[dbo].[sp_ProcessSuperviser_InstList] 'neowaydc\20140504035','','','','',10,1,'张紫婷'
CREATE  PROC [dbo].[sp_ProcessSuperviser_InstList] --'neowaydc\20140504035','','','',''10,1,'张紫婷'
(
	@CurrentUser VARCHAR(50),
	@Folio NVARCHAR(100),
	@ProcessSetIds VARCHAR(100),
	@StartTime VARCHAR(50),
	@FinishTime VARCHAR(50),
	@PageSize INT ,
  	@pageIndex INT,
  	@StartUser VARCHAR(20)
)
AS
BEGIN
	 DECLARE @SqlText NVARCHAR(MAX)
     DECLARE @sqlWhere NVARCHAR(1000)
     DECLARE @replaceStr VARCHAR(4000)
     SET @sqlWhere=' 1=1'
	 set @replaceStr=''''+','+'''';
	 SET @sqlWhere += ' and vpl.Folio like N''%'+@Folio+'%'''
	 IF @ProcessSetIds<>'' AND LEN( @ProcessSetIds)<>0
		BEGIN
			set @ProcessSetIds = replace(@ProcessSetIds,',',@replaceStr)
		    SET @sqlWhere +=' and cps.[ProcessSetID] in ('''+@ProcessSetIds+''')'
		END
	 IF @StartUser<>''
	SET @sqlWhere +=' AND vpl.StartName like ''%'+@StartUser+'%'''
	 IF @StartTime<>''
	   SET @sqlWhere += ' and vpl.StartDate>='''+@StartTime+''''
     IF @FinishTime<>''
	   SET @sqlWhere +=' and vpl.StartDate<='''+@FinishTime+''''
	 SET @SqlText ='SELECT * FROM (
SELECT ROW_NUMBER() OVER (ORDER BY vpl.ProcInstID DESC) AS ROWID,vpl.ProcInstID,vpl.Destination,vpl.TaskStartDate,vpl.StartDate,vpl.Originator,vpl.StartName,vpl.Folio,vpl.ProcID,
vpl.ActName,vpl.SN,vpl.FinishDate,p.ProcSetID,cps.ProcessName,cps.ViewUrl,cps.ApproveUrl,pfh.FormID,(select StatusName from ProcessStatus where statusID=vpl.Status) as STATUS
,(SELECT FirstName FROM [User] AS u WHERE u.UserName COLLATE Chinese_PRC_CI_AS =RIGHT(vpl.destination,len(vpl.destination)-CHARINDEX('':'',vpl.destination))) as HandlerUser
  FROM view_ProcinstList AS vpl INNER JOIN k2.ServerLog.[Proc] AS p ON vpl.ProcID=p.ID INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON  ps.ID= cps.ProcessSetID INNER JOIN ProcessFormHeader AS pfh ON vpl.ProcInstID=pfh.ProcInstID

WHERE pfh.ProcInstID IN 
(
select id as procinstid from k2.ServerLog.ProcInst where Originator = ''K2:'+@CurrentUser+'''
union
select  DISTINCT procinstid from [K2].[ServerLog] .[ActInstSlot] where [user] = ''K2:'+@CurrentUser+'''
union
select h.ProcInstID 
from [aZaaS.Framework].[dbo].[ProcessFormCC] c
	inner join [aZaaS.Framework].[dbo].[ProcessFormHeader] h
		on c.FormId = h.FormID
where c.Receiver = '''+@CurrentUser+''' AND c.ReceiverStatus=1
) and  Status<>5 and status<>3 and status<>0 and'+@sqlWhere+') AS t WHERE 
t.ROWID BETWEEN '+CAST(((@pageIndex-1)*@PageSize+1) AS VARCHAR(20))+' AND '+cast(@pageIndex*@PageSize as varchar(20))+'
SELECT count(vpl.ProcInstID)
  FROM view_ProcinstList AS vpl INNER JOIN k2.ServerLog.[Proc] AS p ON vpl.ProcID=p.ID INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON  ps.ID= cps.ProcessSetID INNER JOIN ProcessFormHeader AS pfh ON vpl.ProcInstID=pfh.ProcInstID
WHERE pfh.ProcInstID IN 
(
select id as procinstid from k2.ServerLog.ProcInst where Originator = ''K2:'+@CurrentUser+'''
union
select DISTINCT procinstid from [K2].[ServerLog] .[ActInstSlot] where [user] = ''K2:'+@CurrentUser+'''
union
select h.ProcInstID 
from [aZaaS.Framework].[dbo].[ProcessFormCC] c
	inner join [aZaaS.Framework].[dbo].[ProcessFormHeader] h
		on c.FormId = h.FormID
where c.Receiver = '''+@CurrentUser+''' AND c.ReceiverStatus=1
) and  Status<>5 and status<>3 and status<>0 and'+@sqlWhere+''
 PRINT @SqlText

 exec sp_executesql   @SqlText
END

GO
/****** Object:  StoredProcedure [dbo].[sp_ProcessSuperviser_ReadList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[sp_ProcessSuperviser_ReadList] --'NEOWAYDC\OAFORDEVELOP'
@UserId nvarchar(50)
as
begin
	if(CHARINDEX('\',@UserId,0)<=0)
		set @UserId='NEOWAYDC\'+@UserId

	declare @Count int
	select @Count=count(1)  FROM [aZaaS.Framework]..ProcessFormCC cc
	  join [aZaaS.Framework]..ProcessFormHeader header on cc.FormId=header.FormID
	  and header.ProcessFolio<>'' and ReceiverStatus='0' and Receiver = @UserId


	select top 5 CreatedDate,header.ProcessFolio,header.FormID,cc.FormViewUrl,@Count Num
	  FROM [aZaaS.Framework]..ProcessFormCC cc
	  join [aZaaS.Framework]..ProcessFormHeader header on cc.FormId=header.FormID
	  and header.ProcessFolio<>'' and ReceiverStatus='0' and Receiver = @UserId
	  order by CreatedDate desc
end

GO
/****** Object:  StoredProcedure [dbo].[sp_ProcessSuperviser_TaskList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[sp_ProcessSuperviser_TaskList] --'K2:NEOWAYDC\20141008920'
@UserId nvarchar(50)
as
begin
	if(CHARINDEX('\',@UserId,0)<=0)
		set @UserId='K2:NEOWAYDC\'+@UserId

	declare @Count int
	select @Count=count(1)  FROM view_ProcinstList AS t 
							 INNER JOIN ProcessFormHeader AS pfh ON pfh.ProcInstID=t.ProcInstID
                             INNER JOIN k2.ServerLog.[Proc] AS p ON p.ID=t.ProcID
                             INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
                             INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON ps.ID=cps.ProcessSetID
	where Status=2 and Destination = @UserId

	 select top 5 pfh.FormID,pfh.ProcessFolio,[TaskStartDate],t.ProcInstID,SN,cps.ApproveUrl,'' runingTime,@Count Num  FROM view_ProcinstList AS t 
							 INNER JOIN ProcessFormHeader AS pfh ON pfh.ProcInstID=t.ProcInstID
                             INNER JOIN k2.ServerLog.[Proc] AS p ON p.ID=t.ProcID
                             INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
                             INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON ps.ID=cps.ProcessSetID
	where Status=2 and Destination = @UserId
	  order by TaskStartDate desc
end

GO
/****** Object:  StoredProcedure [dbo].[SP_RecordFormSubmitLog]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [dbo].[SP_RecordFormSubmitLog]
	-- Add the parameters for the stored procedure here
	@FormID int,@ProcessName nvarchar(50),@ActivityName nvarchar(50),@ActionName nvarchar(50) ='Submit'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ProcInstID int, @Comment nvarchar(500) = ' '
	DECLARE @TaskOwnerDisplayName nvarchar(100),@TaskOwner nvarchar(100)
	DECLARE @ActionTakerDisplayName nvarchar(100),@ActionTaker nvarchar(100)

	SELECT 
	@ProcInstID = ProcInstID,@Comment = SubmitComment,
	@TaskOwner = SubmitterAccount,@TaskOwnerDisplayName = SubmitterDisplayName,
	@ActionTaker = ApplicantAccount,@ActionTakerDisplayName = ApplicantDisplayName
	FROM [ProcessFormHeader] WHERE [FormID] = @FormID


	--SET @TaskOwnerDisplayName = ISNULL(@TaskOwnerDisplayName,'Unknown')
	--SET @ActionTakerDisplayName = ISNULL(@ActionTakerDisplayName,'Unknown')

	INSERT INTO [dbo].[ProcessLog]
           ([ProcInstID]
           ,[ActInstID]
           ,[SN]
           ,[ProcessName]
           ,[ActivityName]
           ,[OrigUserName]
           ,[OrigUserAccount]
           ,[UserName]
           ,[UserAccount]
           ,[ProfileID]
           ,[OpType]
           ,[ActionName]
           ,[Comment]
           ,[CommentDate]
           ,[TenantID])
     VALUES
           (@ProcInstID
           ,0
           ,NULL
           ,@ProcessName
           ,@ActivityName
           ,@TaskOwnerDisplayName
           ,@TaskOwner
           ,@ActionTakerDisplayName
           ,@ActionTaker
           ,NULL
           ,NULL
           ,@ActionName
           ,@Comment
           ,GETDATE()
           ,NULL)

END



GO
/****** Object:  StoredProcedure [dbo].[sp_sp_Organazation_GetUserAllInfo]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--[sp_sp_Organazation_GetUserAllInfo] '20140304314'
CREATE PROC [dbo].[sp_sp_Organazation_GetUserAllInfo] 
(@UserID VARCHAR(50))
AS
BEGIN
	 SELECT u.* ,
	 on1.SysID as OrgSysID,
	 on1.Name AS OrgName,
	 on1.Type,
	 P.SysId AS posSysID,
	 p.Name AS posName,
	 one.[Value] AS HeadUserID,
	 (SELECT u2.FirstName FROM [User] AS u2 WHERE u2.UserId COLLATE Chinese_PRC_CI_AS=one.[Value]) AS HeadUserName,
	 CASE WHEN u.UserId COLLATE Chinese_PRC_CI_AS=one.[Value] THEN 'Yes' ELSE 'No' END AS IsLeader,
	 (SELECT t.SysId FROM orgnode t WHERE t.SysId = on1.Parent_SysId) AS UpSysId,
	 (SELECT t.Name FROM orgnode t WHERE t.SysId = on1.Parent_SysId) AS UpName,
	 (SELECT t.[Type]
	    FROM orgnode t WHERE t.SysId = on1.Parent_SysId) AS UpType,
	 em.[Level],
	 em.[TotalServiceYear]
	  FROM OrgNode AS on1 INNER JOIN UserOrgNodes AS uon ON on1.SysId=uon.OrgNode_SysId 
	  INNER JOIN [User] AS u ON U.SysId=uon.User_SysId  
	  INNER JOIN PositionUsers AS pu ON pu.User_SysId=u.SysId
	JOIN Position AS p ON p.SysId=pu.Position_SysId 
	INNER JOIN OrgNodeExtends AS one ON one.SysId=on1.SysId
	LEFT JOIN ehr_EmployeeInfo AS em ON u.UserId COLLATE Chinese_PRC_CI_AS=em.[EmpNo]
	WHERE (U.UserId=@UserID OR U.UserName=@UserID) AND one.Name='DeptOwner'
	AND p.Name NOT LIKE '[v_]%'
END

GO
/****** Object:  StoredProcedure [dbo].[SP_UpdateProcessFormHeader]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		chris.hui
-- Create date: 2014-12-05
-- Description:	表单头
-- =============================================
CREATE PROCEDURE [dbo].[SP_UpdateProcessFormHeader]
	@FormSubject	nvarchar(100)
	,@ProcInstID	int
	,@ProcessFolio	nvarchar(20)
	,@Priority	int
	,@SubmitterAccount	nvarchar(30)
	,@SubmitterDisplayName	nvarchar(30)
	,@SubmitDate	datetime
	,@ApplicantAccount	nvarchar(30)
	,@ApplicantDisplayName	nvarchar(30)
	,@ApplicantTelNo	nvarchar(20)=NULL
	,@ApplicantEmail	nvarchar(20)=NULL
	,@ApplicantPositionID	nvarchar(20)=NULL
	,@ApplicantPositionName	nvarchar(30)=NULL
	,@ApplicantOrgNodeID	nvarchar(20)=NULL
	,@ApplicantOrgNodeName	nvarchar(30)=NULL
	,@SubmitComment	nvarchar(100)=NULL
	,@IsDraft	bit=1
	,@DraftUrl	nvarchar(10)=NULL
	,@FormId	int
AS
BEGIN
	update [ProcessFormHeader] set 
           [FormSubject]=isnull(@FormSubject,[FormSubject])
           ,[ProcInstID]=isnull(@ProcInstID,[ProcInstID])
           ,[ProcessFolio]=isnull(@ProcessFolio,[ProcessFolio])
           ,[Priority]=isnull(@Priority,[Priority])
           ,[SubmitterAccount]=isnull(@SubmitterAccount,[SubmitterAccount])
           ,[SubmitterDisplayName]=isnull(@SubmitterDisplayName,[SubmitterDisplayName])
           ,[SubmitDate]=isnull(@SubmitDate,[SubmitDate])
           ,[ApplicantAccount]=isnull(@ApplicantAccount,[ApplicantAccount])
           ,[ApplicantDisplayName]=isnull(@ApplicantDisplayName,[ApplicantDisplayName])
           ,[ApplicantTelNo]=isnull(@ApplicantTelNo,[ApplicantTelNo])
           ,[ApplicantEmail]=isnull(@ApplicantEmail,[ApplicantEmail])
           ,[ApplicantPositionID]=isnull(@ApplicantPositionID,[ApplicantPositionID])
           ,[ApplicantPositionName]=isnull(@ApplicantPositionName,[ApplicantPositionName])
           ,[ApplicantOrgNodeID]=isnull(@ApplicantOrgNodeID,[ApplicantOrgNodeID])
           ,[ApplicantOrgNodeName]=isnull(@ApplicantOrgNodeName,[ApplicantOrgNodeName])
           ,[SubmitComment]=isnull(@SubmitComment,[SubmitComment])
           ,[IsDraft]=isnull(@IsDraft,[IsDraft])
           ,[DraftUrl]=isnull(@DraftUrl,[DraftUrl])
		   where FormID=@FormId
END





GO
/****** Object:  UserDefinedFunction [dbo].[fGetPy]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  function [dbo].[fGetPy](@Str varchar(500)='')
returns varchar(500)
as
begin
declare @strlen int,@return varchar(500),@ii int
declare @n int,@c char(1),@chn nchar(1)

select @strlen=len(@str),@return='',@ii=0
set @ii=0
while @ii<@strlen
begin
select @ii=@ii+1,@n=63,@chn=substring(@str,@ii,1)
if @chn>'z'
select @n = @n +1
,@c = case chn when @chn then char(@n) else @c end
from(
select top 27 * from (
select chn = '吖'
union all select '八'
union all select '嚓'
union all select '咑'
union all select '妸'
union all select '发'
union all select '旮'
union all select '铪'
union all select '丌' --because have no 'i'
union all select '丌'
union all select '咔'
union all select '垃'
union all select '嘸'
union all select '拏'
union all select '噢'
union all select '妑'
union all select '七'
union all select '呥'
union all select '仨'
union all select '他'
union all select '屲' --no 'u'
union all select '屲' --no 'v'
union all select '屲'
union all select '夕'
union all select '丫'
union all select '帀'
union all select @chn) as a
order by chn COLLATE Chinese_PRC_CI_AS 
) as b
else set @c=@chn
set @return=@return+@c
end
return(@return)
end




GO
/****** Object:  UserDefinedFunction [dbo].[GetChildren]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Function [dbo].[GetChildren](@UserID VARCHAR(50))  
Returns @Tree Table (SysId VARCHAR(50),FirstName NVARCHAR(50),UserName VARCHAR(50),UserId VARCHAR(50),
OrgID VARCHAR(50),OrgName NVARCHAR(100),[Type] VARCHAR(50),Parent_OrgId VARCHAR(50))  
As  
Begin  
Insert @Tree SELECT u.SysId,u.FirstName,u.UserName,u.UserId,on1.SysId AS OrgID,on1.Name,on1.[Type],on1.Parent_SysId
  FROM [User] AS u INNER JOIN UserOrgNodes AS uon ON u.SysId=uon.User_SysId
INNER JOIN OrgNode AS on1 ON uon.OrgNode_SysId=on1.SysId
WHERE u.UserId=@UserID  
While @@Rowcount > 0  
Insert @Tree SELECT distinct u.SysId,u.FirstName,u.UserName,u.UserId,on1.SysId AS OrgID,on1.Name,on1.[Type],on1.Parent_SysId
  FROM [User] AS u INNER JOIN UserOrgNodes AS uon ON u.SysId=uon.User_SysId
INNER JOIN OrgNode AS on1 ON uon.OrgNode_SysId=on1.SysId Inner Join @Tree B On on1.Parent_SysId = B.OrgID And 
u.SysId Not In (SELECT SysId From @Tree)  
Return  
End 

GO
/****** Object:  UserDefinedFunction [dbo].[GetJsonFieldValue]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[GetJsonFieldValue](@fieldName as nvarchar(200),@jsonData as nvarchar(max) )
RETURNS nvarchar(200)  
AS  
begin
  if(@jsonData='' or @jsonData=null) return ''
 declare @jsonString nvarchar(max),@fieldValue nvarchar(max), @index int, @jsonLen int 
 set @index=1
 
 set @fieldValue=''
 set @jsonString=@jsonData;
set @jsonLen= charindex('"'+@fieldName+'":',@jsonString) +len('"'+@fieldName+'":')
 
set @jsonString=substring(@jsonString,@jsonLen,len(@jsonString)-@jsonLen)
  
  set @jsonString=replace(@jsonString,CHAR(10),'')
 set @jsonString=replace(@jsonString,CHAR(13),'')
  set @jsonString=replace(@jsonString,'",  "','","')
    set @jsonString =LTRIM(@jsonString)
 while  (substring(@jsonString,@index,2)!=',"')
 begin
 
   set @fieldValue=@fieldValue+substring(@jsonString,@index,1)
   set @index=@index+1
 end
  
   if(  substring(@fieldValue,1,1)='"') 
       set @fieldValue= substring(@fieldValue,2,LEN(@fieldValue)-2)
	  
return @fieldValue
end





GO
/****** Object:  UserDefinedFunction [dbo].[UF_GetChildDept]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[UF_GetChildDept]( @DeptId uniqueidentifier )
    returns  @tb table (id uniqueidentifier)
as 
begin
    insert into @tb
    select [SysId] from [dbo].[OrgNode] where [Parent_SysId] = @deptid
    while @@Rowcount >0  --只要有下级节点就循环
    begin
        insert into @tb
        select [SysId]   --取出刚刚插入的deptid,去部门表里找parentdept_id = deptid的记录。
            from [dbo].[OrgNode] as a inner join @tb as b on a.[Parent_SysId] = b.id and a.[SysId] not in(select id from @tb)
        
         
    end
    return
end

GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 7/16/2015 2:48:51 PM ******/
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
/****** Object:  Table [dbo].[ActivityControlSetting]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityControlSetting](
	[SysId] [uniqueidentifier] NOT NULL,
	[ActivityId] [int] NULL,
	[WorkMode] [int] NULL,
	[ControlRenderId] [nvarchar](50) NULL,
	[ControlName] [nvarchar](50) NULL,
	[ControlType] [nvarchar](50) NULL,
	[IsHide] [bit] NULL,
	[IsDisable] [bit] NULL,
	[IsCustom] [bit] NULL,
	[RenderTemplateId] [nvarchar](max) NULL,
	[ProcessFullName] [nvarchar](500) NULL,
 CONSTRAINT [PK_ActivityControlDisplaySetting] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ActivityFormSetting]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityFormSetting](
	[SysId] [uniqueidentifier] NOT NULL,
	[ActivityId] [int] NULL,
	[WorkMode] [int] NULL,
	[IsCustom] [bit] NULL,
	[IsEditable] [bit] NULL,
	[IsSettingEnabled] [bit] NULL,
 CONSTRAINT [PK_FormDisplaySetting] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BusinessDataColumn]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessDataColumn](
	[Id] [uniqueidentifier] NOT NULL,
	[ColumnName] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ValueType] [nvarchar](max) NULL,
	[IsVisible] [bit] NOT NULL,
	[Config_Id] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BusinessDataConfig]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessDataConfig](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationName] [nvarchar](max) NULL,
	[ProcessName] [nvarchar](max) NULL,
	[DbConnectionString] [nvarchar](max) NULL,
	[DataTable] [nvarchar](max) NULL,
	[WhereQuery] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommonReportConfig_DisplayArea]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CommonReportConfig_DisplayArea](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcSetID] [int] NULL,
	[FieldName] [nvarchar](100) NULL,
	[FieldType] [varchar](20) NULL,
	[DataResource] [varchar](500) NULL,
	[XPATH] [varchar](1000) NULL,
	[Memo] [nvarchar](200) NULL,
	[FieldID] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CommonReportConfig_SearchArea]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CommonReportConfig_SearchArea](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcSetID] [int] NULL,
	[FieldName] [nvarchar](100) NULL,
	[FieldType] [varchar](20) NULL,
	[DataResource] [varchar](500) NULL,
	[XPATH] [varchar](1000) NULL,
	[Memo] [nvarchar](200) NULL,
	[FieldID] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ContentTemplate]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContentTemplate](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Template] [nvarchar](max) NULL,
	[Variable] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ControlRenderTemplate]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ControlRenderTemplate](
	[SysId] [uniqueidentifier] NULL,
	[DisplayName] [nvarchar](50) NULL,
	[HtmlTemplate] [nvarchar](max) NULL,
	[CategoryId] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ControlTemplateCategory]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ControlTemplateCategory](
	[SysId] [uniqueidentifier] NULL,
	[ParentId] [uniqueidentifier] NULL,
	[ControlType] [int] NULL,
	[CategoryName] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Custom_BaseData_Customer]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Custom_BaseData_Customer](
	[PK] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [nvarchar](500) NULL,
	[Code] [varchar](50) NULL,
	[BelongIndustry] [nvarchar](500) NULL,
	[BelongArea] [nvarchar](max) NULL,
	[SalePerson] [nvarchar](50) NULL,
 CONSTRAINT [PK_NeoWay_BaseData_Customer] PRIMARY KEY CLUSTERED 
(
	[PK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Custom_BaseData_labelTemplate]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Custom_BaseData_labelTemplate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Number] [nvarchar](400) NOT NULL,
	[Principal] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[Picture] [varbinary](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_NeoWay_BaseData_labelTemplate] PRIMARY KEY CLUSTERED 
(
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Custom_BaseData_ModuleType]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Custom_BaseData_ModuleType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[PMCCharge] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_NeoWay_BaseData_ModuleType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Custom_BaseData_ProcessingVendor]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Custom_BaseData_ProcessingVendor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_NeoWay_BaseData_ProcessingVendor] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Custom_BaseData_ToolSoft]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Custom_BaseData_ToolSoft](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Number] [nvarchar](100) NOT NULL,
	[Principal] [nvarchar](100) NULL,
	[ToolType] [nvarchar](100) NULL,
	[FileName] [nvarchar](1000) NOT NULL,
	[Size] [int] NULL,
	[IssueDate] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_NeoWay_BaseData_ToolSoft] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Custom_Business_Daily]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Custom_Business_Daily](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SysId] [nvarchar](100) NOT NULL,
	[FormID] [int] NOT NULL,
	[CreateDate] [date] NOT NULL,
 CONSTRAINT [PK_NeoWay_Business_Daily] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Custom_Business_WeeklyDaily]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Custom_Business_WeeklyDaily](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SysId] [nvarchar](100) NOT NULL,
	[FormID] [int] NOT NULL,
	[CreateDate] [date] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
 CONSTRAINT [PK_NeoWay_WeeklyDaily] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CustomEmailSubject]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomEmailSubject](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessInstID] [int] NULL,
	[Subject] [nvarchar](2000) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CustomerRoleConfig]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerRoleConfig](
	[PK] [int] IDENTITY(1,1) NOT NULL,
	[ProcessFullName] [nvarchar](300) NULL,
	[ActivityName] [nvarchar](300) NULL,
	[FieldID] [varchar](100) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DBLogEntry]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBLogEntry](
	[Id] [uniqueidentifier] NOT NULL,
	[Priority] [int] NOT NULL,
	[Source] [nvarchar](max) NULL,
	[Category] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[OccurTime] [datetime] NOT NULL,
	[Exception] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Delegation]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Delegation](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[GroupID] [uniqueidentifier] NULL,
	[DeleType] [nvarchar](50) NULL,
	[ActionType] [nvarchar](50) NULL,
	[FromUser] [nvarchar](50) NULL,
	[ToUser] [nvarchar](max) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[FullName] [nvarchar](500) NULL,
	[ActivityName] [nvarchar](50) NULL,
	[ProcInstID] [int] NULL,
	[ActInstDestID] [int] NULL,
	[Reason] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[WorkTypeID] [nvarchar](50) NULL,
	[IsEnable] [bit] NULL,
	[DeleInstCount] [int] NULL,
	[TenantID] [nvarchar](50) NULL,
 CONSTRAINT [PK__Delegati__3214EC270EA330E9] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DelegationLog]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DelegationLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ActionType] [nvarchar](50) NULL,
	[FromUser] [nvarchar](50) NULL,
	[ToUser] [nvarchar](50) NULL,
	[SN] [nvarchar](500) NULL,
	[ProcessName] [nvarchar](50) NULL,
	[ActivityName] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[Comment] [nvarchar](500) NULL,
	[TenantID] [nvarchar](50) NULL,
 CONSTRAINT [PK__Delegati__3214EC271273C1CD] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ehr_EmployeeInfo]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ehr_EmployeeInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpNo] [nvarchar](50) NOT NULL,
	[EmpName] [nvarchar](50) NULL,
	[Sex] [nvarchar](10) NULL,
	[IsMarried] [nvarchar](10) NULL,
	[BirthDay] [date] NULL,
	[Age] [int] NULL,
	[Native] [nvarchar](50) NULL,
	[National] [nvarchar](50) NULL,
	[PoliticalAffiliation] [nvarchar](50) NULL,
	[IdNo] [nvarchar](18) NULL,
	[IdAddr] [nvarchar](200) NULL,
	[SettledInAddr] [nvarchar](200) NULL,
	[ContactInfo] [nvarchar](200) NULL,
	[MailingAddr] [nvarchar](200) NULL,
	[Degree] [nvarchar](50) NULL,
	[Education] [nvarchar](50) NULL,
	[GraduateSchool] [nvarchar](100) NULL,
	[GraduateDate] [date] NULL,
	[CollegeLevel] [nvarchar](50) NULL,
	[Professional] [nvarchar](50) NULL,
	[RecruitmentChannel] [nvarchar](100) NULL,
	[IsEntryAgain] [nvarchar](10) NULL,
	[Office] [nvarchar](100) NULL,
	[CurrBelong] [nvarchar](100) NULL,
	[Department] [nvarchar](100) NULL,
	[Section] [nvarchar](100) NULL,
	[Jobs] [nvarchar](100) NULL,
	[PositionType] [nvarchar](50) NULL,
	[PositionClass] [nvarchar](50) NULL,
	[PositionGroup] [nvarchar](50) NULL,
	[BelongType] [nvarchar](50) NULL,
	[BelongSubType] [nvarchar](50) NULL,
	[Level] [nvarchar](50) NULL,
	[Position] [nvarchar](50) NULL,
	[IsDevelopment] [nvarchar](50) NULL,
	[EmpStatus] [nvarchar](50) NULL,
	[InDate] [date] NULL,
	[PositiveDate] [date] NULL,
	[OutDate] [date] NULL,
	[OutType] [nvarchar](50) NULL,
	[PreServiceYear] [numeric](10, 1) NULL,
	[CompanyServiceYear] [numeric](10, 1) NULL,
	[TotalServiceYear] [numeric](10, 1) NULL,
	[InternshipeBegDate] [date] NULL,
	[InternshipEndDate] [date] NULL,
	[IsRelatives] [nvarchar](10) NULL,
	[RelativeName] [nvarchar](50) NULL,
	[RelativeDept] [nvarchar](50) NULL,
	[RelativeSection] [nvarchar](50) NULL,
	[Relationship] [nvarchar](50) NULL,
 CONSTRAINT [PK_ehr_EmployeeInfo] PRIMARY KEY CLUSTERED 
(
	[EmpNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailTemplate](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NULL,
	[Subject_Id] [uniqueidentifier] NULL,
	[Body_Id] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmailTransferConfig]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailTransferConfig](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](100) NULL,
	[TypeKey] [nvarchar](300) NULL,
	[TypeValue] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FlowSerialNo]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FlowSerialNo](
	[FormId] [bigint] NULL,
	[SerialNo] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FormAttachment]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormAttachment](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FileGuid] [uniqueidentifier] NULL,
	[FormId] [nvarchar](100) NULL,
	[ProcInstId] [int] NULL,
	[ProcessName] [nvarchar](100) NULL,
	[ActivityName] [nvarchar](100) NULL,
	[FileBytes] [bigint] NULL,
	[FileType] [nvarchar](200) NULL,
	[FileExtension] [nvarchar](50) NULL,
	[OldFileName] [nvarchar](200) NULL,
	[NewFileName] [nvarchar](200) NULL,
	[StoragePath] [nvarchar](200) NULL,
	[DownloadUrl] [nvarchar](200) NULL,
	[Uploader] [nvarchar](200) NULL,
	[UploaderName] [nvarchar](200) NULL,
	[UploadedDate] [datetime] NULL,
	[FileComment] [nvarchar](500) NULL,
 CONSTRAINT [PK_FormAttachment] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Fx_Extend]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fx_Extend](
	[SysID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[DefinitionXml] [nvarchar](max) NULL,
 CONSTRAINT [PK_Fx_Extend] PRIMARY KEY CLUSTERED 
(
	[SysID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogRequest]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogRequest](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[RequestUrl] [nvarchar](500) NULL,
	[RequestType] [nvarchar](5) NULL,
	[Parameters] [nvarchar](500) NULL,
	[Message] [nvarchar](500) NULL,
	[Details] [nvarchar](max) NULL,
	[IPAddress] [nvarchar](20) NULL,
	[RequestUser] [nvarchar](100) NULL,
	[RequestTime] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NetworkRole]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NetworkRole](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](250) NULL,
	[RoleID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_NeoWay_Business_NetworkRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrgChart]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrgChart](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Root_SysId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_OrgChart] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrgNode]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrgNode](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NULL,
	[Chart_SysId] [uniqueidentifier] NULL,
	[Parent_SysId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_OrgNode] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrgNodeExtends]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrgNodeExtends](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_OrgNodeExField] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Position]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Position](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Category_SysId] [uniqueidentifier] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PositionCategory]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PositionCategory](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Parent_SysId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PositionCategory] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PositionExtends]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PositionExtends](
	[SysID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_PositionExtends] PRIMARY KEY CLUSTERED 
(
	[SysID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PositionOrgNodes]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PositionOrgNodes](
	[Position_SysId] [uniqueidentifier] NOT NULL,
	[OrgNode_SysId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_PositionOrgNodes] PRIMARY KEY CLUSTERED 
(
	[Position_SysId] ASC,
	[OrgNode_SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PositionUsers]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PositionUsers](
	[Position_SysId] [uniqueidentifier] NOT NULL,
	[User_SysId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_PositionUsers] PRIMARY KEY CLUSTERED 
(
	[Position_SysId] ASC,
	[User_SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessFormApprovalDraft]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessFormApprovalDraft](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SerialNo] [nvarchar](50) NULL,
	[ActionName] [nvarchar](50) NULL,
	[ActionComment] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessFormCancel]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessFormCancel](
	[Id] [uniqueidentifier] NOT NULL,
	[ProcInstId] [int] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_ProcessFormCancel_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessFormCC]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessFormCC](
	[SysId] [uniqueidentifier] NOT NULL,
	[FormId] [int] NOT NULL,
	[Originator] [nvarchar](50) NULL,
	[OriginatorName] [nvarchar](50) NULL,
	[Receiver] [nvarchar](500) NULL,
	[ReceiverName] [nvarchar](500) NULL,
	[FormViewUrl] [nvarchar](1000) NULL,
	[CreatedDate] [datetime] NULL,
	[ReceiverStatus] [bit] NULL,
	[ReviewComment] [nvarchar](500) NULL,
	[ReceiverDate] [datetime] NULL,
	[ActivityId] [int] NULL,
	[ActivityName] [nvarchar](50) NULL,
	[Comment] [nvarchar](100) NULL,
 CONSTRAINT [PK_ProcessFormCC] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessFormContent]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessFormContent](
	[SysId] [int] IDENTITY(1,1) NOT NULL,
	[FormID] [int] NULL,
	[JsonData] [nvarchar](max) NULL,
	[XmlData] [xml] NULL,
 CONSTRAINT [PK_ProcessFormContent] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessFormFlowTheme]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessFormFlowTheme](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessFullName] [nvarchar](255) NOT NULL,
	[ModelFullName] [nvarchar](255) NOT NULL,
	[RuleString] [nvarchar](500) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_NeoWay_Configuration_FlowTheme] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessFormHeader]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessFormHeader](
	[FormID] [int] IDENTITY(1,1) NOT NULL,
	[FormSubject] [nvarchar](200) NULL,
	[ProcInstID] [int] NULL,
	[ProcessFolio] [nvarchar](max) NULL,
	[Priority] [int] NULL,
	[SubmitterAccount] [nvarchar](50) NULL,
	[SubmitterDisplayName] [nvarchar](200) NULL,
	[SubmitDate] [datetime] NULL,
	[ApplicantAccount] [nvarchar](50) NULL,
	[ApplicantDisplayName] [nvarchar](200) NULL,
	[ApplicantTelNo] [nvarchar](50) NULL,
	[ApplicantEmail] [nvarchar](200) NULL,
	[ApplicantPositionID] [nvarchar](200) NULL,
	[ApplicantPositionName] [nvarchar](200) NULL,
	[ApplicantOrgNodeID] [nvarchar](200) NULL,
	[ApplicantOrgNodeName] [nvarchar](200) NULL,
	[SubmitComment] [nvarchar](500) NULL,
	[IsDraft] [bit] NULL,
	[DraftUrl] [nvarchar](500) NULL,
 CONSTRAINT [PK_ProcessFormHeader_1] PRIMARY KEY CLUSTERED 
(
	[FormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessFormSigner]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessFormSigner](
	[ProcessInstId] [int] NOT NULL,
	[ActivityName] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[SignerName] [nvarchar](50) NOT NULL,
	[SignerDate] [datetime] NULL,
	[IsApproval] [bit] NULL,
	[ActionName] [nvarchar](50) NULL,
	[ApprovalDate] [datetime] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessLog]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProcInstID] [int] NULL,
	[ActInstID] [int] NULL,
	[SN] [nvarchar](500) NULL,
	[ProcessName] [nvarchar](50) NULL,
	[ActivityName] [nvarchar](50) NULL,
	[OrigUserName] [nvarchar](50) NULL,
	[OrigUserAccount] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[UserAccount] [nvarchar](50) NULL,
	[ProfileID] [nvarchar](50) NULL,
	[OpType] [nvarchar](50) NULL,
	[ActionName] [nvarchar](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[CommentDate] [datetime] NULL,
	[TenantID] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProcessLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessSerialNumber]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProcessSerialNumber](
	[ID] [uniqueidentifier] NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[StartValue] [int] NOT NULL,
	[StepValue] [int] NOT NULL,
	[CurrentValue] [int] NOT NULL,
	[CurrentValueTime] [datetime] NOT NULL,
	[NeedFill] [bit] NOT NULL,
	[FillLenth] [int] NOT NULL,
	[FillChar] [char](1) NOT NULL,
	[ZeroMode] [varchar](50) NOT NULL,
	[ZeroTime] [datetime] NOT NULL,
	[ZeroTimes] [int] NOT NULL,
	[Remark] [nvarchar](400) NULL,
 CONSTRAINT [PK_FRM_NUMBERING] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProcessStatus]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcessStatus](
	[StatusID] [int] NOT NULL,
	[StatusName] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcSetConfig]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProcSetConfig](
	[ProcSetID] [int] NULL,
	[ProcFullName] [varchar](500) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[QuartzCacheSendMails]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuartzCacheSendMails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[ReplyTo] [nvarchar](max) NOT NULL,
	[Prompt] [varchar](max) NULL,
	[Forward] [bit] NOT NULL,
	[ReplyDate] [datetime] NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_QuartzCacheSendMails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[QuartzSendMails]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuartzSendMails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](2000) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[ReplyTo] [nvarchar](max) NOT NULL,
	[Prompt] [varchar](max) NULL,
	[Forward] [bit] NOT NULL,
	[ReplyDate] [datetime] NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_QuartzSendMails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Role]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Category_SysId]  [uniqueidentifier] NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
/****** Object:  Table [dbo].[RoleProcess]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleProcess](
	[RoleSysId] [uniqueidentifier] NULL,
	[ProcessFullName] [nvarchar](200) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleUsers]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleUsers](
	[Role_SysId] [uniqueidentifier] NOT NULL,
	[User_SysId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RoleUsers] PRIMARY KEY CLUSTERED 
(
	[Role_SysId] ASC,
	[User_SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SE_TEMP]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SE_TEMP](
	[ProcSetID] [int] NULL,
	[FieldName] [nvarchar](100) NULL,
	[FieldType] [varchar](20) NULL,
	[DataResource] [varchar](500) NULL,
	[XPATH] [varchar](1000) NULL,
	[Memo] [nvarchar](200) NULL,
	[FieldID] [varchar](500) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SendErrorLog]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SendErrorLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ErrorID] [int] NOT NULL,
 CONSTRAINT [PK_SendErrorLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TEMP]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TEMP](
	[ProcSetID] [int] NULL,
	[FieldName] [nvarchar](100) NULL,
	[FieldType] [varchar](20) NULL,
	[DataResource] [varchar](500) NULL,
	[XPATH] [varchar](1000) NULL,
	[Memo] [nvarchar](200) NULL,
	[FieldID] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TempSupervision]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempSupervision](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SN] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TempSupervision] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
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
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserExtends]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserExtends](
	[SysId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserExField] PRIMARY KEY CLUSTERED 
(
	[SysId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserOrgNodes]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserOrgNodes](
	[User_SysId] [uniqueidentifier] NOT NULL,
	[OrgNode_SysId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserOrgNodes] PRIMARY KEY CLUSTERED 
(
	[User_SysId] ASC,
	[OrgNode_SysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserReportTos]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserReportTos](
	[User_SysID] [uniqueidentifier] NOT NULL,
	[ReportTo_SysID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserReportTos] PRIMARY KEY CLUSTERED 
(
	[User_SysID] ASC,
	[ReportTo_SysID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[GetOrgTree]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create function [dbo].[GetOrgTree](@SysId varchar(36))
returns table
as
return(
 WITH OrgTree 
 AS( 
  SELECT *, CAST(NULL AS nvarchar(max)) AS ParentName, 0 AS Generation FROM [OrgNode] 
  WHERE [Parent_SysId]=@SysId
  UNION ALL 
  SELECT org.*,OrgTree.Name AS ParentName, Generation + 1 FROM [OrgNode] AS org 
  INNER JOIN OrgTree ON org.Parent_SysId = OrgTree.[SysId] 
 )
 select * from OrgTree
)


GO
/****** Object:  View [dbo].[V_NW_PersonPlan]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE view [dbo].[V_NW_PersonPlan]
AS
	select config.[id]
		  ,UserNode.UserName [UserName]
		  ,UserNode.FirstName [UName]
		  --,case [StartDate] when null then '' else SUBSTRING([StartDate],0,11) end as [StartDate]
		  --,case [EndDate] when null then '' else SUBSTRING([EndDate],0,11) end as [EndDate]
		  ,[StartDate]
		  ,[EndDate]
		  ,[Status]
		  ,[CreatedBy]
		  ,[CreatedDate]
		  ,[UpdatedBy]
		  ,[UpdatedDate] from [dbo].NW_PersonPlan config right join
	(
	select top 100 u.UserName,u.FirstName from [User] u join [UserOrgNodes] org on u.SysId=org.User_SysId
	join OrgNode node on org.OrgNode_SysId=node.SysId
	and node. name ='JSR软件测试科'
	) UserNode on config.UserName=UserNode.UserName
	

	--select top 100 * from [User] u order by u.FirstName COLLATE Chinese_PRC_CS_AS_KS_WS
	--select * from [V_NW_PersonPlan]






GO
/****** Object:  View [dbo].[view_ProcinstList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  view [dbo].[view_ProcinstList]
    AS 
    
  SELECT 
	p.ID AS ProcInstID,
	p.StartDate,
	p.[Status],
	p.Originator,
	p.Folio,
    p.ProcID,
    p.FinishDate,
    act.Name AS ActName,
    wl.StartDate AS TaskStartDate,
    wl.Destination,
    wh.ActID,
    cast(wL.ProcInstID AS VARCHAR(20))+'_'+CAST(wL.ActInstDestID AS VARCHAR(20)) AS SN,
   (SELECT u.FirstName FROM [User] AS u WHERE CHARINDEX(u.UserName COLLATE Chinese_PRC_CI_AS,p.Originator )>0) as StartName
  FROM  K2.ServerLog.ProcInst AS p 
	LEFT JOIN k2.ServerLog.Worklist AS wl
		on p.ID = wl.procinstid
			AND wl.Destination<>'K2:NEOWAYDC\FALSE'
			AND wl.[Status] = 0
	LEFT JOIN K2.[Server].WorklistHeader AS wh
		ON wl.ProcInstID = wh.ProcInstID
			AND wl.ActInstDestID = wh.ActInstDestID
	LEFT JOIN k2.[Server].Act AS act
		ON wh.ActID = act.ID
  



GO
/****** Object:  View [dbo].[VW_ErrorLog]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VW_ErrorLog]
AS
SELECT   K2.Server.ErrorLog.ID, K2.Server.ProcInst.Folio, K2.Server.ErrorLog.ProcID, K2.Server.ErrorLog.ProcInstID, 
                K2.Server.ErrorLog.State, K2.Server.ErrorLog.Context, K2.Server.ErrorLog.ObjectID, K2.Server.ErrorLog.Descr, 
                K2.Server.ErrorLog.Date, K2.Server.ErrorLog.Code, K2.Server.ErrorLog.ItemName, K2.Server.ErrorLog.StackTrace, 
                K2.Server.ErrorLog.CodeID
FROM      K2.Server.ErrorLog LEFT OUTER JOIN
                K2.Server.ProcInst ON K2.Server.ProcInst.ID = K2.Server.ErrorLog.ProcInstID
WHERE   (K2.Server.ErrorLog.ID NOT IN
                    (SELECT   ErrorID
                     FROM      dbo.SendErrorLog))


GO
/****** Object:  View [dbo].[VW_UserContactList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[VW_UserContactList]
AS
SELECT     a.SysId, a.UserName, a.FirstName, a.LastName, a.Sex, a.Email, a.Address, a.Phone, a.Status, a.Remark, a.UserId AS EmpNo, a.CreateDate, a.OrgId, a.OrgName, 
                      a.OrgType, ue1.Value AS ShortNum, ue2.Value AS phoneInPrivate, ISNULL(a.FirstName, N'') + ISNULL(a.LastName, N'') AS EmpName
FROM         (SELECT     u.SysId, u.UserName, u.FirstName, u.LastName, u.Sex, u.Email, u.Address, u.Phone, u.Status, u.Remark, u.UserId, u.CreateDate, o.SysId AS OrgId, 
                                              o.Name AS OrgName, o.Type AS OrgType
                       FROM          (SELECT     SysId, UserName, FirstName, LastName, Sex, Email, Address, Phone, Status, Remark, UserId, CreateDate
                                               FROM          dbo.[User]
                                               WHERE      (Status = 'True')) AS u LEFT OUTER JOIN
                                              dbo.UserOrgNodes AS uo ON u.SysId = uo.User_SysId LEFT OUTER JOIN
                                              dbo.OrgNode AS o ON uo.OrgNode_SysId = o.SysId) AS a LEFT OUTER JOIN
                      dbo.UserExtends AS ue1 ON a.SysId = ue1.SysId AND ue1.Name = '短号' LEFT OUTER JOIN
                      dbo.UserExtends AS ue2 ON a.SysId = ue2.SysId AND ue2.Name = '不公开电话号码'



GO
/****** Object:  View [dbo].[VW_UserDepartmentList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[VW_UserDepartmentList]
AS
SELECT     u.UserName,u.LastName+u.FirstName Name, o.SysId AS DepartmentID, o.Name AS DepartmentName
FROM         dbo.[User] AS u INNER JOIN
                      dbo.UserOrgNodes AS uo ON u.SysId = uo.User_SysId INNER JOIN
                      dbo.OrgNode AS o ON uo.OrgNode_SysId = o.SysId





GO
/****** Object:  View [dbo].[VW_UserList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VW_UserList]
AS
SELECT     SysId, UserName, FirstName, LastName, Sex, Email, Address, Phone, Status, Remark, FirstName + ' ' + LastName AS DisplayName
FROM         dbo.[User]



GO
/****** Object:  View [dbo].[VW_UserPositionList]    Script Date: 7/16/2015 2:48:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[VW_UserPositionList]
AS
SELECT     u.UserName, p.SysId AS PositionID, p.Name AS PositionName
FROM         dbo.[User] AS u INNER JOIN
                      dbo.PositionUsers AS pu ON u.SysId = pu.User_SysId INNER JOIN
                      dbo.Position AS p ON pu.Position_SysId = p.SysId



GO
ALTER TABLE [dbo].[ProcessFormCC] ADD  CONSTRAINT [DF_ProcessFormCC_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_Customer', @level2type=N'COLUMN',@level2name=N'PK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_Customer', @level2type=N'COLUMN',@level2name=N'CustomerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_Customer', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属行业' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_Customer', @level2type=N'COLUMN',@level2name=N'BelongIndustry'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属地区' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_Customer', @level2type=N'COLUMN',@level2name=N'BelongArea'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_Customer', @level2type=N'COLUMN',@level2name=N'SalePerson'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_labelTemplate', @level2type=N'COLUMN',@level2name=N'Number'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'负责人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_labelTemplate', @level2type=N'COLUMN',@level2name=N'Principal'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标贴模板名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_labelTemplate', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图片' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_labelTemplate', @level2type=N'COLUMN',@level2name=N'Picture'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_labelTemplate', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'模块型号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ModuleType', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ModuleType', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'加工厂商' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ProcessingVendor', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ProcessingVendor', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目负责人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ToolSoft', @level2type=N'COLUMN',@level2name=N'Principal'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'工具类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ToolSoft', @level2type=N'COLUMN',@level2name=N'ToolType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'版本/文件名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ToolSoft', @level2type=N'COLUMN',@level2name=N'FileName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'大小' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ToolSoft', @level2type=N'COLUMN',@level2name=N'Size'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发布时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ToolSoft', @level2type=N'COLUMN',@level2name=N'IssueDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Custom_BaseData_ToolSoft', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程实例ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProcessFormSigner', @level2type=N'COLUMN',@level2name=N'ProcessInstId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前流程节点名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProcessFormSigner', @level2type=N'COLUMN',@level2name=N'ActivityName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'加签人账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProcessFormSigner', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'被加签人账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProcessFormSigner', @level2type=N'COLUMN',@level2name=N'SignerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'加签时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProcessFormSigner', @level2type=N'COLUMN',@level2name=N'SignerDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否审批' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProcessFormSigner', @level2type=N'COLUMN',@level2name=N'IsApproval'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批动作' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProcessFormSigner', @level2type=N'COLUMN',@level2name=N'ActionName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审批时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProcessFormSigner', @level2type=N'COLUMN',@level2name=N'ApprovalDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'第一个字' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'FirstName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后一个字' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'LastName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'性别' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'Sex'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'Email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'Address'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'Phone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'User', @level2type=N'COLUMN',@level2name=N'CreateDate'
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
         Begin Table = "ErrorLog (K2.Server)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 190
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ProcInst (K2.Server)"
            Begin Extent = 
               Top = 6
               Left = 228
               Bottom = 146
               Right = 420
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_ErrorLog'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_ErrorLog'
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
         Begin Table = "u"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 189
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "uo"
            Begin Extent = 
               Top = 6
               Left = 227
               Bottom = 84
               Right = 383
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "o"
            Begin Extent = 
               Top = 84
               Left = 227
               Bottom = 192
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_UserDepartmentList'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_UserDepartmentList'
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
         Begin Table = "User"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 159
               Right = 292
            End
            DisplayFlags = 280
            TopColumn = 1
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_UserList'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_UserList'
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
         Begin Table = "u"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 189
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "pu"
            Begin Extent = 
               Top = 6
               Left = 227
               Bottom = 84
               Right = 378
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "p"
            Begin Extent = 
               Top = 84
               Left = 227
               Bottom = 177
               Right = 385
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
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_UserPositionList'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_UserPositionList'
GO


CREATE PROC [dbo].[SP_KStar_Common_ProcInstManagerSign]
  (
  		@ProcessName NVARCHAR(200),
  		@Folio NVARCHAR(200),
  		@StartTime VARCHAR(50),
  		@FinishTime VARCHAR(50),
  		@StartUser NVARCHAR(100),
  		@Status int	,
  		@PageSize INT ,
  		@pageIndex INT,
  		@SysId VARCHAR(100)
  )
  AS 
  DECLARE @SqlText NVARCHAR(MAX)
  DECLARE @sqlWhere NVARCHAR(1000)
  DECLARE @CurrentUser varchar(1050)
  DECLARE @straa VARCHAR(4000)
   set @straa=''''+','+'''';
  set @CurrentUser=''

  SET @sqlWhere = 'Folio like N''%'+@Folio+'%'''
	IF @ProcessName<>'' AND LEN( @ProcessName)<>0
	BEGIN
		set @ProcessName = replace(@ProcessName,',',@straa)
		SET @sqlWhere +=' and cps.[ProcessSetID] in ('''+@ProcessName+''')'
	END
		
		
    IF @StartTime<>''
	SET @sqlWhere += ' and StartDate>='''+@StartTime+''''
  IF @FinishTime<>''
	SET @sqlWhere +=' and StartDate<='''+@FinishTime+''''
  IF @Status<>0
	SET @sqlWhere +=' and Status='+@Status+''
  IF @StartUser<>''
	SET @sqlWhere +=' AND t.StartName like ''%'+@StartUser+'%'''
  IF @SysId<>''
     Set @CurrentUser='where ru.User_SysId='''+@SysId+''''
  
  set @SqlText =N'SELECT * FROM 
  ( SELECT  ROW_NUMBER() OVER (ORDER BY t.TaskStartDate DESC) AS ROWID,
   t.ProcInstID as ID,
   t.ProcID,
   t.StartDate,
   t.TaskStartDate,
   t.FinishDate,
   t.Originator,
   t.StartName,
   t.Folio,
   t.SN,
   t.ActName,
   t.Destination,
   isnull(replace(cast(pfc2.[Status] AS NVARCHAR(20)),''5'',''作废''),(SELECT ps2.StatusName
                           FROM ProcessStatus AS ps2 WHERE ps2.StatusID=t.[Status])) AS STATUS,
  cps.ProcessFullName,
  cps.ProcessName,
  cps.ProcessSetID,
  cps.ViewUrl,
  pfh.[FormID],
  (SELECT FirstName FROM [User] AS u WHERE u.UserName COLLATE Chinese_PRC_CI_AS =RIGHT(destination,len(destination)-CHARINDEX('':'',destination))) as HandlerUser 
 FROM view_ProcinstList AS t INNER JOIN ProcessFormHeader AS pfh ON pfh.ProcInstID=t.ProcInstID
 INNER JOIN ProcessFormContent AS pfc ON pfc.FormID=pfh.FormID
 INNER JOIN k2.ServerLog.[Proc] AS p ON p.ID=t.ProcID
 INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
 INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON ps.ID=cps.ProcessSetID
 LEFT JOIN ProcessFormCancel AS pfc2 ON pfc2.ProcInstId=t.ProcInstID
                  where '+@sqlWhere+' and t.STATUS =2  ) AS tp
                
  WHERE tp.ROWID BETWEEN '+CAST(((@pageIndex-1)*@PageSize+1) AS VARCHAR(20))+' AND '+cast(@pageIndex*@PageSize as varchar(20))+'
  select count(*)  FROM view_ProcinstList AS t INNER JOIN ProcessFormHeader AS pfh ON pfh.ProcInstID=t.ProcInstID
 INNER JOIN ProcessFormContent AS pfc ON pfc.FormID=pfh.FormID
 INNER JOIN k2.ServerLog.[Proc] AS p ON p.ID=t.ProcID
 INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
 INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON ps.ID=cps.ProcessSetID
 LEFT JOIN ProcessFormCancel AS pfc2 ON pfc2.ProcInstId=t.ProcInstID
                  where '+@sqlWhere+' AND t.STATUS=2'
 PRINT @SqlText

 exec sp_executesql   @SqlText

GO