using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.KstarMobile
{
    public class LabelContentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ID { get; set; }
        public virtual Guid? LabelID { get; set; }
        public virtual string Language { get; set; }
        public virtual string Content { get; set; }
    }
}
