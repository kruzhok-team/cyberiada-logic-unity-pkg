using System.Collections.Generic;

namespace Talent.Graph.Cyberiada
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
        public List<ActionParameter> Parameters { get; }

        public ActionData(string id, List<ActionParameter> parameters = null)
        {
            ID = id;
            Parameters = parameters;
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
