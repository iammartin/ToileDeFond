using System.Collections.Generic;
using System.Linq;

//Thanks : http://tawani.blogspot.ca/2009/02/topological-sorting-and-cyclic.html

namespace ToileDeFond.Modularity
{
    public class DependencyManager : Dictionary<string, string[]>
    {
        public IEnumerable<string> GetSortedDependencies()
        {
            int[] sortOrder = GetTopologicalSortOrder(this);

            for (int i = sortOrder.Length - 1; i >= 0; i--)
            {
                yield return this.ElementAt(sortOrder[i]).Key;
            }
        }

        private static int[] GetTopologicalSortOrder(Dictionary<string, string[]> fields)
        {
            var g = new TopologicalSorter(fields.Count);
            var indexes = new Dictionary<string, int>();

            //add vertices  
            for (int i = 0; i < fields.Count; i++)
            {
                indexes[fields.ElementAt(i).Key] = g.AddVertex(i);
            }

            //add edges  
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields.ElementAt(i).Value != null)
                {
                    for (int j = 0; j < fields.ElementAt(i).Value.Length; j++)
                    {
                        g.AddEdge(i, indexes[fields.ElementAt(i).Value[j]]);
                    }
                }
            }

            int[] result = g.Sort();

            return result;
        }
    }
}