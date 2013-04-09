using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace ToileDeFond.Modularity.Web
{
    public class VirtualPhysicalFile : VirtualFile
    {
        private readonly string _path;

        public VirtualPhysicalFile(string virtualPath)
            : base(virtualPath)
        {
            _path = virtualPath;
        }

        public override Stream Open()
        {
            var fileName = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, VirtualFileHelper.GetRealFileName(_path, false));

            if (!File.Exists(fileName))
                return null;

            var memoryStream = new MemoryStream();

            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(memoryStream);
                fileStream.Close();
            }

            return memoryStream;
        }
    }
}
