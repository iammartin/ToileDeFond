using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Client;
using ToileDeFond.ContentManagement;
using ToileDeFond.ContentManagement.RavenDB;
using ToileDeFond.ContentManagement.Reflection;
using ToileDeFond.Modularity;
using ToileDeFond.Utilities.RavenDB;

namespace ToileDeFond.Website.Administration
{
    [PrioritisedExport(typeof(IPropertyEditorRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PropertyEditorRepository : IPropertyEditorRepository
    {
        private readonly IReflectionContentManager _reflectionContentManager;
        private readonly IDocumentSession _documentSession;
        private readonly IContentPublicationStateManager _contentPublicationStateManager;
        private readonly IContentPublicationDateTimeManager _contentPublicationDateTimeManager;

        //TODO: Faire en sorte que s'il n'y a pas de ImportingConstructor on prend le eager constructor - voir comme pour le controllers

        public PropertyEditorRepository(IReflectionContentManager contentManager, IDocumentSession documentSession,
            IContentPublicationStateManager contentPublicationStateManager,
            IContentPublicationDateTimeManager contentPublicationDateTimeManager)
        {
            _reflectionContentManager = contentManager;
            _documentSession = documentSession;
            _contentPublicationStateManager = contentPublicationStateManager;
            _contentPublicationDateTimeManager = contentPublicationDateTimeManager;
        }

        [ImportingConstructor]
        public PropertyEditorRepository(IReflectionContentManager contentManager, IDocumentSession documentSession,
          [ImportMany]IEnumerable<Lazy<IContentPublicationStateManager, IPrioritisedMefMetaData>> contentPublicationStateManagers,
        [ImportMany]IEnumerable<Lazy<IContentPublicationDateTimeManager, IPrioritisedMefMetaData>> contentPublicationDateTimeManagers)
            : this(contentManager, documentSession,
            contentPublicationStateManagers.OrderByDescending(x => x.Metadata.Priority).First().Value,
            contentPublicationDateTimeManagers.OrderByDescending(x => x.Metadata.Priority).First().Value)
        {

        }

        public void Dispose()
        {
            _reflectionContentManager.Dispose();
            //TODO: Enlever tout les IDisposable et utiliser un sessionfactory partout
        }

        public IPropertyEditor GetPropertyEditorByName(string name)
        {
            var propertyEditor = _documentSession.Advanced.LuceneQuery<Content, PropertyEditorIndex>()
                .WhereEquals("Name", name)
                .AddContentManagementQueryTerms(_contentPublicationStateManager, _contentPublicationDateTimeManager)
                         .SelectFields<PropertyEditor>().FirstOrDefault();

            return propertyEditor;
        }

        public IPropertyEditor[] GetAll()
        {
          return  _documentSession.Advanced.LuceneQuery<Content, PropertyEditorIndex>()
                    .AddContentManagementQueryTerms(_contentPublicationStateManager, _contentPublicationDateTimeManager)
                    .SelectFields<PropertyEditor>().ToArray();
        }

        public void AddPropertyEditor(IPropertyEditor propertyEditor, Publication publication = null)
        {
            var contentReport = _reflectionContentManager.GetNewOrUpdatedContent(propertyEditor as PropertyEditor);

            if (publication != null)
                contentReport.Item.CreateTranslationVersions(publication);

            _reflectionContentManager.Store(contentReport.Item);
        }

        public void SaveChanges()
        {
            _reflectionContentManager.SaveChanges();
        }

        public void DeleteAllPropertyEditors()
        {
            RavenDBUtilities.WaitForStaleIndexes(_documentSession.Advanced.DocumentStore);

            _documentSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("PropertyEditorIndex", new IndexQuery());

            RavenDBUtilities.WaitForStaleIndexes(_documentSession.Advanced.DocumentStore);
        }
    }
}