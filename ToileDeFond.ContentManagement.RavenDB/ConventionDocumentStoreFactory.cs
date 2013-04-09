using System.ComponentModel.Composition;
using System.Configuration;
using ToileDeFond.Modularity;

namespace ToileDeFond.ContentManagement.RavenDB
{
    [PrioritisedExport(typeof(IDocumentStoreFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConventionDocumentStoreFactory : DocumentStoreFactory
    {
        protected override string ConnectionStringName
        {
            get { return ConfigurationManager.AppSettings["DefaultConnectionString"]; }
        }
    }
}