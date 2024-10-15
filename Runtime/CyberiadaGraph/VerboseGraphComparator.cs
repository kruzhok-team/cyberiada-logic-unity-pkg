using System.Text;

namespace Talent.Graphs
{
    public class VerboseGraphComparator : IGraphComparator
    {
        public bool IsGraphEqual(CyberiadaGraph graph, CyberiadaGraph otherGraph)
        {
            return VerboseString(graph) == VerboseString(otherGraph);
        }

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
