using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using aZaaS.Framework;
using aZaaS.Framework.Organization.OrgChart;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.Framework.Organization.Facade;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Extend;

namespace aZaaS.KStar.MgmtServices
{
    public class OrgChartService
    {
        private readonly OrgChartFacade chartFacade;

        public OrgChartService()
        {
            this.chartFacade = new OrgChartFacade();
        }

        public OrgChartWithRelationshipsDto ReadChart(string chartName)
        {
            return Mapper.Map<OrgChart, OrgChartWithRelationshipsDto>(this.chartFacade.ReadChart(chartName));
        }
        public OrgChartBaseDto ReadChartBase(Guid chartId)
        {
            return Mapper.Map<OrgChart, OrgChartBaseDto>(this.chartFacade.ReadChart(chartId));
        }
        public OrgChartWithRootDto ReadChartWithRoot(string chartName)
        {
            return Mapper.Map<OrgChart, OrgChartWithRootDto>(this.chartFacade.ReadChart(chartName));
        }
        public OrgChartWithRootDto ReadChartWithRoot(Guid chartId)
        {
            return Mapper.Map<OrgChart, OrgChartWithRootDto>(this.chartFacade.ReadChart(chartId));
        }
        public IEnumerable<OrgChartBaseDto> GetAllChartBases()
        {
            return Mapper.Map<IEnumerable<OrgChart>, IEnumerable<OrgChartBaseDto>>(this.chartFacade.GetAllCharts());
        }
        public OrgChartWithRelationshipsDto GetChartWithNodes(Guid chartId)
        {
            return Mapper.Map<OrgChart, OrgChartWithRelationshipsDto>(this.chartFacade.ReadChartWithNodes(chartId));
        }
        public IEnumerable<OrgNodeBaseDto> GetAllNodes()
        {
            return Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeBaseDto>>(this.chartFacade.GetAllNodes());
        }

        public OrgNodeWithRelationshipsDto ReadNode(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeWithRelationshipsDto>(this.chartFacade.ReadNodeWithFields(nodeId));
        }
        public OrgNodeBaseDto ReadNodeBase(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeBaseDto>(this.chartFacade.ReadNode(nodeId));
        }
        public OrgNodeWithChildNodesDto ReadNodeWithChildNodes(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeWithChildNodesDto>(this.chartFacade.ReadNode(nodeId));
        }
        public OrgNodeWithFieldsDto ReadNodeWithFields(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeWithFieldsDto>(this.chartFacade.ReadNodeWithFields(nodeId));
        }
        public OrgNodeWithParentDto ReadNodeWithParent(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeWithParentDto>(this.chartFacade.ReadNode(nodeId));
        }
        public OrgNodeWithUsersDto ReadNodeWithUsers(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeWithUsersDto>(this.chartFacade.ReadNode(nodeId));
        }
        public OrgNodeWithPositionsDto ReadNodeWithPositions(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeWithPositionsDto>(this.chartFacade.ReadNode(nodeId));
        }
        public OrgNodeWithChartDto ReadNodeWithChart(Guid nodeId)
        {
            return Mapper.Map<OrgNode, OrgNodeWithChartDto>(this.chartFacade.ReadNode(nodeId));
        }
        public IEnumerable<OrgNodeWithParentDto> GetNodesWithParent(QueryExpression expression)
        {
            return Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeWithParentDto>>(this.chartFacade.GetNodes(expression));
        }
        public IEnumerable<OrgNodeWithFieldsDto> GetNodesWithFields(QueryExpression expression)
        {
            return Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeWithFieldsDto>>(this.chartFacade.GetNodes(expression));
        }
        public IEnumerable<OrgNodeWithFieldsDto> GetChildNodesWithFields(Guid nodeId)
        {
            return Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeWithFieldsDto>>(this.chartFacade.GetChildNodes(nodeId));
        }

        public FieldBase[] GetNodeExtendFieldsDefition()
        {
            return this.chartFacade.GetExtendFiledsDefintion();
        }

        public void SaveNodeExtendFieldsDefition(FieldBase[] fields)
        {
            this.chartFacade.SaveExtendFiledsDefintion(fields);
        }
    }
}
