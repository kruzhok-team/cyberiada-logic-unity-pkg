using System.Collections.Generic;
using System.Linq;
using Talent.Graph;
using Talent.Graph.Cyberiada;
using Action = Talent.Graph.Cyberiada.Action;
using Event = Talent.Graph.Cyberiada.Event;

namespace Talent.GraphEditor.Core
{
    public class GraphEditor
    {
        public Graph<GraphData, NodeData, EdgeData> Graph { get; private set; }

        private IGraphElementViewFactory GraphElementViewFactory { get; }
        private Dictionary<string, Node<GraphData, NodeData, EdgeData>> _nodes = new Dictionary<string, Node<GraphData, NodeData, EdgeData>>();
        private Dictionary<string, Edge<EdgeData>> _edges = new Dictionary<string, Edge<EdgeData>>();

        private BidirectionalDictionary<Graph<GraphData, NodeData, EdgeData>, IGraphView> _graphViews = new BidirectionalDictionary<Graph<GraphData, NodeData, EdgeData>, IGraphView>();
        private BidirectionalDictionary<Node<GraphData, NodeData, EdgeData>, INodeView> _nodeViews = new BidirectionalDictionary<Node<GraphData, NodeData, EdgeData>, INodeView>();
        private BidirectionalDictionary<Edge<EdgeData>, IEdgeView> _edgeViews = new BidirectionalDictionary<Edge<EdgeData>, IEdgeView>();
        private BidirectionalDictionary<Event, INodeEventView> _nodeEventViews = new BidirectionalDictionary<Event, INodeEventView>();
        private BidirectionalDictionary<Action, INodeActionView> _nodeActionViews = new BidirectionalDictionary<Action, INodeActionView>();
        private BidirectionalDictionary<Action, IEdgeActionView> _edgeActionViews = new BidirectionalDictionary<Action, IEdgeActionView>();

        private Node<GraphData, NodeData, EdgeData> _initialNode;
        private Edge<EdgeData> _initialEdge;

        public GraphEditor(IGraphElementViewFactory factory)
        {
            GraphElementViewFactory = factory;
        }

        /// <summary>
        /// Set graph and rebuild views
        /// </summary>
        /// <param name="graph"></param>
        public void SetGraph(Graph<GraphData, NodeData, EdgeData> graph)
        {
            Graph = graph;
            RebuildView();
        }

        /// <summary>
        /// Go through the current graph, generate missing views, delete unnecessary views
        /// Useful if something broke or the graph was changed bypassing the editor
        /// </summary>
        public void RefreshView()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Destroy all current views and generate all views for current graph
        /// </summary>
        public void RebuildView()
        {
            foreach (IEdgeActionView edgeActionView in _edgeActionViews.Values)
            {
                GraphElementViewFactory.DestroyElementView(edgeActionView);
            }

            _edgeActionViews.Clear();

            foreach (INodeActionView nodeActionView in _nodeActionViews.Values)
            {
                GraphElementViewFactory.DestroyElementView(nodeActionView);
            }

            _nodeActionViews.Clear();

            foreach (INodeEventView nodeEventView in _nodeEventViews.Values)
            {
                GraphElementViewFactory.DestroyElementView(nodeEventView);
            }

            _nodeEventViews.Clear();

            foreach (INodeView nodeView in _nodeViews.Values)
            {
                GraphElementViewFactory.DestroyElementView(nodeView);
            }

            _nodeViews.Clear();

            foreach (IEdgeView edgeView in _edgeViews.Values)
            {
                GraphElementViewFactory.DestroyElementView(edgeView);
            }

            _edgeViews.Clear();

            //TODO: придумать как делать DestroyElementView только для граф вью в котором уничтожили все нод вью,
            //сейчас они просто уничтожаются вместе с нодами из-за того что дочерние к нодам в реализации
            _graphViews.Clear();

            _nodes.Clear();
            _edges.Clear();

            _initialNode = null;
            _initialEdge = null;

            if (Graph == null)
            {
                return;
            }

            foreach (Node<GraphData, NodeData, EdgeData> node in Graph.Nodes)
            {
                if (node.Data.Vertex == NodeData.Vertex_Initial)
                {
                    _initialNode = node;
                    _nodeViews.Set(_initialNode, GraphElementViewFactory.CreateNodeView(_initialNode.Data.VisualData, _initialNode.Data.Vertex, true));
                }
                else
                {
                    CreateViewForNode(node, false, true);
                }
            }

            foreach (Edge<EdgeData> edge in Graph.Edges)
            {
                if (edge.SourceNode == _initialNode.ID || edge.TargetNode == _initialNode.ID)
                {
                    _initialEdge = edge;

                    if (_nodeViews.TryGetValue(_initialEdge.SourceNode == _initialNode.ID ? _initialNode : _nodes[_initialEdge.SourceNode], out INodeView sourceNodeView)
                        && _nodeViews.TryGetValue(_initialEdge.TargetNode == _initialNode.ID ? _initialNode : _nodes[_initialEdge.TargetNode], out INodeView targetNodeView))
                    {
                        IEdgeView edgeView = GraphElementViewFactory.CreateEdgeView(sourceNodeView, targetNodeView, _initialEdge.Data.VisualData, _initialEdge.Data.TriggerID, _initialEdge.Data.Condition);
                        _edgeViews.Add(_initialEdge, edgeView);
                    }
                }
                else
                {
                    CreateViewForEdge(edge);
                }
            }

            if (_nodes.Count > 0)
            {
                CreateInitialNode();
            }
        }

        private void CreateInitialNode()
        {
            if (_initialNode == null)
            {
                _initialNode = new Node<GraphData, NodeData, EdgeData>("initial", new NodeData(NodeData.Vertex_Initial));

                Graph.AddNode(_initialNode);
                _initialNode.Data.VisualData.Name = "INIT";

                _nodeViews.Set(_initialNode, GraphElementViewFactory.CreateNodeView(_initialNode.Data.VisualData, _initialNode.Data.Vertex, false));

                CreateInitialEdge();
            }
        }

        private void CreateInitialEdge()
        {
            if (_initialEdge == null && _initialNode != null && _nodes.Count > 0)
            {
                _initialEdge = new Edge<EdgeData>(_initialNode.ID + _nodes.Values.First().ID, _initialNode.ID, _nodes.Values.First().ID, new EdgeData(""));
                Graph.AddEdge(_initialEdge);

                if (_nodeViews.TryGetValue(_initialNode, out INodeView sourceNodeView) && _nodeViews.TryGetValue(_nodes.Values.First(), out INodeView targetNodeView))
                {
                    IEdgeView edgeView = GraphElementViewFactory.CreateEdgeView(sourceNodeView, targetNodeView, _initialEdge.Data.VisualData, _initialEdge.Data.TriggerID, _initialEdge.Data.Condition);
                    _edgeViews.Add(_initialEdge, edgeView);
                }
            }
        }

        #region Public Create Methods

        public IGraphView CreateNewGraph(string id, INodeView nodeView)
        {
            if (_nodeViews.TryGetValue(nodeView, out Node<GraphData, NodeData, EdgeData> node) && node.NestedGraph == null)
            {
                node.NestedGraph = new Graph<GraphData, NodeData, EdgeData>(id, new GraphData());
            }

            IGraphView graphView = GraphElementViewFactory.CreateGraphView(id, nodeView);
            _graphViews.Add(node.NestedGraph, graphView);
            return graphView;
        }

        public INodeView CreateNewNode(string name)
        {
            Node<GraphData, NodeData, EdgeData> node = new Node<GraphData, NodeData, EdgeData>(System.Guid.NewGuid().ToString(), new NodeData());
            Graph.AddNode(node);
            node.Data.VisualData.Name = name;

            return CreateViewForNode(node, true, false);
        }

        public IEdgeView CreateNewEdge(INodeView sourceView, INodeView targetView, string triggerID)
        {
            Node<GraphData, NodeData, EdgeData> sourceNode = _nodeViews.Get(sourceView);
            Node<GraphData, NodeData, EdgeData> targetNode = _nodeViews.Get(targetView);

            if ((sourceNode == _initialNode || targetNode == _initialNode) && _initialEdge != null)
            {
                return null;
            }

            Edge<EdgeData> edge = new Edge<EdgeData>(System.Guid.NewGuid().ToString(), sourceNode.ID, targetNode.ID, new EdgeData(triggerID));
            Graph.AddEdge(edge);

            return CreateViewForEdge(edge);
        }

        public INodeEventView CreateNewNodeEvent(INodeView nodeView, string triggerID)
        {
            INodeEventView result = default;

            if (_nodeViews.TryGetValue(nodeView, out Node<GraphData, NodeData, EdgeData> node))
            {
                if (!node.Data.Events.TryGetValue(triggerID, out Event nodeEvent))
                {
                    nodeEvent = new Event(triggerID);
                    node.Data.AddEvent(nodeEvent);
                }

                if (!_nodeEventViews.ContainsKey(nodeEvent))
                {
                    _nodeEventViews.Set(nodeEvent, GraphElementViewFactory.CreateNodeEventView(_nodeViews.Get(node), triggerID));
                }

                result = _nodeEventViews.Get(nodeEvent);
            }

            return result;
        }

        public INodeActionView CreateNewNodeAction(INodeEventView eventView, string actionID)
        {
            INodeActionView result = default;

            if (_nodeEventViews.TryGetValue(eventView, out Event nodeEvent))
            {
                Action eventAction = new Action(actionID);

                nodeEvent.AddAction(eventAction);
                _nodeActionViews.Set(eventAction, GraphElementViewFactory.CreateNodeActionView(eventView, actionID));
                result = _nodeActionViews.Get(eventAction);
            }

            return result;
        }

        public IEdgeActionView CreateNewEdgeAction(IEdgeView edgeView, string actionID)
        {
            IEdgeActionView result = default;

            if (_edgeViews.TryGetValue(edgeView, out Edge<EdgeData> edge))
            {
                Action eventAction = new Action(actionID);

                edge.Data.AddAction(eventAction);
                _edgeActionViews.Set(eventAction, GraphElementViewFactory.CreateEdgeActionView(edgeView, actionID));
                result = _edgeActionViews.Get(eventAction);
            }

            return result;
        }

        #endregion

        public void ChangeNodeEventTrigger(INodeView nodeView, INodeEventView nodeEventView, string triggerID)
        {
            if (_nodeViews.TryGetValue(nodeView, out Node<GraphData, NodeData, EdgeData> node) && _nodeEventViews.TryGetValue(nodeEventView, out Event nodeEvent) && nodeEvent.TriggerID != triggerID)
            {
                if (!node.Data.Events.TryGetValue(triggerID, out Event newNodeEvent) || !_nodeEventViews.TryGetValue(newNodeEvent, out INodeEventView newNodeEventView))
                {
                    newNodeEventView = CreateNewNodeEvent(nodeView, triggerID);
                }

                foreach(Action action in nodeEvent.Actions)
                {
                    var actionView = CreateNewNodeAction(newNodeEventView, action.ID);
                    ChangeNodeActionParameter(actionView, action.Parameter);
                }

                RemoveNodeEvent(nodeView, nodeEventView);
            }
        }

        public void ChangeEdgeTrigger(IEdgeView edgeView, string triggerID)
        {
            if (_edgeViews.TryGetValue(edgeView, out Edge<EdgeData> edge))
            {
                edge.Data.SetTrigger(triggerID);
                edgeView.SetTrigger(triggerID);
            }
        }

        public void ChangeEdgeCondition(IEdgeView edgeView, string condition)
        {
            if (_edgeViews.TryGetValue(edgeView, out Edge<EdgeData> edge))
            {
                edge.Data.SetCondition(condition);
                edgeView.SetCondition(condition);
            }
        }

        public void ChangeNodeActionParameter(INodeActionView actionView, string parameter)
        {
            if (_nodeActionViews.TryGetValue(actionView, out Action action))
            {
                action.SetParameter(parameter);
                actionView.SetParameter(parameter);
            }
        }

        public void ChangeEdgeActionParameter(IEdgeActionView actionView, string parameter)
        {
            if (_edgeActionViews.TryGetValue(actionView, out Action action))
            {
                action.SetParameter(parameter);
                actionView.SetParameter(parameter);
            }
        }

        #region Public Remove Methods

        public void RemoveNode(INodeView nodeView, bool createInitialEdge = true)
        {
            if (_nodeViews.TryGetValue(nodeView, out Node<GraphData, NodeData, EdgeData> node))
            {
                Node<GraphData, NodeData, EdgeData> oldParent = node.ParentNode;

                foreach (KeyValuePair<string, Event> nodeEvent in node.Data.Events)
                {
                    foreach (Action action in nodeEvent.Value.Actions)
                    {
                        INodeActionView nodeActionView = _nodeActionViews.Get(action);

                        _nodeActionViews.Remove(nodeActionView);
                        GraphElementViewFactory.DestroyElementView(nodeActionView);
                    }

                    if (_nodeEventViews.TryGetValue(nodeEvent.Value, out INodeEventView eventView))
                    {
                        _nodeEventViews.Remove(nodeEvent.Value);
                        GraphElementViewFactory.DestroyElementView(eventView);
                    }
                }

                foreach (Edge<EdgeData> edge in Graph.Edges.Where(edge => edge.SourceNode == node.ID || edge.TargetNode == node.ID).ToArray())
                {
                    if (_edgeViews.TryGetValue(edge, out IEdgeView edgeView))
                    {
                        foreach (Action action in edge.Data.Actions)
                        {
                            IEdgeActionView edgeActionView = _edgeActionViews.Get(action);

                            if (_edgeViews.TryGetValue(edgeView, out Edge<EdgeData> nodeEvent) && _edgeActionViews.TryGetValue(edgeActionView, out Action eventAction))
                            {
                                _edgeActionViews.Remove(edgeActionView);
                                GraphElementViewFactory.DestroyElementView(edgeActionView);
                            }
                        }

                        _edges.Remove(edge.ID);
                        _edgeViews.Remove(edgeView);
                        Graph.DeleteEdge(edge);
                        GraphElementViewFactory.DestroyElementView(edgeView);

                        if (edge.SourceNode == _initialNode.ID || edge.TargetNode == _initialNode.ID)
                        {
                            _initialEdge = null;
                        }
                    }
                }

                if (node.NestedGraph != null)
                {
                    foreach (Node<GraphData, NodeData, EdgeData> child in node.NestedGraph.Nodes.ToArray())
                    {
                        if (_nodeViews.TryGetValue(child, out INodeView childView))
                        {
                            RemoveNode(childView, false);
                        }
                    }
                }

                if (oldParent != null && oldParent.NestedGraph != null)
                {
                    oldParent.NestedGraph.DeleteNode(node);
                }

                _nodes.Remove(node.ID);
                Graph.DeleteNode(node);
                _nodeViews.Remove(node);
                GraphElementViewFactory.DestroyElementView(nodeView);

                if (_nodes.Count > 0)
                {
                    if (createInitialEdge)
                    {
                        CreateInitialEdge();
                    }
                }
                else
                {
                    Graph.DeleteNode(_initialNode);
                    GraphElementViewFactory.DestroyElementView(_nodeViews.Get(_initialNode));
                    _nodeViews.Remove(_initialNode);
                    _initialNode = null;
                }

                RemoveGraphFromNode(oldParent);
            }
        }

        public void RemoveEdge(IEdgeView edgeView)
        {
            if (_edgeViews.TryGetValue(edgeView, out Edge<EdgeData> edge))
            {
                foreach (Action action in edge.Data.Actions)
                {
                    IEdgeActionView edgeActionView = _edgeActionViews.Get(action);

                    _edgeActionViews.Remove(edgeActionView);
                    GraphElementViewFactory.DestroyElementView(edgeActionView);
                }

                _edges.Remove(edge.ID);
                Graph.DeleteEdge(edge);
                _edgeViews.Remove(edgeView);
                GraphElementViewFactory.DestroyElementView(edgeView);
            }
        }

        public void RemoveNodeEvent(INodeView nodeView, INodeEventView nodeEventView)
        {
            Node<GraphData, NodeData, EdgeData> node = _nodeViews.Get(nodeView);
            Event nodeEvent = _nodeEventViews.Get(nodeEventView);

            foreach (Action action in nodeEvent.Actions)
            {
                if (_nodeActionViews.TryGetValue(action, out INodeActionView nodeActionView))
                {
                    _nodeActionViews.Remove(nodeActionView);
                    GraphElementViewFactory.DestroyElementView(nodeActionView);
                }
            }

            if (_nodeEventViews.TryGetValue(nodeEvent, out INodeEventView eventView))
            {
                _nodeEventViews.Remove(nodeEvent);
                GraphElementViewFactory.DestroyElementView(eventView);
            }

            node.Data.RemoveEvent(nodeEvent);
        }

        public void RemoveNodeAction(INodeEventView nodeEventView, INodeActionView nodeActionView)
        {
            if (_nodeEventViews.TryGetValue(nodeEventView, out Event nodeEvent) && _nodeActionViews.TryGetValue(nodeActionView, out Action eventAction))
            {
                nodeEvent.RemoveAction(eventAction);
                _nodeActionViews.Remove(nodeActionView);
                GraphElementViewFactory.DestroyElementView(nodeActionView);
            }
        }

        public void RemoveEdgeAction(IEdgeView edgeView, IEdgeActionView edgeActionView)
        {
            if (_edgeViews.TryGetValue(edgeView, out Edge<EdgeData> nodeEvent) && _edgeActionViews.TryGetValue(edgeActionView, out Action eventAction))
            {
                nodeEvent.Data.RemoveAction(eventAction);
                _edgeActionViews.Remove(edgeActionView);
                GraphElementViewFactory.DestroyElementView(edgeActionView);
            }
        }

        #endregion

        public void SetParent(INodeView childView, INodeView parentView, bool haveSavedData)
        {
            if (childView == null || !_nodeViews.TryGetValue(childView, out Node<GraphData, NodeData, EdgeData> child) || (_initialNode != null && child == _initialNode))
            {
                return;
            }

            Node<GraphData, NodeData, EdgeData> oldParent = null;

            if (parentView == null)
            {
                if (child.ParentNode != null)
                {
                    child.ParentNode.NestedGraph.DeleteNode(child);
                    Graph.AddNode(child);
                }

                oldParent = child.ParentNode;
                child.ParentNode = null;

                _nodeViews.Get(child).SetParent(null, haveSavedData);
            }
            else if (_nodeViews.TryGetValue(parentView, out Node<GraphData, NodeData, EdgeData> parent) && child != parent && (_initialNode == null || parent != _initialNode))
            {
                if (child.ParentNode != parent)
                {
                    if (NodeHasNodeRecursively(child, parent))
                    {
                        return;
                    }

                    //удаление чайлд ноды из корневого графа или из графа текущего родителя
                    if (Graph.TryGetNode(child.ID, out _))
                    {
                        Graph.DeleteNode(child);
                    }
                    else
                    {
                        child.ParentNode?.NestedGraph.DeleteNode(child);
                    }

                    //смена родителя
                    oldParent = child.ParentNode;
                    child.ParentNode = parent;

                    //создание вложенного графа родителю, если его нет
                    if (parent.NestedGraph == null)
                    {
                        CreateNewGraph(System.Guid.NewGuid().ToString(), parentView);
                    }

                    //добавление чайлд ноды в вложенный граф нового родителя
                    parent.NestedGraph.AddNode(child);
                }

                _nodeViews.Get(child).SetParent(_graphViews.Get(parent.NestedGraph), haveSavedData);
            }

            RemoveGraphFromNode(oldParent);
        }

        private bool NodeHasNodeRecursively(Node<GraphData, NodeData, EdgeData> node1, Node<GraphData, NodeData, EdgeData> node2)
        {
            if (node1 == null || node1.NestedGraph == null || node2 == null)
            {
                return false;
            }

            if (node1.NestedGraph.HasNode(node2.ID))
            {
                return true;
            }

            foreach (Node<GraphData, NodeData, EdgeData> nestedNode in node1.NestedGraph.Nodes)
            {
                if (NodeHasNodeRecursively(nestedNode, node2))
                {
                    return true;
                }
            }

            return false;
        }

        private void RemoveGraphFromNode(Node<GraphData, NodeData, EdgeData> node)
        {
            if (node == null || node.NestedGraph == null)
            {
                return;
            }

            if (node.NestedGraph.Nodes.Count == 0)
            {
                if (_graphViews.TryGetValue(node.NestedGraph, out IGraphView graphView))
                {
                    _graphViews.Remove(graphView);
                    GraphElementViewFactory.DestroyElementView(graphView);
                }

                node.NestedGraph = null;
            }
        }

        #region Internal Create Methods

        private INodeView CreateViewForNode(Node<GraphData, NodeData, EdgeData> node, bool createInitialNode, bool haveSavedData)
        {
            _nodes[node.ID] = node;
            INodeView view = GraphElementViewFactory.CreateNodeView(node.Data.VisualData, node.Data.Vertex, haveSavedData);
            _nodeViews.Set(node, view);

            foreach (KeyValuePair<string, Event> nodeEvent in node.Data.Events)
            {
                string[] actionsArray = new string[nodeEvent.Value.Actions.Count];

                for (int i = 0; i < actionsArray.Length; i++)
                {
                    actionsArray[i] = nodeEvent.Value.Actions[i].ID;
                }

                INodeEventView eventView = GraphElementViewFactory.CreateNodeEventView(view, nodeEvent.Key);
                _nodeEventViews.Add(nodeEvent.Value, eventView);

                foreach (Action action in nodeEvent.Value.Actions)
                {
                    var actionView = GraphElementViewFactory.CreateNodeActionView(eventView, action.ID);
                    _nodeActionViews.Add(action, actionView);
                    ChangeNodeActionParameter(actionView, action.Parameter);
                }
            }

            if (node.NestedGraph != null)
            {
                CreateNewGraph(node.NestedGraph.ID, view);

                foreach (Node<GraphData, NodeData, EdgeData> nestedNode in node.NestedGraph.Nodes)
                {
                    INodeView nestedView = CreateViewForNode(nestedNode, false, haveSavedData);
                    SetParent(nestedView, view, haveSavedData);
                }
            }

            if (createInitialNode)
            {
                if (_initialNode == null && _nodes.Count > 0)
                {
                    CreateInitialNode();
                }
            }

            return view;
        }

        private IEdgeView CreateViewForEdge(Edge<EdgeData> edge)
        {
            IEdgeView edgeView = null;

            if (_nodeViews.TryGetValue(_nodes[edge.SourceNode], out INodeView sourceNodeView) && _nodeViews.TryGetValue(_nodes[edge.TargetNode], out INodeView targetNodeView))
            {
                _edges[edge.ID] = edge;
                edgeView = GraphElementViewFactory.CreateEdgeView(sourceNodeView, targetNodeView, edge.Data.VisualData, edge.Data.TriggerID, edge.Data.Condition);
                _edgeViews.Add(edge, edgeView);

                foreach (Action action in edge.Data.Actions)
                {
                    var actionView = GraphElementViewFactory.CreateEdgeActionView(edgeView, action.ID);
                    _edgeActionViews.Add(action, actionView);
                    ChangeEdgeActionParameter(actionView, action.Parameter);
                }
            }

            return edgeView;
        }

        #endregion
    }

    #region Interfaces

    public interface IGraphElementViewFactory
    {
        IGraphView CreateGraphView(string graphID, INodeView parentNodeView);
        INodeView CreateNodeView(NodeVisualData visualData, string vertex, bool haveSavedData);
        INodeEventView CreateNodeEventView(INodeView nodeView, string triggerID);
        INodeActionView CreateNodeActionView(INodeEventView eventView, string actionID);
        IEdgeView CreateEdgeView(INodeView sourceNode, INodeView targetNode, EdgeVisualData edgeVisualData, string triggerID, string condition);
        IEdgeActionView CreateEdgeActionView(IEdgeView edgeView, string actionID);
        void DestroyElementView(IGraphElementView view);
    }

    public interface IGraphView : IGraphElementView { }

    public interface IGraphElementView { }

    public interface INodeView : IGraphElementView
    {
        void SetParent(IGraphView parent, bool haveSavedData);
    }

    public interface INodeEventView : IGraphElementView { }

    public interface INodeActionView : IGraphElementView
    {
        void SetParameter(string parameter);
    }

    public interface IEdgeView : IGraphElementView
    {
        void SetTrigger(string triggerID);
        void SetCondition(string condition);
    }

    public interface IEdgeActionView : IGraphElementView
    {
        void SetParameter(string parameter);
    }

    #endregion
}
