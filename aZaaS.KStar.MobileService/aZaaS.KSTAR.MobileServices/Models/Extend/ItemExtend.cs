using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class ItemExtend
    {
        public ItemExtend(ItemDefinition def)
        {
            this.ID = def.ID;
            this.Name = def.Name;
            this.LabelID = def.LabelID;
            this.Visible = def.Visible;
            this.Editable = def.Editable;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public Guid? LabelID { get; set; }
        public bool Visible { get; set; }
        public bool Editable { get; set; }

        public string Mapping { get; set; }
        public string Label { get; set; }

        public Item ConvertToItem()
        {
            Item item = new Item()
            {
                Editable = this.Editable,
                Label = this.Label,
                Name = this.Name,
                Visible = this.Visible
            };
            return item;
        }
    }
}