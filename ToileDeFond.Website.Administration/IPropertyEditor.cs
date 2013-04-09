using System;

namespace ToileDeFond.Website.Administration
{
    public interface IPropertyEditor
    {
        string GetRoute { get; set; }
        string PostRoute { get; set; }
        string Name { get; set; }
        string Id { get; set; }
    }
}