using System;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    public interface IEntityWithMetadata
    {
        //bool? ConvertEmptyStringToNull { get; set; }
        ReadOnlyDictionary<string, string> Metadata { get; }
        //string DisplayName { get; set; }
        //string ShortDisplayName { get; set; }
        //string TemplateHint { get; set; }
        //string Description { get; set; }
        //bool? IsReadOnly { get; set; }
        //bool? IsRequired { get; set; }
        //bool? HideSurroundingHtml { get; set; }
        //bool? RequestValidationEnabled { get; set; }
        //bool ShowForDisplay { get; set; }
        //bool? ShowForEdit { get; set; }
        //string NullDisplayText { get; set; }
        //string Watermark { get; set; }
        //int? Order { get; set; }
        //string DisplayFormatString { get; set; }
        //string EditFormatString { get; set; }
        //bool ApplyFormatInEditMode { get; set; }
        void SetOrOverrideMetadata(string name, object value);
        void StopOverridingMetadata(string name);
        bool IsOverridingMetadata(string name);
        bool TryGetMetadata<TType>(string name, out TType metadata);
        T GetMetadataOrDefault<T>(string name);
    }
}