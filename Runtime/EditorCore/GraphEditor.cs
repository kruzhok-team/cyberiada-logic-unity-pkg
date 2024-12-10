using System;
using System.Collections.Generic;
using System.Linq;
using Talent.Graphs;
using Action = Talent.Graphs.Action;
using Event = Talent.Graphs.Event;

namespace Talent.GraphEditor.Core
{
    public class GraphEditor
    {
        public CyberiadaGraphDocument GraphDocument { get; private set; }

        private IGraphElementViewFactory GraphElementViewFactory { get; }
        private Dictionary<string, Node> _nodes = new Dictionary<string, Node>();
        private Dictionary<string, Edge> _edges = new Dictionary<string, Edge>();

        private BidirectionalDictionary<CyberiadaGraph, IGraphView> _graphViews = new BidirectionalDictionary<CyberiadaGraph, IGraphView>();
        private BidirectionalDictionary<Node, INodeView> _nodeViews = new BidirectionalDictionary<Node, INodeView>();
        private BidirectionalDictionary<Edge, IEdgeView> _edgeViews = new BidirectionalDictionary<Edge, IEdgeView>();
        private BidirectionalDictionary<Event, INodeEventView> _nodeEventViews = new BidirectionalDictionary<Event, INodeEventView>();
        private BidirectionalDictionary<Action, INodeActionView> _nodeActionViews = new BidirectionalDictionary<Action, INodeActionView>();
        private BidirectionalDictionary<Action, IEdgeActionView> _edgeActionViews = new BidirectionalDictionary<Action, IEdgeActionView>();

        private INodeView _initialNodeView;
        private IEdgeView _initialEdgeView;

        public GraphEditor(IGraphElementViewFactory factory)
        {
            GraphElementViewFactory = factory;
        }

        /// <summary>
        /// Set document and rebuild views
        /// </summary>
        /// <param name="graphDocument"></param>
        public void SetGraphDocument(CyberiadaGraphDocument graphDocument)
        {
            GraphDocument = graphDocument;
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

            _initialNodeView = null;
            _initialEdgeView = null;

            if (GraphDocument?.RootGraph == null)
            {
                return;
            }

            foreach (Node node in GraphDocument.RootGraph.Nodes)
            {
                if (node.Data.Vertex == NodeData.Vertex_Initial)
                {
                    _initialNodeView = CreateViewForNode(node, false);
                }
                else
                {
                    CreateViewForNode(node, false);
                }
            }

            if (_nodes.Count > 0)
            {
                CreateInitialNode();
            }

            foreach (Edge edge in GraphDocument.RootGraph.Edges)
            {
                if (edge.SourceNode == NodeData.Vertex_Initial)
                {
                    _initialEdgeView = CreateViewForEdge(edge);
                }
                else
                {
                    CreateViewForEdge(edge);
                }
            }
        }

        public void ConnectToInitialNode(INodeView nodeView)
        {
            if (_initialNodeView == null)
            {
                return;
            }

            if (_initialEdgeView != null)
            {
                RemoveEdge(_initialEdgeView);
            }

            _initialEdgeView = CreateNewEdge(_initialNodeView, nodeView, "");
        }

        public bool TryDuplicateNode(INodeView nodeView, out INodeView duplicatedNode)
        {
            if (!_nodeViews.TryGetValue(nodeView, out Node node))
            {
                duplicatedNode = null;
                return false;
            }

            Node newNode = node.GetCopy(node.Data.GetCopy(), parentNode: node.ParentNode, newID: Guid.NewGuid().ToString());

            GraphDocument.RootGraph.AddNode(newNode);

            duplicatedNode = CreateViewForNode(newNode, true);
            return true;
        }

        private void CreateInitialNode()
        {
            if (_initialNodeView == null)
            {
                Node nodeData = new(NodeData.Vertex_Initial, new NodeData(NodeData.Vertex_Initial));

                GraphDocument.RootGraph.AddNode(nodeData);
                nodeData.Data.VisualData.Name = "INIT";

                _initialNodeView = CreateViewForNode(nodeData, true);
                _nodeViews.Set(nodeData, _initialNodeView);
            }
        }

        private void CreateRandomInitialEdge()
        {
            if (_initialEdgeView == null && _initialNodeView != null)
            {
                Node targetNode = _nodes.Values.FirstOrDefault(node => node.Data.Vertex != NodeData.Vertex_Initial);

                if (targetNode == null)
                {
                    return;
                }

                Edge edge = new Edge(NodeData.Vertex_Initial + targetNode.ID, NodeData.Vertex_Initial, targetNode.ID, new EdgeData(""));
                GraphDocument.RootGraph.AddEdge(edge);

                if (_nodeViews.TryGetValue(targetNode, out INodeView targetNodeView))
                {
                    _initialEdgeView = GraphElementViewFactory.CreateEdgeView(_initialNodeView, targetNodeView, edge.Data.VisualData, edge.Data.TriggerID, edge.Data.Condition);
                    _edgeViews.Add(edge, _initialEdgeView);
                }
            }
        }

        #region Public Create Methods

        public IGraphView CreateNewGraph(string id, INodeView nodeView)
        {
            if (_nodeViews.TryGetValue(nodeView, out Node node) && node.NestedGraph == null)
            {
                node.NestedGraph = new CyberiadaGraph(id, new GraphData());
            }

            IGraphView graphView = GraphElementViewFactory.CreateGraphView(id, nodeView);
            _graphViews.Add(node.NestedGraph, graphView);
            return graphView;
        }

        public INodeView CreateNewNode(string name)
        {
            Node node = new Node(System.Guid.NewGuid().ToString(), new NodeData());
            GraphDocument.RootGraph.AddNode(node);
            node.Data.VisualData.Name = name;

            INodeView newNode = CreateViewForNode(node, true);

            if (_initialNodeView == null)
            {
                CreateInitialNode();
                ConnectToInitialNode(newNode);
            }

            return newNode;
        }

        public IEdgeView CreateNewEdge(INodeView sourceView, INodeView targetView, string triggerID)
        {
            Node sourceNode = _nodeViews.Get(sourceView);
            Node targetNode = _nodeViews.Get(targetView);

            if ((sourceView == _initialNodeView || targetView == _initialNodeView) && _initialEdgeView != null)
            {
                return null;
            }

            Edge edge = new Edge(System.Guid.NewGuid().ToString(), sourceNode.ID, targetNode.ID, new EdgeData(triggerID));
            GraphDocument.RootGraph.AddEdge(edge);

            return CreateViewForEdge(edge);
        }

        public IEdgeView CreatePreviewEdgeView(INodeView sourceView)
        {
            IEdgeView edgeView = GraphElementViewFactory.CreateEmptyEdgeView(sourceView);
            return edgeView;
        }

        public bool TryApplyPreview(IEdgeView edgeView, INodeView sourceView, INodeView targetView, string triggerID, EdgeVisualData visualData)
        {
            Node sourceNode = _nodeViews.Get(sourceView);
            Node targetNode = _nodeViews.Get(targetView);
            if (sourceNode == targetNode)
            {
                return false;
            }

            Edge edge = new Edge(System.Guid.NewGuid().ToString(), sourceNode.ID, targetNode.ID, new EdgeData(triggerID));
            edge.Data.VisualData = visualData;
            GraphDocument.RootGraph.AddEdge(edge);
            _edges[edge.ID] = edge;
            _edgeViews.Add(edge, edgeView);
            foreach (Action action in edge.Data.Actions)
            {
                IEdgeActionView actionView = GraphElementViewFactory.CreateEdgeActionView(edgeView, action.ID);
                _edgeActionViews.Add(action, actionView);
                ChangeEdgeActionParameter(actionView, action.Parameters);
            }

            return true;
        }

        public INodeEventView CreateNewNodeEvent(INodeView nodeView, string triggerID)
        {
            INodeEventView result = default;

            if (_nodeViews.TryGetValue(nodeView, out Node node))
            {
                Event nodeEvent = new Event(triggerID);
                node.Data.AddEvent(nodeEvent);

                if (!_nodeEventViews.ContainsKey(nodeEvent))
                {
                    _nodeEventViews.Set(nodeEvent, GraphElementViewFactory.CreateNodeEventView(_nodeViews.Get(node), triggerID, nodeEvent));
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

            if (_edgeViews.TryGetValue(edgeView, out Edge edge))
            {
                Action eventAction = new Action(actionID);

                edge.Data.AddAction(eventAction);
                _edgeActionViews.Set(eventAction, GraphElementViewFactory.CreateEdgeActionView(edgeView, actionID));
                result = _edgeActionViews.Get(eventAction);
            }

            return result;
        }

        #endregion

        public void ChangeNodeEventTrigger(INodeEventView nodeEventView, Event @event, string triggerID)
        {
            @event.SetTrigger(triggerID);
            nodeEventView.SetTrigger(triggerID);
        }

        public void ChangeEdgeTrigger(IEdgeView edgeView, string triggerID)
        {
            if (_edgeViews.TryGetValue(edgeView, out Edge edge))
            {
                edge.Data.SetTrigger(triggerID);
                edgeView.SetTrigger(triggerID);
            }
        }

        public void ChangeNodeEventCondition(INodeView nodeView, INodeEventView nodeEventView, string condition)
        {
            if (_nodeViews.TryGetValue(nodeView, out Node node) && _nodeEventViews.TryGetValue(nodeEventView, out Event nodeEvent) && nodeEvent.Condition != condition)
            {
                nodeEvent.SetCondition(condition);
                nodeEventView.SetCondition(condition);
            }
        }

        public void ChangeEdgeCondition(IEdgeView edgeView, string condition)
        {
            if (_edgeViews.TryGetValue(edgeView, out Edge edge))
            {
                edge.Data.SetCondition(condition);
                edgeView.SetCondition(condition);
            }
        }

        public void ChangeNodeActionParameter(INodeActionView actionView, List<Tuple<string, string>> parameters)
        {
            if (_nodeActionViews.TryGetValue(actionView, out Action action))
            {
                action.SetParameters(parameters);
                actionView.SetParameters(parameters);
            }
        }

        public void ChangeEdgeActionParameter(IEdgeActionView actionView, List<Tuple<string, string>> parameters)
        {
            if (_edgeActionViews.TryGetValue(actionView, out Action action))
            {
                action.SetParameters(parameters);
                actionView.SetParameters(parameters);
            }
        }

        #region Public Remove Methods

        public void RemoveNode(INodeView nodeView, bool createInitialEdge = true)
        {
            if (_nodeViews.TryGetValue(nodeView, out Node node))
            {
                Node oldParent = node.ParentNode;

                foreach (Event nodeEvent in node.Data.Events)
                {
                    foreach (Action action in nodeEvent.Actions)
                    {
                        INodeActionView nodeActionView = _nodeActionViews.Get(action);

                        _nodeActionViews.Remove(nodeActionView);
                        GraphElementViewFactory.DestroyElementView(nodeActionView);
                    }

                    if (_nodeEventViews.TryGetValue(nodeEvent, out INodeEventView eventView))
                    {
                        _nodeEventViews.Remove(nodeEvent);
                        GraphElementViewFactory.DestroyElementView(eventView);
                    }
                }

                foreach (Edge edge in GraphDocument.RootGraph.Edges.Where(edge => edge.SourceNode == node.ID || edge.TargetNode == node.ID).ToArray())
                {
                    if (_edgeViews.TryGetValue(edge, out IEdgeView edgeView))
                    {
                        foreach (Action action in edge.Data.Actions)
                        {
                            IEdgeActionView edgeActionView = _edgeActionViews.Get(action);

                            if (_edgeViews.TryGetValue(edgeView, out Edge nodeEvent) && _edgeActionViews.TryGetValue(edgeActionView, out Action eventAction))
                            {
                                _edgeActionViews.Remove(edgeActionView);
                                GraphElementViewFactory.DestroyElementView(edgeActionView);
                            }
                        }

                        _edges.Remove(edge.ID);
                        _edgeViews.Remove(edgeView);
                        GraphDocument.RootGraph.DeleteEdge(edge);
                        GraphElementViewFactory.DestroyElementView(edgeView);

                        if (edge.SourceNode == NodeData.Vertex_Initial || edge.TargetNode == NodeData.Vertex_Initial)
                        {
                            _initialEdgeView = null;
                        }
                    }
                }

                if (node.NestedGraph != null)
                {
                    foreach (Node child in node.NestedGraph.Nodes.ToArray())
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
                GraphDocument.RootGraph.DeleteNode(node);
                _nodeViews.Remove(node);
                GraphElementViewFactory.DestroyElementView(nodeView);

                if (_nodes.Count > 1)
                {
                    if (createInitialEdge)
                    {
                        CreateRandomInitialEdge();
                    }
                }
                else
                {
                    GraphDocument.RootGraph.DeleteNode(_nodes[NodeData.Vertex_Initial]);
                    GraphElementViewFactory.DestroyElementView(_initialNodeView);
                    _nodeViews.Remove(_initialNodeView);
                    _initialNodeView = null;
                }

                RemoveGraphFromNode(oldParent);
            }
        }

        public void RemoveEdge(IEdgeView edgeView)
        {
            if (_edgeViews.TryGetValue(edgeView, out Edge edge))
            {
                foreach (Action action in edge.Data.Actions)
                {
                    IEdgeActionView edgeActionView = _edgeActionViews.Get(action);

                    _edgeActionViews.Remove(edgeActionView);
                    GraphElementViewFactory.DestroyElementView(edgeActionView);
                }

                _edges.Remove(edge.ID);
                GraphDocument.RootGraph.DeleteEdge(edge);
                _edgeViews.Remove(edgeView);
                GraphElementViewFactory.DestroyElementView(edgeView);
            }
        }

        public void RemoveNodeEvent(INodeView nodeView, INodeEventView nodeEventView)
        {
            Node node = _nodeViews.Get(nodeView);
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
            if (_edgeViews.TryGetValue(edgeView, out Edge nodeEvent) && _edgeActionViews.TryGetValue(edgeActionView, out Action eventAction))
            {
                nodeEvent.Data.RemoveAction(eventAction);
                _edgeActionViews.Remove(edgeActionView);
                GraphElementViewFactory.DestroyElementView(edgeActionView);
            }
        }

        #endregion

        public void SetParent(INodeView childView, INodeView parentView, bool layoutAutomatically)
        {
            if (childView == null || !_nodeViews.TryGetValue(childView, out Node child) || _initialNodeView != null && childView == _initialNodeView)
            {
                return;
            }

            Node oldParent = null;

            if (parentView == null)
            {
                if (child.ParentNode != null)
                {
                    child.ParentNode.NestedGraph.DeleteNode(child);
                    GraphDocument.RootGraph.AddNode(child);
                }

                oldParent = child.ParentNode;
                child.ParentNode = null;

                _nodeViews.Get(child).SetParent(null, layoutAutomatically);
            }
            else if (_nodeViews.TryGetValue(parentView, out Node parent) && child != parent && (_initialNodeView == null || parentView != _initialNodeView))
            {
                if (child.ParentNode != parent)
                {
                    if (NodeHasNodeRecursively(child, parent))
                    {
                        return;
                    }

                    //удаление чайлд ноды из корневого графа или из графа текущего родителя
                    if (GraphDocument.RootGraph.TryGetNode(child.ID, out _))
                    {
                        GraphDocument.RootGraph.DeleteNode(child);
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

                _nodeViews.Get(child).SetParent(_graphViews.Get(parent.NestedGraph), layoutAutomatically);
            }

            RemoveGraphFromNode(oldParent);
        }

        // TODO: remove recursion
        private bool NodeHasNodeRecursively(Node node1, Node node2)
        {
            if (node1 == null || node1.NestedGraph == null || node2 == null)
            {
                return false;
            }

            if (node1.NestedGraph.HasNode(node2.ID))
            {
                return true;
            }

            foreach (Node nestedNode in node1.NestedGraph.Nodes)
            {
                if (NodeHasNodeRecursively(nestedNode, node2))
                {
                    return true;
                }
            }

            return false;
        }

        private void RemoveGraphFromNode(Node node)
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

        private INodeView CreateViewForNode(Node node, bool layoutAutomatically)
        {
            _nodes[node.ID] = node;
            INodeView view = GraphElementViewFactory.CreateNodeView(node.Data.VisualData, node.Data.Vertex, layoutAutomatically);
            _nodeViews.Set(node, view);

            foreach (Event nodeEvent in node.Data.Events)
            {
                string[] actionsArray = new string[nodeEvent.Actions.Count];

                for (int i = 0; i < actionsArray.Length; i++)
                {
                    actionsArray[i] = nodeEvent.Actions[i].ID;
                }

                INodeEventView eventView = GraphElementViewFactory.CreateNodeEventView(view, nodeEvent.TriggerID, nodeEvent);
                _nodeEventViews.Add(nodeEvent, eventView);
                eventView.SetCondition(nodeEvent.Condition);

                foreach (Action action in nodeEvent.Actions)
                {
                    var actionView = GraphElementViewFactory.CreateNodeActionView(eventView, action.ID);
                    _nodeActionViews.Add(action, actionView);
                    ChangeNodeActionParameter(actionView, action.Parameters);
                }
            }

            if (node.NestedGraph != null)
            {
                CreateNewGraph(node.NestedGraph.ID, view);

                foreach (Node nestedNode in node.NestedGraph.Nodes)
                {
                    INodeView nestedView = CreateViewForNode(nestedNode, layoutAutomatically);
                    SetParent(nestedView, view, layoutAutomatically);
                }
            }

            return view;
        }

        private IEdgeView CreateViewForEdge(Edge edge)
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
                    ChangeEdgeActionParameter(actionView, action.Parameters);
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
        INodeView CreateNodeView(NodeVisualData visualData, string vertex, bool layoutAutomatically);
        INodeEventView CreateNodeEventView(INodeView nodeView, string triggerID, Event @event);
        INodeActionView CreateNodeActionView(INodeEventView eventView, string actionID);
        IEdgeView CreateEmptyEdgeView(INodeView sourceNode);

        IEdgeView CreateEdgeView(INodeView sourceNode, INodeView targetNode, EdgeVisualData edgeVisualData,
            string triggerID, string condition);

        IEdgeActionView CreateEdgeActionView(IEdgeView edgeView, string actionID);
        void DestroyElementView(IGraphElementView view);
    }

    public interface IGraphView : IGraphElementView { }

    public interface IGraphElementView { }

    public interface INodeView : IGraphElementView
    {
        void SetParent(IGraphView parent, bool layoutAutomatically);
    }

    public interface INodeEventView : IGraphElementView
    {
        void SetCondition(string condition);
        void SetTrigger(string triggerID);
    }

    public interface INodeActionView : IGraphElementView
    {
        void SetParameters(List<Tuple<string, string>> parameters);
    }

    public interface IEdgeView : IGraphElementView
    {
        void SetTrigger(string triggerID);
        void SetCondition(string condition);
    }

    public interface IEdgeActionView : IGraphElementView
    {
        void SetParameters(List<Tuple<string, string>> parameters);
    }

    #endregion
}
