using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Hosting.Filter;
using MefContrib.Web.Mvc;
using MefContrib.Web.Mvc.Filter;

namespace ToileDeFond.Modularity.Web
{
    public class WebFiltering : IFilter
    {
        private readonly PartCreationScope _partCreationScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="HasPartCreationScope"/> class.
        /// </summary>
        /// <param name="partCreationScope">The part creation scope.</param>
        public WebFiltering(PartCreationScope partCreationScope)
        {
            _partCreationScope = partCreationScope;
        }

        /// <summary>
        /// Decides whether given part satisfies a filter.
        /// </summary>
        /// <param name="part"><see cref="ComposablePartDefinition"/> being filtered.</param>
        /// <returns>True if a given <see cref="ComposablePartDefinition"/> satisfies the filter.
        /// False otherwise.</returns>
        public bool Filter(ComposablePartDefinition part)
        {
            var metadata = new Dictionary<string, object>();

            foreach (var md in part.Metadata)
            {
                metadata.Add(md.Key, md.Value);
            }

            var additionalMetadata = from ed in part.ExportDefinitions
                                     from md in ed.Metadata
                                     select md;

            foreach (var md in additionalMetadata)
            {
                if (!metadata.ContainsKey(md.Key))
                {
                    metadata.Add(md.Key, md.Value);
                }
            }

            const string key = "Scope";

            if (metadata.ContainsKey(key))
            {
                PartCreationScope scope;
                if(!Enum.TryParse(metadata[key].ToString(), out scope))
                {
                    scope = PartCreationScope.Default;
                }

                return scope == _partCreationScope;
            }

            return _partCreationScope == PartCreationScope.Default;
        }
    }
}
