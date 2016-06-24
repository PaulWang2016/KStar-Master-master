using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.WorkflowData
{
    public abstract class AbstractEntity
    {
        [Key]
        public Guid ID { get; set; }


        protected void IsChange(Guid FormID, ref  List<ChangeLogEntity> changelog, string orderBy, string original, string newvalue, string FieldName)
        {
            if (original != newvalue)
            {
                ChangeLogEntity item = new ChangeLogEntity();
                item.NewValue = newvalue;
                item.OriginalValue = original;
                item.ID = Guid.NewGuid();
                item.FK_Form_ID = FormID;
                item.FieldName = FieldName;
                item.ModifiedDate = DateTime.Now;
                item.OrderBy = orderBy;
                changelog.Add(item);
            }
        }

        protected void IsChange(Guid FormID, ref  List<ChangeLogEntity> changelog, string orderBy, Decimal original, Decimal newvalue, string FieldName)
        {
            if (!original.Equals(newvalue))
            {
                ChangeLogEntity item = new ChangeLogEntity();
                item.NewValue = newvalue.ToString("C");
                item.OriginalValue = original.ToString("C");
                item.ID = Guid.NewGuid();
                item.FK_Form_ID = FormID;
                item.FieldName = FieldName;
                item.ModifiedDate = DateTime.Now;
                item.OrderBy = orderBy;
                changelog.Add(item);
            }
        }

        protected void IsChange(Guid FormID, ref  List<ChangeLogEntity> changelog, string orderBy, DateTime? original, DateTime? newvalue, string FieldName)
        {
            if (!original.Equals(newvalue))
            {
                ChangeLogEntity item = new ChangeLogEntity();
                item.NewValue = newvalue != null ? newvalue.Value.ToString("dd/MM/yyyy") : "";
                item.OriginalValue = original != null ? original.Value.ToString("dd/MM/yyyy") : "";
                item.ID = Guid.NewGuid();
                item.FK_Form_ID = FormID;
                item.FieldName = FieldName;
                item.ModifiedDate = DateTime.Now;
                item.OrderBy = orderBy;
                changelog.Add(item);
            }
        }

        protected void IsChange(Guid FormID, ref  List<ChangeLogEntity> changelog, string orderBy, bool? original, bool? newvalue, string FieldName)
        {
            if (original != null && newvalue != null && !original.Equals(newvalue))
            {
                ChangeLogEntity item = new ChangeLogEntity();
                item.NewValue = newvalue.ToString();
                item.OriginalValue = original.ToString();
                item.ID = Guid.NewGuid();
                item.FK_Form_ID = FormID;
                item.FieldName = FieldName;
                item.ModifiedDate = DateTime.Now;
                item.OrderBy = orderBy;
                changelog.Add(item);
            }
        }

    }
}