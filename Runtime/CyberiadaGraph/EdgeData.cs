using System.Collections.Generic;
using UnityEngine;

namespace Talent.Graphs
{
    public class EdgeData : IClonable<EdgeData>
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
        /// Creates a copy of the edge data
        /// </summary>
        public EdgeData GetCopy()
        {
            EdgeData resultData = new EdgeData(TriggerID);
            resultData.SetCondition(Condition);

            foreach (var action in _actions)
            {
                resultData.AddAction(action.GetCopy());
            }

            resultData.VisualData.Position = VisualData.Position;

            return resultData;
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
