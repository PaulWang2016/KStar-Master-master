using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;

using aZaaS.Framework.Facade;
using aZaaS.Framework.Logging;

namespace aZaaS.KStar.Authentication
{
    public static class KStarUserAuthenticator
    {
        private static bool _initialized;
        private static bool _userExistsValidation = true;
        private static IUserAuthProvider _authProvider;
        private static Dictionary<string, IUserAuthProvider> _authProviders;
        private static Dictionary<string, Dictionary<string, string>> _parameters;
        private static readonly object _syncLock = new object();

        public static void Initialize()
        {
            if (_initialized && _authProvider != null && _authProviders != null)
                return;

            lock (_syncLock)
            {
                if (!_initialized || _authProvider == null || _authProviders == null)
                {
                    var authProviderSection = ConfigurationManager.GetSection("userAuthProvider") as UserAuthProviderSection;

                    if (authProviderSection == null)
                        throw new ArgumentException("Invalid auth provider configuration!");
                    if (string.IsNullOrEmpty(authProviderSection.DefaultProvider))
                        throw new ArgumentException("Default provider is not assigned!");
                    if (authProviderSection.AuthProviders.Count == 0)
                        throw new ArgumentException("Please setting auth provider under the <authProviders> section!");

                    _authProviders = new Dictionary<string, IUserAuthProvider>();
                    _parameters = new Dictionary<string, Dictionary<string, string>>();

                    _userExistsValidation = authProviderSection.UserExistsValidation;

                    foreach (AuthProviderElement item in authProviderSection.AuthProviders)
                    {
                        Type providerType = Type.GetType(item.AssemblyType, true, true);
                        if (!typeof(IUserAuthProvider).IsAssignableFrom(providerType))
                            throw new ArgumentException("Auth provider type must be implemented by IUserAuthProvider!");

                        var authProvider = Activator.CreateInstance(providerType) as IUserAuthProvider;

                        var paramMap = new Dictionary<string, string>();
                        foreach (ParameterElement parameter in item.Parameters)
                        {
                            paramMap.Add(parameter.Key, parameter.Value);
                        }

                        _parameters.Add(authProvider.GetType().Name, paramMap);
                        _authProviders.Add(item.Name, authProvider);
                    }

                    if (!_authProviders.ContainsKey(authProviderSection.DefaultProvider))
                        throw new InvalidOperationException("The default provider was not found!");

                    _authProvider = _authProviders[authProviderSection.DefaultProvider];

                    _initialized = true;
                }
            }
        }

        public static IUserAuthProvider AuthProvider
        {
            get
            {
                if (_authProvider == null)
                    throw new InvalidOperationException("The AuthProvider is not assigned!");

                return _authProvider;
            }
        }

        public static Dictionary<string, IUserAuthProvider> AuthProviders
        {
            get
            {
                if (_authProviders == null)
                    throw new InvalidOperationException("The AuthProviders is not assigned!");

                return _authProviders;
            }
        }


        public static bool UserExists(string userName)
        {
            var userBO = new UserBO();

            return !_userExistsValidation ? true : userBO.UserNameExists(userName); 
        }

        public static bool Authenticate(string userName, string password)
        {
            return AuthProvider.Authenticate(userName, password);
        }

        public static bool Authenticate(string userName, string password, out string message)
        {
            bool success = false;
            message = string.Empty;

            try
            {
                success = AuthProvider.Authenticate(userName, password);
            }
            catch (Exception ex)
            {
                //TODO:Support multi-language

                if (ex is UserNotExistsException)
                    message = string.Format("The sepecified user {0} is not exists!",(ex as UserNotExistsException).UserName);
                else if (ex is InvalidParameterException)
                    message = string.Format("The configured parameter {0} is not valid!", (ex as InvalidParameterException).ParameterName);
                else
                    message = string.Format("Unknown error,please contact administrator!");
               
                TraceException(ex);
                return success;
            }

            message = success ? string.Empty : "Invalid credentials!";
            return success;
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            return AuthProvider.ChangePassword(userName, oldPassword, newPassword);
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword,out string message)
        {
            var success = false;
            message = string.Empty;

            try
            {
                success = AuthProvider.ChangePassword(userName, oldPassword, newPassword);
            }
            catch (Exception ex)
            {
                //TODO:Support multi-language

                if (ex is UserNotExistsException)
                    message = string.Format("The sepecified user {0} is not exists!");
                else if (ex is InvalidParameterException)
                    message = string.Format("The configured parameter {0} is not valid!");
                else if (ex is PasswordNotMatchException)
                    message = string.Format("The specified old password is not match!");
                else
                    message = string.Format("Unknown error,please contact administrator!");

                TraceException(ex);
                return success;
            }

            message = success ? string.Empty : "fail";
            return success;
        }

        public static string LoginName(string userName)
        {
            return AuthProvider.LoginName(userName);
        }

        public static string GetParameter(string parameterName)
        {
            var parameters = KStarUserAuthenticator.GetParameterMap(AuthProvider.GetType(),AuthProvider.ParameterMapValidator);

            return parameters.ContainsKey(parameterName) ? parameters[parameterName] : string.Empty;
        }

        public static Dictionary<string, string> GetParameterMap(Type providerType, Action<Dictionary<string, string>> parameterMapValidator = null)
        {
            if (!_parameters.ContainsKey(providerType.Name))
                throw new InvalidOperationException("The specified provider type was not found!");

            var parameters = _parameters[providerType.Name];

            if (parameterMapValidator != null)
                parameterMapValidator(parameters);

            return parameters;
        }

        internal static void TraceException(Exception ex)
        {
            LogFactory.GetLogger().Write(new LogEvent
            {
                Source = "aZaaS.KStar",
                Category = "KStar User Authentication",
                Exception = ex,
                Message = ex.Message,
                OccurTime = DateTime.Now
            });
        }
    }
    

}
