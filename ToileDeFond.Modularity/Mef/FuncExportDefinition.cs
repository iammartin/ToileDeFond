using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace ToileDeFond.Modularity.Mef
{
    internal class FuncExportDefinition : ExportDefinition
    {
        public FuncExportDefinition(Type exportedType,
            Func<ExportProvider, object> factory) :
            base(AttributedModelServices.GetTypeIdentity(exportedType),
            new Dictionary<string, object> { { "ExportTypeIdentity", AttributedModelServices.GetTypeIdentity(exportedType) } })
        {
            this.Factory = factory;
        }

        public Func<ExportProvider, object> Factory { get; private set; }
    }
}