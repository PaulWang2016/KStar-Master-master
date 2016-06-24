using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.Kstar.DAL
{
    /// <summary>
    /// 员工入职
    /// 花名册数据Model
    /// </summary>
    public class EmployeeInfo
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string EmpNo { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string EmpName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 是否二次入职
        /// </summary>
        public string IsEntryAgain { set; get; }


        /// <summary>
        /// 学历
        /// </summary>
        public string Education { set; get; }
        /// <summary>
        /// 毕业院校
        /// </summary>
        public string GraduateSchool { set; get; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        public Nullable<System.DateTime> GraduateDate { set; get; }
        /// <summary>
        /// 院校属性
        /// </summary>
        public string CollegeLevel { set; get; }
        /// <summary>
        /// 所属专业
        /// </summary>
        public string Professional { set; get; }


        /// <summary>
        /// 所属类别
        /// </summary>
        public string BelongType { set; get; }
        /// <summary>
        /// 应聘部门
        /// </summary>
        public string Department { set; get; }
        /// <summary>
        /// 应聘科室
        /// </summary>
        public string Section { set; get; }
        /// <summary>
        /// 应聘岗位
        /// </summary>
        public string Jobs { set; get; }


        /// <summary>
        /// 是否与公司其他员工有亲属关系 
        /// </summary>
        public string IsRelatives { set; get; }
        /// <summary>
        ///  亲属姓名
        /// </summary>
        public string RelativeName { get; set; }
        /// <summary>
        /// 亲属所属单位
        /// </summary>
        public string RelativeDept { get; set; }
        /// <summary>
        /// 亲属科室
        /// </summary>
        public string RelativeSection { get; set; }
        /// <summary>
        /// 与亲属之关系
        /// </summary>
        public string Relationship { get; set; }


        /// <summary>
        /// 入职时间
        /// </summary>
        public Nullable<System.DateTime> InDate { set; get; }
        /// <summary>
        /// 审批级别
        /// </summary>
        public string Position { get; set; }
    }
}
