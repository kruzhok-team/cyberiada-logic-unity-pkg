using System.Collections.Generic;
using System.Text;

namespace Talent.Graphs
{
    public class LogicalGraphComparator : IGraphComparator
    {
        public bool IsGraphEqual(CyberiadaGraph graph, CyberiadaGraph otherGraph)
        {
            return LogicalString(graph) == LogicalString(otherGraph);
        }

        private string LogicalString(CyberiadaGraph graph)
        {
            StringBuilder stringBuilder = new();

            foreach (Node node in graph.Nodes)
            {
                stringBuilder.AppendLine($"{LogicalNodeString(node)}\n");
            }

            foreach (Edge edge in graph.Edges)
            {
                stringBuilder.AppendLine($"{LogicalEdgeString(edge)}\n");
            }

            return stringBuilder.ToString();
        }

        private string LogicalNodeString(Node node)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine($"NODE({node.ID})({LogicalNodeDataString(node.Data)})");
            stringBuilder.AppendLine($"{nameof(node.ParentNode)}={node.ParentNode?.ID}");

            if (node.NestedGraph != null)
            {
                stringBuilder.AppendLine($"{LogicalString(node.NestedGraph)}");
            }

            return stringBuilder.ToString();
        }

        public string LogicalNodeDataString(NodeData nodeData)
        {
            StringBuilder stringBuilder = new();

            foreach (KeyValuePair<string, Event> @event in nodeData.Events)
            {
                stringBuilder.AppendLine($"{@event}\n");
            }

            return stringBuilder.ToString();
        }

        private string LogicalEdgeString(Edge edge)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine($"EDGE");
            stringBuilder.AppendLine($"{nameof(edge.SourceNode)}={edge.SourceNode}");
            stringBuilder.AppendLine($"{nameof(edge.TargetNode)}={edge.TargetNode}");
            stringBuilder.AppendLine($"{nameof(edge.Data)}={LogicalEdgeDataString(edge.Data)}");

            return stringBuilder.ToString();
        }

        public string LogicalEdgeDataString(EdgeData edgeData)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"EdgeData({edgeData.TriggerID})({edgeData.Condition})");

            foreach (Action action in edgeData.Actions)
            {
                stringBuilder.AppendLine($"\n{action}");
            }

            return stringBuilder.ToString();
        }
    }
}
