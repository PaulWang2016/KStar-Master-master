using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.Infrastructure;

using aZaaS.KStar.Web.Areas.BusinessTrip.Data;
using aZaaS.KStar.Web.Areas.BusinessTrip.Models;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Services
{
    public class BusinessTripDataService
    {
        private StorageContext _dataContext;
        public KStarFormModel _formModel;

        static BusinessTripDataService()
        {
            //Mappding ViewModels with Entities
            Mapper.CreateMap<TravelRequestModel, TravelRequest>(); //This item not mapped here for demonstration.
            Mapper.CreateMap<TravelRequest, TravelRequestModel>()
                .ForMember(dest => dest.Schedules, meo => meo.ResolveUsing(bdc => bdc.ScheduleItems));
            Mapper.CreateMap<ScheduleItemModel, ScheduleItem>();
            Mapper.CreateMap<ScheduleItem, ScheduleItemModel>();
        }

        public BusinessTripDataService(StorageContext dataContext)
        {
            
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            _dataContext = dataContext;
            _formModel = _dataContext.FormModel;
        }

        public TravelRequestModel AddTravelRequest()
        {
            using (var db = new BusinessTripDBContext())
            {
                var formId = _dataContext.FormId;
                var departmentId = Guid.Parse(_formModel.ApplicantOrgNodeID);

                //Convert form data to target model dynamically by using DataModel<>
                var requestModel = _dataContext.DataModel<TravelRequestModel>();

                if (requestModel == null)
                    throw new InvalidCastException("Data conversion was failure!");

                //TIPS:
                //You can use AutoMapper or other tools to auto convert models to entitys

                //Setting TravelRequest properties
                var entity = new TravelRequest()
                {
                    FormId = formId,
                    Applicant = _formModel.ApplicantAccount,
                    ApplicantName = _formModel.ApplicantDisplayName,
                    DepartmentId = departmentId,
                    DepartmetName = _formModel.ApplicantOrgNodeName,
                    StartDate = requestModel.StartDate.ToLocalTime(),
                    BackDate = requestModel.BackDate.ToLocalTime(),
                    IsUsingCar = requestModel.IsUsingCar,
                    Phone = _formModel.ApplicantTelNo,
                    Entourage = requestModel.Entourage,
                    TotalDays = requestModel.TotalDays,
                    TravelReason = requestModel.TravelReason
                };

                //Setting SheduleItem properties
                var items = requestModel.Schedules;
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var itemEntity = new ScheduleItem()
                        {
                            FromDate = item.FromDate.ToLocalTime(),
                            ToDate = item.ToDate.ToLocalTime(),
                            Departure = item.Departure,
                            Destination = item.Destination,
                            Comment = item.Comment
                        };

                        entity.ScheduleItems.Add(itemEntity);
                    }
                }

                db.TravelRequests.Add(entity);
                db.SaveChanges();
  
                return Mapper.Map<TravelRequest,TravelRequestModel>(entity);
            }

        }


    }
}
