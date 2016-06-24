using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.WorkflowData
{
    public enum FormStatus
    {
        //FormNo不可见 可编辑
        New,
        //FormNo不可见 可编辑
        Draft,
        //FormNo可见 可编辑
        Preparing,
        //FormNo可见 不可编辑
        Approving,

        Endorsed,//第一次同意
        //FormNo可见 不可编辑
        Approved,
        //FormNo可见 不可编辑
        Rejected,

        Withdrawn,

        Deleted,
        
        Cancelled
    }
}
