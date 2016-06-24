using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.KstarMobile
{
    public class ProcessDefinitionEntity
    {
        public ProcessDefinitionEntity()
        {
            this.Childs = new List<ProcessDefinitionEntity>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ID { get; set; }        
        public virtual string ProcessFullName { get; set; }        
        public virtual int? ParentID { get; set; }
        public virtual string ChildType { get; set; }
        public virtual int ChildID { get; set; }
        public virtual int OrderNo { get; set; }
        public virtual string ConnectionString { get; set; }
        public virtual string Mapping { get; set; }
        public virtual string WhereString { get; set; }
        [ForeignKey("ParentID")]        
        public virtual List<ProcessDefinitionEntity> Childs { get; set; }
    }
}
