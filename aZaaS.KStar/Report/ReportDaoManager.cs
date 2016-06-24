using aZaaS.KStar.DTOs.Report;
using aZaaS.KStar.Repositories;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.Workflow.Configuration.Models;
using aZaaS.KStar.Authentication;

namespace aZaaS.KStar.Report
{
    public class ReportDaoManager
    {
        string domain = ConfigurationManager.AppSettings["WindowDomain"].ToString();

        public ReportDaoManager()
        {
            domain=KStarUserAuthenticator.GetParameter("Domain");
        }
        //string domain = ConfigurationManager.AppSettings["WindowDomain"].ToString();
        public Decimal GetProcInstCount(DateTime? _sDate = null, DateTime? _fDate = null, string _deptId = "", string _procSetID = "", string _startUserId = "", int _processCategory = 0)
        {
            string deptGuid = string.IsNullOrEmpty(_deptId)?Guid.NewGuid().ToString():_deptId;
            var sql = @"
                        select 
                               Cast((select count(*) from [K2].[ServerLog].[ProcInst] ins
		                    join [K2].[ServerLog].[Proc] pr on ins.ProcID = pr.ID
		                    join [aZaaS.Framework].[dbo].[User] u on lower(ins.originator) COLLATE Chinese_PRC_CI_AS = 'k2:";
            sql += domain+ @"\'+u.UserId";
		                 sql += @"   join [aZaaS.Framework].[dbo].[UserOrgNodes] uon on u.SysId = uon.User_SysId
		                    join [aZaaS.Framework].[dbo].[OrgNode] org on uon.OrgNode_SysId = org.SysId
		                    join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] pc on pr.ProcSetID = pc.ProcessSetID
		                    join [aZaaS.KStar].[dbo].[Configuration_Category] cc on pc.Configuration_CategoryID = cc.ID
		                     where ins.Status = 3 and 
		                      ('{0}'='0' or pr.ProcSetID in ({0})) and 
		                     ('{1}'='' or ins.StartDate>=cast('{1}' as datetime)) and 
		                     ('{2}'='' or ins.StartDate<=cast('{2}' as datetime)) and 
		                     ('{3}'='' or u.SysId = '{3}') and 
		                     ('{4}'='' or org.SysId=cast('{6}' as uniqueidentifier)) and
		                     ({5} = 0 or cc.ID = {5}))
                               *1.0/(
                        select case count(*) when 0 then 1 else count(*) end from [K2].[ServerLog].[ProcInst] ins
		                    join [K2].[ServerLog].[Proc] pr on ins.ProcID = pr.ID";

		                   sql += " join [aZaaS.Framework].[dbo].[User] u on lower(ins.originator) COLLATE Chinese_PRC_CI_AS = 'k2:"+domain+@"\'+u.UserId";
		                    sql +=@" join [aZaaS.Framework].[dbo].[UserOrgNodes] uon on u.SysId = uon.User_SysId
		                    join [aZaaS.Framework].[dbo].[OrgNode] org on uon.OrgNode_SysId = org.SysId
		                    join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] pc on pr.ProcSetID = pc.ProcessSetID
		                    join [aZaaS.KStar].[dbo].[Configuration_Category] cc on pc.Configuration_CategoryID = cc.ID
		                     where 
		                     ('{0}'='0' or pr.ProcSetID in({0})) and 
		                     ('{1}'='' or ins.StartDate>=cast('{1}' as datetime)) and 
		                     ('{2}'='' or ins.StartDate<=cast('{2}' as datetime)) and 
		                     ('{3}'='' or u.SysId = '{3}') and 
		                     ('{4}'='' or org.SysId=cast('{6}' as uniqueidentifier)) and
		                     ({5} = 0 or cc.ID = {5})) As Dec(4,3))
                               as rate ";
            sql = string.Format(sql,!string.IsNullOrEmpty(_procSetID.Trim(','))?_procSetID.Trim(','):"0", _sDate, _fDate, _startUserId, _deptId, _processCategory, deptGuid);
            using (K2DBContext context = new K2DBContext())
            {
                var rate = context.Database.SqlQuery<Decimal>(sql).SingleOrDefault();
                return rate > 1 ? 1 : rate;
            }
        }

        /// <summary>
        /// 按状态分组的流程实例数量统计
        /// </summary>
        /// <param name="_sDate">发起开始时间</param>
        /// <param name="_fDate">发起结束时间</param>
        /// <param name="_deptId">部门编号</param>
        /// <param name="_procSetID">流程集ID</param>
        /// <param name="_startUserId">发起人ID</param>
        /// <param name="_processCategory">所属公司</param>
        /// <returns></returns>
        public List<ProcInstStatusGroupDTO> GetProcInstGroupByStatus(DateTime? _sDate = null, DateTime? _fDate = null, string _deptId = "", string _procSetID = "", string _startUserId = "", int _processCategory = 0)
        {
            string deptGuid = string.IsNullOrEmpty(_deptId) ? Guid.NewGuid().ToString() : _deptId;
            string userGuid = string.IsNullOrEmpty(_startUserId) ? Guid.NewGuid().ToString() : _startUserId;
            var sql = string.Format(@"
                            SELECT count(ins.id) as Num
                                  ,cast(ins.[Status] as int) as Status
                              FROM [ServerLog].[ProcInst] ins
	                            join [K2].[ServerLog].[Proc] pr on ins.ProcID = pr.ID");
	                           sql+= " join [aZaaS.Framework].[dbo].[User] u on lower(ins.originator) COLLATE Chinese_PRC_CI_AS = 'k2:";
                               sql += domain + @"\'+u.UserId";
                               sql += @" join [aZaaS.Framework].[dbo].[UserOrgNodes] uon on u.SysId = uon.User_SysId
	                            join [aZaaS.Framework].[dbo].[OrgNode] org on uon.OrgNode_SysId = org.SysId
	                            join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] pc on pr.ProcSetID = pc.ProcessSetID
	                            join [aZaaS.KStar].[dbo].[Configuration_Category] cc on pc.Configuration_CategoryID = cc.ID
		                            where 
		                            ('{0}'='0' or pr.ProcSetID in ({0})) and 
		                            ('{1}'='' or ins.StartDate>=cast('{1}' as datetime)) and 
		                            ('{2}'='' or ins.StartDate<=cast('{2}' as datetime)) and 
		                            ('{3}'='' or u.SysId = cast('{7}' as uniqueidentifier)) and 
		                            ('{4}'='' or org.SysId=cast('{6}' as uniqueidentifier)) and
		                            ({5} = 0 or cc.ID = {5})
                              group by ins.[Status] 
                              order by Status";
             sql = string.Format(sql,!string.IsNullOrEmpty(_procSetID.Trim(',')) ? _procSetID.Trim(',') : "0", _sDate, _fDate, _startUserId, _deptId, _processCategory, deptGuid, userGuid);
            using (K2DBContext context = new K2DBContext())
            {
                var rate = context.Database.SqlQuery<ProcInstStatusGroupDTO>(sql).ToList();
                return rate;
            }
        }

        /// <summary>
        /// 按状态取流程实例数据
        /// </summary>
        /// <param name="_sDate">发起开始时间</param>
        /// <param name="_fDate">发起结束时间</param>
        /// <param name="_deptId">部门编号</param>
        /// <param name="_procSetID">流程集ID</param>
        /// <param name="_startUserId">发起人ID</param>
        /// <param name="_processCategory">所属公司</param>
        /// <param name="_Status">流程实例状态</param>
        /// <returns></returns>
        public List<ProcDealDurationMasterDTO> GetProcInstByStatus(out int total, DateTime? _sDate = null, DateTime? _fDate = null, string _deptId = ""
            , int _procSetID = 0, string _startUserId = "", int _processCategory = 0, int _Status = 0, int page = 1, int pageSize = 20)
        {
            string deptGuid = string.IsNullOrEmpty(_deptId) ? Guid.NewGuid().ToString() : _deptId;
            string userGuid = string.IsNullOrEmpty(_startUserId) ? Guid.NewGuid().ToString() : _startUserId;
            var sql = string.Format(@"
                                    SELECT ins.Folio,
	                                       cast(ins.[Status] as int) as Status,
	                                       ins.StartDate as Startswith,
	                                       case when ins.StartDate = ins.FinishDate then GETDATE() else ins.FinishDate end as Finishwith,
	                                       (SELECT a.Name+',' FROM K2.Server.ActionActInstRights  ar
	                                           inner join  K2.[Server].[Act] a on ar.ActID = a.ID
	                                           WHERE ar.ProcInstID = ins.ID
	                                           GROUP BY a.Name
	                                           FOR XML PATH('')
	                                       ) as ActiveActName
                                      FROM [ServerLog].[ProcInst] ins
	                                    join [K2].[ServerLog].[Proc] pr on ins.ProcID = pr.ID");

	                                    sql+="join [aZaaS.Framework].[dbo].[User] u on lower(ins.originator) COLLATE Chinese_PRC_CI_AS = 'k2:"+domain+@"\'+u.UserId";
	                                   sql+=@" join [aZaaS.Framework].[dbo].[UserOrgNodes] uon on u.SysId = uon.User_SysId
	                                    join [aZaaS.Framework].[dbo].[OrgNode] org on uon.OrgNode_SysId = org.SysId
	                                    join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] pc on pr.ProcSetID = pc.ProcessSetID
	                                    join [aZaaS.KStar].[dbo].[Configuration_Category] cc on pc.Configuration_CategoryID = cc.ID
		                            WHERE 
		                            ({0}=0 or pr.ProcSetID = {0}) and 
		                            ('{1}'='' or ins.StartDate>=cast('{1}' as datetime)) and 
		                            ('{2}'='' or ins.StartDate<=cast('{2}' as datetime)) and 
		                            ('{3}'='' or u.SysId = '{8}') and 
		                            ('{4}'='' or org.SysId=cast('{7}' as uniqueidentifier)) and
		                            ({5} = 0 or cc.ID = {5}) and
                                    cast(ins.[Status] as int) = {6}
	                     ";
                           sql = string.Format(sql,_procSetID, _sDate, _fDate, _startUserId, _deptId, _processCategory, _Status, deptGuid, userGuid);
            using (K2DBContext context = new K2DBContext())
            {
                var rate = context.Database.SqlQuery<ProcDealDurationMasterDTO>(sql);
                total = rate.Count();
                return rate.Skip(pageSize*(page-1)).Take(pageSize).ToList();
            }
        }


        public List<ProcDealDurationMasterDTO> GetProcDealDurationList(out int total,DateTime? _sDate = null, DateTime? _fDate = null
            , string _deptId = "",  string _procSetID = "", string _startUserId = "", int _processCategory = 0, int page = 1, int pageSize = 20)
        {
            string deptGuid = string.IsNullOrEmpty(_deptId) ? Guid.NewGuid().ToString() : _deptId;
            string userGuid = string.IsNullOrEmpty(_startUserId) ? Guid.NewGuid().ToString() : _startUserId;
            var sql = string.Format(@"
                    SELECT pi.[ID] as ProcInstID,
                    	   pi.StartDate as Startswith,
                    	   (CASE WHEN pi.StartDate = pi.FinishDate THEN GETDATE() ELSE pi.FinishDate END) as Finishwith,
                           u.FirstName as StartUser,
                    	   cps.ProcessName as ProcessName,
                    	   datediff(second,pi.StartDate ,(case pi.Status when 3 then pi.FinishDate else GETDATE() end)) as TotalConsumingSecond,
                           pi.Folio
                    	  FROM [ServerLog].[ProcInst] pi ");
            sql += " join [aZaaS.Framework].[dbo].[User] u on lower(pi.originator) COLLATE Chinese_PRC_CI_AS = 'k2:" + domain + @"\'+ u.UserId";
            sql += @" join [K2].[ServerLog].[Proc] pr on pi.ProcID = pr.ID
                    		join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] cps on pr.ProcSetID = cps.ProcessSetID
                    		join [aZaaS.Framework].[dbo].[UserOrgNodes] uon on u.SysId = uon.User_SysId
                    		join [aZaaS.Framework].[dbo].[OrgNode] org on uon.OrgNode_SysId = org.SysId
                    		join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] pc on pr.ProcSetID = pc.ProcessSetID
                    		join [aZaaS.KStar].[dbo].[Configuration_Category] cc on pc.Configuration_CategoryID = cc.ID
		                     where 
                             pi.status = 3 and
		                     ('{0}'='0' or pr.ProcSetID in ({0})) and 
		                     ('{1}'='' or pi.StartDate>=cast('{1}' as datetime)) and 
		                     ('{2}'='' or pi.StartDate<=cast('{2}' as datetime)) and 
		                     ('{3}'='' or u.SysId = cast('{7}' as uniqueidentifier)) and 
		                     ('{4}'='' or org.SysId=cast('{6}' as uniqueidentifier)) and
		                     ({5} = 0 or cc.ID = {5})
	                     ";
            sql = string.Format(sql,!string.IsNullOrEmpty(_procSetID.Trim(',')) ? _procSetID.Trim(',') : "0", _sDate, _fDate, _startUserId, _deptId, _processCategory, deptGuid, userGuid);
            //pi.status not in(0,4,5) and
            using (K2DBContext context = new K2DBContext())
            {
                var list = context.Database.SqlQuery<ProcDealDurationMasterDTO>(sql);
                total = list.Count();
                return list.Skip((page-1)*pageSize).Take(pageSize).ToList();
            }


        }

        public List<ProcDealDurationItemDTO> GetProcDealDurationItemList(int prosInstId)
        {
            var sql = string.Format(@"
                    SELECT a.Name as ActivityName
                          ,ais.StartDate as Arrivewith
                          ,ais.FinishDate as Submitwith
	                      ,ais.Status as status
	                      ,datediff(second,ai.StartDate ,(CASE WHEN ais.StartDate = ais.FinishDate THEN GETDATE() ELSE ais.FinishDate END)) as TotalConsumingSecond
                      FROM [K2].[ServerLog].[ActInstSlot] ais
                     inner join [K2]. [ServerLog]. [ActInst] ai
                          on ais .ProcInstID = ai . ProcInstID
                                 and ais .ActInstID = ai.ID
                      join [K2].[ServerLog].[Act] a on ai.ActID = a.ID
                      where 
                      ai.ProcInstID = {0}
	                     ", prosInstId);
            using (K2DBContext context = new K2DBContext())
            {
                var list = context.Database.SqlQuery<ProcDealDurationItemDTO>(sql).ToList();
                return list;
            }


        }

        public List<UseFrequencyDTO> GetUseFrequencyList(out int total, DateTime? _sDate, DateTime? _fDate, string _deptId, int _processCategory = 0, int page = 1, int pageSize = 20)
        {
            string deptGuid = string.IsNullOrEmpty(_deptId) ? Guid.NewGuid().ToString() : _deptId;
            string sqlstr = @"
                         SELECT
                             ROW_NUMBER() over (order by count(i.ID) desc) as RowNumber,
                             cps.ProcessName,
                             avg( datediff(second , StartDate,CASE WHEN i.StartDate = i.FinishDate THEN GETDATE ()
                             ELSE i . FinishDate END )) as Avg_Consuming_Second,
                         	count(i.ID) UseCount
                         FROM [K2] .[ServerLog] . [ProcInst] i";
            sqlstr += " inner join [aZaaS.Framework].[dbo].[User] u on lower(i.originator) COLLATE Chinese_PRC_CI_AS = 'k2:"+domain+"\'+u.UserId";
                             sqlstr+=@" inner join [K2].[ServerLog].[Proc] pr on i.ProcID = pr.ID
                             inner join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] cps on pr.ProcSetID = cps.ProcessSetID
                             inner join [K2] . [ServerLog] .[Proc] c
                                     on i .ProcID = c.id
                             inner join [K2] . [ServerLog] .[ProcSet] s
                                     on c .ProcSetID = s.id
                             inner join [aZaaS.Framework].[dbo].[UserOrgNodes] uon on u.SysId = uon.User_SysId
                             inner join [aZaaS.Framework].[dbo].[OrgNode] org on uon.OrgNode_SysId = org.SysId
                             inner join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] pc on pr.ProcSetID = pc.ProcessSetID
                             inner join [aZaaS.KStar].[dbo].[Configuration_Category] cc on pc.Configuration_CategoryID = cc.ID
                         	where 
                         	('{0}'='' or i.StartDate>=cast('{0}' as datetime)) and 
                         	('{1}'='' or i.StartDate<=cast('{1}' as datetime)) and 
                         	('{2}'='' or org.SysId=cast('{4}' as uniqueidentifier)) and
                         	({3} = 0 or cc.ID = {3})
                         group by cps.ProcessName
                         order by UseCount desc
	                     ";
            var sqlAll = string.Format(sqlstr, _sDate, _fDate, string.Empty, 0, deptGuid);
            var sql = string.Format(sqlstr, _sDate, _fDate, _deptId, _processCategory, deptGuid);
            using (K2DBContext context = new K2DBContext())
            {
                List<UseFrequencyDTO> listAll = context.Database.SqlQuery<UseFrequencyDTO>(sqlAll).ToList();
                List<UseFrequencyDTO> listCondtion = context.Database.SqlQuery<UseFrequencyDTO>(sql).ToList();
                total = listCondtion.Count;
                listCondtion = listCondtion.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                foreach (UseFrequencyDTO entity in listAll) {
                    foreach (UseFrequencyDTO item in listCondtion)
                    {
                        if (item.ProcessName == entity.ProcessName)
                        {
                            item.FrequencyType = GetFrequencyType(entity.RowNumber, listAll.Count());
                        }
                    }
                }
                return listCondtion;
            }


        }

        public List<ProcessApprovalConsumingSecondDTO> GetProcessApprovalConsumingSecondData(DateTime? _sDate, DateTime? _fDate, int _topOrbottom = 0, string _procSetID = "", int _processCategory = 0, int _actID = 0)
        {
            string sort = _topOrbottom == 0 ? "asc" : "desc";
            var sql =string.Empty;
            if (_actID > 0)
            {
                sql = string.Format(@"
                                    SELECT top 10
										 ais.ProcInstID as ProcInstID,
                                        sum(datediff (SECOND , ais .StartDate, CASE WHEN ais. StartDate = ais.FinishDate THEN GETDATE ()
                                    ELSE ais.FinishDate END)) as CasumeSecond,
                                    i.Folio
                                  FROM [K2].[ServerLog].[ActInstSlot] ais
                                        inner join [K2].[ServerLog].[ActInst] ai
                                               on ais .ProcInstID = ai.ProcInstID
                                                      and ais .ActInstID = ai.ID
                                        inner join [K2].[ServerLog].[Act] a
                                               on ai .ActID = a.ID
                                        inner join [K2].[ServerLog].[Proc] pc
                                               on a .ProcID = pc.ID
                                        inner join [K2].[ServerLog].[ProcSet] ps
                                               on pc .ProcSetID = ps.ID
										inner join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] cps 
											   on pc.ProcSetID = cps.ProcessSetID
										inner join [aZaaS.KStar].[dbo].[Configuration_Category] cc 
										       on cps.Configuration_CategoryID = cc.ID
										inner join k2.ServerLog.ProcInst i
											   on ais.ProcInstID = i.ID
                                where i.Status=3
									  and pc.ProcSetID in ({0})
									  and ('{1}'='' or i.StartDate>=cast('{1}' as datetime))
									  and ('{2}'='' or i.StartDate<=cast('{2}' as datetime))
									  and ({3}=0 or cc.ID = {3})
									  and ({4}=0 or a.ID = {4})
								group by ais.ProcInstID,i.Folio
                                order by CasumeSecond {5}
	                     ", !string.IsNullOrEmpty(_procSetID.Trim(',')) ? _procSetID.Trim(',') : "0", _sDate, _fDate, _processCategory, _actID, sort);

            }
            else
            {
                sql = string.Format(@"
                                SELECT
                                top 10
                                 i.[ID] as ProcInstID
                                      ,datediff(second , i.StartDate,i . FinishDate) as CasumeSecond,
                                 i.Folio
                                  FROM [ServerLog].[ProcInst] i
                                  inner join [K2].[ServerLog].[Proc] pr on i.ProcID = pr.ID
                                  inner join [aZaaS.KStar].[dbo].[Configuration_ProcessSet] pc on pr.ProcSetID = pc.ProcessSetID
                                  inner join [aZaaS.KStar].[dbo].[Configuration_Category] cc on pc.Configuration_CategoryID = cc.ID
                                  where i.Status=3
                                  and pr.ProcSetID in ({0})
                                  and ('{1}'='' or i.StartDate>=cast('{1}' as datetime))
                                  and ('{2}'='' or i.StartDate<=cast('{2}' as datetime))
                                  and ({3}=0 or cc.ID = {3})
                                  order by CasumeSecond {4}
	                     ", !string.IsNullOrEmpty(_procSetID.Trim(',')) ? _procSetID.Trim(',') : "0", _sDate, _fDate, _processCategory, sort);
            }
            using (K2DBContext context = new K2DBContext())
            {
                return context.Database.SqlQuery<ProcessApprovalConsumingSecondDTO>(sql).ToList();
            }
        }


        public List<ProcessApprovalConsumingSecondDTO> GetProcessActConsumingSecondData(int _procInstID = 0)
        {
            var sql = string.Format(@"
                                    SELECT
                                         a.Name as ActivityName,
                                        datediff (SECOND , ais .StartDate, CASE WHEN ais. StartDate = ais.FinishDate THEN GETDATE ()
                                    ELSE ais.FinishDate END) as CasumeSecond,
							            i.Folio
                                  FROM [K2].[ServerLog].[ActInstSlot] ais
                                        inner join [K2].[ServerLog].[ActInst] ai
                                               on ais .ProcInstID = ai.ProcInstID
                                                      and ais .ActInstID = ai.ID
                                        inner join [K2].[ServerLog].[Act] a
                                               on ai .ActID = a.ID
                                        inner join [K2].[ServerLog].[Proc] pc
                                               on a .ProcID = pc.ID
                                        inner join [K2].[ServerLog].[ProcSet] ps
                                               on pc .ProcSetID = ps.ID
							            inner join [K2].[ServerLog].[ProcInst] i 
								               on ais.ProcInstID = i.ID
                                where ais.ProcInstID ='{0}' 
                                order by ais.ID Asc
	                     ", _procInstID);
            using (K2DBContext context = new K2DBContext())
            {
                return context.Database.SqlQuery<ProcessApprovalConsumingSecondDTO>(sql).ToList();
            }
        }

        public List<ActDTO> getProcActList(int procSetId)
        {
            var sql = string.Format(@"
								select a.Name as ActivityName,
                                       a.ID as ActID
                                from [ServerLog].[ProcSet] ps
								    inner join [ServerLog].[Act] a on ps.ProcVerID = a.ProcID
                                where ps.ID = {0}
	                     ", procSetId);
            using (K2DBContext context = new K2DBContext())
            {
                return context.Database.SqlQuery<ActDTO>(sql).ToList();
            }
        }

        private string GetFrequencyType(Int64 rowNumber, int totalNumber)
        {
            if (rowNumber * 1.0 / totalNumber <= 0.2)
            {
                return "高";
            }
            else if (rowNumber * 1.0 / totalNumber <= 0.8)
            {
                return "中";
            }
            else
            {
                return "低";
            }
        }

        /// <summary>
        /// 报表选择所属 下拉框
        /// </summary>
        /// <returns></returns>
        public List<Configuration_Category> GetAllCategoryList()
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                return dbContext.Configuration_CategorySet.ToList();
            }

        }
    }
}
