using System;
using System.Linq;
using ToileDeFond.Utilities;

namespace ToileDeFond.ContentManagement.DataTransfer
{
    public class ModuleMapper
    {
        public ModuleDto ToDto(Module module)
        {
            var contentTypeMapper = new ContentTypeMapper();

            var moduleDto = new ModuleDto
                {
                    ContentTypes = module.ContentTypes.Select(contentTypeMapper.ToDto).ToList(),
                    Id = module.Id,
                    Name = module.Name,
                    Version = module.Version
                };

            return moduleDto;
        }
    }

    public class ContentTypeMapper
    {
        public ContentTypeDto ToDto(ContentType contentType)
        {
            var contentTypePropertyMapper = new ContentTypePropertyMapper();

            var contentTypeDto = new ContentTypeDto
            {
                BaseContentType = contentType.BaseContentType == null ? default(Guid?) : contentType.BaseContentType.Id,
                Id = contentType.Id,
                Metadata = contentType.Metadata,
                Name = contentType.Name,
                Properties = contentType.Properties.Select(contentTypePropertyMapper.ToDto).ToList()
            };

            return contentTypeDto;
        }
    }

    public class ContentTypePropertyMapper
    {
        public ContentTypePropertyDto ToDto(ContentType.ContentTypeProperty contentTypeProperty)
        {
            var contentTypePropertyDto = new ContentTypePropertyDto
            {
                CultureVariant = contentTypeProperty.CultureVariant,
                Id = contentTypeProperty.Id,
                DefaultValue = contentTypeProperty.DefaultValue.GetStringValue(contentTypeProperty.Type),
                Name = contentTypeProperty.Name,
                Metadata = contentTypeProperty.Metadata,
                Type = contentTypeProperty.Type.FullName
            };

            return contentTypePropertyDto;
        }
    }
}
