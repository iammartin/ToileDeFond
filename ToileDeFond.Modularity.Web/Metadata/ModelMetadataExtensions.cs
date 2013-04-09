using System.Web.Mvc;

namespace ToileDeFond.Modularity.Web.Metadata
{
    public static class ModelMetadataExtensions
    {
        public static void SetContainerInstance(this ModelMetadata modelMetadata, object value)
        {
            modelMetadata.AdditionalValues.Add("containerInstance", value);
        }

        public static object GetContainerInstance(this ModelMetadata modelMetadata)
        {
            object value;

            return modelMetadata.AdditionalValues.TryGetValue("containerInstance", out value) ? value : default(object);
        }
    }
}