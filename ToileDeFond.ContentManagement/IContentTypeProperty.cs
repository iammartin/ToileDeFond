using System;

namespace ToileDeFond.ContentManagement
{
    public interface IContentTypeProperty : IEntityWithMetadata
    {
        bool IsCultureInvariant { get; }
        ContentType ContentType { get; }
        string Name { get; }
        string FullName { get; }
        Guid Id { get; }
        bool IsOwnProperty { get; }
        T GetDefaultValue<T>();
        IContentTypeProperty ParentProperty { get; }
        string SerializedDefaultValue { get;  }
        void SetDefaultValue(object value);
        ContentType.ContentTypeProperty GetOriginalContentTypeProperty();
        bool TryGetSerializedMetadata(string name, out string metadata);
        object GetDefaultValue(Type getType);
        bool TryGetMetadata(string name, Type type, out object metadata);
    }
}