using System.Text;

namespace Talent.Graph
{
    /// <summary>
    /// Class representing single edge from one node to another
    /// </summary>
    public class Edge<TEdgeData>
    {
        /// <summary>
        /// Unique id of a edge
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Source node of edge
        /// </summary>
        public string SourceNode { get; private set; }
        
        /// <summary>
        /// Target node of edge
        /// </summary>
        public string TargetNode { get; private set; }

        /// <summary>
        /// Data of a concrete edge implementation
        /// </summary>
        public TEdgeData Data { get; }

        public Edge(string id, string sourceNode, string targetNode, TEdgeData data)
        {
            ID = id;
            SourceNode = sourceNode;
            TargetNode = targetNode;
            Data = data;
        }

        /// <summary>
        /// Custom ToString realization for creating more representive string visualization of edge data
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"EDGE");
            stringBuilder.AppendLine($"{nameof(SourceNode)}={SourceNode}");
            stringBuilder.AppendLine($"{nameof(TargetNode)}={TargetNode}");
            return stringBuilder.ToString();
        }
    }
}
