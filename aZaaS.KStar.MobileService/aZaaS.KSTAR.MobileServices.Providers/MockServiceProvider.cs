using aZaaS.KSTAR.MobileServices.Helpers;
using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;

namespace aZaaS.KSTAR.MobileServices.Providers
{
    class MockServiceProvider : BaseServiceProvider
    {
        string[] users = new string[] { "Collin Huang", "Xin Gao", "Evan Chen", "Mike", "Johnny Fang", "Pattarawat Teparagul", "Dave Marcus", "Michael Morse", "Justin Pedersen", "Jeff Belnap" };
        public MockServiceProvider()
        {
        }

        public override LoginResult Login(LoginInfo info)
        {
            LoginResult result = new LoginResult();
            result.Result = "S";
            result.Message = "登录成功";
            result.Content = new ResultContent()
            {
                Mask = ApiHelper.Encode(info.UserName, ApiHelper.DefaultMask),
                UserInfo = new UserInfo() { UserName = info.UserName }
            };
            result.Content.UserToken = ApiHelper.Encode(info.Language, result.Content.Mask);

            return result;
        }

        public override TaskInfo GetTaskInfo(string userToken, string mask, string sn,string destination)
        {
            ProcBaseInfo baseInfo = new ProcBaseInfo()
            {
                Group = new Group()
                {
                    Type = "Single",
                    Label = "流程基础信息",
                    Items = new List<Item>() { 
                            new Item(){ Name="ProcessName",Label="流程名称", Value="出差报销流程"},
                            new Item(){ Name="SN",Label="SN", Visible=false,Value="1_1"},
                            new Item(){Name="Folio",Label="流程主题",Value="Proc_" + DateTime.Now.ToString("yyyyMMdd") + "_1"},
                            new Item(){Name="Requestor",Label="申请人",Value="Collin Huang"},
                            new Item(){Name="ActivityName",Label="环节名称",Value="部门经理审批"}
                        }
                }
            };
            ProcLogInfo logInfo = new ProcLogInfo()
            {
                Group = new Group()
                {
                    Type = "Table",
                    Label = "流程审批历史记录",
                    Header = new Header()
                    {
                        Items = new List<Item>()
                        {
                            new Item(){Name = "ID", Label = "序号"},
                            new Item(){Name = "ActivityName", Label = "环节名称"},
                            new Item(){Name = "OperatorName", Label = "审批人"},
                            new Item(){Name = "Action", Label = "操作"}
                        }
                    },
                    Rows = new List<Row>()
                    {
                        new Row()
                        {
                         Data=new Data(){
                              Items=new List<Item>()
                                {
                                    new Item(){Name = "ID",Value="1"},
                                    new Item(){Name = "ActivityName", Value="填写表单"},
                                    new Item(){Name = "OperatorName",Value="Collin Huang"},
                                    new Item(){Name = "Action", Value="Submit"}
                                }
                         },
                         More=new More(){
                             Items=new List<Item>()
                                {
                                    new Item(){Name = "StartDate", Label = "任务开始时间",Value="2014-03-16"},
                                    new Item(){Name = "EndDate", Label = "任务结束时间",Value="2014-03-16"},
                                    new Item(){Name = "Opinion", Label = "审批意见",Value="申请提交！"}
                                }
                             }
                        },

                        new Row()
                        {
                         Data=new Data(){
                              Items=new List<Item>()
                                {
                                    new Item(){Name = "ID",Value="2"},
                                    new Item(){Name = "ActivityName",Value="部门经理审批"},
                                    new Item(){Name = "OperatorName",Value="Michael"},
                                    new Item(){Name = "Action",Value="Approve"},
                                }
                         },
                         More=new More(){
                             Items=new List<Item>()
                                {
                                    new Item(){Name = "StartDate", Label = "任务开始时间",Value="2014-03-16"},
                                    new Item(){Name = "EndDate", Label = "任务结束时间",Value="2014-03-16"},
                                    new Item(){Name = "Opinion", Label = "审批意见",Value="同意，请速办！"}
                                }
                             }
                        }
                    }
                }
            };
            Actions actions = new Actions()
            {
                Items = new List<Item>() 
                { 
                     new Item(){Name = "Approve"},
                     new Item(){Name = "Decline"},
                     new Item(){Name = "Rework"}
                }
            };
            BizInfo bizInfo = new BizInfo()
            {
                Groups = new List<Group>() { 
                new Group()
                {
                     Type="Single",
                     Label="出差信息",
                     Items=new List<Item>(){
                     new Item(){Name="StartDate", Label="开始时间", Value="2014-03-15"},
                     new Item(){Name="EndDate", Label="结束时间", Value="2014-03-16"},
                     new Item(){Name="Destination", Label="目的地",Value="北京"}
                     }
                },
                new Group()
                {
                 Type="Single", 
                 Label="财务信息",
                 Items=new List<Item>(){
                     new Item(){Name="CostCenter", Label="成本中心",Value="研发部"},
                     new Item(){Name="TotalAmount", Label="费用合计",Value="2500.00"}
                     }
                },
                new Group()
                {
                 Type = "Table",
                 Label="费用明细",
                 Header=new Header(){
                 Items=new List<Item>(){
                 new Item(){Name="ID",Label="序号"},
                 new Item(){Name="CostType",Label="费用类型"},
                 new Item(){Name="Amount",Label="费用"}
                 }
                },
                 Rows=new List<Row>(){
                 new Row(){
                     Data=new Data(){
                  Items=new List<Item>(){
                  new Item(){Name="ID",Value="1"},
                  new Item(){Name="CostType",Value="交通费"},
                  new Item(){Name="Amount",Value="2000.00"}
                  }
                 },
                  More=new More(){
                   Items=new List<Item>(){
                   new Item(){Name="StartDate",Label="开始时间",Value="2014-03-15"},
                   new Item(){Name="EndDate", Label="结束时间",Value="2014-03-16"},
                   new Item(){Name="Remark",Label="说明",Value="深圳-北京 机票"}
                   }
                  }
                 },
                 new Row(){
                     Data=new Data(){
                      Items=new List<Item>(){
                      new Item(){Name="ID",Value="2"},
                      new Item(){Name="CostType",Value="食宿费"},
                      new Item(){Name="Amount",Value="500.00"}
                      }
                    },
                  More=new More(){
                   Items=new List<Item>(){
                   new Item(){Name="StartDate",Label="开始时间",Value="2014-03-15"},
                   new Item(){Name="EndDate", Label="结束时间",Value="2014-03-16"},
                   new Item(){Name="Remark",Label="说明", Value="香格里拉酒店住宿，香格里拉酒店住宿，香格里拉酒店住宿香格里拉酒店住宿香格里拉酒店住宿，香格里拉酒店住宿香格里拉酒店住宿，香格里拉酒店住宿香格里拉酒店住宿"}
                   }
                  }                 
                 },
                 new Row(){
                     Data=new Data(){
                  Items=new List<Item>(){
                  new Item(){Name="ID",Value="3"},
                  new Item(){Name="CostType",Value="交通费"},
                  new Item(){Name="Amount",Value="2000.00"}
                  }
                 },
                  More=new More(){
                   Items=new List<Item>(){
                   new Item(){Name="StartDate",Label="开始时间",Value="2014-03-15"},
                   new Item(){Name="EndDate", Label="结束时间",Value="2014-03-16"},
                   new Item(){Name="Remark",Label="说明",Value="深圳-北京 机票 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票 "}
                   }
                  }
                 },
                 new Row(){
                     Data=new Data(){
                  Items=new List<Item>(){
                  new Item(){Name="ID",Value="4"},
                  new Item(){Name="CostType",Value="交通费"},
                  new Item(){Name="Amount",Value="2000.00"}
                  }
                 },
                  More=new More(){
                   Items=new List<Item>(){
                   new Item(){Name="StartDate",Label="开始时间",Value="2014-03-15"},
                   new Item(){Name="EndDate", Label="结束时间",Value="2014-03-16"},
                   new Item(){Name="Remark",Label="说明",Value="深圳-北京 机票 深圳-北京， 机票 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票， 深圳-北京 机票 深圳-北京 机票 深圳-北京 机票， 深圳-北京 机票 深圳-北京 机票 "}
                   }
                  }
                 },
                 new Row(){
                     Data=new Data(){
                  Items=new List<Item>(){
                  new Item(){Name="ID",Value="5"},
                  new Item(){Name="CostType",Value="交通费"},
                  new Item(){Name="Amount",Value="2000.00"}
                  }
                 },
                  More=new More(){
                   Items=new List<Item>(){
                   new Item(){Name="StartDate",Label="开始时间",Value="2014-03-15"},
                   new Item(){Name="EndDate", Label="结束时间",Value="2014-03-16"},
                   new Item(){Name="Remark",Label="说明",Value="深圳-北京 机票"}
                   }
                  }
                 },
                 new Row(){
                     Data=new Data(){
                  Items=new List<Item>(){
                  new Item(){Name="ID",Value="6"},
                  new Item(){Name="CostType",Value="交通费"},
                  new Item(){Name="Amount",Value="2000.00"}
                  }
                 },
                  More=new More(){
                   Items=new List<Item>(){
                   new Item(){Name="StartDate",Label="开始时间",Value="2014-03-15"},
                   new Item(){Name="EndDate", Label="结束时间",Value="2014-03-16"},
                   new Item(){Name="Remark",Label="说明",Value="深圳-北京 机票"}
                   }
                  }
                 },
                 }
                }
                }
            };
            TaskInfo info = new TaskInfo()
            {
                Actions = actions,
                BizInfo = bizInfo,
                ProcLogInfo = logInfo,
                ProcBaseInfo = baseInfo
            };
            return info;
        }

        public override List<Task> GetTaskList(string userToken, string mask, Filter filter, Paging paging, Sorting sorting)
        {
            List<Task> list = new List<Task>();

            DateTime now = DateTime.Now;
            int count = 2;
            Task task;
            for (int i = 1; i <= count; i++)
            {
                task = new Task()
                {
                    BaseInfo = new BaseInfo()
                    {
                        Items = new List<Item>()
                        {
                            new Item(){Name="ActvityName", Value="Default Activity " + i.ToString()},
                            new Item(){Name="Folio", Value="Proc_" + now.ToString("yyyyMMdd") + "_" + i.ToString()},
                            new Item(){Name="Originator", Value=users[i-1]},
                            new Item(){Name="ProcessName", Value="Test Process " + i.ToString()},
                            new Item(){Name="ProcInstID", Value=i.ToString()},
                            new Item(){Name="SN", Value=i.ToString() + "_" + i.ToString()},
                            new Item(){Name="StartDate", Value=now.AddMinutes(-i).ToString("yyyy-MM-dd'T'HH:mm:ss")},
                            new Item(){Name="Summary", Value="Summary " + i.ToString()}
                        }
                    },
                    ExtendInfo = new ExtendInfo() { Items = new List<Item>() { } }
                };
                list.Add(task);

                task = new Task()
                {
                    BaseInfo = new BaseInfo()
                    {
                        Items = new List<Item>()
                        {
                            new Item(){Name="ActvityName", Value="Default Activity 1" + i.ToString()},
                            new Item(){Name="Folio", Value="Proc_" + now.AddDays(-1).ToString("yyyyMMdd") + "_" + i.ToString()},
                            new Item(){Name="Originator", Value=users[i-1]},
                            new Item(){Name="ProcessName", Value="Test Process " + i.ToString()},
                            new Item(){Name="ProcInstID", Value=i.ToString()},
                            new Item(){Name="SN", Value="1"+i.ToString() + "_" + i.ToString()},
                            new Item(){Name="StartDate", Value=now.AddDays(-1).AddMinutes(-i).ToString("yyyy-MM-dd'T'HH:mm:ss")},
                            new Item(){Name="Summary", Value="Summary " + i.ToString()}
                        }
                    },
                    ExtendInfo = new ExtendInfo() { Items = new List<Item>() { } }
                };
                list.Add(task);

                task = new Task()
                {
                    BaseInfo = new BaseInfo()
                    {
                        Items = new List<Item>()
                        {
                            new Item(){Name="ActvityName", Value="Default Activity 2" + i.ToString()},
                            new Item(){Name="Folio", Value="Proc_" + now.AddDays(-5).ToString("yyyyMMdd") + "_" + i.ToString()},
                            new Item(){Name="Originator", Value=users[i-1]},
                            new Item(){Name="ProcessName", Value="Test Process " + i.ToString()},
                            new Item(){Name="ProcInstID", Value=i.ToString()},
                            new Item(){Name="SN", Value="2"+i.ToString() + "_" + i.ToString()},
                            new Item(){Name="StartDate", Value=now.AddDays(-5).AddMinutes(-i).ToString("yyyy-MM-dd'T'HH:mm:ss")},
                            new Item(){Name="Summary", Value="Summary " + i.ToString()}
                        }
                    },
                    
                    ExtendInfo = new ExtendInfo() { Items = new List<Item>() { } }
                };
                list.Add(task);

                task = new Task()
                {
                    BaseInfo = new BaseInfo()
                    {
                        Items = new List<Item>()
                        {
                            new Item(){Name="ActvityName", Value="Default Activity 3" + i.ToString()},
                            new Item(){Name="Folio", Value="Proc_" + now.AddDays(-12).ToString("yyyyMMdd") + "_" + i.ToString()},
                            new Item(){Name="Originator", Value=users[i-1]},
                            new Item(){Name="ProcessName", Value="Test Process " + i.ToString()},
                            new Item(){Name="ProcInstID", Value=i.ToString()},
                            new Item(){Name="SN", Value="3"+i.ToString() + "_" + i.ToString()},
                            new Item(){Name="StartDate", Value=now.AddDays(-5).AddMinutes(-i)},
                            new Item(){Name="Summary", Value="Summary " + i.ToString()}
                        }
                    },

                    ExtendInfo = new ExtendInfo() { Items = new List<Item>() { } }

                };
                list.Add(task);
            }
            return list;
        }

        public override ExecuteTaskResult ExecuteTask(string userToken, string mask, string sn, string action, string opinion,string destination)
        {
            string userName;
            string language;
            ApiHelper.ParseUser(userToken, mask, out userName, out language);

            ExecuteTaskResult result = new ExecuteTaskResult();
            result.Result = "S";
            result.Message = "提交成功";
            return result;
        }

        public override void WriteServiceLog(LogEntity log)
        {
            //base.WriteServiceLog(log);
        }
    }
}