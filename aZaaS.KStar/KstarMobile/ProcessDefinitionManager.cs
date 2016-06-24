using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Transactions;

namespace aZaaS.KStar.KstarMobile
{
    public sealed class ProcessDefinitionManager
    {
        private static GroupDefinitionManager gdm = new GroupDefinitionManager();
        private static ItemDefinitionManager idm = new ItemDefinitionManager();

        public List<ProcessDefinitionEntity> GetAllProcessDefinition()
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.ProcessDefinition.ToList();
            }
        }

        public ProcessDefinitionEntity GetProcessDefinitionById(int id)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.ProcessDefinition.Where(x => x.ID == id).SingleOrDefault();
            }
        }

        public List<ProcessDefinitionEntity> GetProcessDefinitionByParendId(int parentid)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.ProcessDefinition.Where(x => x.ParentID == parentid).ToList<ProcessDefinitionEntity>();
            }
        }

        public List<ProcessDefinitionEntity> GetProcessDefinitionByParendIdandName(int parentid, string processname)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.ProcessDefinition.Where(x => x.ParentID == parentid && x.ProcessFullName == processname).ToList<ProcessDefinitionEntity>();
            }
        }

        public string GetProcessName(int id)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.ProcessDefinition.Where(x => x.ID == id).SingleOrDefault().ProcessFullName;
            }
        }

        public bool AddProcessDefinition(ProcessDefinitionEntity process)
        {
            var result = true;

            ProcessDefinitionEntity processtask = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processtaskinfo = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processbaseinfo = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processextendinfo = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processprocbaseinfo = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processbizinfo = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processprocloginfo = new ProcessDefinitionEntity();

            ProcessDefinitionEntity processheader = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processrow = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processdata = new ProcessDefinitionEntity();
            ProcessDefinitionEntity processmore = new ProcessDefinitionEntity();

            ProcessDefinitionEntity baseinfo_sn_item = new ProcessDefinitionEntity();
            ProcessDefinitionEntity baseinfo_destination_item = new ProcessDefinitionEntity();

            ProcessDefinitionEntity extendinfo_stafficon_item = new ProcessDefinitionEntity(); 
            //displayname
            ProcessDefinitionEntity baseinfo_displayname_item = new ProcessDefinitionEntity();


            ProcessDefinitionEntity ActivityName = new ProcessDefinitionEntity();
            ProcessDefinitionEntity AssignedDate = new ProcessDefinitionEntity();
            ProcessDefinitionEntity Folio = new ProcessDefinitionEntity();
            ProcessDefinitionEntity ProcDispName = new ProcessDefinitionEntity();
            ProcessDefinitionEntity ProcInstID = new ProcessDefinitionEntity();
            ProcessDefinitionEntity StartDate = new ProcessDefinitionEntity();


            ProcessDefinitionEntity ApplicantDisplayName = new ProcessDefinitionEntity();
            ProcessDefinitionEntity ApplicantOrgNodeName = new ProcessDefinitionEntity();
            ProcessDefinitionEntity ApplicantPositionName = new ProcessDefinitionEntity();
            ProcessDefinitionEntity SubmitDate = new ProcessDefinitionEntity();
            ProcessDefinitionEntity SubmitterDisplayName = new ProcessDefinitionEntity();


            ProcessDefinitionEntity CommentDate = new ProcessDefinitionEntity();
            ProcessDefinitionEntity UserName = new ProcessDefinitionEntity();
            ProcessDefinitionEntity ActionName = new ProcessDefinitionEntity();
            ProcessDefinitionEntity Comment = new ProcessDefinitionEntity();
            
             
            using (TransactionScope ts = new TransactionScope())
            {
                GroupDefinitionEntity task = gdm.GetGroupDefinitionByName("Task");                
                //查询group初始数据  没有则初始化
                if (task == null)
                {
                    gdm.InitGroupData();
                }

                #region  baseinfo->SN  Destination
               // baseinfo_sn_item.ID = 1;
                baseinfo_sn_item.ProcessFullName = process.ProcessFullName;
                baseinfo_sn_item.ChildType = "Item";
                baseinfo_sn_item.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "SN", LabelName = "SN" });
                baseinfo_sn_item.OrderNo = 1;
                baseinfo_sn_item.ConnectionString = process.ConnectionString;
                baseinfo_sn_item.Mapping = process.Mapping;
                baseinfo_sn_item.WhereString = process.WhereString;

              //  baseinfo_destination_item.ID = 2;
                baseinfo_destination_item.ProcessFullName = process.ProcessFullName;
                baseinfo_destination_item.ChildType = "Item";
                baseinfo_destination_item.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "Destination", LabelName = "Destination" });
                baseinfo_destination_item.OrderNo = 2;
                baseinfo_destination_item.ConnectionString = process.ConnectionString;
                baseinfo_destination_item.Mapping = process.Mapping;
                baseinfo_destination_item.WhereString = process.WhereString;

                // extendinfo_displayname_item.ID = 2;
                baseinfo_displayname_item.ProcessFullName = process.ProcessFullName;
                baseinfo_displayname_item.ChildType = "Item";
                baseinfo_displayname_item.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "DisplayName", LabelName = "DisplayName", Visible = true });
                baseinfo_displayname_item.OrderNo = 3;
                baseinfo_displayname_item.ConnectionString = process.ConnectionString;
                baseinfo_displayname_item.Mapping = "Originator";
                baseinfo_displayname_item.WhereString = process.WhereString;


                ActivityName.ProcessFullName = process.ProcessFullName;
                ActivityName.ChildType = "Item";
                ActivityName.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "ActivityName", LabelName = "节点名称",Visible=true });
                ActivityName.OrderNo = 4;
                ActivityName.ConnectionString = process.ConnectionString;
                ActivityName.Mapping = process.Mapping;
                ActivityName.WhereString = process.WhereString;


                AssignedDate.ProcessFullName = process.ProcessFullName;
                AssignedDate.ChildType = "Item";
                AssignedDate.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "AssignedDate", LabelName = "AssignedDate", Visible = true });
                AssignedDate.OrderNo = 5;
                AssignedDate.ConnectionString = process.ConnectionString;
                AssignedDate.Mapping = process.Mapping;
                AssignedDate.WhereString = process.WhereString;


                Folio.ProcessFullName = process.ProcessFullName;
                Folio.ChildType = "Item";
                Folio.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "Folio", LabelName = "Folio", Visible = true });
                Folio.OrderNo = 6;
                Folio.ConnectionString = process.ConnectionString;
                Folio.Mapping = process.Mapping;
                Folio.WhereString = process.WhereString;


                ProcDispName.ProcessFullName = process.ProcessFullName;
                ProcDispName.ChildType = "Item";
                ProcDispName.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "ProcDispName", LabelName = "ProcDispName", Visible = true });
                ProcDispName.OrderNo = 7;
                ProcDispName.ConnectionString = process.ConnectionString;
                ProcDispName.Mapping = process.Mapping;
                ProcDispName.WhereString = process.WhereString;


                ProcInstID.ProcessFullName = process.ProcessFullName;
                ProcInstID.ChildType = "Item";
                ProcInstID.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "ProcInstID", LabelName = "ProcInstID", Visible = true });
                ProcInstID.OrderNo = 8;
                ProcInstID.ConnectionString = process.ConnectionString;
                ProcInstID.Mapping = process.Mapping;
                ProcInstID.WhereString = process.WhereString;


                StartDate.ProcessFullName = process.ProcessFullName;
                StartDate.ChildType = "Item";
                StartDate.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "StartDate", LabelName = "StartDate", Visible = true });
                StartDate.OrderNo = 9;
                StartDate.ConnectionString = process.ConnectionString;
                StartDate.Mapping = process.Mapping;
                StartDate.WhereString = process.WhereString;


                ApplicantDisplayName.ProcessFullName = process.ProcessFullName;
                ApplicantDisplayName.ChildType = "Item";
                ApplicantDisplayName.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "ApplicantDisplayName", LabelName = "申请人名称", Visible = true });
                ApplicantDisplayName.OrderNo = 2;
                ApplicantDisplayName.ConnectionString = process.ConnectionString;
                ApplicantDisplayName.Mapping = process.Mapping;
                ApplicantDisplayName.WhereString = process.WhereString;



                ApplicantOrgNodeName.ProcessFullName = process.ProcessFullName;
                ApplicantOrgNodeName.ChildType = "Item";
                ApplicantOrgNodeName.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "ApplicantOrgNodeName", LabelName = "申请人部门", Visible = true });
                ApplicantOrgNodeName.OrderNo = 3;
                ApplicantOrgNodeName.ConnectionString = process.ConnectionString;
                ApplicantOrgNodeName.Mapping = process.Mapping;
                ApplicantOrgNodeName.WhereString = process.WhereString;




                ApplicantPositionName.ProcessFullName = process.ProcessFullName;
                ApplicantPositionName.ChildType = "Item";
                ApplicantPositionName.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "ApplicantPositionName", LabelName = "申请人职位", Visible = true });
                ApplicantPositionName.OrderNo = 4;
                ApplicantPositionName.ConnectionString = process.ConnectionString;
                ApplicantPositionName.Mapping = process.Mapping;
                ApplicantPositionName.WhereString = process.WhereString;



                SubmitDate.ProcessFullName = process.ProcessFullName;
                SubmitDate.ChildType = "Item";
                SubmitDate.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "SubmitDate", LabelName = "提交时间", Visible = true,Format="yyyy-MM-dd HH:mm:ss" });
                SubmitDate.OrderNo =5;
                SubmitDate.ConnectionString = process.ConnectionString;
                SubmitDate.Mapping = process.Mapping;
                SubmitDate.WhereString = process.WhereString;

                 
                SubmitterDisplayName.ProcessFullName = process.ProcessFullName;
                SubmitterDisplayName.ChildType = "Item";
                SubmitterDisplayName.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "SubmitterDisplayName", LabelName = "填单人名称", Visible = true });
                SubmitterDisplayName.OrderNo =6;
                SubmitterDisplayName.ConnectionString = process.ConnectionString;
                SubmitterDisplayName.Mapping = process.Mapping;
                SubmitterDisplayName.WhereString = process.WhereString;


                CommentDate.ProcessFullName = process.ProcessFullName;
                CommentDate.ChildType = "Item";
                CommentDate.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "CommentDate", LabelName = "操作时间", Visible = true, Format = "yyyy-MM-dd HH:mm:ss" });
                CommentDate.OrderNo = 2;
                CommentDate.ConnectionString = process.ConnectionString;
                CommentDate.Mapping = process.Mapping;
                CommentDate.WhereString = process.WhereString;


                UserName.ProcessFullName = process.ProcessFullName;
                UserName.ChildType = "Item";
                UserName.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "UserName", LabelName = "执行人", Visible = true });
                UserName.OrderNo = 3;
                UserName.ConnectionString = process.ConnectionString;
                UserName.Mapping = process.Mapping;
                UserName.WhereString = process.WhereString;


                ActionName.ProcessFullName = process.ProcessFullName;
                ActionName.ChildType = "Item";
                ActionName.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "ActionName", LabelName = "审批意见", Visible = true });
                ActionName.OrderNo = 1;
                ActionName.ConnectionString = process.ConnectionString;
                ActionName.Mapping = process.Mapping;
                ActionName.WhereString = process.WhereString;


                Comment.ProcessFullName = process.ProcessFullName;
                Comment.ChildType = "Item";
                Comment.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "Comment", LabelName = "意见说明", Visible = true });
                Comment.OrderNo = 2;
                Comment.ConnectionString = process.ConnectionString;
                Comment.Mapping = process.Mapping;
                Comment.WhereString = process.WhereString;






                #endregion

                #region  extendinfo->StaffIcon
              //  extendinfo_stafficon_item.ID = 1;
                extendinfo_stafficon_item.ProcessFullName = process.ProcessFullName;
                extendinfo_stafficon_item.ChildType = "Item";
                extendinfo_stafficon_item.ChildID = idm.AddItemDefinition(new ItemDefinitionEntity() { Name = "StaffIcon", LabelName = "StaffIcon" });
                extendinfo_stafficon_item.OrderNo = 1;
                extendinfo_stafficon_item.ConnectionString = process.ConnectionString;
                extendinfo_stafficon_item.Mapping = process.Mapping;
                extendinfo_stafficon_item.WhereString = process.WhereString;
                 
                #endregion



                //BaseInfo
                processbaseinfo.ProcessFullName = process.ProcessFullName;
                processbaseinfo.ChildType = "Group";
                processbaseinfo.ChildID = gdm.GetGroupIdByName("BaseInfo");
                processbaseinfo.OrderNo = 3;
                processbaseinfo.ConnectionString = process.ConnectionString;
                processbaseinfo.Mapping = process.Mapping;
                processbaseinfo.WhereString = process.WhereString;

                processbaseinfo.Childs.Add(baseinfo_sn_item);
                processbaseinfo.Childs.Add(baseinfo_destination_item);
                processbaseinfo.Childs.Add(baseinfo_displayname_item);
                processbaseinfo.Childs.Add(ActivityName);
                processbaseinfo.Childs.Add(AssignedDate);
                processbaseinfo.Childs.Add(Folio);
                processbaseinfo.Childs.Add(ProcDispName);
                processbaseinfo.Childs.Add(ProcInstID);
                processbaseinfo.Childs.Add(StartDate);


              //  processextendinfo.ID = 3;
                processextendinfo.ProcessFullName = process.ProcessFullName;
                processextendinfo.ChildType = "Group";
                processextendinfo.ChildID = gdm.GetGroupIdByName("ExtendInfo");
                processextendinfo.OrderNo = 4;
                processextendinfo.ConnectionString = process.ConnectionString;
                processextendinfo.Mapping = process.Mapping;
                processextendinfo.WhereString = process.WhereString;

                processextendinfo.Childs.Add(extendinfo_stafficon_item);
                //processextendinfo.Childs.Add(probaseinfo_displayname_item);

               // processtask.ID = 0;
                processtask.ProcessFullName = process.ProcessFullName;
                processtask.ChildType = "Group";
                processtask.ChildID = gdm.GetGroupIdByName("Task");
                processtask.OrderNo = 1;
                processtask.ConnectionString = process.ConnectionString;
                processtask.Mapping = process.Mapping;
                processtask.WhereString = process.WhereString;
                processtask.Childs.Add(processbaseinfo);
                processtask.Childs.Add(processextendinfo);


              //  processprocbaseinfo.ID = 4;
                processprocbaseinfo.ProcessFullName = process.ProcessFullName;
                processprocbaseinfo.ChildType = "Group";
                processprocbaseinfo.ChildID = gdm.GetGroupIdByName("ProcBaseInfo");
                processprocbaseinfo.OrderNo = 5;
                processprocbaseinfo.ConnectionString = process.ConnectionString;
                processprocbaseinfo.Mapping = process.Mapping;
                processprocbaseinfo.WhereString = process.WhereString;
                 
                processprocbaseinfo.Childs.Add(ActivityName);
                processprocbaseinfo.Childs.Add(ApplicantDisplayName);
                processprocbaseinfo.Childs.Add(ApplicantOrgNodeName);
                processprocbaseinfo.Childs.Add(ApplicantPositionName);
                processprocbaseinfo.Childs.Add(SubmitDate);
                processprocbaseinfo.Childs.Add(SubmitterDisplayName);

                

              //  processbizinfo.ID = 5;
                processbizinfo.ProcessFullName = process.ProcessFullName;
                processbizinfo.ChildType = "Group";
                processbizinfo.ChildID = gdm.GetGroupIdByName("BizInfo");
                processbizinfo.OrderNo = 6;
                processbizinfo.ConnectionString = process.ConnectionString;
                processbizinfo.Mapping = process.Mapping;
                processbizinfo.WhereString = process.WhereString;

                //table                
              //  processdata.ID = 1;
                processdata.ProcessFullName = process.ProcessFullName;
                processdata.ChildType = "Group";
                processdata.ChildID = gdm.GetGroupIdByName("Data");
                processdata.OrderNo = 1;
                processdata.ConnectionString = process.ConnectionString;
                processdata.Mapping = process.Mapping;
                processdata.WhereString = process.WhereString;

                //processdata.Childs.Add(ActivityName);
                //processdata.Childs.Add(CommentDate);
                //processdata.Childs.Add(UserName);

               // processmore.ID = 2;
                processmore.ProcessFullName = process.ProcessFullName;
                processmore.ChildType = "Group";
                processmore.ChildID = gdm.GetGroupIdByName("More");
                processmore.OrderNo = 2;
                processmore.ConnectionString = process.ConnectionString;
                processmore.Mapping = process.Mapping;
                processmore.WhereString = process.WhereString;

                //processmore.Childs.Add(ActivityName);
                //processmore.Childs.Add(CommentDate);
                //processmore.Childs.Add(UserName);
                

             //   processheader.ID = 1;
                processheader.ProcessFullName = process.ProcessFullName;
                processheader.ChildType = "Group";
                processheader.ChildID = gdm.GetGroupIdByName("Header");
                processheader.OrderNo = 1;
                processheader.ConnectionString = process.ConnectionString;
                processheader.Mapping = process.Mapping;
                processheader.WhereString = process.WhereString;
             
                

               // processrow.ID = 2;
                processrow.ProcessFullName = process.ProcessFullName;
                processrow.ChildType = "Group";
                processrow.ChildID = gdm.GetGroupIdByName("Row");
                processrow.OrderNo = 2;
                processrow.ConnectionString = process.ConnectionString;
                processrow.Mapping = process.Mapping;
                processrow.WhereString = process.WhereString;

                processrow.Childs.Add(processdata);
                processrow.Childs.Add(processmore);


              //  processprocloginfo.ID = 6;
                processprocloginfo.ProcessFullName = process.ProcessFullName;
                processprocloginfo.ChildType = "Group";
                processprocloginfo.ChildID = gdm.GetGroupIdByName("ProcLogInfo");
                processprocloginfo.OrderNo = 7;
                processprocloginfo.ConnectionString = process.ConnectionString;
                processprocloginfo.Mapping = process.Mapping;
                processprocloginfo.WhereString = process.WhereString;
                processprocloginfo.Childs.Add(processheader);
                processprocloginfo.Childs.Add(processrow);



               // processtaskinfo.ID = 1;
                processtaskinfo.ProcessFullName = process.ProcessFullName;
                processtaskinfo.ChildType = "Group";
                processtaskinfo.ChildID = gdm.GetGroupIdByName("TaskInfo");
                processtaskinfo.OrderNo = 2;
                processtaskinfo.ConnectionString = process.ConnectionString;
                processtaskinfo.Mapping = process.Mapping;
                processtaskinfo.WhereString = process.WhereString;
                processtaskinfo.Childs.Add(processprocbaseinfo);
                processtaskinfo.Childs.Add(processbizinfo);
                processtaskinfo.Childs.Add(processprocloginfo);

                using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
                {
                    try
                    {
                        ctx.ProcessDefinition.Add(processtask);
                        ctx.ProcessDefinition.Add(processbaseinfo);
                        ctx.ProcessDefinition.Add(processextendinfo);
                        ctx.SaveChanges();
                    }
                    catch (DbEntityValidationException)
                    {
                        result = false;
                    }
                }

                using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
                {
                    try
                    {
                        ctx.ProcessDefinition.Add(processtaskinfo);
                        ctx.ProcessDefinition.Add(processprocbaseinfo);
                        ctx.ProcessDefinition.Add(processbizinfo);
                        ctx.ProcessDefinition.Add(processprocloginfo);
                        ctx.SaveChanges();
                    }
                    catch (DbEntityValidationException)
                    {
                        result = false;
                    }
                }

                using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
                {
                    List<ProcessDefinitionEntity> list = ctx.ProcessDefinition.Where(p => p.ParentID == null).ToList<ProcessDefinitionEntity>();
                    list.ForEach(p => p.ParentID = 0);
                    ctx.SaveChanges();
                }

                ts.Complete();
            }
            return result;
        }

        public ProcessDefinitionEntity AddGroupProcessDefinition(ProcessDefinitionEntity process, GroupDefinitionEntity entity)
        {
            ProcessDefinitionEntity result = null;
            using (TransactionScope ts = new TransactionScope())
            {                
                using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
                {
                    try
                    {
                        ProcessDefinitionEntity processheader = null;
                        ProcessDefinitionEntity processrow = null;
                        ProcessDefinitionEntity processdata = null;
                        ProcessDefinitionEntity processmore = null;
                        ProcessDefinitionEntity processitem = ctx.ProcessDefinition.Where(p => p.ID == process.ID).SingleOrDefault();

                        if (entity.Type.ToLower() == "table")
                        {
                            processdata = new ProcessDefinitionEntity();
                            processdata.ID = 1;
                            processdata.ProcessFullName = processitem.ProcessFullName;
                            processdata.ChildType = "Group";
                            processdata.ChildID = gdm.GetGroupIdByName("Data");
                            processdata.OrderNo = 1;
                            processdata.ConnectionString = process.ConnectionString;
                            processdata.Mapping = process.Mapping;
                            processdata.WhereString = process.WhereString;

                            processmore = new ProcessDefinitionEntity();
                            processmore.ID = 2;
                            processmore.ProcessFullName = processitem.ProcessFullName;
                            processmore.ChildType = "Group";
                            processmore.ChildID = gdm.GetGroupIdByName("More");
                            processmore.OrderNo = 2;
                            processmore.ConnectionString = process.ConnectionString;
                            processmore.Mapping = process.Mapping;
                            processmore.WhereString = process.WhereString;

                            processheader = new ProcessDefinitionEntity();
                            processheader.ID = 1;
                            processheader.ProcessFullName = processitem.ProcessFullName;
                            processheader.ChildType = "Group";
                            processheader.ChildID = gdm.GetGroupIdByName("Header");
                            processheader.OrderNo = 1;
                            processheader.ConnectionString = process.ConnectionString;
                            processheader.Mapping = process.Mapping;
                            processheader.WhereString = process.WhereString;


                            processrow = new ProcessDefinitionEntity();
                            processrow.ID = 2;
                            processrow.ProcessFullName = processitem.ProcessFullName;
                            processrow.ChildType = "Group";
                            processrow.ChildID = gdm.GetGroupIdByName("Row");
                            processrow.OrderNo = 2;
                            processrow.ConnectionString = process.ConnectionString;
                            processrow.Mapping = process.Mapping;
                            processrow.WhereString = process.WhereString;

                            processrow.Childs.Add(processdata);
                            processrow.Childs.Add(processmore);
                        }
                        ProcessDefinitionEntity child = new ProcessDefinitionEntity();
                        child.ProcessFullName = processitem.ProcessFullName;
                        child.ChildType = "Group";
                        child.ChildID = gdm.AddGroupDefinition(entity);
                        child.OrderNo = processitem.Childs.Count + 1;
                        child.ConnectionString = process.ConnectionString;
                        child.Mapping = process.Mapping;
                        child.WhereString = process.WhereString;

                        if (entity.Type.ToLower() == "table" && processheader != null && processrow != null)
                        {
                            child.Childs.Add(processheader);
                            child.Childs.Add(processrow);
                        }
                        processitem.Childs.Add(child);
                        ctx.SaveChanges();

                        result = child;
                    }
                    catch (Exception)
                    {
                        result = null;
                    }
                }
                ts.Complete();
            }
            return result;
        }

        public List<ProcessDefinitionEntity> AddItemProcessDefinition(ProcessDefinitionEntity process, ItemDefinitionEntity entity)
        {
            List<ProcessDefinitionEntity> result = new List<ProcessDefinitionEntity>();
            using (TransactionScope ts = new TransactionScope())
            {                
                using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
                {
                    try
                    {
                        ProcessDefinitionEntity processitem = ctx.ProcessDefinition.Where(p => p.ID == process.ID).SingleOrDefault();

                        ProcessDefinitionEntity child = new ProcessDefinitionEntity();
                        child.ProcessFullName = processitem.ProcessFullName;
                        child.ChildType = "Item";
                        child.ChildID = idm.AddItemDefinition(entity);
                        child.OrderNo = processitem.Childs.Count + 1;
                        child.ConnectionString = process.ConnectionString;
                        child.Mapping = process.Mapping;
                        child.WhereString = process.WhereString;
                        result.Add(child);
                        //保持Header与Data子节点一致
                        if (processitem.ChildID == gdm.GetGroupIdByName("Header"))
                        {
                            int childid = gdm.GetGroupIdByName("Data");
                            ProcessDefinitionEntity processrow = ctx.ProcessDefinition.Where(p => p.ParentID == processitem.ParentID && p.ID != processitem.ID).FirstOrDefault();
                            ProcessDefinitionEntity processdata = ctx.ProcessDefinition.Where(p => p.ParentID == processrow.ID && p.ChildID == childid).FirstOrDefault();

                            ProcessDefinitionEntity datachild = new ProcessDefinitionEntity();
                            datachild.ProcessFullName = processitem.ProcessFullName;
                            datachild.ChildType = "Item";
                            datachild.ChildID = child.ChildID;
                            datachild.OrderNo = processdata.Childs.Count + 1;
                            datachild.ConnectionString = process.ConnectionString;
                            datachild.Mapping = process.Mapping;
                            datachild.WhereString = process.WhereString;

                            processdata.Childs.Add(datachild);
                            result.Add(datachild);
                        }
                        else
                        {
                            result.Add(new ProcessDefinitionEntity());
                        }
                        processitem.Childs.Add(child);
                        ctx.SaveChanges();
                    }
                    catch (Exception)
                    {
                        result.Clear();
                    }
                }
                ts.Complete();
            }
            return result;
        }

        public bool ExistsProcessName(string processname, int id)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                string orignprocessname = string.Empty;
                if (id > 0)
                {
                    orignprocessname = GetProcessName(id);
                    if (orignprocessname == processname)
                    {
                        return false;
                    }
                }
                ProcessDefinitionEntity process = ctx.ProcessDefinition.Where(x => x.ProcessFullName == processname).FirstOrDefault();
                if (process != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 复制节点
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public int CopyProcessItem(int groupid, int itemid, out ProcessDefinitionEntity process)
        {
            int result = 0;
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                ProcessDefinitionEntity newprocessitem = new ProcessDefinitionEntity();
                ItemDefinitionEntity newitem = new ItemDefinitionEntity();
                LabelContentEntity newlabel = new LabelContentEntity();
                try
                {
                    ProcessDefinitionEntity processgroup = ctx.ProcessDefinition.Where(x => x.ID == groupid).SingleOrDefault();
                    ProcessDefinitionEntity processitem = ctx.ProcessDefinition.Where(x => x.ID == itemid).SingleOrDefault();
                    ItemDefinitionEntity item = ctx.ItemDefinition.Where(x => x.ID == processitem.ChildID).SingleOrDefault();
                    LabelContentEntity label = ctx.LabelContent.Where(x => x.LabelID == item.LabelID).SingleOrDefault();

                    foreach (ProcessDefinitionEntity child in processgroup.Childs)
                    {
                        if (child.ChildID == processitem.ChildID)
                        {
                            result = 1;
                        }
                    }
                    //不存在相同的子节点才进行复制节点操作
                    if (result == 0)
                    {
                        newlabel.Content = label.Content;
                        newlabel.LabelID = Guid.NewGuid();
                        newlabel.Language = label.Language;
                        ctx.LabelContent.Add(newlabel);

                        newitem.Editable = item.Editable;
                        newitem.Format = item.Format;
                        newitem.Name = item.Name;
                        newitem.Visible = item.Visible;
                        newitem.LabelID = newlabel.LabelID;
                        ctx.ItemDefinition.Add(newitem);
                        ctx.SaveChanges(); 

                        newprocessitem.ID = 1;
                        newprocessitem.ProcessFullName = processgroup.ProcessFullName;
                        newprocessitem.ChildType = "Item";
                        newprocessitem.ChildID = newitem.ID;
                        newprocessitem.OrderNo = processgroup.Childs.Count + 1;
                        newprocessitem.ConnectionString = processitem.ConnectionString;
                        newprocessitem.Mapping = processitem.Mapping;
                        newprocessitem.WhereString = processitem.WhereString;

                        processgroup.Childs.Add(newprocessitem);
                        ctx.SaveChanges();                          
                    }
                }
                catch (Exception)
                {
                    result = -1;
                }
                process = newprocessitem;
            }
            return result;
        }

        public bool UpdateProcessName(string processname, int id)
        {
            var result = true;

            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    var entity1 = ctx.ProcessDefinition.FirstOrDefault(x => x.ID == id);
                    List<ProcessDefinitionEntity> list = new List<ProcessDefinitionEntity>();
                    CreateProcessDefinitionEntityList(entity1, list);

                    ProcessDefinitionEntity entity2 = ctx.ProcessDefinition.Where(x => x.ProcessFullName == entity1.ProcessFullName && x.ParentID == 0 && x.ID != id).SingleOrDefault();
                    CreateProcessDefinitionEntityList(entity2, list);

                    foreach (ProcessDefinitionEntity item in list)
                    {
                        item.ProcessFullName = processname;
                    }
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;

        }

        public bool DeleteProcessDefinition(int id)
        {
            var result = true;
            string fixedgroup = "Task,TaskInfo,BaseInfo,ExtendInfo,ProcBaseInfo,BizInfo,ProcLogInfo,Header,Row,Data,More";
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    List<ProcessDefinitionEntity> list = new List<ProcessDefinitionEntity>();
                    ProcessDefinitionEntity entity1 = ctx.ProcessDefinition.Where(x => x.ID == id).SingleOrDefault();
                    CreateProcessDefinitionEntityList(entity1, list);
                    if (entity1.ParentID == 0)
                    {
                        ProcessDefinitionEntity entity2 = ctx.ProcessDefinition.Where(x => x.ProcessFullName == entity1.ProcessFullName && x.ParentID == 0 && x.ID != id).SingleOrDefault();
                        CreateProcessDefinitionEntityList(entity2, list);
                    }

                    ProcessDefinitionEntity parententity = ctx.ProcessDefinition.Where(x => x.ID == entity1.ParentID).SingleOrDefault();
                    //如果删除的节点 父节点为header则同步删除data中对应的子节点
                    if (entity1.ParentID>0&& parententity.ChildID == gdm.GetGroupIdByName("Header"))
                    {
                        int childid = gdm.GetGroupIdByName("Data");
                        ProcessDefinitionEntity processrow = ctx.ProcessDefinition.Where(p => p.ParentID == parententity.ParentID && p.ID != parententity.ID).FirstOrDefault();
                        ProcessDefinitionEntity processdata = ctx.ProcessDefinition.Where(p => p.ParentID == processrow.ID && p.ChildID == childid).FirstOrDefault();
                        ProcessDefinitionEntity processdataitem = ctx.ProcessDefinition.Where(p => p.ParentID == processdata.ID && p.ChildID == entity1.ChildID).FirstOrDefault();
                        ctx.ProcessDefinition.Remove(processdataitem);
                    }

                    list.Reverse(); //反转List中的对象顺序
                    foreach (ProcessDefinitionEntity processitem in list)
                    {
                        switch (processitem.ChildType.ToLower())
                        {
                            case "group":
                                GroupDefinitionEntity group = ctx.GroupDefinition.SingleOrDefault(x => x.ID == processitem.ChildID);
                                if (!fixedgroup.Contains(group.Name))
                                {
                                    ctx.GroupDefinition.Remove(group);
                                    if (group.LabelID != null)
                                    {
                                        LabelContentEntity label = ctx.LabelContent.SingleOrDefault(x => x.LabelID == group.LabelID);
                                        ctx.LabelContent.Remove(label);
                                    }
                                }
                                break;
                            case "item":
                                ItemDefinitionEntity item = ctx.ItemDefinition.SingleOrDefault(x => x.ID == processitem.ChildID);
                                if (item.LabelID != null)
                                {
                                    LabelContentEntity label = ctx.LabelContent.SingleOrDefault(x => x.LabelID == item.LabelID);
                                    ctx.LabelContent.Remove(label);
                                }
                                ctx.ItemDefinition.Remove(item);
                                break;
                        }
                        //删除节点
                        ctx.ProcessDefinition.Remove(processitem);
                    }

                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;

        }

        public bool UpdateProcessOrderNo(int sourceid, int destinationid, string position)
        {
            var result = true;
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    int index = 0, sourceorder = 0, destinationorder = 0;
                    ProcessDefinitionEntity source = ctx.ProcessDefinition.Where(x => x.ID == sourceid).SingleOrDefault();
                    ProcessDefinitionEntity destination = ctx.ProcessDefinition.Where(x => x.ID == destinationid).SingleOrDefault();
                    sourceorder = source.OrderNo;
                    destinationorder = destination.OrderNo;
                    switch (position)
                    {
                        case "before":
                            if (source.OrderNo > destination.OrderNo)
                            {
                                source.OrderNo = destination.OrderNo;
                                index = destination.OrderNo + 1;
                                List<ProcessDefinitionEntity> processbeforelist = ctx.ProcessDefinition.Where(x => x.ParentID == source.ParentID && x.OrderNo >= destination.OrderNo && x.ID != sourceid).ToList<ProcessDefinitionEntity>();
                                processbeforelist = processbeforelist.OrderBy(x => x.OrderNo).ToList<ProcessDefinitionEntity>();
                                foreach (ProcessDefinitionEntity item in processbeforelist)
                                {
                                    item.OrderNo = (index++);
                                }
                            }
                            else
                            {
                                source.OrderNo = destination.OrderNo - 1;
                                index = 1;
                                List<ProcessDefinitionEntity> processbeforelist = ctx.ProcessDefinition.Where(x => x.ParentID == source.ParentID && x.OrderNo < destination.OrderNo && x.ID != sourceid).ToList<ProcessDefinitionEntity>();
                                processbeforelist = processbeforelist.OrderBy(x => x.OrderNo).ToList<ProcessDefinitionEntity>();
                                foreach (ProcessDefinitionEntity item in processbeforelist)
                                {
                                    item.OrderNo = (index++);
                                }
                            }
                            break;
                        case "after":
                            if (source.OrderNo > destination.OrderNo)
                            {
                                source.OrderNo = destination.OrderNo + 1;
                                index = source.OrderNo + 1;
                                List<ProcessDefinitionEntity> processafterlist = ctx.ProcessDefinition.Where(x => x.ParentID == source.ParentID && x.OrderNo > destination.OrderNo && x.ID != sourceid).ToList<ProcessDefinitionEntity>();
                                processafterlist = processafterlist.OrderBy(x => x.OrderNo).ToList<ProcessDefinitionEntity>();
                                foreach (ProcessDefinitionEntity item in processafterlist)
                                {
                                    item.OrderNo = (index++);
                                }
                            }
                            else
                            {
                                source.OrderNo = destination.OrderNo;
                                index = 1;
                                List<ProcessDefinitionEntity> processafterlist = ctx.ProcessDefinition.Where(x => x.ParentID == source.ParentID && x.OrderNo <= destination.OrderNo && x.ID != sourceid).ToList<ProcessDefinitionEntity>();
                                processafterlist = processafterlist.OrderBy(x => x.OrderNo).ToList<ProcessDefinitionEntity>();
                                foreach (ProcessDefinitionEntity item in processafterlist)
                                {
                                    item.OrderNo = (index++);
                                }
                            }
                            break;
                    }
                    //除了移动到原位置的情况才保存
                    if ((position == "before" && sourceorder == destinationorder - 1) || (position == "after" && sourceorder == destinationorder + 1))
                    {
                        result = true;
                    }
                    else
                    {
                        ctx.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        //递归所有子对象并将其加进泛型List
        void CreateProcessDefinitionEntityList(ProcessDefinitionEntity entity, List<ProcessDefinitionEntity> list)
        {
            list.Add(entity);

            if (entity.Childs.Count > 0)
            {
                foreach (ProcessDefinitionEntity element in entity.Childs)
                {
                    CreateProcessDefinitionEntityList(element, list);
                }
            }
        }


    }
}
