using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Hosting;

namespace ToileDeFond.Modularity.Web
{
    public class ModuleVirtualPathProvider : VirtualPathProvider
    {
        public ModuleVirtualPathProvider()
        {
            _fileHashes = new Dictionary<string, string>();
        }

        public override bool FileExists(string virtualPath)
        {
            bool fileExist;

            if (VirtualFileHelper.IsModuleRessource(virtualPath))
            {
                // File exist
                fileExist = base.FileExists(VirtualFileHelper.GetRealFileName(virtualPath, true));

                if (!fileExist)
                    fileExist = VirtualFileHelper.ModuleFileExists(virtualPath);
            }
            else
                fileExist = base.FileExists(virtualPath);

            return fileExist;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (VirtualFileHelper.IsModuleRessource(virtualPath))
            {
                var fileExist = base.FileExists(VirtualFileHelper.GetRealFileName(virtualPath, true));

                if (fileExist)
                    return new VirtualPhysicalFile(virtualPath);

                return new VirtualRazorVirtualFile(virtualPath);
            }

            return base.GetFile(virtualPath);
        }

        //Important sinon mvc regarde dans le répertoire si le fichier change...
        //TODO: Optimiser
        #region CacheDependency

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            return VirtualFileHelper.IsModuleRessource(virtualPath) ? GetFileHash(virtualPath) :
                base.GetFileHash(virtualPath, virtualPathDependencies);
        }

        // TODO: Hein?
        private readonly Dictionary<string, string> _fileHashes;
        public string GetFileHash(string virtualPath)
        {
            lock (this)
            {
                string hashValue;
                if (_fileHashes.TryGetValue(virtualPath, out hashValue))
                    return hashValue;

                hashValue = Guid.NewGuid().ToString("N");
                _fileHashes.Add(virtualPath, hashValue);

                return hashValue;
            }
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return VirtualFileHelper.IsModuleRessource(virtualPath) ? null : base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        #endregion
    }
}
