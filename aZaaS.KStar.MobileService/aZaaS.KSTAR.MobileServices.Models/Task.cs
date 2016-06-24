using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class Task : BaseEntity
    {
        public BaseInfo BaseInfo { get; set; }
        public ExtendInfo ExtendInfo { get; set; }

        public Task()
        {
            this.BaseInfo = new BaseInfo();
            this.ExtendInfo = new ExtendInfo();
        }
    }

    public class BaseInfo : BaseEntity
    {
        public List<Item> Items { get; set; }
    }

    public class ExtendInfo : BaseEntity
    {
        public List<Item> Items { get; set; }
    }
}
