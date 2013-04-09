using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ToileDeFond.Utilities
{
    public class PagedCollection<T> : IEnumerable<T>
    {
        public PagedCollection(IEnumerable<T> collection, int totalCount, int pageIndex, int pageSize)
        {
            Collection = collection.ToList();
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public IList<T> Collection { get; private set; }
        public int TotalCount { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }
    }
}
