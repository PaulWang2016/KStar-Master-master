using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class TreeModel
    {
        public string ID {get;set;}
        public Guid SysId{get;set;}                    
        public Guid? ParentID{get;set;}     
        public string Type{get;set;}
        public bool isParent{get;set;}
        public string NodeName{get;set;}                    
    }
}