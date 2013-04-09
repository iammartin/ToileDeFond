using System.Collections.Generic;
using ToileDeFond.ContentManagement.Reflection;

namespace ToileDeFond.Tests.FakeModules.First
{
    public class FakeClassGraph : ContentTranslationVersion
    {
        public string Name { get; set; }
        //public FakeClassChild FakeClass { get; set; }
        public FakeClass FakeClass { get; set; }

        public List<FakeClass> FakeClasses { get; set; }
    }
}