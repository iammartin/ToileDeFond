using System;

namespace ToileDeFond.ContentManagement.Reflection
{
    public class ModuleInfo
    {
        private readonly string _name;
        private readonly string _installedVersion;
        private readonly string _currentVersion;
        private readonly Guid? _moduleId;
        private readonly ModuleStates _status;

        public ModuleInfo(string name, string installedVersion, string currentVersion, Guid? moduleId = null)
        {
            _name = name;
            _installedVersion = installedVersion;
            _currentVersion = currentVersion;
            _moduleId = moduleId;

            if(installedVersion == null)
            {
                _status = ModuleStates.NotYetInstalled;
            }
            else if(_currentVersion == null)
            {
                _status = ModuleStates.Unreferenced;
            }
            else if(installedVersion == _currentVersion)
            {
                _status = ModuleStates.UpToDate;
            }
            else
            {
                _status = ModuleStates.ToBeUpdated;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public string InstalledVersion
        {
            get { return _installedVersion; }
        }

        public string CurrentVersion
        {
            get { return _currentVersion; }
        }

        public ModuleStates Status { get { return _status; } }

        public Guid? ModuleId
        {
            get { return _moduleId; }
        }
    }
}