 
namespace aZaaS.KStar.ProcessForm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class RoleUser
    {
        public System.Guid Role_SysId { get; set; }
   
        public System.Guid User_SysId { get; set; }
    }
    public partial class Position
    {
        [Key]
        public System.Guid SysId { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> Category_SysId { get; set; }
    }

    public partial class PositionUser
    {
        public System.Guid Position_SysId { get; set; }
        public System.Guid User_SysId { get; set; }
    }

    public partial class PositionOrgNodes
    {
        public System.Guid Position_SysId { get; set; }
        public System.Guid OrgNode_SysId { get; set; }
    }


    public partial class OrgNode
    {
        [Key]
        public System.Guid SysId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Nullable<System.Guid> Chart_SysId { get; set; }
        public Nullable<System.Guid> Parent_SysId { get; set; }
    }
    public partial class UserOrgNode
    {
        public System.Guid User_SysId { get; set; }
        public System.Guid OrgNode_SysId { get; set; }
    }

    public partial class ProcessActivityParticipantSetEntry
    {
          [Key]
        public int ID { get; set; }
        public Nullable<System.Guid> SetID { get; set; }
        public string EntryType { get; set; }
        public Nullable<System.Guid> EntryID { get; set; }
        public string EntryName { get; set; }
    }
    public partial class ProcessActivityParticipantSet
    {
          [Key]
        public System.Guid SetID { get; set; }
        public string Assigner { get; set; }
        public string AssignerName { get; set; }
        public string Setter { get; set; }
        public string SetterName { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<int> ProcInstID { get; set; }
        public string ProcessFullName { get; set; }
        public Nullable<int> ActivityID { get; set; }
        public string ActivityName { get; set; }
        public Nullable<bool> IsPeeked { get; set; }
        public Nullable<bool> IsOriginal { get; set; }
        public Nullable<bool> SkipAssigner { get; set; }
        public Nullable<bool> SkipSet { get; set; }
        public Nullable<System.DateTime> DateAssigned { get; set; }
        public string Remark { get; set; }
    }
}
