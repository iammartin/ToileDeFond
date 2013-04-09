using System;
using ToileDeFond.ContentManagement.Reflection;

namespace ToileDeFond.Tests.FakeModules.First
{
    public class FakeClass : ContentTranslationVersion
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Date { get; set; }
    }

    //public class FakeClassChild
    //{
    //    public string Name { get; set; }
    //    public int Age { get; set; }
    //    public DateTime Date { get; set; }
    //}
}
