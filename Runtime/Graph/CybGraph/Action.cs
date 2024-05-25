namespace Talent.Graph.Cyberiada
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
        public string Parameter { get; private set; }

        public Action(string id, string parameter = null)
        {
            ID = id;
            Parameter = parameter;
        }

        /// <summary>
        /// Set parameter of action
        /// </summary>
        public void SetParameter(string parameter)
        {
            Parameter = parameter;
        }

        /// <summary>
        /// Custom ToString realization for creating more representive string visualization of action data
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(Action)}, {nameof(ID)}={ID}, {nameof(Parameter)}={Parameter}";
        }
    }
}
