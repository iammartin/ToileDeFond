using System;
using System.Web.UI.WebControls;
using ToileDeFond.ContentManagement;

namespace ToileDeFond.Website.Administration
{
    public interface IPropertyEditorRepository : IDisposable
    {
        void DeleteAllPropertyEditors();
        void SaveChanges();
        void AddPropertyEditor(IPropertyEditor propertyEditor, Publication publication = null);
        IPropertyEditor GetPropertyEditorByName(string rewriteFromUrl);
        IPropertyEditor[] GetAll();
    }
}