using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Talent.Graphs
{
    public class NodeData : IClonable<NodeData>
    {
        public const string Vertex_Initial = "initial";
        public const string Vertex_Final = "final";
        public const string Vertex_Choice = "choice";
        public const string Vertex_Terminate = "terminate";

        public NodeVisualData VisualData { get; private set; } = new();
        public string Vertex { get; private set; }

        private readonly List<Event> _events = new();

        /// <summary>
        /// Events of node
        /// </summary>
        public IReadOnlyList<Event> Events => _events;

        public NodeData(string vertex = "")
        {
            Vertex = vertex;
        }

        /// <summary>
        /// Creates a copy of the node data
        /// </summary>
        public NodeData GetCopy()
        {
            NodeData resultData = new NodeData(Vertex);

            foreach (Event nodeEvent in _events)
            {
                resultData.AddEvent(nodeEvent.GetCopy());
            }

            resultData.VisualData.Name = VisualData.Name;
            resultData.VisualData.Position = VisualData.Position;

            return resultData;
        }

        #region Events API

        /// <summary>
        /// Add event to node
        /// </summary>
        public void AddEvent(Event nodeEvent)
        {
            if (!_events.Contains(nodeEvent))
            {
                _events.Add(nodeEvent);
            }
        }

        /// <summary>
        /// Remove existing event from node
        /// </summary>
        public void RemoveEvent(Event nodeEvent)
        {
            _events.Remove(nodeEvent);
        }

        #endregion
    }

    public class NodeVisualData
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
    }
}
