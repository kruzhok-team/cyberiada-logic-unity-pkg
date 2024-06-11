using System;
using System.Collections.Generic;
using System.Linq;
using Talent.Graph;
using Talent.Graph.Cyberiada;
using Talent.Logic.Bus;
using Talent.Logic.HSM.Builders;
using Action = Talent.Graph.Cyberiada.Action;

namespace Talent.Logic.HSM
{
    /// <summary>
    ///     Class that converts a graph source into a hierarchical state machine.
    /// </summary>
    public class CyberiadaHSMConverter : ILogicInterpreter<Graph<GraphData, NodeData, EdgeData>, IBus>
    {
        private const string EnterEventId = "entry";
        private const string ExitEventId = "exit";

        private Graph<GraphData, NodeData, EdgeData> _sourceGraph;

        /// <summary>
        ///     Processes the graph source to build a hierarchical state machine.
        /// </summary>
        /// <param name="source">The graph data containing nodes and edges.</param>
        /// <param name="bus">The bus for event handling.</param>
        /// <returns>The built hierarchical state machine behavior.</returns>
        public IBehavior Process(Graph<GraphData, NodeData, EdgeData> source, IBus bus)
        {
            StateBuilder builder = new StateBuilder(bus, source.ID);
            _sourceGraph = source;

            foreach (Node<GraphData, NodeData, EdgeData> node in source.Nodes)
            {
                IEnumerable<Edge<EdgeData>> edges = GetEdges(node.ID);

                StateBuilder childBuilder = CreateChildBuilder(node, edges, bus);
                builder.AddChildState(childBuilder);
            }

            State state = builder.Build();
            return new HSMBehavior(state, bus, NodeData.Vertex_Initial);
        }

        private StateBuilder CreateChildBuilder(
            Node<GraphData, NodeData, EdgeData> node,
            IEnumerable<Edge<EdgeData>> edges,
            IBus bus)
        {
            StateBuilder builder = new StateBuilder(bus, node.ID);

            builder.AddLabel(node.Data.VisualData.Name);
            AddEventToCommands(node, builder);
            AddTransitions(edges, builder);
            AddSubHsm(node, bus, builder);

            return builder;
        }

        private void AddEventToCommands(Node<GraphData, NodeData, EdgeData> node, StateBuilder builder)
        {
            foreach (Graph.Cyberiada.Event @event in node.Data.Events.Values)
            {
                foreach (Action action in @event.Actions)
                {
                    (string module, string command, string parameters) data = ParseActionData(action);
                    string commandName = GetFullCommandName(data);

                    switch (@event.TriggerID)
                    {
                        case EnterEventId:
                            builder.AddEnter(commandName, data.parameters);

                            break;

                        case ExitEventId:
                            builder.AddExit(commandName, data.parameters);

                            break;
                        default:
                            builder.AddCommandOnEvent(@event.TriggerID, commandName, data.parameters);

                            break;
                    }
                }
            }
        }

        private void AddTransitions(IEnumerable<Edge<EdgeData>> edges, StateBuilder builder)
        {
            foreach (Edge<EdgeData> edge in edges)
            {
                TransitionBuilder transitionBuilder = new TransitionBuilder();
                transitionBuilder.AddDebugId($"{edge.SourceNode}:{edge.TargetNode}");

                transitionBuilder.ToNextStateOnEvent(
                    edge.TargetNode,
                    edge.Data.TriggerID == "" ? NodeData.Vertex_Initial : edge.Data.TriggerID);

                transitionBuilder.WithCondition(edge.Data.Condition);

                foreach (Action action in edge.Data.Actions)
                {
                    (string module, string command, string parameters) actionData = ParseActionData(action);
                    transitionBuilder.AddCommand(GetFullCommandName(actionData), actionData.parameters);
                }

                builder.AddTransition(transitionBuilder);
            }
        }

        private void AddSubHsm(Node<GraphData, NodeData, EdgeData> rootNode, IBus bus, StateBuilder builder)
        {
            if (rootNode.NestedGraph == null)
                return;

            foreach (Node<GraphData, NodeData, EdgeData> node in rootNode.NestedGraph.Nodes)
            {
                StateBuilder childBuilder = CreateChildBuilder(node, GetEdges(node.ID), bus);
                builder.AddChildState(childBuilder);
            }
        }

        private string GetFullCommandName((string module, string command, string parameters) data)
        {
            return $"{data.module}.{data.command}";
        }

        private (string module, string command, string parameters) ParseActionData(Action action)
        {
            string[] actionId = action.ID.Split('.');
            string module = actionId[0];
            string command = actionId[1];
            string parameters = action.Parameter;

            return new ValueTuple<string, string, string>(module, command, parameters);
        }

        private IEnumerable<Edge<EdgeData>> GetEdges(string bySourceNodeId)
        {
            IEnumerable<Edge<EdgeData>> edges = _sourceGraph.Edges.Where(edge => edge.SourceNode == bySourceNodeId);
            return edges;
        }
    }
}
