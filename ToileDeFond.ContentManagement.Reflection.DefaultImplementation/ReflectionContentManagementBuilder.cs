using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using ToileDeFond.Modularity;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement.Reflection.DefaultImplementation
{
    //TODO: Refactoring - bizarre les Add.. et Update... Car update peu refaire une action du Add (Ex. Modifier le nom, le default value, etc)
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    [PrioritisedExport(typeof(IReflectionModuleBuilder))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReflectionContentManagementBuilder : IReflectionModuleBuilder
    {
        private readonly DataAnnotationsModelMetadataProvider _dataAnnotationsModelMetadataProvider;

        public ReflectionContentManagementBuilder()
        {
            _dataAnnotationsModelMetadataProvider = new DataAnnotationsModelMetadataProvider();
        }

        public Module BuildModuleFromAssembly(Assembly assembly)
        {
            return UpdateModuleFromAssembly(assembly);
        }

        //TODO: Déplacer au niveau du contrat.. pas de détails d'implémentation mais juste du business logique dans cette méthode
        public Module UpdateModuleFromAssembly(Assembly assembly, Module module = null)
        {
            //TODO: Supporter les id sur module, type, propriété un jour - permet de renommer 
            //var moduleId = GetModuleIdFromAssembly(assembly); 
            var moduleName = GetModuleNameFromAssembly(assembly);
            var moduleVersion = GetModuleVersionFromAssembly(assembly);

            if (module == null)
            {
                module = new Module(moduleName, moduleVersion);
            }
            else
            {
                module.Version = moduleVersion;
            }

            UpdateModuleFromAssembly(module, assembly);

            return module;
        }


        //TODO: Tenir compte de si le type existe deja ds un module qui existe deja... meme chose pour les propriétés
        //TODO: Supporter les remove de types
        protected virtual void UpdateModuleFromAssembly(Module module, Assembly assembly)
        {
            var types = GetContentTypeTypesFromAssembly(assembly);

            foreach (var type in types)
            {
                UpdateContentTypeFromType(module, type);
            }
        }

        public virtual ContentType UpdateContentTypeFromType(Module module, Type type)
        {
            var contentTypeName = GetContentTypeNameFromType(type);

            ContentType contentType;

            if (!module.TryGetContentType(contentTypeName, out contentType))
            {
                //TODO: Héritage
                contentType = module.AddContentType(contentTypeName);
            }

            UpdateContentTypeFromType(contentType, type);

            return contentType;
        }

        protected virtual IEnumerable<Type> GetContentTypeTypesFromAssembly(Assembly assembly)
        {
            return assembly.GetContentTypeTypes();
        }

        //TODO: Supporter les remove de properties
        protected virtual void UpdateContentTypeFromType(ContentType contentType, Type type)
        {
            //TODO: Retirer (soft delete) les ContentType qui n'existe plus
            //TODO: Update du nom et du BaseContentType

            var propertyInfos = PropertyInfosFromType(type);

            var typeInstance = Activator.CreateInstance(type);

            //TODO: Mettre le metadata sur le ContentType
            //var contentModelMetadataBasedOnDataAnnotationsModelMetadataProvider = _dataAnnotationsModelMetadataProvider.GetMetadataForType(() => typeInstance, type);

            foreach (var propertyInfo in propertyInfos)
            {
                //TODO: Tenir compte des metadata Ex. CultureAttribute, ContentIdAttribute, etc...
                //TODO: Si une propriété n'avait pas un attribut et elle l'a lors d'un update, il faudra supprimer cette propriété
                //TODO: Centraliser la logique concernant les attributs -- ReflectionContentBuilder

                if (!propertyInfo.HasAttribute<IdAttribute>() && !propertyInfo.Name.Equals("Id") &&
                    !propertyInfo.HasAttribute<CultureAttribute>() && !propertyInfo.Name.Equals("Culture"))
                {
                    var contentTypeProperty = contentType.GetContentTypeProperty(propertyInfo.Name) ??
                                              AddContentTypeProperty(contentType, propertyInfo);

                    UpdateContentTypePropertyFromPropertyInfo(contentTypeProperty, propertyInfo, type, typeInstance);
                }
            }
        }

        protected virtual void UpdateContentTypePropertyFromPropertyInfo(ContentType.ContentTypeProperty contentTypeProperty, PropertyInfo propertyInfo,
            Type containerType, object containerTypeInstance)
        {
            //TODO: Attention,  modification de InvariantCulture impossible (pour l'instant)... a traiter
            var propertyModelMetadataBasedOnDataAnnotationsModelMetadataProvider =
                _dataAnnotationsModelMetadataProvider.GetMetadataForProperty(() => containerTypeInstance, containerType, propertyInfo.Name);

            UpdateContentTypePropertyFromModelMetadata(contentTypeProperty, propertyModelMetadataBasedOnDataAnnotationsModelMetadataProvider);
        }

        protected virtual void UpdateContentTypePropertyFromModelMetadata(ContentType.ContentTypeProperty contentTypeProperty, ModelMetadata metadata)
        {
            contentTypeProperty.SetOrOverrideMetadata("ShowForDisplay", metadata.ShowForDisplay);

            if (metadata.DisplayName != null)
            {
                contentTypeProperty.SetOrOverrideMetadata("DisplayName", metadata.DisplayName);
            }

            if (metadata.ShortDisplayName != null)
            {
                contentTypeProperty.SetOrOverrideMetadata("ShortDisplayName",metadata.ShortDisplayName);
            }

            if (!string.IsNullOrEmpty(metadata.TemplateHint))
            {
                contentTypeProperty.SetOrOverrideMetadata("TemplateHint",metadata.TemplateHint) ;
            }

            if (metadata.Description != null)
            {
                contentTypeProperty.SetOrOverrideMetadata("Description",metadata.Description) ;
            }

            if (metadata.NullDisplayText != null)
            {
                contentTypeProperty.SetOrOverrideMetadata("NullDisplayText",metadata.NullDisplayText);
            }

            if (metadata.Watermark != null)
            {
                contentTypeProperty.SetOrOverrideMetadata("Watermark", metadata.Watermark) ;
            }

            contentTypeProperty.SetOrOverrideMetadata("HideSurroundingHtml", metadata.HideSurroundingHtml);
            contentTypeProperty.SetOrOverrideMetadata("RequestValidationEnabled", metadata.RequestValidationEnabled);
            contentTypeProperty.SetOrOverrideMetadata("IsReadOnly", metadata.IsReadOnly);
            contentTypeProperty.SetOrOverrideMetadata("IsRequired", metadata.IsRequired);
            contentTypeProperty.SetOrOverrideMetadata("ShowForEdit", metadata.ShowForEdit);
            contentTypeProperty.SetOrOverrideMetadata("Order", metadata.Order);

            if (!string.IsNullOrEmpty(metadata.DisplayFormatString))
            {
                contentTypeProperty.SetOrOverrideMetadata("DisplayFormatString", metadata.DisplayFormatString);
            }

            if (!string.IsNullOrEmpty(metadata.EditFormatString))
            {
                contentTypeProperty.SetOrOverrideMetadata("EditFormatString", metadata.EditFormatString);
            }

            contentTypeProperty.SetOrOverrideMetadata("ConvertEmptyStringToNull", metadata.ConvertEmptyStringToNull);

            //var validators = metadata.GetValidators()
            //TODO: Validateurs... (Voir ce qui hérite de ModelValidator)
            //TODO: Attention aux resources

            //FluentModelMetadataTransformer.SerializedValue.Transform(contentTypeProperty);
            //DisplayNameTransformer.SerializedValue.Transform(contentTypeProperty);
        }

        protected virtual ContentType.ContentTypeProperty AddContentTypeProperty(ContentType contentType, PropertyInfo propertyInfo)
        {
            //TODO: Différencier les Strong Relation des Relation (weak)... Ex. AddAggregationProperty (avec un attribut?)
            if (propertyInfo.PropertyType.IsContentTranslationVersion())
            {
                return contentType.AddRelationProperty(GetContentTypePropertyNameFromPropertyInfo(propertyInfo),
                        GetContentTypePropertyIsCultureInvariantFromPropertyInfo(propertyInfo));
            }

            if (propertyInfo.PropertyType.IsGenericList() && propertyInfo.PropertyType.GenericTypeArguments[0].IsContentTranslationVersion())
            {
                return contentType.AddRelationsProperty(GetContentTypePropertyNameFromPropertyInfo(propertyInfo),
                        GetContentTypePropertyIsCultureInvariantFromPropertyInfo(propertyInfo));
            }

            return contentType.AddProperty(GetContentTypePropertyNameFromPropertyInfo(propertyInfo),
                        GetContentTypePropertyDefaultValueFromPropertyInfo(propertyInfo),
                        GetContentTypePropertyIsCultureInvariantFromPropertyInfo(propertyInfo));
        }

        protected virtual string GetContentTypePropertyNameFromPropertyInfo(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name;
        }

        protected virtual bool GetContentTypePropertyIsCultureInvariantFromPropertyInfo(PropertyInfo propertyInfo)
        {
            return propertyInfo.HasAttribute<CultureInvariantAttribute>();
        }

        protected virtual object GetContentTypePropertyDefaultValueFromPropertyInfo(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.GetDefaultValue();
        }

        protected virtual IList<PropertyInfo> PropertyInfosFromType(Type type)
        {
            var properties = type.GetOwnProperties().Where(p => !p.GetCustomAttributes(typeof(IgnoreAttribute), true).Any()).ToList();

            return properties;
        }

        public virtual string GetContentTypeNameFromType(Type type)
        {
            return type.Name;
        }

        public virtual string GetModuleNameFromAssembly(Assembly assembly)
        {
            return assembly.GetName().Name;
        }

        //AssemblyFileVersion
        public string GetModuleVersionFromAssembly(Assembly assembly)
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
        }
    }
}
