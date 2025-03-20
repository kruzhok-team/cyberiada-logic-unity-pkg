using System;
using System.Collections.Generic;
namespace Talent.Graphs
{
    /// <summary>
    /// Элемент графа, представляющий узел
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Уникальный идентификатор узла
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Родительский узел данного узла
        /// </summary>
        public Node ParentNode { get; set; }

        /// <summary>
        /// Подграф хранимый в данном узле
        /// </summary>
        public CyberiadaGraph NestedGraph { get; set; }

        /// <summary>
        /// Данные узла
        /// </summary>
        public NodeData Data { get; }

        /// <summary>
        /// Конструктор узла
        /// </summary>
        /// <param name="id">Уникальный идентификатор узла</param>
        /// <param name="data">Состояние узла</param>
        /// <exception cref="ArgumentNullException">Если идентификатор равен null, выбрасывается исключение</exception>
        public Node(string id, NodeData data)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException($"Can't create Node with id '{id}'. ID can't be null or empty");
            }

            ID = id;
            Data = data;
        }

        /// <summary>
        /// Создает копию узла
        /// </summary>
        /// <param name="data">Состояние узла</param>
        /// <param name="parentNode">Родительский узел для копии узла</param>
        /// <param name="newID">Новый идентификатор узла, если отсутствует, используется оригинальный идентификатор</param>
        /// <returns>Копия узла</returns>
        public Node GetCopy(NodeData data, Node parentNode = null, string newID = null)
        {
            if (newID == "")
            {
                throw new ArgumentNullException($"Can't copy Node with newID '{newID}'. ID can't be null or empty");
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
        /// Создает отдельную независимую копию узла и всех дочерних элементов
        /// </summary>
        /// <param name="rootGraph">Корневой граф</param>
        /// <returns>Копия узла</returns>
        public Node Duplicate(CyberiadaGraph rootGraph)
        {
            Node resultNode = null;
            List<(Edge edge, CyberiadaGraph owner)> edges = new List<(Edge, CyberiadaGraph)>();

            foreach (Edge edge in rootGraph.Edges)
            {
                edges.Add((edge, rootGraph));
            }
            
            Dictionary<string, string> newNodeIdByOld = new Dictionary<string, string>();
            Stack<(Node originNode, CyberiadaGraph duplicatedGraph, Node duplicatedParentNode)> nodes = 
                new Stack<(Node originNode, CyberiadaGraph duplicatedGraph, Node duplicatedParentNode)>();
            nodes.Push((this, ParentNode?.NestedGraph, ParentNode));

            do
            {
                (Node originNode, CyberiadaGraph duplicatedGraph, Node duplicatedParentNode) = nodes.Pop();
                string nodeId = Guid.NewGuid().ToString();
                newNodeIdByOld[originNode.ID] = nodeId;
                Node duplicatedNode = new Node(nodeId, originNode.Data.GetCopy());
                resultNode ??= duplicatedNode;
                duplicatedNode.ParentNode = duplicatedParentNode;

                if (originNode.NestedGraph != null)
                {
                    CyberiadaGraph resultGraph = new CyberiadaGraph(Guid.NewGuid().ToString(), originNode.NestedGraph.Data.GetCopy());

                    foreach (Node childNode in originNode.NestedGraph.Nodes)
                    {
                        nodes.Push((childNode, resultGraph, duplicatedNode));
                    }

                    foreach (Edge edge in originNode.NestedGraph.Edges)
                    {
                        edges.Add((edge, resultGraph));
                    }

                    duplicatedNode.NestedGraph = resultGraph;
                }
                
                duplicatedGraph?.AddNode(duplicatedNode);
                
            } while (nodes.Count > 0);

            for (int i = 0; i < edges.Count; i++)
            {
                if (newNodeIdByOld.ContainsKey(edges[i].edge.SourceNode) && newNodeIdByOld.ContainsKey(edges[i].edge.TargetNode))
                {
                    Edge edge = new Edge(Guid.NewGuid().ToString(), newNodeIdByOld[edges[i].edge.SourceNode], 
                        newNodeIdByOld[edges[i].edge.TargetNode], edges[i].edge.Data.GetCopy());
                    edges[i].owner.AddEdge(edge);
                }
            }
            
            return resultNode;
        }
    }
}
