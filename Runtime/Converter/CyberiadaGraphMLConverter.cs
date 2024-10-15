using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Talent.Graphs;
using UnityEngine;
using Action = Talent.Graphs.Action;
using Event = Talent.Graphs.Event;

namespace Talent.Graph.Cyberiada.Converter
{
    public static class CyberiadaGraphMLConverter
    {
        public static CyberiadaGraph Deserialize(XElement xElement)
        {
            if (xElement == null)
                throw new ArgumentNullException(nameof(xElement));

            XElement graphElement = xElement.Element(FullName("graph"));

            CyberiadaGraph graph = CreateGraph(graphElement);

            CreateEdges(graphElement, graph);

            return graph;
        }

        public static CyberiadaGraph DeserializeFromFile(string filePath)
        {
            XDocument xml = XDocument.Load(filePath);

            return Deserialize(xml.Root);
        }

        public static XElement Serialize(CyberiadaGraph graph)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            XNamespace nameSpace = "http://graphml.graphdrawing.org/xmlns";
            var root = new XElement(nameSpace + "graphml");
            var label = new XElement(FullName("data"), new XAttribute("key", "gFormat"), "Cyberiada-GraphML-1.0");
            root.Add(label);
            XElement graphElement = CreateXmlGraph(graph, root);
            CreateXmlEdges(graph, graphElement);

            return root;
        }

        public static void SerializeToFile(CyberiadaGraph graph, string filePath)
        {
            XElement xmlElement = Serialize(graph);
            var xmlDoc = new XDocument
            {
                Declaration = new XDeclaration("1.0", "UTF-8", null)
            };
            xmlDoc.Add(xmlElement);
            xmlDoc.Save(filePath);
        }

        #region Serialization

        private static XElement CreateXmlGraph(CyberiadaGraph graph, XContainer parentElement)
        {
            var graphElement = new XElement(FullName("graph"), new XAttribute("id", graph.ID), new XAttribute("edgedefault", "directed"));
            var stateMachineDefinition = new XElement(FullName("data"), new XAttribute("key", "dStateMachine"));
            graphElement.Add(stateMachineDefinition);
            AddGraphData(graphElement, graph.Data);
            parentElement.Add(graphElement);
            CreateXmlNodes(graph, graphElement);

            return graphElement;
        }

        private static void AddGraphData(XElement graphElement, GraphData graphData)
        {
            if (graphData == null)
                throw new ArgumentNullException(nameof(graphData));
            
            var graphName = new XElement(FullName("data"), new XAttribute("key", "dName"), graphData.Name);
            var referenceId = new XElement(FullName("data"), new XAttribute("key", "referenceId"), graphData.ReferenceGraphID);
            graphElement.Add(graphName);
            graphElement.Add(referenceId);
        }

        private static void CreateXmlEdges(CyberiadaGraph graph, XElement parentElement)
        {
            foreach (Edge edge in graph.Edges)
                CreateXmlEdge(edge, parentElement);
        }

        private static void CreateXmlEdge(Edge edge, XContainer parentElement)
        {
            var edgeElement = new XElement(FullName("edge"),
                new XAttribute("id", edge.ID),
                new XAttribute("source", edge.SourceNode),
                new XAttribute("target", edge.TargetNode));

            AddGeometryToXmlElement(edgeElement, "dLabelGeometry", edge.Data.VisualData.Position);

            if (string.IsNullOrEmpty(edge.Data.TriggerID) == false)
            {
                var sb = new StringBuilder();

                sb.Append(edge.Data.TriggerID);
                sb.Append(string.IsNullOrEmpty(edge.Data.Condition) ? "" : $"[{edge.Data.Condition}]");
                sb.AppendLine("/");

                foreach (Action action in edge.Data.Actions)
                    sb.AppendLine($"{action.ID}({action.Parameter})");


                AddDataToXmlElement(edgeElement, sb.ToString());
            }

            parentElement.Add(edgeElement);
        }

        private static void CreateXmlNodes(CyberiadaGraph graph, XElement parentElement)
        {
            foreach (Node node in graph.Nodes)
                CreateXmlNode(node, parentElement);
        }

        private static void CreateXmlNode(Node node, XContainer parentElement)
        {
            var nodeElement = new XElement(FullName("node"), new XAttribute("id", node.ID));

            if (IsInitialNode(node))
                MarkAsInitial(nodeElement);

            AddNameToXmlElement(nodeElement, node.Data.VisualData.Name);
            AddGeometryToXmlElement(nodeElement, "dGeometry", node.Data.VisualData.Position);

            var sb = new StringBuilder();

            foreach ((string _, Event value) in node.Data.Events)
            {
                sb.Append(value.TriggerID);
                sb.AppendLine("/");

                foreach (Action action in value.Actions)
                    sb.AppendLine($"{action.ID}({action.Parameter})");

                sb.AppendLine();
            }

            AddDataToXmlElement(nodeElement, sb.ToString());

            if (HasSubGraph(node))
                CreateXmlGraph(node.NestedGraph, nodeElement);

            parentElement.Add(nodeElement);
        }

        private static void AddDataToXmlElement(XElement nodeElement, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            var dataElement = new XElement(FullName("data"), new XAttribute("key", "dData"), value);
            nodeElement.Add(dataElement);
        }

        private static void AddGeometryToXmlElement(XElement nodeElement, string keyName, Vector2 position)
        {
            XElement geometryElement = new XElement(FullName("data"), new XAttribute("key", keyName));
            geometryElement.Add(new XElement(FullName("point"),
                new XAttribute("x", position.x),
                new XAttribute("y", position.y)));

            nodeElement.Add(geometryElement);
        }

        private static void AddNameToXmlElement(XElement nodeElement, string name) =>
            nodeElement.Add(new XElement(FullName("data"), new XAttribute("key", "dName"), name));

        private static bool IsInitialNode(Node node) =>
            node.Data.Vertex == "initial";

        private static void MarkAsInitial(XContainer nodeElement)
        {
            nodeElement.Add(new XElement(FullName("data"), new XAttribute("key", "dVertex"), "initial"));
        }

        private static bool HasSubGraph(Node node) =>
            node.NestedGraph != null;

        #endregion

        #region Deserialization

        private static void CreateNodes(
            XElement xElement, CyberiadaGraph graph,
            Node parentNode = null)
        {
            IEnumerable<XElement> nodes = xElement.Elements(FullName("node"));

            foreach (XElement nodeElement in nodes)
            {
                if (IsNote(nodeElement) || nodeElement.Attribute("id")?.Value == "")
                    continue;

                Node node = CreateNode(nodeElement);
                node.ParentNode = parentNode;

                foreach (XElement subGraph in nodeElement.Elements(FullName("graph")))
                    node.NestedGraph = CreateGraph(subGraph, node);

                graph.AddNode(node);
            }
        }

        private static Node CreateNode(XElement nodeElement)
        {
            string nodeId = nodeElement.Attribute("id")?.Value ?? "";
            NodeData data = CreateNodeData(nodeElement);

            foreach (XElement dataElement in nodeElement.Elements(FullName("data")))
            {
                string name = dataElement.Attribute("key")?.Value ?? "";

                switch (name)
                {
                    case "dName":
                        data.VisualData.Name = dataElement.Value;
                        break;
                    case "dGeometry":
                        data.VisualData.Position = GetGeometryData(dataElement).position;
                        break;
                    case "dData":
                        string dataString = dataElement.Value.Trim();

                        if (string.IsNullOrEmpty(dataString))
                        {
                            break;
                        }

                        string[] lines = dataString.Split(new[] { "\n\n" }, StringSplitOptions.None);

                        foreach (string line in lines)
                        {
                            string trigger = line[..line.IndexOf('/')];

                            var nodeEvent = new Event(trigger);
                            data.AddEvent(nodeEvent);

                            foreach ((string id, string @params) actionData in ParseActions(line))
                                nodeEvent.AddAction(new Action(actionData.id, actionData.@params));
                        }

                        break;
                }
            }

            var node = new Node(nodeId, data);
            return node;
        }

        private static NodeData CreateNodeData(XElement nodeElement)
        {
            XElement dataElement = nodeElement.Elements(FullName("data")).FirstOrDefault(element => element.Attribute("key")?.Value == "dVertex");

            if (dataElement != null)
                return new NodeData(dataElement.Value);

            return new NodeData();
        }

        private static void CreateEdges(XElement xElement, CyberiadaGraph graph)
        {
            IEnumerable<XElement> edges = xElement.Elements(FullName("edge"));

            foreach (XElement edgeElement in edges)
            {
                Edge edge = CreateEdge(edgeElement);

                graph.AddEdge(edge);
            }
        }

        private static Edge CreateEdge(XElement edgeElement)
        {
            string edgeId = edgeElement.Attribute("id")?.Value ?? "";
            string sourceNodeId = edgeElement.Attribute("source")?.Value ?? "";
            string targetNodeId = edgeElement.Attribute("target")?.Value ?? "";

            EdgeData data = CreateEdgeData(edgeElement);

            foreach (XElement dataElement in edgeElement.Elements(FullName("data")))
            {
                string name = dataElement.Attribute("key")?.Value ?? "";

                switch (name)
                {
                    case "dLabelGeometry":
                        data.VisualData.Position = GetGeometryData(dataElement).position;
                        break;
                }
            }

            var edge = new Edge(edgeId, sourceNodeId, targetNodeId, data);
            return edge;
        }

        private static EdgeData CreateEdgeData(XElement edgeElement)
        {
            XElement dataElement = edgeElement.Elements(FullName("data"))
                .FirstOrDefault(data => data.Attribute("key")?.Value == "dData");

            var edgeData = new EdgeData("");

            if (dataElement == null)
                return edgeData;

            string dataString = dataElement.Value;
            string trigger;
            string condition = "";

            Regex regex = new Regex(@"\[(.*?)\]");
            Match match = regex.Match(dataString);

            if (match.Success)
            {
                condition = match.Groups[1].Value;
                trigger = dataString[..match.Index];
            }
            else
            {
                trigger = dataString[..dataString.IndexOf('/')];
            }

            edgeData.SetTrigger(trigger);
            edgeData.SetCondition(condition);

            foreach ((string id, string @params) actionData in ParseActions(dataString))
                edgeData.AddAction(new Action(actionData.id, actionData.@params));

            return edgeData;
        }

        private static CyberiadaGraph CreateGraph(
            XElement graphElement, Node parentNode = null)
        {
            string graphId = graphElement.Attribute("id")?.Value ?? "";
            string graphName = graphElement.Attribute("name")?.Value ?? "";
            string referenceGraphID = graphElement.Attribute("referenceGraphID")?.Value ?? "";
            var graphData = new GraphData();
            var graph = new CyberiadaGraph(graphId, graphData);

            graph.Data.Name = graphName;
            graph.Data.ReferenceGraphID = referenceGraphID;

            CreateNodes(graphElement, graph, parentNode);

            return graph;
        }

        private static (Vector2 position, Vector2 size) GetGeometryData(XElement dataElement)
        {
            XElement rectElement = dataElement.Element(FullName("rect"));
            XElement pointElement = dataElement.Element(FullName("point"));

            if (pointElement != null)
            {
                Vector2 position = ParseVector2("x", "y", pointElement);
                return (position, Vector2.zero);
            }

            if (rectElement != null)
            {
                Vector2 position = ParseVector2("x", "y", rectElement);
                Vector2 size = ParseVector2("width", "height", rectElement);
                return (position, size);
            }

            return (Vector2.zero, Vector2.zero);
        }

        private static Vector2 ParseVector2(string firstKey, string secondKey, XElement dataElement)
        {
            string x = dataElement.Attribute(firstKey)?.Value ?? "";
            string y = dataElement.Attribute(secondKey)?.Value ?? "";
            return new Vector2(
                float.Parse(x, CultureInfo.InvariantCulture),
                float.Parse(y, CultureInfo.InvariantCulture));
        }

        private static bool IsNote(XContainer element)
        {
            bool isNote = element.Elements(FullName("data"))
                .Any(data => data.Attribute("key")?.Value == "dNote");

            return isNote;
        }

        private static IEnumerable<(string, string)> ParseActions(string source)
        {
            const string pattern = @"(?:^|\n)([^()\n]+)\((.*?)\)";
            MatchCollection matches = Regex.Matches(source, pattern);

            var result = new List<(string, string)>();

            foreach (Match match in matches)
            {
                if (match.Groups.Count != 3)
                    continue;

                string id = match.Groups[1].Value.Trim();
                string @params = match.Groups[2].Value.Trim();

                result.Add((id, @params));
            }

            return result;
        }

        #endregion

        private static XName FullName(string localName) =>
            XName.Get(localName, "http://graphml.graphdrawing.org/xmlns");
    }
}