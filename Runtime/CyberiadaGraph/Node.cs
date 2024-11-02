using System.Text;

namespace Talent.Graphs
{
    /// <summary>
    /// Class representing single node of graph
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Unique id of a node
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Parent node of a node if it has one
        /// </summary>
        public Node ParentNode { get; set; }

        /// <summary>
        /// Nested graph of a node if it has one, this graph contains child nodes
        /// </summary>
        public CyberiadaGraph NestedGraph { get; set; }

        /// <summary>
        /// Data of a concrete node implementation
        /// </summary>
        public NodeData Data { get; }

        public Node(string id, NodeData data)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new System.ArgumentNullException($"Can't create Node with id '{id}'. ID can't be null or empty");
            }

            ID = id;
            Data = data;
        }

        /// <summary>
        /// Creates a copy of the node
        /// </summary>
        /// <param name="data">Data for a copy of the node</param>
        /// <param name="parentNode">The parent node of this node</param>
        /// <param name="newID">The ID of node copy, if id is null, the ID will be given from the original edge</param>
        /// <returns>A copy of the node</returns>
        public Node GetCopy(NodeData data, Node parentNode = null, string newID = null)
        {
            if (newID == "")
            {
                throw new System.ArgumentNullException($"Can't copy Node with newID '{newID}'. ID can't be null or empty");
            }

            Node resultNode = new Node(newID ?? ID, data);

            if (ParentNode != null)
            {
                resultNode.ParentNode = parentNode;
            }

            if (NestedGraph != null)
            {
                resultNode.NestedGraph = NestedGraph.GetCopy(NestedGraph.Data.GetCopy(), resultNode);
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

            stringBuilder.AppendLine($"NODE({ID})({Data})");
            stringBuilder.AppendLine($"{nameof(ParentNode)}={ParentNode?.ID}");

            if (NestedGraph != null)
            {
                stringBuilder.AppendLine($"{NestedGraph}");
            }

            return stringBuilder.ToString();
        }
    }
}
