using System;
using System.Collections.Generic;

namespace Talent.Graphs
{
    /// <summary>
    /// Class representing abstract action
    /// </summary>
    public class Action
    {
        /// <summary>
        /// Action ID
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Parameter of action
        /// </summary>
        public List<Tuple<string, string>> Parameters { get; private set; }

        public Action(string id, List<Tuple<string, string>> parameters = null)
        {
            ID = id;
            Parameters = parameters;
        }

        /// <summary>
        /// Creates a copy of the action
        /// </summary>
        public Action GetCopy()
        {
            Action resultAction = new Action(ID, Parameters);

            return resultAction;
        }

        /// <summary>
        /// Set parameter of action
        /// </summary>
        public void SetParameters(List<Tuple<string, string>> parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Custom ToString realization for creating more representive string visualization of action data
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(Action)}, {nameof(ID)}={ID}, {nameof(Parameters)}={Parameters}";
        }
    }
}
