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

        private string VerboseString(CyberiadaGraph graph, StringBuilder stringBuilder = null)
        {
            stringBuilder ??= new StringBuilder();
            stringBuilder.AppendLine($"GRAPH({graph.ID})\n");

            foreach (Node node in graph.Nodes)
            {
                AddNode(node, stringBuilder);
            }

            foreach (Edge edge in graph.Edges)
            {
                AddEdge(edge, stringBuilder);
            }

            return stringBuilder.ToString();
        }
        
        private static void AddNode(Node node, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"NODE({node.ID})(");
            stringBuilder.AppendLine($"NodeData({node.Data.VisualData.Name})({node.Data.VisualData.Position})");

            foreach (KeyValuePair<string, Event> @event in node.Data.Events)
            {
                stringBuilder.AppendLine($"{@event}\n");
            }
            
            stringBuilder.AppendLine(")");
            stringBuilder.AppendLine($"{nameof(node.ParentNode)}={node.ParentNode?.ID}");

            if (node.NestedGraph != null)
            {
                stringBuilder.AppendLine($"{node.NestedGraph}");
            }
            
            stringBuilder.Append('\n');
        }

        private static void AddEdge(Edge edge, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"EDGE");
            stringBuilder.AppendLine($"{nameof(edge.SourceNode)}={edge.SourceNode}");
            stringBuilder.AppendLine($"{nameof(edge.TargetNode)}={edge.TargetNode}");
            stringBuilder.AppendLine($"{nameof(edge.Data)}=");
            stringBuilder.AppendLine($"EdgeData({edge.Data.TriggerID})({edge.Data.Condition})({edge.Data.VisualData.Position})");

            stringBuilder.AppendLine();
            foreach (Action action in edge.Data.Actions)
            {
                stringBuilder.Append($"\n{action}");
            }
            
            stringBuilder.Append('\n');
        }
    }
}
