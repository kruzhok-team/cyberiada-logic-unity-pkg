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
            if (string.IsNullOrEmpty(id))
            {
                throw new System.ArgumentNullException($"Can't create Edge with id '{id}'. ID can't be null or empty");
            }

            if (string.IsNullOrEmpty(sourceNode))
            {
                throw new System.ArgumentNullException($"Can't create Edge with source node id '{sourceNode}'. ID can't be null or empty");
            }

            if (string.IsNullOrEmpty(targetNode))
            {
                throw new System.ArgumentNullException($"Can't create Edge with target node id '{targetNode}'. ID can't be null or empty");
            }

            ID = id;
            SourceNode = sourceNode;
            TargetNode = targetNode;
            Data = data;
        }

        /// <summary>
        /// Creates a copy of the edge
        /// </summary>
        /// <param name="newID">The ID of edge copy, if id is null, the ID will be given from the original edge</param>
        /// <returns>A copy of the edge</returns>
        public Edge<TEdgeData> GetCopy(string newID = null)
        {
            if (newID == "")
            {
                throw new System.ArgumentNullException($"Can't copy Edge with newID '{newID}'. ID can't be null or empty");
            }

            Edge<TEdgeData> resultEdge = new Edge<TEdgeData>(newID ?? ID, SourceNode, TargetNode, Data);

            return resultEdge;
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
