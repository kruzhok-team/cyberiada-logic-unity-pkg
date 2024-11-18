using System.Collections.Generic;

namespace Talent.Graphs
{
    public class Metadata : Note, IClonable<Metadata>
    {
        public new const string Name = "CGML_META";
        public new const string Type = "formal";
        public IReadOnlyDictionary<string, string> Data => _data;
        private readonly Dictionary<string, string> _data = new();
        
        public Metadata(Dictionary<string, string> data = null) : base("coreMeta")
        {
            if (data != null)
            {
                _data = data;
            }
        }

        public new Metadata GetCopy()
        {
            Metadata resultData = new Metadata(new Dictionary<string, string>(_data));
            return resultData;
        }
    }
}