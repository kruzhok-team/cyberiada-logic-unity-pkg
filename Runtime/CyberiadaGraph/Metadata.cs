using System.Collections.Generic;

namespace Talent.Graphs
{
    public class Metadata : Note, IClonable<Metadata>
    {
        public new const string Name = "CGML_META";
        public new const string Type = "formal";
        public IReadOnlyDictionary<string, string> Data => _data;
        private readonly Dictionary<string, string> _data;
        
        public Metadata(string id, Dictionary<string, string> data) : base(id)
        {
            _data = data;
        }

        public new Metadata GetCopy()
        {
            Metadata resultData = new Metadata(ID, new Dictionary<string, string>(_data));
            return resultData;
        }
    }
}