using System.Web.Optimization;

namespace ToileDeFond.Modularity.Web
{
    public interface IBundleConfig
    {
        void RegisterBundles(BundleCollection bundles);
    }
}
