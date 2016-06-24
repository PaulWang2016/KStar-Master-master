using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IOrganizationService
    {
        UserModel GetUserInfo(string userName);

        UserBaseDto GetUser(string userName);

        UserDto GetUserData(string userName);

        string GetDisplayName(string userName);
    }
}
