using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IFormControlFilter
    {
        string FilterFromControls(int activityId);
    }
}
