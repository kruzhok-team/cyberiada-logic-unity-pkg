using System.Collections.Generic;
using System.Text;

namespace Talent.Graphs
{
    /// <summary>
    /// Main class for graph representation
    /// </summary>
    public class CyberiadaGraph
    {
        /// <summary>
        /// Unique id of a graph
        /// </summary>
        public string ID { get; }

        private readonly Dictionary<string, Node> _nodes = new();

        /// <summary>
        /// Root nodes that the graph contains, without child nodes
        /// </summary>
        public IReadOnlyCollection<Node> Nodes => _nodes.Values;

        private readonly HashSet<Edge> _edges = new();

        /// <summary>
        /// Root edges that the graph contains
        /// </summary>
        public IReadOnlyCollection<Edge> Edges => _edges;

        /// <summary>
        /// Data of a concrete graph implementation
        /// </summary>
        public GraphData Data { get; }

        public CyberiadaGraph(string id, GraphData data)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new System.ArgumentNullException($"Can't create Graph with id '{id}'. ID can't be null or empty");
            }

            ID = id;
            Data = data;
        }

        /// <summary>
        /// Creates a copy of the graph
        /// </summary>
        /// <param name="data">Data for a copy of the graph</param>
        /// <param name="parentNode">The parent node of this graph, is needed to set the reference in the nodes of the graph</param>
        /// <param name="newID">The ID of graph copy, if id is null, the ID will be given from the original edge</param>
        /// <returns>A copy of the graph</returns>
        public CyberiadaGraph GetCopy(GraphData data, Node parentNode = null, string newID = null)
        {
            if (newID == "")
            {
                throw new System.ArgumentNullException($"Can't copy Graph with newID '{newID}'. ID can't be null or empty");
            }

            CyberiadaGraph resultGraph = new CyberiadaGraph(newID ?? ID, data);

            foreach (Node node in Nodes)
            {
                resultGraph.AddNode(node.GetCopy(node.Data.GetCopy(), parentNode));
            }

            foreach (Edge edge in Edges)
            {
                resultGraph.AddEdge(edge.GetCopy(edge.Data.GetCopy()));
            }

            return resultGraph;
        }

        public bool IsGraphEqual(CyberiadaGraph graph, IEqualityComparer<CyberiadaGraph> comparer)
        {
            return comparer.Equals(this, graph);
        }

        #region Nodes API

        /// <summary>
        /// Check if graph contains node with this id
        /// </summary>
        /// <param name="id">Node id</param>
        /// <returns>True if succesfully found node, false if not</returns>
        public bool HasNode(string id)
        {
            return id != null && _nodes.ContainsKey(id);
        }
        
        /// <summary>
        /// Get node if graph has one
        /// </summary>
        /// <param name="id">Node id</param>
        /// <param name="node">Result node, null if graph doesn't have corresponding node</param>
        /// <returns>True if succesfully found node, false if not</returns>
        public bool TryGetNode(string id, out Node node)
        {
            return _nodes.TryGetValue(id, out node);
        }

        /// <summary>
        /// Add node to graph
        /// </summary>
        /// <param name="node">Node to adding</param>
        public void AddNode(Node node)
        {
            _nodes[node.ID] = node;
        }

        /// <summary>
        /// Delete existing node
        /// </summary>
        /// <param name="node">Target node</param>
        public void DeleteNode(Node node)
        {
            if (HasNode(node.ID))
            {
                _nodes.Remove(node.ID);
            }
        }

        #endregion

        #region Edges API

        /// <summary>
        /// Add edge to graph
        /// </summary>
        /// <param name="edge">Edge to adding</param>
        public void AddEdge(Edge edge)
        {
            _edges.Add(edge);
        }

        /// <summary>
        /// Deleting existing edge between nodes
        /// </summary>
        /// <param name="edge">Target edge</param>
        public void DeleteEdge(Edge edge)
        {
            _edges.Remove(edge);
        }

        #endregion

        /// <summary>
        /// Custom ToString realization for creating more representive string visualization of graph data
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"GRAPH({ID})\n");

            foreach (Node node in Nodes)
            {
                stringBuilder.AppendLine($"{node}\n");
            }

            foreach (Edge edge in Edges)
            {
                stringBuilder.AppendLine($"{edge}\n");
            }

            return stringBuilder.ToString();
        }
    }
}
