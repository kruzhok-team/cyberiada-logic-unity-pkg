using System.Collections.Generic;

namespace Talent.Graphs
{
    public class Metadata : Note, IClonable<Metadata>
    {
        public IReadOnlyDictionary<string, string> Data => _data;
        private readonly Dictionary<string, string> _data;
        
        public Metadata(string id, Dictionary<string, string> data) : base(id)
        {
            _data = data;
        }

        public new Metadata GetCopy()
        {
            Note note = base.GetCopy();
            Metadata resultData = new Metadata(note.ID, _data);
            return resultData;
        }
    }
}