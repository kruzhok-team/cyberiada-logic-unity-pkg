using System.Collections.Generic;

namespace Talent.Graph.Cyberiada
{
    /// <summary>
    /// Class representing node event with condition and corresponding list of actions
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Node that event is connected to
        /// </summary>
        public string TriggerID { get; private set; }

        private readonly List<Action> _actions = new();

        /// <summary>
        /// List of actions
        /// </summary>
        public IReadOnlyList<Action> Actions => _actions;

        public Event(string triggerID)
        {
            TriggerID = triggerID;
        }

        /// <summary>
        /// Change trigger id for this event
        /// </summary>
        public void SetTrigger(string triggerID)
        {
            TriggerID = triggerID;
        }

        #region EventAction API

        /// <summary>
        /// Add action to event
        /// </summary>
        public void AddAction(Action action)
        {
            _actions.Add(action);
        }

        /// <summary>
        /// Remove action from event if it has provided one
        /// </summary>
        public void RemoveAction(Action eventAction)
        {
            if (_actions.Contains(eventAction))
            {
                _actions.Remove(eventAction);
            }
        }

        #endregion

        /// <summary>
        /// Custom ToString realization for creating more representive string visualization of event data
        /// </summary>
        public override string ToString()
        {
            return $"{TriggerID}\n{string.Join("\n", Actions)}";
        }
    }
}
