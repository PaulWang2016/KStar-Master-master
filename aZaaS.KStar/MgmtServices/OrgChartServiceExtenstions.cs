using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtServices
{
    public static class OrgChartServiceExtenstions
    {
        public static string GetClusterCode(this OrgChartService orgChartService, string propertyCode)
        {
            //LogicalExpression exp = new LogicalExpression(LogicalKind.And)
            //{
            //    Fields = new List<Field>
            //            {
            //                new Field("PropertyName", "Code".SQLString(), "OrgNode", OperatorKind.Equal),
            //                new Field("ValueString", propertyCode.SQLString(), "OrgNode", OperatorKind.Equal),
            //                new Field("Type", "Property".SQLString(), "OrgNode", OperatorKind.Equal)
            //            }
            //};
            //IEnumerable<OrgNodeWithParentDto> nodes = orgChartService.GetNodesWithParent(exp);//orgChartBO.GetNodes(exp);
            //if (nodes == null)
            //{
            //    return "";
            //}
            //foreach (OrgNodeWithParentDto node in nodes)
            //{
            //    foreach (var item in orgChartService.ReadNodeWithFields(node.Parent.Id).ExFields)
            //    {
            //        if (item.PropertyName == "Code")
            //            return item.ValueString;
            //    }
            //}
            return "";
        }
        public static string GetDivisionCode(this OrgChartService orgChartService, string propertyCode)
        {
            //LogicalExpression exp = new LogicalExpression(LogicalKind.And)
            //{
            //    Fields = new List<Field>
            //            {
            //                new Field("PropertyName", "Code".SQLString(), "OrgNode", OperatorKind.Equal),
            //                new Field("ValueString", propertyCode.SQLString(), "OrgNode", OperatorKind.Equal),
            //                new Field("Type", "Property".SQLString(), "OrgNode", OperatorKind.Equal)
            //            }
            //};
            //IEnumerable<OrgNodeWithParentDto> nodes = orgChartService.GetNodesWithParent(exp);//orgChartBO.GetNodes(exp);
            //if (nodes == null)
            //{
            //    return "";
            //}
            //foreach (OrgNodeWithParentDto node in nodes)
            //{
            //    Guid id = orgChartService.ReadNodeWithParent(node.Parent.SysID).Parent.Id;
            //    if (id == null)
            //    {
            //        return "";
            //    }
            //    foreach (var item in orgChartService.ReadNodeWithFields(id).ExFields)
            //    {
            //        if (item.PropertyName == "Code")
            //            return item.ValueString;
            //    }
            //}
            return "";
        }
    }
}
