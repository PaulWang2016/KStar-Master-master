using aZaaS.KStar.OAEhrInteraction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaas.Kstar.DAL
{
    public class LeaveRegulations
    {
        private static IOAEhr _ioaEhr;

        public LeaveRegulations()
        {
            _ioaEhr = new OAEhrDAL();
        }

        #region 枚举类型

        /// <summary>
        /// 请假类型
        /// </summary>
        public enum LeaveType
        {
            /// <summary>
            /// 年假
            /// </summary>
            Annual,

            /// <summary>
            /// 事假
            /// </summary>
            Compassionate,

            /// <summary>
            /// 病假
            /// </summary>
            Sick,

            /// <summary>
            /// 婚假
            /// </summary>
            Marriage,

            /// <summary>
            /// 丧假
            /// </summary>
            Bereavement,

            /// <summary>
            /// 看护假
            /// </summary>
            NurseFalse,

            /// <summary>
            /// 基本产假
            /// </summary>
            BasicMaternity,

            /// <summary>
            /// 难产假
            /// </summary>
            Dystocia,

            /// <summary>
            /// 多胞胎假
            /// </summary>
            MultipleBirths,

            /// <summary>
            /// 产前检查
            /// </summary>
            PrenatalExamination,

            /// <summary>
            /// 计划生育假
            /// </summary>
            FamilyPlanning,

            /// <summary>
            /// 哺乳假
            /// </summary>
            Lactation,

            /// <summary>
            /// 调休
            /// </summary>
            Adjust,

            /// <summary>
            /// 特殊假
            /// </summary>
            Special,

            /// <summary>
            /// 外出假
            /// </summary>
            OutOfOffice,

            /// <summary>
            /// 出差
            /// </summary>
            BusinessTrip
        }

        /// <summary>
        /// 请假时间类型
        /// </summary>
        public enum LeaveDayType
        {
            /// <summary>
            /// 当天开始
            /// </summary>
            Start,

            /// <summary>
            /// 当天结束
            /// </summary>
            End
        }

        /// <summary>
        /// 请假时长单位
        /// </summary>
        public enum LeaveUnit
        {
            /// <summary>
            /// 天
            /// </summary>
            Day,

            /// <summary>
            /// 小时
            /// </summary>
            Hour,

            /// <summary>
            /// 默认
            /// </summary>
            Default
        }

        /// <summary>
        /// 日期类型
        /// </summary>
        public enum DateType
        {
            /// <summary>
            /// 年
            /// </summary>
            Year,

            /// <summary>
            /// 月
            /// </summary>
            Month,

            /// <summary>
            /// 日
            /// </summary>
            Day
        }

        #endregion

        #region Public Method
  
        /// <summary>
        /// 获取总共可休天数
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveType"></param>
        /// <returns></returns>
        public double GetHaveRestDays(string empNo, LeaveType leaveType, DateTime leaveStartDate)
        {
            double restDays = 0; //可休天数
            decimal workYears;   //工龄
            int age;             //年龄
            string sex;          //性别

            //员工基本信息
            var dtEmpInfo = GetEmpInfo(empNo);

            if (dtEmpInfo == null || dtEmpInfo.Rows.Count == 0)
            {
                return 0;
            }

            DataRow drEmpInfo = dtEmpInfo.Rows[0];

            Decimal.TryParse(drEmpInfo["TotalServiceYear"].ToString(), out workYears);
            Int32.TryParse(drEmpInfo["Age"].ToString(), out age);
            sex = drEmpInfo["Sex"].ToString();

            switch (leaveType)
            {
                case LeaveType.Annual:
                    //年假
                    restDays = CalcAnualDays(drEmpInfo, leaveStartDate);
                    break;
                case LeaveType.Marriage:
                    //婚假
                    restDays = CalcMarriageDays(age, sex);
                    break;
                case LeaveType.NurseFalse:
                    //看护假
                    restDays = CalcNurseFalseDays(sex);
                    break;
                case LeaveType.BasicMaternity:
                    //基本产假
                    restDays = CalcBasicMaternityDays(age, sex);
                    break;
                case LeaveType.Dystocia:
                    //难产假
                    restDays = CalcDystociaDays(age, sex);
                    break;
                case LeaveType.MultipleBirths:
                    //多胞胎假
                    restDays = CalcMultipleBirthsDays(age, sex, 2);
                    break;
                case LeaveType.Bereavement:
                    restDays = 3;
                    break;
            }

            return restDays;
        }

        /// <summary>
        /// 获取已休假时间
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveType"></param>
        /// <returns></returns>
        public double GetAlreadyRestTimeByYear(string empNo, LeaveType leaveType, LeaveUnit leaveUnit, DateTime leaveStartDate)
        {
            double restTime = CalcHaveRestTimeByYear(empNo, leaveType, leaveUnit, leaveStartDate);

            return restTime;
        }  
        /// <summary>
        /// 获取请假类型信息
        /// </summary>
        /// <param name="leaveType">请假类型</param>
        /// <returns></returns>
        public EhrLeaveTypeEntity GetLeaveTypeInfo(string leaveType)
        {
            var dt = GetLeaveType(leaveType);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            var dr = dt.Rows[0];
            int maxLeaveCount;
            double minLeaveTime;
            bool isAudit;
            bool isContinuous;
            bool isOnlyRestOneTime;

            Int32.TryParse(dr["MaxLeaveCount"].ToString(), out maxLeaveCount);
            Double.TryParse(dr["MinLeaveTime"].ToString(), out minLeaveTime);
            Boolean.TryParse(dr["IsAudit"].ToString(), out isAudit);
            Boolean.TryParse(dr["IsContinuous"].ToString(), out isContinuous);
            Boolean.TryParse(dr["IsOnlyRestOneTime"].ToString(), out isOnlyRestOneTime);

            var entity = new EhrLeaveTypeEntity()
            {
                LeaveType = dr["Name"].ToString(),
                IsAudit = isAudit,
                IsContinuous = isContinuous,
                IsOnlyRestOneTime = isOnlyRestOneTime,
                MaxLeaveCount = maxLeaveCount,
                MinLeaveTime = minLeaveTime,
                Caption = dr["Caption"].ToString()
            };

            return entity;
        }
 
        /// <summary>
        /// 是否已经休过一次假
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveTypeName"></param>
        /// <returns></returns>
        public bool IsAlreadyRestOneTime(string empNo, string leaveTypeName)
        {
            var isRest = false;

            var sql = @"SELECT count(*) total
                          FROM [NeoWay_BaseData_Leave]
                          where userid=@EmpNo
                          and LeaveType=@LeaveType
                          and status>=0";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo),
                new SqlParameter("@LeaveType",leaveTypeName)
            };

            var dt = SqlHelper.ExecuteTable(sql, paras);

            if (dt != null && dt.Rows.Count > 0)
            {
                isRest = Convert.ToInt32(dt.Rows[0]["total"]) > 0;
            }

            return isRest;
        }

       

        /// <summary>
        /// 获取请假天数
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveType"></param>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public double GetLeaveDays(string empNo, LeaveType leaveType, DateTime begDate, DateTime endDate, bool mustFullDay, out string errMsg)
        {
            double leaveDays = 0;
            errMsg = string.Empty;

            var isContinuous = GetLeaveIsContinuous(leaveType);  //假期是否连续计算
            var dtEhrRowInfo = GetEhrRowInfo(empNo);             //获取员工排班信息
            var shiftList = GetEhrShiftInfo();                   //获取班次作息时间信息

            for (var date = begDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var dayOfWeek = date.DayOfWeek;

                //获取员工排班作息信息
                var shiftInfo = GetEmpScheduing(dtEhrRowInfo, shiftList, date);

                if (shiftInfo == null)
                {
                    errMsg = "找不到员工对应请假日期的排班信息,无法进行请假申请！";
                    return 0;
                }

                //不连续计算
                if (!isContinuous)
                {
                    ////周末不算请假
                    //if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                    //{
                    //    continue;
                    //}

                    //非工作天不算请假
                    if (shiftInfo.WorkHour == null || shiftInfo.WorkHour.Value == 0)
                    {
                        continue;
                    }

                    //法定假日不算请假
                    if (IsHoliday(date))
                    {
                        continue;
                    }
                }

                //默认请假时长为一天
                double currLeaveDay = 1;
                var workStartHour = shiftInfo.Start.Value.Hour;
                var workStartMinute = shiftInfo.Start.Value.Minute;
                var workEndHour = shiftInfo.End.Value.Hour;
                var workEndMinute = shiftInfo.End.Value.Minute;

                //跨天班
                if (workStartHour * 60 + workStartMinute >= workEndHour * 60 + workEndMinute)
                {
                    workEndHour += 24;
                }

                if (date == begDate.Date && date != endDate.Date)       //请假开始为当天
                {
                    if (begDate.Hour == workStartHour && begDate.Minute == workStartMinute) //请假开始时间为上班时间
                    {
                        currLeaveDay = 1;
                    }
                    else if (!mustFullDay && shiftInfo.RestEnd == null
                        && begDate.Hour * 60 + begDate.Minute == ((workStartHour + workEndHour) * 60 + workStartMinute + workEndMinute) / 2)
                    {
                        //请假半天
                        currLeaveDay = 0.5;
                    }
                    else if (!mustFullDay && shiftInfo.RestEnd != null
                        && begDate.Hour == shiftInfo.RestEnd.Value.Hour && begDate.Minute == shiftInfo.RestEnd.Value.Minute)
                    {
                        //请假半天
                        currLeaveDay = 0.5;
                    }
                    else
                    {
                        if (mustFullDay)
                        {
                            errMsg = "请假最小单位为1天,请以上午上班开始时间作为请假开始时间!";
                        }
                        else
                        {
                            errMsg = "请假最小单位为0.5天,请以上午上班开始时间或下午上班时间作为请假开始时间!";
                        }
                        return 0;
                    }
                }
                else if (date != begDate.Date && date == endDate.Date)  //请假结束为当天
                {
                    if ((endDate.Hour == workEndHour || endDate.Hour + 24 == workEndHour) && endDate.Minute == workEndMinute) //请假结束时间为下班时间
                    {
                        currLeaveDay = 1;
                    }
                    else if (!mustFullDay && shiftInfo.RestStart == null
                        && endDate.Hour * 60 + endDate.Minute == ((workStartHour + workEndHour) * 60 + workStartMinute + workEndMinute) / 2)
                    {
                        //请假半天
                        currLeaveDay = 0.5;
                    }
                    else if (!mustFullDay && shiftInfo.RestStart != null
                        && endDate.Hour == shiftInfo.RestStart.Value.Hour && endDate.Minute == shiftInfo.RestStart.Value.Minute)
                    {
                        //请假半天
                        currLeaveDay = 0.5;
                    }
                    else
                    {
                        if (mustFullDay)
                        {
                            errMsg = "请假最小单位为1天,请以上午上班时间为请假开始时间，下午下班时间作为请假结束时间!";
                        }
                        else
                        {
                            errMsg = "请假最小单位为0.5天,请以上午下班结束时间或下午下班时间作为请假结束时间!";
                        }
                        return 0;
                    }
                }
                else if (date == begDate.Date && date == endDate.Date)  //请假发生在当天
                {
                    //请假时间为上下班时间
                    if ((begDate.Hour == workStartHour && begDate.Minute == workStartMinute)
                        && ((endDate.Hour == workEndHour || endDate.Hour + 24 == workEndHour) && endDate.Minute == workEndMinute))
                    {
                        currLeaveDay = 1;
                    }
                    else if (!mustFullDay && shiftInfo.RestEnd == null
                        && begDate.Hour * 60 + begDate.Minute == ((workStartHour + workEndHour) * 60 + workStartMinute + workEndMinute) / 2
                        && ((endDate.Hour == workEndHour || endDate.Hour + 24 == workEndHour) && endDate.Minute == workEndMinute))
                    {
                        //请假半天
                        currLeaveDay = 0.5;
                    }
                    else if (!mustFullDay && shiftInfo.RestStart == null
                        && endDate.Hour * 60 + endDate.Minute == ((workStartHour + workEndHour) * 60 + workStartMinute + workEndMinute) / 2
                        && (begDate.Hour == workStartHour && begDate.Minute == workStartMinute))
                    {
                        //请假半天
                        currLeaveDay = 0.5;
                    }
                    else if (!mustFullDay && shiftInfo.RestEnd != null
                        && begDate.Hour == shiftInfo.RestEnd.Value.Hour && begDate.Minute == shiftInfo.RestEnd.Value.Minute
                        && ((endDate.Hour == workEndHour || endDate.Hour + 24 == workEndHour) && endDate.Minute == workEndMinute))
                    {
                        //请假半天
                        currLeaveDay = 0.5;
                    }

                    else if (!mustFullDay && shiftInfo.RestStart != null
                        && endDate.Hour == shiftInfo.RestStart.Value.Hour && begDate.Minute == shiftInfo.RestStart.Value.Minute
                        && (begDate.Hour == workStartHour && begDate.Minute == workStartMinute))
                    {
                        //请假半天
                        currLeaveDay = 0.5;
                    }
                    else
                    {
                        if (mustFullDay)
                        {
                            errMsg = "请假最小单位为1天,请以上午上班开始时间和下午下班时间作为请假的开始结束时间!";
                        }
                        else
                        {
                            errMsg = "请假最小单位为0.5天,请以上下午上班开始时间和上下午下班时间作为请假的开始结束时间!";
                        }
                        return 0;
                    }
                }

                leaveDays += currLeaveDay;
            }

            return leaveDays;
        }

      

        #endregion

        #region Private Method

        /// <summary>
        /// 获取当年已休假时间
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveType"></param>
        /// <returns></returns>
        private double CalcHaveRestTimeByYear(string empNo, LeaveType leaveType, LeaveUnit leaveUnit, DateTime leaveStartDate)
        {
            var restTime = CalcHaveRestTime(empNo, leaveType, leaveUnit, DateType.Year, leaveStartDate);

            return restTime;
        }
 
        /// <summary>
        /// 获取已休假时间
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveType"></param>
        /// <param name="leaveUnit"></param>
        /// <param name="dateType"></param>
        /// <returns></returns>
        private double CalcHaveRestTime(string empNo, LeaveType leaveType, LeaveUnit leaveUnit, DateType dateType, DateTime leaveStartDate)
        {
            double leaveTime = 0;
            double currLeaveTime = 0;
            string message = string.Empty;
            DateTime begDate;
            DateTime endDate;

            var sql = string.Format(@"SELECT [LeaveNo]
                              ,[UserId]
                              ,[UserName]
                              ,[DeptName]
                              ,[OfficeName]
                              ,[LeaveType]
                              ,(case [Status]
	                           when 0 then [StartTime]
	                           when 1 then [StartTime]
	                           when 2 then [RealStartTime] end) [StartTime]
	                          ,(case [Status]
	                           when 0 then [EndTime]
	                           when 1 then [EndTime]
	                           when 2 then [RealEndTime] end) [EndTime]
                              ,[LeaveReason]
                              ,[Status]
                          FROM [NeoWay_BaseData_Leave]
                      WHERE UserId=@EmpNo
                      AND LeaveType=@Type
                      AND Status>=0
                      AND (datediff({0},(case [Status]
	                           when 0 then [StartTime]
	                           when 1 then [StartTime]
	                           when 2 then [RealStartTime] end) ,@BegDate)=0
                            or
							datediff({0},(case [Status]
	                           when 0 then [EndTime]
	                           when 1 then [EndTime]
	                           when 2 then [RealEndTime] end) ,@BegDate)=0)", dateType.ToString());

            var leaveTypeName = GetLeaveTypeName(leaveType);

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BegDate",leaveStartDate),
                new SqlParameter("@EmpNo",empNo),
                new SqlParameter("@Type",leaveTypeName)
            };

            var dt = SqlHelper.ExecuteTable(sql, parameters);

            if (dt == null || dt.Rows.Count == 0)
            {
                return 0;
            }

            var dtEhrRowInfo = GetEhrRowInfo(empNo);             //获取员工排班信息
            var shiftList = GetEhrShiftInfo();                   //获取班次作息时间信息

            foreach (DataRow dr in dt.Rows)
            {
                DateTime.TryParse(dr["StartTime"].ToString(), out begDate);
                DateTime.TryParse(dr["EndTime"].ToString(), out endDate);
                var status = dr["Status"].ToString();

                GetActualBegEndDate(dateType, dtEhrRowInfo, shiftList, leaveStartDate, ref begDate, ref endDate);

                if (leaveUnit == LeaveUnit.Day)
                {
                    currLeaveTime = GetLeaveDays(empNo, leaveType, begDate, endDate, false, out message);
                }
                else if (leaveUnit == LeaveUnit.Hour)
                {
                    currLeaveTime = CalcLeaveHours(empNo, leaveType, begDate, endDate);
                }

                //有销假过的假期不用处理错误
                if (!string.IsNullOrEmpty(message) && status != "2")
                {
                    throw new ArgumentException(message);
                }

                leaveTime += currLeaveTime;
            }

            return leaveTime;
        }

        /// <summary>
        /// 获取请假小时数
        /// </summary>
        /// <param name="empNo">工号</param>
        /// <param name="leaveType">请假类型</param>
        /// <param name="begDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>请假小时数</returns>
        private double CalcLeaveHours(string empNo, LeaveType leaveType, DateTime begDate, DateTime endDate)
        {
            var leaveHours = 0.0;     //请假天数

            //假期是否连续计算
            var isContinuous = GetLeaveIsContinuous(leaveType);

            //获取本次请假小时数
            leaveHours = CalcLeaveHours(empNo, begDate, endDate, isContinuous);

            return leaveHours;
        }

        /// <summary>
        /// 计算请假时长
        /// </summary>
        /// <param name="empNo">工号</param>
        /// <param name="begDate">请假开始时间</param>
        /// <param name="endDate">请假结束时间</param>
        /// <param name="isContinuous">是否连续计算</param>
        /// <returns></returns>
        public double CalcLeaveHours(string empNo, DateTime begDate, DateTime endDate, bool isContinuous)
        {
            var leaveHours = 0.0;                    //请假时长
            var dtEhrRowInfo = GetEhrRowInfo(empNo); //获取员工排班信息
            var shiftList = GetEhrShiftInfo();       //获取班次作息时间信息

            if (dtEhrRowInfo == null || dtEhrRowInfo.Rows.Count == 0)
            {
                throw new ArgumentException("找不到员工排班信息,无法计算请假时间!");
            }

            for (var date = begDate; date <= endDate; date = date.Date.AddDays(1))
            {
                var dayOfWeek = date.DayOfWeek;

                //获取员工排班作息信息
                var shiftInfo = GetEmpScheduing(dtEhrRowInfo, shiftList, date);

                if (shiftInfo == null)
                {
                    throw new ArgumentException("找不到员工对应请假日期的排班信息,无法进行请假申请！");
                }

                //不连续计算
                if (!isContinuous)
                {
                    ////周末不算请假
                    //if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                    //{
                    //    continue;
                    //}

                    //非工作天不算请假
                    if (shiftInfo.WorkHour == null || shiftInfo.WorkHour.Value == 0)
                    {
                        continue;
                    }

                    //法定假日不算请假
                    if (IsHoliday(date))
                    {
                        continue;
                    }
                }

                //默认请假时间为工作时长
                var currLeaveHour = shiftInfo.WorkHour.Value;

                if (date.Date == begDate.Date && date.Date != endDate.Date)       //请假开始为当天
                {
                    //获取请假时长
                    currLeaveHour = CalcCurrDateLeaveHour(begDate, shiftInfo, LeaveDayType.Start);
                }
                else if (date.Date != begDate.Date && date.Date == endDate.Date)  //请假结束为当天
                {
                    //获取请假时长
                    currLeaveHour = CalcCurrDateLeaveHour(endDate, shiftInfo, LeaveDayType.End);
                }
                else if (date.Date == begDate.Date && date.Date == endDate.Date)  //请假发生在当天
                {
                    var currLeaveHourByStart = CalcCurrDateLeaveHour(begDate, shiftInfo, LeaveDayType.Start);
                    var currLeaveHourByEnd = CalcCurrDateLeaveHour(endDate, shiftInfo, LeaveDayType.End);

                    //获取请假时长
                    currLeaveHour = currLeaveHour - (currLeaveHour - currLeaveHourByStart) - (currLeaveHour - currLeaveHourByEnd);
                }

                leaveHours += currLeaveHour;
            }

            return leaveHours;
        }

        /// <summary>
        /// 计算指定日期请假时长
        /// </summary>
        /// <param name="date">请假时间</param>
        /// <param name="shiftInfo">作息时间</param>
        /// <param name="leaveDayType">请假类型</param>
        /// <returns></returns>
        private double CalcCurrDateLeaveHour(DateTime date, EhrShiftEntity shiftInfo, LeaveDayType leaveDayType)
        {
            double currLeaveHour = 0;
            var currLeaveMinute = 0;

            var leaveMinute = date.Hour * 60 + date.Minute;                                       //请假开始分钟
            var workStartMinute = shiftInfo.Start.Value.Hour * 60 + shiftInfo.Start.Value.Minute; //工作开始分钟
            var workEndMinute = shiftInfo.End.Value.Hour * 60 + shiftInfo.End.Value.Minute;       //工作结束分钟
            var workRestStartTime = shiftInfo.RestStart;                                          //休息开始时间
            var workRestStartMinute = 0;                                                          //休息开始分钟
            var workRestEndTime = shiftInfo.RestEnd;                                              //休息结束时间
            var workRestEndMinute = 0;                                                            //休息结束分钟
            var workRestMinute = 0;                                                               //休息时间

            //上班时间跨天,结束时间 + 24h*60
            if (workEndMinute <= workStartMinute)
            {
                workEndMinute += 24 * 60;
            }

            //中间有休息时间
            if (workRestStartTime != null)
            {
                workRestStartMinute = workRestStartTime.Value.Hour * 60 + workRestStartTime.Value.Minute;
                workRestEndMinute = workRestEndTime.Value.Hour * 60 + workRestEndTime.Value.Minute;

                //休息开始时间跨天,+24h*60
                if (workRestStartTime.Value < shiftInfo.Start.Value)
                {
                    workRestStartMinute += 24 * 60;
                }

                //休息结束时间跨天,+24h*60
                if (workRestEndTime.Value < shiftInfo.Start.Value)
                {
                    workRestEndMinute += 24 * 60;
                }
            }

            switch (leaveDayType)
            {
                case LeaveDayType.Start:
                    //请假开始时间 < 开始上班时间,以上班开始时间开始计算
                    if (leaveMinute < workStartMinute && leaveMinute + 24 * 60 > workEndMinute)
                    {
                        leaveMinute = workStartMinute;
                    }

                    //中间有休息时间
                    if (workRestStartTime != null)
                    {
                        //请假开始时间处于休息时间,以休息结束时间为开始时间
                        if (leaveMinute >= workRestStartMinute && leaveMinute < workRestEndMinute)
                        {
                            leaveMinute = workRestEndMinute;
                        }

                        //请假开始时间 >= 休息开始时间
                        if (leaveMinute <= workRestStartMinute)
                        {
                            //计算休息时间
                            workRestMinute = workRestEndMinute - workRestStartMinute;
                        }
                    }

                    //请假时长 = 上班结束时间 - 请假开始时间 - 休息时间
                    currLeaveMinute = workEndMinute - leaveMinute - workRestMinute;
                    break;
                case LeaveDayType.End:
                    //请假结束时间 > 上班结束时间,以上班结束时间开始计算
                    if (leaveMinute > workEndMinute)
                    {
                        leaveMinute = workEndMinute;
                    }

                    //中间有休息时间
                    if (workRestStartTime != null)
                    {
                        //请假结束时间处于休息时间,以休息开始时间为结束时间
                        if (leaveMinute > workRestStartMinute && leaveMinute <= workRestEndMinute)
                        {
                            leaveMinute = workRestStartMinute;
                        }

                        if (leaveMinute >= workRestEndMinute)
                        {
                            //计算休息时间
                            workRestMinute = workRestEndMinute - workRestStartMinute;
                        }
                    }

                    //请假时长 = 请假结束时间 - 上班开始时间 - 休息时间
                    currLeaveMinute = leaveMinute - workStartMinute - workRestMinute;
                    break;
            }

            int hour = currLeaveMinute / 60;
            int minute = currLeaveMinute % 60;
            if (minute <= 30 && minute > 0)
            {
                currLeaveHour = hour + 0.5;
            }
            else if (minute > 30)
            {
                currLeaveHour = hour + 1;
            }
            else
            {
                currLeaveHour = hour;
            }

            return currLeaveHour;
        }

        /// <summary>
        /// 计算前两月已调休时数
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        private double CalcAdjustHourByTwoMonthAgo(string empNo, DateTime leaveStartDate)
        {
            double restTime = 0;
            DateTime begDate;
            DateTime endDate;

            var sql = @"SELECT [LeaveNo]
                              ,[UserId]
                              ,[UserName]
                              ,[DeptName]
                              ,[OfficeName]
                              ,[LeaveType]
                              ,(case [Status]
		                        when 0 then [StartTime]
		                        when 1 then [StartTime]
		                        when 2 then [RealStartTime] end) [StartTime]
		                        ,(case [Status]
		                        when 0 then [EndTime]
		                        when 1 then [EndTime]
		                        when 2 then [RealEndTime] end) [EndTime]
                              ,[LeaveTime]
                              ,[LeaveReason]
                              ,[Status]
                          FROM [NeoWay_BaseData_Leave]
                          WHERE UserId=@EmpNo
                           AND LeaveType='调休假'
                           AND (datediff(Month,(case [Status]
	                               when 0 then [StartTime]
	                               when 1 then [StartTime]
	                               when 2 then [RealStartTime] end) ,@BegDate) between 1 and 2
                                or
							    datediff(Month,(case [Status]
	                               when 0 then [EndTime]
	                               when 1 then [EndTime]
	                               when 2 then [RealEndTime] end) ,@BegDate) between 1 and 2)
                            AND [Status]>=0";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo),
                new SqlParameter("@BegDate",leaveStartDate)
            };

            var dt = SqlHelper.ExecuteTable(sql, paras);

            if (dt == null || dt.Rows.Count == 0)
            {
                restTime = 0;
            }

            foreach (DataRow dr in dt.Rows)
            {
                DateTime.TryParse(dr["StartTime"].ToString(), out begDate);
                DateTime.TryParse(dr["EndTime"].ToString(), out endDate);

                //获取已调休时间
                restTime += CalcLeaveHours(empNo, begDate, endDate, false);
            }

            return restTime;
        }

        /// <summary>
        /// 假期是否连续计算
        /// </summary>
        /// <param name="leaveType"></param>
        /// <returns></returns>
        private bool GetLeaveIsContinuous(LeaveType leaveType)
        {
            var isContinuous = false; //是否连续计算

            var leaveTypeName = GetLeaveTypeName(leaveType);

            var leaveInfo = GetLeaveTypeInfo(leaveTypeName);

            if (leaveInfo != null)
            {
                isContinuous = leaveInfo.IsContinuous;
            }

            return isContinuous;
        }

        /// <summary>
        /// 计算是否允许休年假
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveType"></param>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool CalcIsAllowAnnual(string empNo, LeaveType leaveType, DateTime begDate, DateTime endDate, out double restDays, out string message)
        {
            message = string.Empty;
            var haveRestDays = GetHaveRestDays(empNo, leaveType, begDate);
            var alreadyRestDays = GetAlreadyRestTimeByYear(empNo, leaveType, LeaveUnit.Day, begDate);
            restDays = GetLeaveDays(empNo, leaveType, begDate, endDate, false, out message);

            if (!string.IsNullOrEmpty(message))
            {
                return false;
            }

            //TimeSpan sp = endDate.Subtract(begDate);
            //if (sp.Days > 9 && begDate.Date.AddMonths(1) > DateTime.Now.Date)
            //{
            //    message = string.Format("本次连休天数为:{0}天,当单次年假休假天数超过9天时,须提前一个月申请!", sp.Days);
            //    return false;
            //}

            //if (haveRestDays <= 5 && sp.Days > 9 && sp.Days <= 16)
            //{
            //    message = "年假与和节假日（除春节）连休，一次休假（含节假日）总天数不得超过9天（含周末）!";
            //    return false;
            //}

            //if (haveRestDays <= 5 && sp.Days > 16)
            //{
            //    message = "年休假可与春节连休，一次休假（含节假日）总天数可以超过9天，但是不能超过16天（含周末）!";
            //    return false;
            //}

            //if (haveRestDays > 5 && sp.Days > 16)
            //{
            //    message = "年假可以和节假日一次休假超过9天，但是不能超过16天（含周末）！";
            //    return false;
            //}

            var canRestDays = haveRestDays - alreadyRestDays;
            var day = restDays - canRestDays;
            if (day > 0)
            {
                message = string.Format(@"您本年度可休年假为:{0}天,已休:{1}天,剩余可休天数为:{2},
                        本次休假天数为:{3}天,已超过剩余可休天数:{4}天,请修改您的请假开始结束时间!", haveRestDays, alreadyRestDays, canRestDays, restDays, day);

                return false;
            }

            return true;
        }

        /// <summary>
        /// 是否丧失年假
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="workYears"></param>
        /// <returns>True:丧失;False:不丧失</returns>
        private bool IsLostAnnual(string empNo, decimal workYears, string empStatus, DateTime leaveStartDate)
        {
            if (empStatus == "实习生" || empStatus == "试用期")
            {
                return true;
            }

            //病假天数
            double sickHours = GetAlreadyRestTimeByYear(empNo, LeaveType.Sick, LeaveUnit.Hour, leaveStartDate);

            var sickDays = Math.Floor(sickHours / 8);
            var hours = sickHours % 8;

            if (hours > 0 && hours <= 4)
            {
                sickDays += 0.5;
            }
            else if (hours > 4)
            {
                sickDays += 1;
            }

            if (workYears < 10 && sickDays > 2 * 30)
            {
                return true;
            }

            if (workYears >= 10 && workYears < 20 && sickDays > 3 * 30)
            {
                return true;
            }

            if (workYears >= 20 && sickDays > 4 * 30)
            {
                return true;
            }

            //本年度请事假天数
            if (GetHaveRestCompassionateDaysByCurrYear(empNo, leaveStartDate) > 1 * 30)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 计算是否一次性休完假期
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveType"></param>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool CalcRestOneTimes(string empNo, LeaveType leaveType, DateTime begDate, DateTime endDate, out double restTimes, out string message)
        {
            restTimes = 0;
            var leaveTypeName = GetLeaveTypeName(leaveType);

            var haveRestDays = GetHaveRestDays(empNo, leaveType, begDate);

            if (haveRestDays == 0)
            {
                message = string.Format("您的”{0}“假期为0天,请选择别的请假类型！", leaveTypeName);
                return false;
            }

            var isRestOneTime = IsAlreadyRestOneTime(empNo, leaveTypeName);

            if (isRestOneTime)
            {
                message = string.Format("您已休过一次{0},不允许再次申请!", leaveTypeName);
                return false;
            }

            var currRestDays = GetLeaveDays(empNo, leaveType, begDate, endDate, true, out message);

            if (!string.IsNullOrEmpty(message))
            {
                return false;
            }

            var days = currRestDays - haveRestDays;
            if (days > 0)
            {
                message = string.Format("您的可休{0}天数为:{1}天,本次休假天数为:{2}天,已超出您的可休天数:{3}天,请修改您的请假开始时间或结束时间", leaveTypeName, haveRestDays, currRestDays, days);
                return false;
            }

            restTimes = currRestDays;

            return true;
        }

        /// <summary>
        /// 获取员工基本信息
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        private DataTable GetEmpInfo(string empNo)
        {
            var sql = @"select * from ehr_EmployeeInfo where EmpNo=@EmpNo";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo)
            };

            var dt = SqlHelper.ExecuteTable(sql, parameters);

            return dt;
        }

        /// <summary>
        /// 获取本年度请事假天数
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        private double GetHaveRestCompassionateDaysByCurrYear(string empNo, DateTime leaveStartDate)
        {
            double restDays = 0;
            var restTime = CalcHaveRestTime(empNo, LeaveType.Compassionate, LeaveUnit.Hour, DateType.Year, leaveStartDate);

            restDays = Math.Floor((double)restTime / 8);

            if (restTime % 8 > 0 && restTime % 8 <= 4)
            {
                restDays += 0.5;
            }
            else if (restTime % 8 > 4)
            {
                restDays += 1;
            }

            return restDays;
        }

        /// <summary>
        /// 是否为节假日
        /// </summary>
        /// <returns></returns>
        private bool IsHoliday(DateTime date)
        {
            var sql = @"select count(*) 
                        from ehr_holiday
                        where datestart>=@date
                        and dateend<=@date";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@date",date.Date)
            };

            var dt = SqlHelper.ExecuteTable(sql, paras);
            var count = Convert.ToInt32(dt.Rows[0][0]);

            var isHoliday = count > 0;

            return isHoliday;
        }

        /// <summary>
        /// 获取员工排班作息信息
        /// </summary>
        /// <param name="dtEhrRowInfo">员工排班信息</param>
        /// <param name="shiftList">班次作息时间信息</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        private EhrShiftEntity GetEmpScheduing(DataTable dtEhrRowInfo, IList<EhrShiftEntity> shiftList, DateTime date)
        {
            var year = date.Year;
            var month = date.Month;
            var day = date.Day;
            var classType = string.Empty;

            //获取员工排班信息
            var drs = dtEhrRowInfo.Select(string.Format("year={0} and month={1}", year, month));

            if (drs == null || drs.Length == 0)
            {
                throw new ArgumentException(string.Format("找不到员工{0}日排班信息,无法计算请假时间!", date.ToString("yyyy-MM-dd")));
            }

            //获取班次作息时间信息
            classType = drs[0][string.Format("d{0}", day)].ToString();

            if (string.IsNullOrWhiteSpace(classType))
            {
                return null;
            }

            var shift = shiftList.FirstOrDefault(r => r.ClassType == classType);

            return shift;
        }

        /// <summary>
        /// 获取员工排班信息
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        private DataTable GetEhrRowInfo(string empNo)
        {
            var sql = @"SELECT *
                      FROM ehr_row
                      WHERE NUMBER=@EmpNo";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo)
            };

            var dtScheduing = SqlHelper.ExecuteTable(SqlHelper.EhrCon, sql, parameters);

            return dtScheduing;
        }

        /// <summary>
        /// 获取班次作息时间信息
        /// </summary>
        /// <returns></returns>
        private IList<EhrShiftEntity> GetEhrShiftInfo()
        {
            var shiftList = new List<EhrShiftEntity>();

            var sql = @"SELECT es.number,
                    est.start,
                    est.[end],
                    est.reststart,
                    est.restend,
                    est.overtimestart,
                    round(es.timer/60,1) workhour
                      FROM ehr_shift es,ehr_shift_time est
                      WHERE es.id=est.shiftid";

            var dt = SqlHelper.ExecuteTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                double workHour;

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime start;
                    DateTime end;
                    DateTime restStart;
                    DateTime restEnd;
                    DateTime overtimeStart;

                    DateTime.TryParse(dr["start"].ToString(), out start);
                    DateTime.TryParse(dr["end"].ToString(), out end);
                    var isRestStart = DateTime.TryParse(dr["reststart"].ToString(), out restStart);
                    var isRestEnd = DateTime.TryParse(dr["restend"].ToString(), out restEnd);
                    var isOvertimeStart = DateTime.TryParse(dr["overtimestart"].ToString(), out overtimeStart);
                    Double.TryParse(dr["workhour"].ToString(), out workHour);

                    var entity = new EhrShiftEntity();
                    entity.ClassType = dr["number"].ToString();
                    entity.Start = start;
                    entity.End = end;
                    if (isRestStart)
                    {
                        entity.RestStart = restStart;
                    }
                    else
                    {
                        entity.RestStart = null;
                    }

                    if (isRestEnd)
                    {
                        entity.RestEnd = restEnd;
                    }
                    else
                    {
                        entity.RestEnd = null;
                    }

                    if (isOvertimeStart)
                    {
                        entity.OvertimeStart = overtimeStart;
                    }
                    else
                    {
                        entity.OvertimeStart = null;
                    }

                    entity.WorkHour = workHour;

                    shiftList.Add(entity);
                }
            }

            return shiftList;
        }

        /// <summary>
        /// 获取班次工作时长列表
        /// </summary>
        /// <returns></returns>
        private DataTable GetShiftWorkHourList()
        {
            var sql = "select * from ehr_shift_time";

            var dt = new DataTable();

            return dt;
        }

        /// <summary>
        /// 获取请假类型名称
        /// </summary>
        /// <param name="leaveType"></param>
        /// <returns></returns>
        private string GetLeaveTypeName(LeaveType leaveType)
        {
            var name = string.Empty;

            switch (leaveType)
            {
                case LeaveType.Annual:
                    name = "年休假";
                    break;
                case LeaveType.Compassionate:
                    name = "事假";
                    break;
                case LeaveType.Sick:
                    name = "病假";
                    break;
                case LeaveType.Marriage:
                    name = "婚假";
                    break;
                case LeaveType.Bereavement:
                    name = "丧假";
                    break;
                case LeaveType.NurseFalse:
                    name = "看护假";
                    break;
                case LeaveType.BasicMaternity:
                    name = "产假";
                    break;
                case LeaveType.Dystocia:
                    name = "难产假";
                    break;
                case LeaveType.MultipleBirths:
                    name = "多胞胎假";
                    break;
                case LeaveType.PrenatalExamination:
                    name = "产前检查假";
                    break;
                case LeaveType.FamilyPlanning:
                    name = "计划生育假";
                    break;
                case LeaveType.Lactation:
                    name = "哺乳假";
                    break;
                case LeaveType.Adjust:
                    name = "调休假";
                    break;
                case LeaveType.BusinessTrip:
                    name = "出差";
                    break;
                case LeaveType.Special:
                    name = "特殊假";
                    break;
                case LeaveType.OutOfOffice:
                    name = "外出假";
                    break;
            }

            return name;
        }

        /// <summary>
        /// 获取请假类型
        /// </summary>
        /// <param name="leaveType"></param>
        /// <returns></returns>
        private DataTable GetLeaveType(string leaveType)
        {
            var sql = @"SELECT *
                        FROM ehr_leveatype
                        WHERE NAME=@LeaveType";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@LeaveType",leaveType)
            };

            var dt = SqlHelper.ExecuteTable(sql, paras);
            return dt;
        }

        /// <summary>
        /// 获取是否允许填单
        /// </summary>
        /// <param name="empNo">工号</param>
        /// <param name="begDate">填单开始日期</param>
        /// <param name="endDate">填单结束日期</param>
        /// <returns></returns>
        private bool IsAllowSubmitBill(string empNo, DateTime begDate, DateTime endDate)
        {
            var isAllow = false;

            var sql = @"SELECT Count(*) counts
                      FROM [Ehr_Bill_Supplement]
                      where EmpNo=@EmpNo
                      and InvalidDate>=convert(varchar(10),getdate(),120)
                      and BegDate<=@BegDate
                      and EndDate>=@EndDate";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo),
                new SqlParameter("@BegDate",begDate.Date),
                new SqlParameter("@EndDate",endDate.Date)
            };

            var dt = SqlHelper.ExecuteTable(sql, paras);

            if (dt != null && dt.Rows.Count > 0)
            {
                isAllow = Convert.ToInt32(dt.Rows[0]["counts"]) > 0;
            }

            return isAllow;
        }

        /// <summary>
        /// 是否请假时间重复提交
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private bool IsRepeatSubmit(string empNo, DateTime begDate, DateTime endDate, out string errMsg)
        {
            var isRepeat = false;
            errMsg = string.Empty;

            var sql = @"SELECT TOP 1 LeaveNo,LeaveType,RealStartTime,RealEndTime
                              FROM [NeoWay_BaseData_Leave] 
                            WHERE UserId=@EmpNo
                            AND [STATUS]>=0
                            AND LeaveType!='哺乳假'
                            AND ((RealStartTime > @BegDate
                                  AND RealStartTime < @EndDate)
	                              OR(RealEndTime > @BegDate
	                              AND RealEndTime < @EndDate)
	                              OR(RealStartTime <= @BegDate
	                              AND RealEndTime  >= @EndDate))
                            AND RealStartTime<RealEndTime
                            ORDER BY RealStartTime";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo),
                new SqlParameter("@BegDate",begDate),
                new SqlParameter("@EndDate",endDate)
            };

            var dt = SqlHelper.ExecuteTable(sql, paras);

            if (dt != null && dt.Rows.Count > 0)
            {
                var leaveNo = dt.Rows[0]["LeaveNo"].ToString();
                var leaveType = dt.Rows[0]["LeaveType"].ToString();
                var start = dt.Rows[0]["RealStartTime"].ToString();
                var end = dt.Rows[0]["RealEndTime"].ToString();

                errMsg = string.Format("您当前提交的请假开始结束时间段与\r\n流程实例编号为:{0},请假类型:{1},开始时间:{2},结束时间:{3}\r\n的单据有重复,请检查!",
                    leaveNo, leaveType, start, end);

                isRepeat = true;
            }

            return isRepeat;
        }

        /// <summary>
        /// 插入调休时间记录
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="month"></param>
        /// <param name="overTime"></param>
        /// <param name="canRestTime"></param>
        /// <returns></returns>
        private int InsertLeaveTime(string empNo, string month, int overTime, int canRestTime)
        {
            var sql = @"INSERT INTO [NeoWay_BaseData_LeaveTime]
                                       ([UserId]
                                       ,[Month]
                                       ,[OverTime]
                                       ,[CanRestTime]
                                       ,[CreateDate]
                                       ,[UpdatedDate])
                                 VALUES
                                       (@UserId
                                       ,@Month
                                       ,@OverTime
                                       ,@CanRestTime
                                       ,@CreateDate
                                       ,@UpdatedDate)";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@UserId",empNo),
                new SqlParameter("@Month",month),
                new SqlParameter("@OverTime",overTime),
                new SqlParameter("@CanRestTime",canRestTime),
                new SqlParameter("@CreateDate",DateTime.Now),
                new SqlParameter("@UpdatedDate",DateTime.Now)
            };

            var result = SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, paras);

            return result;
        }

        /// <summary>
        /// 更新调休时间记录
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="month"></param>
        /// <param name="overTime"></param>
        /// <param name="canRestTime"></param>
        /// <returns></returns>
        private int UpdateLeaveTime(string empNo, string month, int overTime, int canRestTime)
        {
            var sql = @"UPDATE [NeoWay_BaseData_LeaveTime]
                           SET [OverTime] = @OverTime
                              ,[CanRestTime] = @CanRestTime
                              ,[UpdatedDate] = @UpdatedDate
                         WHERE [UserId] = @UserId
                           AND [Month] = @Month";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@UserId",empNo),
                new SqlParameter("@Month",month),
                new SqlParameter("@OverTime",overTime),
                new SqlParameter("@CanRestTime",canRestTime),
                new SqlParameter("@UpdatedDate",DateTime.Now)
            };

            var result = SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, paras);

            return result;
        }

        /// <summary>
        /// 更新调休时间记录
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="month"></param>
        /// <param name="restTime"></param>
        /// <returns></returns>
        private int UpdateLeaveTime(string empNo, string month, int restTime)
        {
            var sql = @"UPDATE [NeoWay_BaseData_LeaveTime]
                           SET [CanRestTime] = [CanRestTime] + @RestTime
                              ,[UpdatedDate] = @UpdatedDate
                         WHERE [UserId] = @UserId
                           AND [Month] = @Month";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@UserId",empNo),
                new SqlParameter("@Month",month),
                new SqlParameter("@RestTime",restTime),
                new SqlParameter("@UpdatedDate",DateTime.Now)
            };

            var result = SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, paras);

            return result;
        }

        /// <summary>
        /// 获取实际开始结束时间(处理跨年跨月问题)
        /// </summary>
        /// <param name="dateType"></param>
        /// <param name="dtEhrRowInfo"></param>
        /// <param name="shiftList"></param>
        /// <param name="leaveStartDate"></param>
        /// <param name="begDate"></param>
        /// <param name="endDate"></param>
        private void GetActualBegEndDate(DateType dateType, DataTable dtEhrRowInfo, IList<EhrShiftEntity> shiftList, DateTime leaveStartDate, ref DateTime begDate, ref DateTime endDate)
        {
            var shiftInfo = new EhrShiftEntity();                //员工排班作息信息

            //处理跨年
            if (dateType == DateType.Year && begDate.Year < endDate.Year)
            {
                if (begDate.Year == leaveStartDate.Year)
                {
                    shiftInfo = GetEmpScheduing(dtEhrRowInfo, shiftList, new DateTime(begDate.Year, 12, 31));
                    endDate = new DateTime(begDate.Year, 12, 31, shiftInfo.End.Value.Hour, shiftInfo.End.Value.Minute, 0);
                }
                else if (endDate.Year == leaveStartDate.Year)
                {
                    shiftInfo = GetEmpScheduing(dtEhrRowInfo, shiftList, new DateTime(begDate.Year, 1, 1));
                    endDate = new DateTime(begDate.Year, 1, 1, shiftInfo.Start.Value.Hour, shiftInfo.Start.Value.Minute, 0);
                }
            }

            //处理跨月
            if (dateType == DateType.Month && begDate.Month < endDate.Month)
            {
                if (begDate.Month == leaveStartDate.Month)
                {
                    var monthLastDate = begDate.Date.AddMonths(1).AddDays(-1 * (begDate.Date.Day));

                    shiftInfo = GetEmpScheduing(dtEhrRowInfo, shiftList, monthLastDate);
                    endDate = new DateTime(monthLastDate.Year, monthLastDate.Month, monthLastDate.Day, shiftInfo.End.Value.Hour, shiftInfo.End.Value.Minute, 0);
                }
                else if (endDate.Month == leaveStartDate.Month)
                {
                    var monthFirstDate = endDate.Date.AddDays(-endDate.Day + 1);
                    shiftInfo = GetEmpScheduing(dtEhrRowInfo, shiftList, monthFirstDate);
                    begDate = new DateTime(monthFirstDate.Year, monthFirstDate.Month, monthFirstDate.Day, shiftInfo.Start.Value.Hour, shiftInfo.Start.Value.Minute, 0);
                }
            }
        }

        /// <summary>
        /// 获取可调休时数
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveStartDate"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public double GetCanRestHour(string empNo, DateTime leaveStartDate, int range)
        {
            var sql = @"SELECT isnull(Sum(CanRestTime),0) CanRestTime
                          FROM [NeoWay_BaseData_LeaveTime] 
                         WHERE UserId=@EmpNo
                           AND [month]  between @BegMonth and @EndMonth";

            var begMonth = leaveStartDate.AddMonths(-(range - 1)).ToString("yyyyMM");
            var endMonth = leaveStartDate.ToString("yyyyMM");

            var paras = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo),
                new SqlParameter("@BegMonth",begMonth),
                new SqlParameter("@EndMonth",endMonth)
            };

            var result = Convert.ToDouble(SqlHelper.ExecuteScalar(SqlHelper.Con, CommandType.Text, sql, paras));

            double hour = Math.Floor(result / 60);

            if (result % 60 >= 30)
            {
                hour += 0.5;
            }

            return hour;
        }

        /// <summary>
        /// 获取近几月调休记录
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="leaveStartDate"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private DataTable GetLeaveRecord(string empNo, DateTime leaveStartDate, int range)
        {
            var sql = @"SELECT *
                          FROM [NeoWay_BaseData_LeaveTime] 
                         WHERE UserId=@EmpNo
                           AND [month]  between @BegMonth and @EndMonth
                         ORDER BY [Month]";

            var begMonth = leaveStartDate.AddMonths(-(range - 1)).ToString("yyyyMM");
            var endMonth = leaveStartDate.ToString("yyyyMM");

            var paras = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo),
                new SqlParameter("@BegMonth",begMonth),
                new SqlParameter("@EndMonth",endMonth)
            };

            var dt = SqlHelper.ExecuteTable(sql, paras);

            return dt;
        }

        private int InsertLeaveDetail(int procInstId, string empNo, string month, int restTime)
        {
            var sql = @"INSERT INTO [NeoWay_BaseData_LeaveDetail]
                                   ([ProcInstId]
                                   ,[UserId]
                                   ,[Month]
                                   ,[RestTime]
                                   ,[CreateDate]
                                   ,[UpdatedDate]
                                   ,[Status])
                             VALUES
                                   (@ProcInstId
                                   ,@UserId
                                   ,@Month
                                   ,@RestTime
                                   ,@CreateDate
                                   ,@UpdatedDate
                                   ,@Status)";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@ProcInstId",procInstId),
                new SqlParameter("@UserId",empNo),
                new SqlParameter("@Month",month),
                new SqlParameter("@RestTime",restTime),
                new SqlParameter("@CreateDate",DateTime.Now),
                new SqlParameter("@UpdatedDate",DateTime.Now),
                new SqlParameter("@Status",1)
            };

            var result = SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, paras);

            return result;
        }

        private int DeleteLeaveDetail(int procInstId, string empNo)
        {
            var sql = @"UPDATE [NeoWay_BaseData_LeaveDetail]
                       SET [Status] = 0,
	                       [UpdatedDate]=@UpdatedDate
                     WHERE UserId=@EmpNo
                       AND [ProcInstId]=@ProcInstId";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo),
                new SqlParameter("@ProcInstId",procInstId),
                new SqlParameter("@UpdatedDate",DateTime.Now)
            };

            var result = SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, paras);

            return result;
        }

        private DataTable GetLeaveDetail(int procInstId, string empNo)
        {
            var sql = @"SELECT *
                          FROM [NeoWay_BaseData_LeaveDetail]
                          WHERE ProcInstId=@ProcInstId
                            AND UserId=@EmpNo
	                        and [Status]=1";

            var paras = new SqlParameter[]
            {
                new SqlParameter("@ProcInstId",procInstId),
                new SqlParameter("EmpNo",empNo)
            };

            var dt = SqlHelper.ExecuteTable(sql, paras);

            return dt;
        }

        #endregion

        #region 计算假期天数

        /// <summary>
        /// 计算年假天数
        /// </summary>
        /// <param name="empNo">工号</param>
        /// <param name="workYears">工作年限</param>
        /// <returns></returns>
        private int CalcAnualDays(DataRow drEmployeeInfo, DateTime leaveStartDate)
        {
            var restDays = 0;
            DateTime inDate;                                                 //入职时间
            double preServiceYear;                                           //入司前工作时间
            double onServiceYear;                                            //在司时间
            double totalServiceYear;                                         //累计工作时间
            var currYearFirstDate = new DateTime(leaveStartDate.Year, 1, 1);   //本年度第一天
            var currYearLastDate = new DateTime(leaveStartDate.Year, 12, 31);  //本年度最后一天

            var empNo = drEmployeeInfo["EmpNo"].ToString();
            var empStatus = drEmployeeInfo["EmpStatus"].ToString();
            var isDate = DateTime.TryParse(drEmployeeInfo["InDate"].ToString(), out inDate);
            if (!isDate) { return -1; }
            Double.TryParse(drEmployeeInfo["PreServiceYear"].ToString(), out preServiceYear);
            onServiceYear = Math.Round(((double)(currYearLastDate.Subtract(inDate).Days + 1) / 365), 1);
            totalServiceYear = preServiceYear + onServiceYear;
            var workYears = (int)totalServiceYear;
            var workMonth = Math.Round(totalServiceYear - workYears, 1);

            //是否丧失年假
            if (IsLostAnnual(empNo, workYears, empStatus, leaveStartDate))
            {
                restDays = 0;
            }
            else if (workYears == 20)
            {
                restDays = (int)Math.Floor(10 * (1 - workMonth) + 15 * workMonth);
            }
            else if (workYears == 10)
            {
                restDays = (int)Math.Floor(5 * (1 - workMonth) + 10 * workMonth);
            }
            else if (workYears == 1)
            {
                restDays = (int)Math.Floor(5 * workMonth);
            }
            else if (workYears <= 1)
            {
                restDays = 0;
            }
            else if (workYears > 1 && workYears <= 10)
            {
                restDays = 5;
            }
            else if (workYears > 10 && workYears <= 20)
            {
                restDays = 10;
            }
            else if (workYears > 20)
            {
                restDays = 15;
            }

            if (inDate.Date > currYearFirstDate.Date)
            {
                restDays = (int)Math.Floor((double)onServiceYear * restDays);
            }

            return restDays;
        }

        /// <summary>
        /// 计算婚假天数
        /// </summary>
        /// <param name="age">年龄</param>
        /// <param name="sex">性别</param>
        /// <returns></returns>
        private int CalcMarriageDays(int age, string sex)
        {
            var restDays = 0;

            if ((sex == "男" && age >= 22 && age < 25) || (sex == "女" && age >= 20 && age < 23))
            {
                restDays = 3;
            }
            else if ((sex == "男" && age >= 25) || (sex == "女" && age >= 23))
            {
                restDays = 13;
            }

            return restDays;
        }

        /// <summary>
        /// 计算看护假
        /// </summary>
        /// <param name="sex">性别</param>
        /// <returns></returns>
        private int CalcNurseFalseDays(string sex)
        {
            var restDays = 0;

            if (sex == "男")
            {
                restDays = 10;
            }

            return restDays;
        }

        /// <summary>
        /// 计算基本产假天数
        /// </summary>
        /// <param name="age">年龄</param>
        /// <param name="sex">性别</param>
        /// <returns></returns>
        private int CalcBasicMaternityDays(int age, string sex)
        {
            var restDays = 0;

            if (sex == "女")
            {
                restDays = 98;

                if (age > 24)
                {
                    restDays += 30;
                }
            }

            return restDays;
        }

        /// <summary>
        /// 计算难产假天数
        /// </summary>
        /// <param name="age">年龄</param>
        /// <param name="sex">性别</param>
        /// <returns></returns>
        private int CalcDystociaDays(int age, string sex)
        {
            var restDays = 0;

            if (sex == "女")
            {
                restDays = 15;
            }

            return restDays;
        }

        /// <summary>
        /// 计算多胞胎假天数
        /// </summary>
        /// <param name="age">年龄</param>
        /// <param name="sex">性别</param>
        /// <param name="number">胎数</param>
        /// <returns></returns>
        private int CalcMultipleBirthsDays(int age, string sex, int number)
        {
            var restDays = 0;

            if (sex == "女")
            {
                restDays = (number - 1) * 15;
            }

            return restDays;
        }

        #endregion
    }
}
