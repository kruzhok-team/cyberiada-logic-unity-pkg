using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Graph
{
    /// <summary>
    /// Main class for graph representation
    /// </summary>
    public class Graph<TGraphData, TNodeData, TEdgeData>
    {
        /// <summary>
        /// Unique id of a graph
        /// </summary>
        public string ID { get; }

        private readonly Dictionary<string, Node<TGraphData, TNodeData, TEdgeData>> _nodes = new();

        /// <summary>
        /// Root nodes that the graph contains, without child nodes
        /// </summary>
        public IReadOnlyCollection<Node<TGraphData, TNodeData, TEdgeData>> Nodes => _nodes.Values;

        private readonly HashSet<Edge<TEdgeData>> _edges = new();

        /// <summary>
        /// Root edges that the graph contains
        /// </summary>
        public IReadOnlyCollection<Edge<TEdgeData>> Edges => _edges;

        /// <summary>
        /// Data of a concrete graph implementation
        /// </summary>
        public TGraphData Data { get; }

        public Graph(string id, TGraphData data)
        {
            ID = id;
            Data = data;
        }

        /// <summary>
        /// Creates a copy of the graph
        /// </summary>
        /// <param name="parentNode">The parent node of this graph, is needed to set the reference in the nodes of the graph</param>
        /// <param name="newID">The ID of graph copy, if id is null, the ID will be given from the original edge</param>
        /// <returns>A copy of the graph</returns>
        public Graph<TGraphData, TNodeData, TEdgeData> GetCopy(Node<TGraphData, TNodeData, TEdgeData> parentNode = null, string newID = null)
        {
            Graph<TGraphData, TNodeData, TEdgeData> resultGraph = new Graph<TGraphData, TNodeData, TEdgeData>(newID ?? ID, Data);

            foreach (Node<TGraphData, TNodeData, TEdgeData> node in Nodes)
            {
                resultGraph.AddNode(node.GetCopy(parentNode));
            }

            foreach (Edge<TEdgeData> edge in Edges)
            {
                resultGraph.AddEdge(edge.GetCopy());
            }

            return resultGraph;
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
        public bool TryGetNode(string id, out Node<TGraphData, TNodeData, TEdgeData> node)
        {
            return _nodes.TryGetValue(id, out node);
        }

        /// <summary>
        /// Add node to graph
        /// </summary>
        /// <param name="node">Node to adding</param>
        public void AddNode(Node<TGraphData, TNodeData, TEdgeData> node)
        {
            _nodes[node.ID] = node;
        }

        /// <summary>
        /// Delete existing node
        /// </summary>
        /// <param name="node">Target node</param>
        public void DeleteNode(Node<TGraphData, TNodeData, TEdgeData> node)
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
        public void AddEdge(Edge<TEdgeData> edge)
        {
            _edges.Add(edge);
        }

        /// <summary>
        /// Deleting existing edge between nodes
        /// </summary>
        /// <param name="edge">Target edge</param>
        public void DeleteEdge(Edge<TEdgeData> edge)
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
            stringBuilder.AppendLine($"GRAPH({ID})");
            stringBuilder.AppendLine($"{nameof(Nodes)}={string.Join(", ", Nodes.Select(node => node.ID))}");
            stringBuilder.AppendLine($"{nameof(Edges)}=\n{string.Join("\n", Edges)}");

            return stringBuilder.ToString();
        }
    }
}
