using System.Collections.Generic;
using UnityEngine;

namespace Talent.Graph.Cyberiada
{
    public class EdgeData
    {
        public string TriggerID { get; private set; }
        public string Condition { get; private set; }
        public EdgeVisualData VisualData { get; private set; } = new();

        private readonly List<Action> _actions = new();

        /// <summary>
        /// List of actions
        /// </summary>
        public IReadOnlyList<Action> Actions => _actions;

        public EdgeData(string triggerID)
        {
            TriggerID = triggerID;
        }

        /// <summary>
        /// Change trigger id for this edge
        /// </summary>
        public void SetTrigger(string triggerID)
        {
            TriggerID = triggerID;
        }

        /// <summary>
        /// Change condition for this edge
        /// </summary>
        public void SetCondition(string condition)
        {
            Condition = condition;
        }

        #region EventAction API

        /// <summary>
        /// Add action to edge
        /// </summary>
        public void AddAction(Action eventAction)
        {
            _actions.Add(eventAction);
        }

        /// <summary>
        /// Remove existing action from edge
        /// </summary>
        public void RemoveAction(Action eventAction)
        {
            if (_actions.Contains(eventAction))
            {
                _actions.Remove(eventAction);
            }
        }

        #endregion
    }

    public class EdgeVisualData
    {
        public Vector2 Position { get; set; }
    }
}
