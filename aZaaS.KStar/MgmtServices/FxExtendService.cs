using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework;
using aZaaS.Framework.Organization.Extensions;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Organization.Facade;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.Framework.Extend;
using aZaaS.Framework.Organization.FieldExtend;


namespace aZaaS.KStar.MgmtServices
{
    public class FxExtendService
    {
        private readonly FxExtendFacade fxextendFacade;

        public FxExtendService()
        {
            this.fxextendFacade = new FxExtendFacade();
        }

        /// <summary>
        /// Creates a new Fx_Extend.
        /// </summary>
        /// <param name="role">The new Fx_Extend instance</param>
        /// <returns>The new Fx_Extend id</returns>
        public void CreateFxExtend(string name, FieldBase[] fields)
        {
            this.fxextendFacade.CreateFxExtend(name, fields);
        }

        /// <summary>
        /// Retrieves Fx_Extend according to the specified Fx_Extend name
        /// </summary>
        /// <param name="roleId">The specified Fx_Extend name</param>
        /// <returns>The matching Fx_Extend instance</returns>
        public FieldBase[] ReadFxExtend(string name)
        {
            return this.fxextendFacade.ReadFxExtend(name);
        }

        /// <summary>
        /// Updates the specified Fx_Extend.
        /// </summary>
        /// <param name="role">The specified Fx_Extend instance</param>
        public void UpdateFxExtend(string name, FieldBase[] fields)
        {
            this.fxextendFacade.UpdateFxExtend(name, fields);
        }

        /// <summary>
        /// Deletes a specified Fx_Extend.
        /// </summary>
        /// <param name="roleId">The specified Fx_Extend name</param>
        public void DeleteFxExtend(string name)
        {
            this.fxextendFacade.DeleteFxExtend(name);
        }

        /// <summary>
        /// Retrieves single Fx_Extend.
        /// </summary>
        /// <returns>Fx_Extend </returns>
        public Fx_ExtendDto GetFxExtends(string name)
        {
            return Mapper.Map<Fx_Extend, Fx_ExtendDto>(this.fxextendFacade.GetFxExtends(name));
        }

        /// <summary>
        /// Retrieves all Fx_Extend.
        /// </summary>
        /// <returns>All Fx_Extend list</returns>
        public IEnumerable<Fx_ExtendDto> GetAllFxExtends()
        {
            return Mapper.Map<IEnumerable<Fx_Extend>, IEnumerable<Fx_ExtendDto>>(this.fxextendFacade.GetAllFxExtends());
        }

        public void DeleteExtendField(string name, string fieldname)
        {
            this.fxextendFacade.DeleteExtendField(name, fieldname);
        }
    }
}
