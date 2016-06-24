using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aZaaS.KStar.DataDictionary
{
    public class DataDictionaryEntity
    {
        public DataDictionaryEntity()
        {
            this.Childs = new List<DataDictionaryEntity>();
        }
        [Key]
        public virtual Guid Id { get; set; }
        public virtual Guid? ParentId { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
        public virtual int Type { get; set; }
        public virtual int Order { get; set; }
        public virtual string Remark { get; set; }
        [ForeignKey("ParentId")]
        public virtual List<DataDictionaryEntity> Childs { get; set; }
    }
}
