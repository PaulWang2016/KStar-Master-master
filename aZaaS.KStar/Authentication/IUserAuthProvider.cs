using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Authentication
{
    public interface IUserAuthProvider
    {
        string LoginName(string userName);
        bool Authenticate(string userName, string password);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        void ParameterMapValidator(Dictionary<string, string> parameters);
    }
}
