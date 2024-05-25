using System.Collections.Generic;
using UnityEngine;

namespace Talent.Graph.Cyberiada
{
    public class NodeData
    {
        public const string Vertex_Initial = "initial";
        public const string Vertex_Final = "final";
        public const string Vertex_Choice = "choice";
        public const string Vertex_Terminate = "terminate";

        public NodeVisualData VisualData { get; private set; } = new();
        public string Vertex { get; private set; }

        private readonly Dictionary<string, Event> _events = new();

        /// <summary>
        /// Events of node
        /// </summary>
        public IReadOnlyDictionary<string, Event> Events => _events;

        public NodeData(string vertex = "")
        {
            Vertex = vertex;
        }

        #region Events API

        /// <summary>
        /// Add event to node
        /// </summary>
        public void AddEvent(Event nodeEvent)
        {
            _events[nodeEvent.TriggerID] = nodeEvent;
        }

        /// <summary>
        /// Remove existing event from node
        /// </summary>
        public void RemoveEvent(Event nodeEvent)
        {
            if (_events.ContainsKey(nodeEvent.TriggerID))
            {
                _events.Remove(nodeEvent.TriggerID);
            }
        }

        #endregion
    }

    public class NodeVisualData
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
    }
}
