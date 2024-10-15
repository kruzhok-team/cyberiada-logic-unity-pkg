using System.Collections.Generic;
using System.Text;

namespace Talent.Graphs
{
    public class LogicalGraphComparator : IEqualityComparer<CyberiadaGraph>
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        
        public bool Equals(CyberiadaGraph graph, CyberiadaGraph otherGraph)
        {
            return ConvertToString(graph) == ConvertToString(otherGraph);
        }

        public int GetHashCode(CyberiadaGraph graph) => graph.GetHashCode();

        private string ConvertToString(CyberiadaGraph graph)
        {
            _stringBuilder.Clear();
            _stringBuilder.AppendLine($"GRAPH({graph.ID})\n");

            foreach (Node node in graph.Nodes)
            {
                AddNode(node);
            }

            foreach (Edge edge in graph.Edges)
            {
                AddEdge(edge);
            }
            
            return _stringBuilder.ToString();
        }

        private void AddNode(Node node)
        {
            _stringBuilder.AppendLine($"NODE({node.ID})(");
            
            foreach (KeyValuePair<string, Event> @event in node.Data.Events)
            {
                _stringBuilder.AppendLine($"{@event}\n");
            }

            _stringBuilder.Append(")");
            _stringBuilder.AppendLine($"{nameof(node.ParentNode)}={node.ParentNode?.ID}");

            if (node.NestedGraph != null)
            {
                _stringBuilder.AppendLine(ConvertToString(node.NestedGraph));
            }
            
            _stringBuilder.Append('\n');
        }

        private void AddEdge(Edge edge)
        {
            _stringBuilder.AppendLine("EDGE");
            _stringBuilder.AppendLine($"{nameof(edge.SourceNode)}={edge.SourceNode}");
            _stringBuilder.AppendLine($"{nameof(edge.TargetNode)}={edge.TargetNode}");
            _stringBuilder.AppendLine($"{nameof(edge.Data)}=");
            _stringBuilder.AppendLine($"EdgeData({edge.Data.TriggerID})({edge.Data.Condition})");

            foreach (Action action in edge.Data.Actions)
            {
                _stringBuilder.Append($"\n{action}");
            }
            
            _stringBuilder.Append('\n');
        }
    }
}
