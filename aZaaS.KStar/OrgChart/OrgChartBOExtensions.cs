using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using aZaaS.Framework.OrgChart;
using aZaaS.Framework.Extensions;

namespace aZaaS.KStar
{
    public static class OrgChartBOExtensions
    {
        public static string ChartToXml(this OrgChartBO chartBO, Guid chartId)
        {
           // return GetChartDocument(chartBO.ReadChart(chartId)).Root.ToString();
            return string.Empty;
        }

        public static string ChartToXml(this OrgChartBO chartBO, string chartName)
        {
           // return GetChartDocument(chartBO.ReadChart(chartName)).Root.ToString();
            return string.Empty;
        }

        public static string ChartsToXml(this OrgChartBO chartBO)
        {
           // return GetChartsDocument(chartBO.GetAllCharts()).Root.ToString();
            return string.Empty;
        }

        private static XElement GetChartNodeElements(OrgNodeDTO node)
        {             
            XElement eleNode = new XElement("Node");

            ////Base properties
            //eleNode.SetAttributeValue("Id", node.Id);
            //eleNode.SetAttributeValue("Name", node.Name);
            //eleNode.SetAttributeValue("Type", node.Type);
            //eleNode.SetAttributeValue("ChartId",node.Chart.Null() ? "NULL" : node.Chart.Id.ToString());
            //eleNode.SetAttributeValue("ParentId", node.Parent.Null() ? "NULL" : node.Parent.Id.ToString());

            ////Extension fields
            //XElement eleNodeExFields = new XElement("Fields");
            //if (node.ExFields.NotNullOrEmpty())
            //{
            //    foreach (OrgNodeExFieldDTO field in node.ExFields)
            //    {
            //        XElement eleExField = new XElement("Field");
            //        eleExField.SetAttributeValue("Id", field.Id);
            //        eleExField.SetAttributeValue("Name", field.PropertyName);
            //        eleExField.SetAttributeValue("TypeCode", field.TypeCode);
            //        eleExField.SetAttributeValue("ValueNumber", field.ValueNumber.ToString());
            //        eleExField.SetAttributeValue("ValueDateTime", field.ValueDateTime.Null() ? string.Empty : field.ValueDateTime.ToString());
            //        eleExField.SetAttributeValue("ValueString", field.ValueString ?? string.Empty);

            //        eleNodeExFields.Add(eleExField);
            //    }
            //}
            //eleNode.Add(eleNodeExFields);

            ////Related users
            //XElement eleNodeUsers = new XElement("Users");
            //if (node.Users.NotNullOrEmpty())
            //{
            //    foreach (UserDTO user in node.Users)
            //    {
            //        XElement eleUser = new XElement("User");
            //        eleUser.SetAttributeValue("Id", user.Id);
            //        eleUser.SetAttributeValue("UserName", user.UserName);
            //        eleUser.SetAttributeValue("FullName", user.FullName);
            //        eleUser.SetAttributeValue("Email", user.Email);
            //        UserExFieldDTO dept = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("Department"));
            //        eleUser.SetAttributeValue("Department", dept.Null() ? string.Empty : dept.ValueString);
            //        UserExFieldDTO title = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("JobTitle"));
            //        eleUser.SetAttributeValue("JobTitle", title.Null() ? string.Empty : title.ValueString);

            //        eleNodeUsers.Add(eleUser);
            //    }                
            //}
            //eleNode.Add(eleNodeUsers);

            ////Related positions
            //XElement eleNodePositions = new XElement("Positions");
            //if (node.Positions.NotNullOrEmpty())
            //{
            //    foreach (PositionDTO position in node.Positions)
            //    {
            //        XElement elePosition = new XElement("Position");
            //        elePosition.SetAttributeValue("Id", position.Id);
            //        elePosition.SetAttributeValue("Name", position.Name);
            //        elePosition.SetAttributeValue("Category", position.Category.Name);

            //        XElement elePositionUsers = new XElement("Users");
            //        if (position.Users.NotNullOrEmpty())
            //        {
            //            foreach (UserDTO user in position.Users)
            //            {
            //                XElement elePostionUser= new XElement("User");
            //                elePostionUser.SetAttributeValue("Id", user.Id);
            //                elePostionUser.SetAttributeValue("UserName", user.UserName);
            //                elePostionUser.SetAttributeValue("FullName", user.FullName);
            //                elePostionUser.SetAttributeValue("Email", user.Email);
            //                UserExFieldDTO dept = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("Department"));
            //                elePostionUser.SetAttributeValue("Department", dept.Null() ? string.Empty : dept.ValueString);
            //                UserExFieldDTO title = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("JobTitle"));
            //                elePostionUser.SetAttributeValue("JobTitle", title.Null() ? string.Empty : title.ValueString);

            //                elePositionUsers.Add(elePostionUser);
            //            }
            //        }
            //        elePosition.Add(elePositionUsers);

            //        eleNodePositions.Add(elePosition);
            //    }
            //}
            //eleNode.Add(eleNodePositions);

            ////Recursive child nodes
            //XElement eleNodeChilds = new XElement("Nodes");
            //if (node.ChildNodes.NotNullOrEmpty())
            //{
            //    foreach (OrgNodeDTO childNode in node.ChildNodes)
            //    {
            //        eleNodeChilds.Add(GetChartNodeElements(childNode));
            //    }                
            //}
            //eleNode.Add(eleNodeChilds);

            return eleNode;
        }

        private static XDocument GetChartDocument(OrgChartDTO chart)
        {
            chart.NullThrowInvalidOpEx("can not found the specified chart.");

            XDocument docChart = new XDocument();

            XElement eleChart = new XElement("Chart");
            eleChart.SetAttributeValue("Id", chart.SysID);
            eleChart.SetAttributeValue("Name", chart.Name);
            eleChart.SetAttributeValue("RootId", chart.Root.Null() ? "NULL" : chart.Root.SysID.ToString());
            eleChart.Add(GetChartNodeElements(chart.Root));

            docChart.Add(eleChart);

            return docChart;
        }

        private static XDocument GetChartsDocument(IEnumerable<OrgChartDTO> charts)
        {
            XDocument docCharts = new XDocument();

            XElement eleCharts = new XElement("Charts");
            foreach (OrgChartDTO chart in charts)
            {
                XElement eleChart = new XElement("Chart");
                eleChart.SetAttributeValue("Id", chart.SysID);
                eleChart.SetAttributeValue("Name", chart.Name);
                eleChart.SetAttributeValue("RootId", chart.Root.Null() ? "NULL" : chart.Root.SysID.ToString());
                eleChart.Add(GetChartNodeElements(chart.Root));

                eleCharts.Add(eleChart);
            }
            docCharts.Add(eleCharts);

            return docCharts;
        }
    }
}
