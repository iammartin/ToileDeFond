using System;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement.Reflection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IdAttribute : Attribute
    {
        private readonly Guid _id;

        public IdAttribute(string id)
        {
            if (!id.IsGuid(out _id))
                throw new ArgumentException("Is not a valid GUID", "id");
        }

        public IdAttribute(Guid? id = null)
        {
            id = id ?? Guid.NewGuid();

            if(id == Guid.Empty)
                throw new ArgumentException("Is not a valid GUID", "id");

            _id = id.Value;
        }

        public Guid Id
        {
            get { return _id; }
        }
    }
}