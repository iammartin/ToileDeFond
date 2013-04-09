using System.Reflection;
using MefContrib.Hosting.Conventions.Configuration;

namespace ToileDeFond.Modularity
{
    public interface IPartRegistryRetriever
    {
        PartRegistry GetPartRegistryForAssembly(Assembly assembly);
    }
}