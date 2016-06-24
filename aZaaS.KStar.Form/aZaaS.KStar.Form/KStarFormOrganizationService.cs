using System.Collections.Generic;
using AutoMapper;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form
{
    public class KStarFormOrganizationService : IOrganizationService
    {
        private readonly UserService _userService;

        public KStarFormOrganizationService()
        {
            _userService = new UserService();
        }

        public UserModel GetUserInfo(string userName)
        {
            var userInfo = GetUser(userName);

            if (userInfo == null)
            {
                return new UserModel();
            }

            var userModel = new UserModel()
            {
                ApplicantAccount = userInfo.UserName,
                ApplicantDisplayName = string.Format("{0} {1}",userInfo.FirstName,userInfo.LastName),
                ApplicantTelNo = userInfo.Phone,
                ApplicantEmail = userInfo.Email,
                Positions = GetPositions(userName),
                Departments = GetDepartments(userName)
            };

            return userModel;
        }

        public UserBaseDto GetUser(string userName)
        {
            return _userService.ReadUserBase(userName);
        }

        public UserDto GetUserData(string userName)
        {
            var userInfo = _userService.ReadUserInfo(userName);

            return userInfo;
        }

        public string GetDisplayName(string userName)
        {
            var userInfo = GetUser(userName);

            if (userInfo == null)
            {
                return string.Empty;
            }

            return userInfo.FullName;
        }

        private IList<ComboxContext> GetPositions(string userName)
        {
            var userPosition = _userService.ReadUserWithPositions(userName);

            var positions = Mapper.Map<IList<PositionWithFieldsDto>, IList<ComboxContext>>(userPosition.Positions);

            return positions;
        }

        private IList<ComboxContext> GetDepartments(string userName)
        {
            var userNode = _userService.ReadUserWithNodes(userName);

            var nodes = Mapper.Map<IList<OrgNodeWithChartDto>, IList<ComboxContext>>(userNode.Nodes);

            return nodes;
        }
    }
}
