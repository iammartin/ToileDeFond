using System;

namespace ToileDeFond.ContentManagement.Reflection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ContentTypeAttribute : Attribute
    {
        //private readonly Guid _id;

        //public ContentTypeAttribute(string id)
        //{
        //    if (!id.IsGuid(out _id))
        //        throw new ArgumentException("Is not a valid GUID", "id");
        //}

        //public ContentTypeAttribute(Guid id)
        //{
        //    _id = id;
        //}

        //public Guid Id
        //{
        //    get { return _id; }
        //}
    }
}