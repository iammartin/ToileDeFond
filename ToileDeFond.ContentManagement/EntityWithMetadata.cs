using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement
{
    //TODO: Va falloir utiliser des JToken/dynamic pour le metadata aussi
    public abstract class EntityWithMetadata<T> : Entity<T>, IEntityWithMetadata where T : Entity<T>, IEntityWithMetadata
    {
        private readonly Dictionary<string, string> _metadata;

        protected EntityWithMetadata(Guid? id = null, IDictionary<string, string> metadata = null)
            : base(id)
        {
            _metadata = metadata == null ? new Dictionary<string, string>() : new Dictionary<string, string>(metadata);
        }

        protected bool TryGetOwnMetadata<TType>(string name, out TType metadata)
        {
            string value;

            if (_metadata.TryGetValue(name, out value))
            {
                if (value == null)
                {
                    metadata = (TType)typeof(TType).GetDefaultValue();
                }
                else
                {
                    metadata = JsonConvert.DeserializeObject<TType>(value, 
                        new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
                }

                return true;
            }

            metadata = (TType)typeof(TType).GetDefaultValue();

            return false;
        }

        protected bool TryGetOwnMetadata(string name, Type type, out object metadata)
        {
            string value;

            if (_metadata.TryGetValue(name, out value))
            {
                if (value == null)
                {
                    metadata = type.GetDefaultValue();
                }
                else
                {
                    metadata = JsonConvert.DeserializeObject(value, type, 
                        new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
                }

                return true;
            }

            metadata = type.GetDefaultValue();

            return false;
        }

        protected bool TryGetOwnSerializedMetadata(string name, out string metadata)
        {
            return _metadata.TryGetValue(name, out metadata);
        }

        public ReadOnlyDictionary<string, string> Metadata
        {
            get { return _metadata.AsReadOnly(); }
        }

        public void SetOrOverrideMetadata(string name, object value)
        {
            if (value == null)
                _metadata[name] = null;
            else
                _metadata[name] = JsonConvert.SerializeObject(value, 
                    new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
        }

        public void SetOrOverrideSerializedMetadata(string name, string value)
        {
            if (value == null)
                _metadata[name] = null;
            else
                _metadata[name] = value;
        }

        public void StopOverridingMetadata(string name)
        {
            _metadata.Remove(name);
        }

        public bool IsOverridingMetadata(string name)
        {
            return _metadata.ContainsKey(name);
        }

        public virtual bool TryGetMetadata<TType>(string name, out TType metadata)
        {
            return TryGetOwnMetadata(name, out metadata);
        }

        public virtual bool TryGetMetadata(string name, Type type, out object metadata)
        {
            return TryGetOwnMetadata(name, type, out metadata);
        }

        public virtual bool TryGetSerializedMetadata(string name, out string metadata)
        {
            return TryGetOwnSerializedMetadata(name, out metadata);
        }

        public  string GetSerializedMetadata(string name)
        {
            string value;
            if (TryGetSerializedMetadata(name, out value))
            {
                return value;
            }

            throw new InvalidOperationException();
        }

        public virtual TType GetMetadataOrDefault<TType>(string name)
        {
            TType value;

            if (TryGetMetadata(name, out value))
            {
                return value;
            }

            return default(TType);
        }

        public virtual object GetMetadataOrDefault(string name, Type type)
        {
            object value;

            if (TryGetMetadata(name, type, out value))
            {
                return value;
            }

            return type.GetDefaultValue();
        }


        //#region Default Metadata

        //public virtual string DisplayName
        //{
        //    get { return GetMetadataOrDefault<String>("DisplayName"); }
        //    set { SetOrOverrideMetadata("DisplayName", value); }
        //}

        //public virtual string ShortDisplayName
        //{
        //    get { return GetMetadataOrDefault<String>("ShortDisplayName"); }
        //    set { SetOrOverrideMetadata("ShortDisplayName", value); }
        //}

        //public virtual string TemplateHint
        //{
        //    get { return GetMetadataOrDefault<String>("TemplateHint"); }
        //    set { SetOrOverrideMetadata("TemplateHint", value); }
        //}

        //public virtual string Description
        //{
        //    get { return GetMetadataOrDefault<String>("Description"); }
        //    set { SetOrOverrideMetadata("Description", value); }
        //}

        //public virtual bool? IsReadOnly
        //{
        //    get { return GetMetadataOrDefault<bool?>("IsReadOnly"); }
        //    set { SetOrOverrideMetadata("IsReadOnly", value); }
        //}

        //public virtual bool? IsRequired
        //{
        //    get { return GetMetadataOrDefault<bool?>("IsRequired"); }
        //    set { SetOrOverrideMetadata("IsRequired", value); }
        //}

        //public virtual bool? HideSurroundingHtml
        //{
        //    get { return GetMetadataOrDefault<bool?>("HideSurroundingHtml"); }
        //    set { SetOrOverrideMetadata("HideSurroundingHtml", value); }
        //}

        //public virtual bool? RequestValidationEnabled
        //{
        //    get { return GetMetadataOrDefault<bool?>("RequestValidationEnabled"); }
        //    set { SetOrOverrideMetadata("RequestValidationEnabled", value); }
        //}

        //public virtual bool ShowForDisplay
        //{
        //    get { return GetMetadataOrDefault<bool>("ShowForDisplay"); }
        //    set { SetOrOverrideMetadata("ShowForDisplay", value); }
        //}

        //public virtual bool? ShowForEdit
        //{
        //    get { return GetMetadataOrDefault<bool?>("ShowForEdit"); }
        //    set { SetOrOverrideMetadata("ShowForEdit", value); }
        //}

        //public virtual string NullDisplayText
        //{
        //    get { return GetMetadataOrDefault<String>("NullDisplayText"); }
        //    set { SetOrOverrideMetadata("NullDisplayText", value); }
        //}

        //public virtual string Watermark
        //{
        //    get { return GetMetadataOrDefault<String>("Watermark"); }
        //    set { SetOrOverrideMetadata("Watermark", value); }
        //}

        //////public IList<IModelValidationMetadata> Validations
        //////{
        //////    get;
        //////    private set;
        //////}


        //////public IList<IModelMetadataAdditionalSetting> AdditionalSettings
        //////{
        //////    get;
        //////    private set;
        //////}


        //public virtual int? Order
        //{
        //    get { return GetMetadataOrDefault<int?>("Order"); }
        //    set { SetOrOverrideMetadata("Order", value); }
        //}

        //public virtual string DisplayFormatString
        //{
        //    get { return GetMetadataOrDefault<String>("DisplayFormatString"); }
        //    set { SetOrOverrideMetadata("DisplayFormatString", value); }
        //}

        //public virtual string EditFormatString
        //{
        //    get { return GetMetadataOrDefault<String>("EditFormatString"); }
        //    set { SetOrOverrideMetadata("EditFormatString", value); }
        //}

        //public virtual bool ApplyFormatInEditMode
        //{
        //    get { return GetMetadataOrDefault<bool>("ApplyFormatInEditMode"); }
        //    set { SetOrOverrideMetadata("ApplyFormatInEditMode", value); }
        //}

        //public virtual bool? ConvertEmptyStringToNull
        //{
        //    get { return GetMetadataOrDefault<bool?>("ConvertEmptyStringToNull"); }
        //    set { SetOrOverrideMetadata("ConvertEmptyStringToNull", value); }
        //}

        //#endregion
    }
}