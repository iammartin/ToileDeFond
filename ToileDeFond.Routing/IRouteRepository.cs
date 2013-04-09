using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ToileDeFond.ContentManagement;

namespace ToileDeFond.Routing
{
    public interface IRouteRepository : IDisposable
    {
        IRoute GetRouteByRewriteFromUrl(string rewriteFromUrl);
        void AddRoute(IRoute route, Publication publication = null);
        void SaveChanges();
        void DeleteAllRoutes();
    }
}
