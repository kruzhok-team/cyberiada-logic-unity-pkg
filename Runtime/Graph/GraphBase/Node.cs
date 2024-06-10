using System.Text;

namespace Talent.Graph
{
    /// <summary>
    /// Class representing single node of graph
    /// </summary>
    public class Node<TGraphData, TNodeData, TEdgeData>
    {
        /// <summary>
        /// Unique id of a node
        /// </summary>
        public string ID { get; }
        
        /// <summary>
        /// Parent node of a node if it has one
        /// </summary>
        public Node<TGraphData, TNodeData, TEdgeData> ParentNode { get; set; }

        /// <summary>
        /// Nested graph of a node if it has one, this graph contains child nodes
        /// </summary>
        public Graph<TGraphData, TNodeData, TEdgeData> NestedGraph { get; set; }

        /// <summary>
        /// Data of a concrete node implementation
        /// </summary>
        public TNodeData Data { get; }

        public Node(string id, TNodeData data)
        {
            ID = id;
            Data = data;
        }

        /// <summary>
        /// Creates a copy of the node
        /// </summary>
        /// <param name="newID">The ID of node copy, if id is null, the ID will be given from the original edge</param>
        /// <returns>A copy of the node</returns>
        public Node<TGraphData, TNodeData, TEdgeData> GetCopy(Node<TGraphData, TNodeData, TEdgeData> parentNode = null, string newID = null)
        {
            Node<TGraphData, TNodeData, TEdgeData> resultNode = new Node<TGraphData, TNodeData, TEdgeData>(newID ?? ID, Data);

            if (ParentNode != null)
            {
                resultNode.ParentNode = parentNode;
            }

            if (NestedGraph != null)
            {
                resultNode.NestedGraph = NestedGraph.GetCopy(resultNode);
            }

            return resultNode;
        }

        /// <summary>
        /// Custom ToString realization for creating more representive string visualization of node data
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"NODE({ID})");
            stringBuilder.AppendLine($"{nameof(ParentNode)}={ParentNode?.ID}");
            return stringBuilder.ToString();
        }
    }
}
