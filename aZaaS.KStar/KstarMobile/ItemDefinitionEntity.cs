using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.KstarMobile
{
    public class ItemDefinitionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ID { get; set; }        
        public virtual string Name { get; set; }        
        public virtual Guid? LabelID { get; set; }
        [NotMapped]
        public virtual string LabelName { get; set; }
        public virtual bool Visible { get; set; }        
        public virtual bool Editable { get; set; }
        public virtual string Format { get; set; }        
    }
}
