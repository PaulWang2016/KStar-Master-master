using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace aZaaS.KStar.CustomRole
{
    public sealed class CustomRoleManager
    {
        private AppDomain _appDomain;
        private CustomRoleLoader _customRoleLoader;
        private static bool _hasInitialized = false;
        private readonly static CustomRoleManager _current = new CustomRoleManager();
        private FileSystemWatcher _watcher;

        private CustomRoleManager()
        {
        }

        public static CustomRoleManager Current
        {
            get
            {
                return _current;
            }
        }

        public void StartRoleLoader()
        {
            if (!_hasInitialized)
            {
                var assemblyDirectory = CustomRoleCfg.AssemblyDirectory;
                if (!Directory.Exists(assemblyDirectory))
                    Directory.CreateDirectory(assemblyDirectory);

                var assemblyCacheDirectory = CustomRoleCfg.AssemblyCacheDirectory;
                if (!Directory.Exists(assemblyCacheDirectory))
                    Directory.CreateDirectory(assemblyCacheDirectory);

                var setup = new AppDomainSetup
                {
                    ApplicationName = CustomRoleCfg.CUSTOMROLE_APPNAME,
                    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                    PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"bin"),
                    CachePath = assemblyCacheDirectory,
                    ShadowCopyFiles = "true",
                    ShadowCopyDirectories = assemblyDirectory,
                    ConfigurationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config")
                };

                _appDomain = AppDomain.CreateDomain(CustomRoleCfg.CUSTOMROLE_DOMAINNAME, AppDomain.CurrentDomain.Evidence, setup);
                _customRoleLoader = (CustomRoleLoader)_appDomain.CreateInstanceAndUnwrap(typeof(CustomRoleLoader).Assembly.FullName, typeof(CustomRoleLoader).FullName);

                _hasInitialized = true;
            }
        }

        public void RestartRoleLoader()
        {
            //NOTE:
            //There are two ways to refresh the CustomRole assemblies.
            //OPTION 1:
            //1-1.After you uploaded your dll,unload the appdomain and renew one.
            //OPTION 2:
            //2-1.After you uploaded you dll,copy the dlls to a temp folder and delete all source dlls.
            //2-2:Invoke the Loader.RefreshRoleParts()  
            //2-3:Copy back the source dlls to the source folder( RootDir\CustomRoles).
            //2-4:Invoke the Loader.RefreshRoleParts() again.

            StopRoleLoader();

            _hasInitialized = false;
            StartRoleLoader();
        }

        public IEnumerable<ICustomRole> CustomRoles
        {
            get
            {
                return _customRoleLoader.AttachedCustomRoles;
            }
        }

        public ICustomRole GetService(Guid key)
        {
            return CustomRoles.FirstOrDefault(r => r.Key == key);
        }

        public void StopRoleLoader()
        {
            if (_hasInitialized && _appDomain != null)
            {
                AppDomain.Unload(_appDomain);
            }
        }

        public void StartRoleWatch()
        {
            string path = CustomRoleCfg.AssemblyDirectory;

            _watcher = new FileSystemWatcher(path, "*.dll");
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;

            _watcher.Created += new FileSystemEventHandler(OnChanged);
            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnChanged);
            
        }

        public void StopRoleWatch()
        {
            _watcher.Dispose();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _watcher.EnableRaisingEvents = false;
            
            RestartRoleLoader();

            _watcher.EnableRaisingEvents = true;  
        }

    }
}
