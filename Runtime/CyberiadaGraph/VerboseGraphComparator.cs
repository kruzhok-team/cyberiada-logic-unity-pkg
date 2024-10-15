using System.Collections.Generic;
using System.Text;

namespace Talent.Graphs
{
    public class VerboseGraphComparator : IEqualityComparer<CyberiadaGraph>
    {
        public bool Equals(CyberiadaGraph graph, CyberiadaGraph otherGraph)
        {
            return VerboseString(graph) == VerboseString(otherGraph);
        }

        public int GetHashCode(CyberiadaGraph graph) => graph.GetHashCode();

        private string VerboseString(CyberiadaGraph graph)
        {
            StringBuilder stringBuilder = new();

            foreach (Node node in graph.Nodes)
            {
                stringBuilder.AppendLine($"{node}\n");
            }

            foreach (Edge edge in graph.Edges)
            {
                stringBuilder.AppendLine($"{edge}\n");
            }

            return stringBuilder.ToString();
        }
    }
}
