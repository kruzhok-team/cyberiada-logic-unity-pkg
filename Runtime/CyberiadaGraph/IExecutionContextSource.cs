using System.Collections.Generic;

namespace Talent.Graphs
{
    public interface IExecutionContextSource
    {
        IEnumerable<ActionData> GetActions();
        IEnumerable<string> GetEvents();
        IEnumerable<string> GetVariables();
    }

    public class ActionData
    {
        public string ID { get; }
        public ActionParameter Parameter { get; }

        public ActionData(string id, ActionParameter parameter = null)
        {
            ID = id;
            Parameter = parameter;
        }
    }

    public class ActionParameter
    {
        public string Name { get; }
        public string Type { get; }
        public string[] Values { get; }

        public ActionParameter(string name, string type, params string[] values)
        {
            Name = name;
            Type = type;
            Values = values;
        }
    }
}
