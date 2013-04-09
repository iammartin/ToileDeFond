using ToileDeFond.Tests.FakeModules.First;

namespace ToileDeFond.Tests.FakeModules.Second
{
    public class PocoTest :  IPocoTest, IPocoFromFirstModule
    {
        public string Name { get; set; }
        public string Rambo { get; set; }
    }
}
