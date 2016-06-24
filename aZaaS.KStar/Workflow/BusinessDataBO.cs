using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using aZaaS.Framework;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Facade;

namespace aZaaS.KStar
{
    public class BusinessDataBO
    {
        private readonly BusinessDataFacade dataFacade;

        public BusinessDataBO()
        {
            this.dataFacade = new BusinessDataFacade();
        }

        public Guid CreateConfig( BusinessDataConfigDTO configDTO)
        {
            return this.dataFacade.CreateConfig(new ServiceContext(), Mapper.Map<BusinessDataConfigDTO,BusinessDataConfig>(configDTO));
        }

        public bool ConfigExists( string processName)
        {
            return this.dataFacade.ConfigExists(new ServiceContext(), processName);
        }

        public bool ConfigColumnExists(Guid configId, string displayName)
        {
            return this.dataFacade.ConfigColumnExists(new ServiceContext(), configId, displayName);
        }

        public BusinessDataConfigDTO ReadConfig( Guid configId)
        {
            return Mapper.Map<BusinessDataConfig,BusinessDataConfigDTO>( this.dataFacade.ReadConfig(new ServiceContext(), configId));
        }

        public BusinessDataConfigDTO ReadConfig( string processName)
        {
            return Mapper.Map<BusinessDataConfig,BusinessDataConfigDTO>(this.dataFacade.ReadConfig(new ServiceContext(), processName));
        }

        public void UpdateConfig( BusinessDataConfigDTO configDTO)
        {
            this.dataFacade.UpdateConfig(new ServiceContext(), Mapper.Map<BusinessDataConfigDTO,BusinessDataConfig>(configDTO));
        }

        public void RemoveConfig(Guid configId)
        {
            this.dataFacade.RemoveConfig(new ServiceContext(), configId);
        }

        public void RemoveConfig(IEnumerable<Guid> configIds)
        {
            this.dataFacade.RemoveConfig(new ServiceContext(), configIds);
        }

        public void AppendColumn( Guid configId, BusinessDataColumnDTO columnDTO)
        {
            this.dataFacade.AppendColumn(new ServiceContext(), configId, Mapper.Map<BusinessDataColumnDTO,BusinessDataColumn>(columnDTO));
        }

        public void RemoveColumn( Guid configId, Guid columnId)
        {
            this.dataFacade.RemoveColumn(new ServiceContext(), configId, columnId);
        }

        public void RemoveColumn(Guid columnId)
        {
            this.dataFacade.RemoveColumn(new ServiceContext(), columnId);
        }

        public void RemoveColumn(IEnumerable<Guid> columnIds)
        {
            this.dataFacade.RemoveColumn(new ServiceContext(), columnIds);
        }

        public void UpdateColumn( BusinessDataColumnDTO column)
        {
            this.dataFacade.UpdateColumn(new ServiceContext(), Mapper.Map<BusinessDataColumnDTO,BusinessDataColumn>(column));
        }

        public IEnumerable<BusinessDataConfigDTO> GetAllConfigs()
        {
            return Mapper.Map<IEnumerable<BusinessDataConfig>,IEnumerable<BusinessDataConfigDTO>>(this.dataFacade.GetAllConfigs(new ServiceContext()));
        }

        public IEnumerable<BusinessDataColumnDTO> GetConfigColumns( Guid configId)
        {
            return Mapper.Map<IEnumerable<BusinessDataColumn>,IEnumerable<BusinessDataColumnDTO>>(this.dataFacade.GetConfigColumns(new ServiceContext(), configId));
        }

        public IEnumerable<BusinessDataField> FetchConfigFields(Guid configId)
        {
            return this.dataFacade.FetchConfigFields(new ServiceContext(), configId);
        }

        public IEnumerable<BusinessDataField> FetchConfigFields(string tableOrView, string connectionString)
        {
            return this.dataFacade.FetchConfigFields(new ServiceContext(), tableOrView, connectionString);
        }
    }
}
