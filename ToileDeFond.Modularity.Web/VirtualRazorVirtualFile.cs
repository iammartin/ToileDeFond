using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace ToileDeFond.Modularity.Web
{
    public class VirtualRazorVirtualFile: VirtualFile
    {
        private readonly string _path;

        public VirtualRazorVirtualFile(string virtualPath)
            : base(virtualPath)
        {
            _path = virtualPath;
        }

        public override Stream Open()
        {
            return VirtualFileHelper.GetResourceStream(_path);
        }
    }
}
