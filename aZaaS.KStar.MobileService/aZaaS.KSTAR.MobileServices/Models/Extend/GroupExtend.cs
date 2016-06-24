using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class GroupExtend
    {
        public GroupExtend() { }
        public GroupExtend(GroupDefinition def)
        {
            this.ID = def.ID;
            this.Name = def.Name;
            this.LabelID = def.LabelID;
            this.Type = def.Type;
            this.Collapsed = def.Collapsed;
            this.ItemList = new List<ItemExtend>();
            this.GroupList = new List<GroupExtend>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public Guid? LabelID { get; set; }
        public string Type { get; set; }
        public bool Collapsed { get; set; }

        public string ConnectionString { get; set; }
        public string Mapping { get; set; }
        public string WhereString { get; set; }
        public string Label { get; set; }
        public List<GroupExtend> GroupList { get; set; }
        public List<ItemExtend> ItemList { get; set; }

        public Group ConvertToGroup()
        {
            Group group = new Group()
            {
                Collapsed = this.Collapsed,
                Label = this.Label,
                Type = this.Type
            };
            return group;
        }

    }
}