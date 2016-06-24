using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using aZaaS.Framework;
using aZaaS.Framework.Organization.Facade;
using aZaaS.Framework.Organization.OrgChart;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.Framework.Organization.Extensions;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.KStar.MgmtDtos;

namespace aZaaS.KStar
{
    public class OrgChartBO : AbstractBO
    {
        private readonly OrgChartFacade chartFacade;

        public OrgChartBO()
        {
            this.chartFacade = new OrgChartFacade();
        }

        ///<summary>
        ///Creates a new chart.
        ///</summary>
        ///<param name="chart">The new  chart instance</param>
        ///<returns>The new chart id</returns>
        public Guid CreateChart(OrgChartWithRootDto chartDTO)
        {
            chartDTO.NullThrowArgumentEx("chart dto is not assigned.");

            return this.chartFacade.CreateChart(Mapper.Map<OrgChartWithRootDto, OrgChart>(chartDTO));
        }

        ///<summary>
        ///Checks whether the specified chart name exists.
        ///</summary>
        ///<param name="chartName">The specified chart name</param>
        ///<returns>True or false</returns>
        public bool ChartNameExists(string chartName)
        {
            return this.chartFacade.ChartNameExists(chartName);
        }

        ///<summary>
        ///Retrieves chart according to the specified chart id.
        ///</summary>
        ///<param name="chartId">The specified chart id </param>
        ///<returns>The matching chart insntace</returns>
        public OrgChartWithRelationshipsDto ReadChart(Guid chartId)
        {
            return Mapper.Map<OrgChart, OrgChartWithRelationshipsDto>(this.chartFacade.ReadChart(chartId));
        }

        public OrgChartWithRelationshipsDto ReadChart(string chartName)
        {
            return Mapper.Map<OrgChart, OrgChartWithRelationshipsDto>(this.chartFacade.ReadChart(chartName));
        }

        ///<summary>
        ///Updates the specified chart.
        ///</summary>
        ///<param name="chart">The specified chart instance</param>
        public void UpdateChart(OrgChartWithRelationshipsDto chartDTO)
        {
            chartDTO.NullThrowArgumentEx("chart dto is not assigned.");

            this.chartFacade.UpdateChart(Mapper.Map<OrgChartWithRelationshipsDto, OrgChart>(chartDTO));          
        }

        ///<summary>
        ///Updates the specified chart root node.
        ///</summary>
        ///<param name="chartId">The specified chart id</param>
        ///<param name="rootId">The root node id</param>
        public void UpdateChartRoot(Guid chartId, Guid rootId)
        {
            this.chartFacade.UpdateChartRoot(chartId, rootId);
        }

        ///<summary>
        ///Deletes a specified chart.
        ///</summary>
        ///<param name="chartId">The specified chart id</param>
        public void DeleteChart(Guid chartId)
        {
            this.chartFacade.DeleteChart(chartId);
        }


        ///<summary>
        ///Creates a new node.
        ///</summary>
        ///<param name="node">The new node instance</param>
        ///<returns>The new node id</returns>
        public Guid CreateNode(OrgNodeWithChartParentDto nodeDTO)
        {
            nodeDTO.NullThrowArgumentEx("node dto is not assigned.");

            return this.chartFacade.CreateNode(Mapper.Map<OrgNodeWithChartParentDto, OrgNode>(nodeDTO));            
        }

        ///<summary>
        ///Retrieves node according to the specified node id.
        ///</summary>
        ///<param name="nodeId">The specified node id</param>
        ///<returns>The matching node instance</returns>
        public OrgNodeWithRelationshipsDto ReadNode(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeWithRelationshipsDto>(this.chartFacade.ReadNode(nodeId));
        }

        ///<summary>
        ///Updates a specified node.
        ///</summary>
        ///<param name="node">The specified node instance</param>
        public void UpdateNode(OrgNodeBaseDto nodeDTO)
        {
            nodeDTO.NullThrowArgumentEx("node dto is not assigned.");

            this.chartFacade.UpdateNode(Mapper.Map<OrgNodeBaseDto, OrgNode>(nodeDTO));
        }

        ///<summary>
        ///Updates the parent node of the specified node.
        ///</summary>
        ///<param name="parentId">The specified parent node id</param>
        public void UpdateNodeParent(Guid nodeId, Guid parentId)
        {
            this.chartFacade.UpdateNodeParent(nodeId, parentId);
        }

        ///<summary>
        ///Updates the chart of the specified node.
        ///</summary>
        ///<param name="chartId">The specified chart id</param>
        public void UpdateNodeChart( Guid nodeId, Guid chartId)
        {
            this.chartFacade.UpdateNodeChart(nodeId, chartId);
        }

        ///<summary>
        ///Deletes the specified node.
        ///</summary>
        ///<param name="nodeId">The specified node id</param>
        public void DeleteNode(Guid nodeId)
        {
            this.chartFacade.DeleteNode(nodeId);
        }


        ///<summary>
        ///Checks whether the specified user is already assigned to the specified node.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<param name="nodeId">The specified node id</param>
        ///<returns>True or false</returns>
        public bool NodeUserExists(Guid userId, Guid nodeId)
        {
            return this.chartFacade.NodeUserExists(userId, nodeId);
        }

        ///<summary>
        ///Checks whether the specified position is already assigned to the specified node.
        ///</summary>
        ///<param name="positionId">The specified position id</param>
        ///<param name="nodeId">The specified node id</param>
        ///<returns>True or false</returns>
        public bool NodePositionExists(Guid positionId, Guid nodeId)
        {
            return this.chartFacade.NodePositionExists(positionId, nodeId);
        }

        ///<summary>
        ///Checks whether the specified field is already assigned to the specified node.
        ///</summary>
        ///<param name="propertyName">The specified property name</param>
        ///<param name="nodeId">The specified node id</param>
        ///<returns>True or false</returns>
        public bool NodeFieldExists(string fieldName, Guid nodeId)
        {
            return this.chartFacade.NodeFieldExists(fieldName, nodeId);
        }

        ///<summary>
        ///Assigns a user to the specified node.
        ///</summary>
        ///<param name="nodeId">The specified node id</param>
        ///<param name="userId">The specified user id</param>
        public void AppendUser(Guid nodeId, Guid userId)
        {
            this.chartFacade.AppendUser(nodeId, userId);         
        }

        ///<summary>
        ///Assigns a position to the specified node.
        ///</summary>
        ///<param name="nodeId">The specified node id</param>
        ///<param name="position">The specified position id</param>
        public void AppendPosition(Guid nodeId, Guid positionId)
        {
            this.chartFacade.AppendPosition(nodeId, positionId);
        }

        ///<summary>
        ///Removes a user from the specified node.
        ///</summary>
        ///<param name="nodeId">The specified node id</param>
        ///<param name="userName">The specified user id</param>
        public void RemoveUser(Guid nodeId, Guid userId)
        {
            this.chartFacade.RemoveUser(nodeId, userId);
        }

        ///<summary>
        ///Removes a position from the specified node.
        ///</summary>
        ///<param name="nodeId">The specified node id</param>
        ///<param name="poistionName">The specified position id</param>
        public void RemovePosition( Guid nodeId, Guid positionId)
        {
            this.chartFacade.RemovePosition(nodeId, positionId);
        }

        ///<summary>
        ///Assigns a field to the specified node.
        ///</summary>
        ///<param name="nodeId">The specified node id</param>
        ///<param name="field">The specified field</param>
        public void AppendExField(Guid nodeId, OrgNodeExFieldDTO fieldDTO)
        {
            fieldDTO.NullThrowArgumentEx("filed is dto is not assigned.");

            this.chartFacade.AppendField(nodeId, Mapper.Map<OrgNodeExFieldDTO, OrgNodeExtend>(fieldDTO));
        }

        ///<summary>
        ///Removes a field from the specified node.
        ///</summary>
        ///<param name="nodeId">The specified node id</param>
        ///<param name="fieldId">The specified field id</param>
        public void RemoveExField(Guid nodeId,string name)
        {
            this.chartFacade.RemoveField(nodeId, name);
        }

        ///<summary>
        ///Updates the specified field.
        ///</summary>
        ///<param name="field">The specified field instance</param>
        public void UpdateExField(OrgNodeExFieldDTO fieldDTO)
        {
            fieldDTO.NullThrowArgumentEx("field dto is not assigned.");

            this.chartFacade.UpdateField(Mapper.Map<OrgNodeExFieldDTO, OrgNodeExtend>(fieldDTO));
        }

        ///<summary>
        ///Check whether specific organization node is child of specific node
        ///</summary>
        ///<param name="parentId">Specific organization parent node id</param>
        ///<param name="nodeId">Specific organization child node id</param>
        ///<returns>True or false</returns>
        public bool ChildOf(Guid parentId, Guid nodeId)
        {
            return this.chartFacade.ChildOf(parentId, nodeId);
        }

        ///<summary>
        ///Retrieves the all childs of the specified node.
        ///</summary>
        ///<param name="node">The specified node id</param>
        ///<returns>The matching child nodes list</returns>
        public IEnumerable<OrgNodeWithRelationshipsDto> GetChildNodes(Guid nodeId)
        {
            return Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeWithRelationshipsDto>>(this.chartFacade.GetChildNodes(nodeId));
        }

        ///<summary>
        ///Retrieves all charts.
        ///</summary>
        ///<returns>All charts list</returns>
        public IEnumerable<OrgChartWithRelationshipsDto> GetAllCharts()
        {
            return Mapper.Map<IEnumerable<OrgChart>, IEnumerable<OrgChartWithRelationshipsDto>>(this.chartFacade.GetAllCharts());
        }

        ///<summary>
        ///Retrieves node's users according to the specified query expression.
        ///</summary>
        ///<param name="expression">The specified expression</param>
        ///<returns>The matching nodes users list</returns>
        public IEnumerable<UserBaseDto> GetUsers(QueryExpression expression)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.chartFacade.GetUsers(expression));
        }

        ///<summary>
        ///Retrieves nodes according to the specified query expression.
        ///</summary>
        ///<param name="expression">The specified expression</param>
        ///<returns>The matching nodes list</returns>
        public IEnumerable<OrgNodeWithRelationshipsDto> GetNodes(QueryExpression expression)
        {
            return Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeWithRelationshipsDto>>(this.chartFacade.GetNodes(expression));
        }

        ///<summary>
        ///Retrieves node's positions according to the specified query expression.
        ///</summary>
        ///<param name="expression">The specified expression</param>
        ///<returns>The matching nodes positions list</returns>
        public IEnumerable<PositionWithRelationshipsDto> GetPositions(QueryExpression expression)
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithRelationshipsDto>>(this.chartFacade.GetPositions(expression));
        }

    }
}
