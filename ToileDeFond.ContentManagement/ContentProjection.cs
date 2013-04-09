using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToileDeFond.ContentManagement
{
    public class ContentProjection
    {
        public bool IsDraft { get { return !PublicationStartDate.HasValue; } }

        public string CultureName { get; set; }
        public DateTime? PublicationStartDate { get; set; }
        public DateTime? PublicationEndDate { get; set; }
        public string ContentTypeFullName { get; set; }
        public List<Property> Properties { get; set; }
        public Guid ContentId { get; set; }

        public bool TryGetProperty(string name, out Property prop)
        {
            return Properties.ToDictionary(p => p.Name, p => p).TryGetValue(name, out prop);
        }
    }
}
