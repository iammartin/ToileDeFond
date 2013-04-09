using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Reflection;
using ToileDeFond.Modularity;
using System.Linq;
using ToileDeFond.Utilities;

//TODO: Peut-etre merger ReflectionContentManager & ReflectionContentBuilder... pas mal de logique partagée/similaire (refactorings)

namespace ToileDeFond.ContentManagement.Reflection.DefaultImplementation
{
    [PrioritisedExport(typeof(IContentBuilder))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReflectionContentBuilder : IContentBuilder
    {
        public Guid? GetContentId(object objectContent)
        {
            var objectContentType = objectContent.GetType();

            var contentIdProperty = objectContentType.GetProperties().FirstOrDefault(p => p.HasAttribute<IdAttribute>());

            if (contentIdProperty == null)
            {
                contentIdProperty = objectContentType.GetProperties().FirstOrDefault(p => p.Name.Equals("Id"));
            }

            if (contentIdProperty == null)
                return null;

            return contentIdProperty.GetValue(objectContent, null) as Guid?;
        }

        public Content UpdateContent(object objectContent, Content content, CultureInfo cultureInfo)
        {
            var translation = content.GetExistingOrNewTranslation(content, cultureInfo);
            var contentProperties = objectContent.GetType().GetProperties().ToDictionary(p => p.Name, p => p);

            foreach (var property in content.ContentType.Properties)
            {
                PropertyInfo contentProperty;
                if (contentProperties.TryGetValue(property.Name, out contentProperty))
                {
                    if (contentProperty.PropertyType.IsContentTranslationVersion())
                    {
                        //if (contentProperty.HasAttribute<StrongAggregationPropertyAttribute>())
                        //{
                        //    throw new NotImplementedException();
                        //}
                        //else
                        //{
                        //    throw new NotImplementedException();
                        //}
                        dynamic value = contentProperty.GetValue(objectContent, null);
                        translation[property].SetValue(value.Id); 
                    }
                    else if (contentProperty.PropertyType.IsGenericList() && contentProperty.PropertyType.GenericTypeArguments[0].IsContentTranslationVersion())
                    {
                        var ids = new List<Guid>();
                        dynamic value = contentProperty.GetValue(objectContent, null);

                        foreach (var item in value)
                        {
                            ids.Add(item.Id);
                        }

                        translation[property].SetValue(ids);
                    }
                    else
                    {
                        var value = contentProperty.GetValue(objectContent, null);
                        translation[property].SetValue(value);
                    }
                }
            }

            return content;
        }

        public CultureInfo GetCultureInfo(object objectContent)
        {
            var objectContentType = objectContent.GetType();

            var cultureInfoProperty = objectContentType.GetProperties().FirstOrDefault(p => p.HasAttribute<CultureAttribute>());

            if (cultureInfoProperty == null)
            {
                cultureInfoProperty = objectContentType.GetProperties().FirstOrDefault(p => p.Name.Equals("Culture"));
            }

            if (cultureInfoProperty == null)
                return null;

            return cultureInfoProperty.GetValue(objectContent, null) as CultureInfo;
        }
    }
}