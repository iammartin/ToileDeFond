using System.Linq;
using NUnit.Framework;
using ToileDeFond.Modularity;
using ToileDeFond.Tests.Common;
using ToileDeFond.Tests.FakeModules.First;

namespace ToileDeFond.Tests.Integration.InMemory.Modularity
{
    [TestFixture]
    public class ModulatityTests
    {
        //Attention!! : Le project client doit absolument avoir une reference a System.ComponentModel.Composition (pas toujours demandé par le compilateur)
        //pour que l'injection de service se fasse... sinon c'est vide!

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var loadedModuleAssemblies = new Starter().Start();
        }

        [Test]
        public void GetServiceWithMultipleDependencyImplementationsWithCtorInjectectionAndImportMany()
        {
            var serviceWithDependencies = DependencyResolver.Current.GetService<IServiceWithDependencies>();
            var services = DependencyResolver.Current.GetServices<IService>().ToList();

            Assert.That(services.Count, Is.EqualTo(3));

            Assert.That(serviceWithDependencies, Is.Not.Null);
            Assert.That(serviceWithDependencies.Test3.Count(), Is.EqualTo(3));
            Assert.That(serviceWithDependencies.Services.Count(), Is.EqualTo(3));
        }
    }
}
