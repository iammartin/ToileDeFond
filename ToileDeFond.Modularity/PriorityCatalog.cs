using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace ToileDeFond.Modularity
{
    public class PriorityCatalog : ComposablePartCatalog
    {
        private readonly ComposablePartCatalog _inner;

        public PriorityCatalog(ComposablePartCatalog inner)
        {
            _inner = inner;
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                List<ExportDefinition> distinctExportDefinitions =
                    _inner.Parts.SelectMany(p => p.ExportDefinitions).DistinctBy(x => x.ContractName).ToList();
                var parts = new List<ComposablePartDefinition>();

                foreach (ExportDefinition exportDefinition in distinctExportDefinitions)
                {
                    IQueryable<ComposablePartDefinition> x =
                        _inner.Parts.Where(
                            p => p.ExportDefinitions.Select(a => a.ContractName).Contains(exportDefinition.ContractName));
                    ComposablePartDefinition highestPriorityPart =
                        x.OrderByDescending(
                            p =>
                            p.ExportDefinitions.First(d => d.ContractName == exportDefinition.ContractName)
                             .Metadata.GetValueOrDefault("Priority", 0)).FirstOrDefault();
                    parts.Add(highestPriorityPart);
                }

                return parts.DistinctBy(p => p.ToString()).AsQueryable();
            }
        }
    }
}