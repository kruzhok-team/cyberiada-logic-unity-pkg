using System.Collections.Generic;
using System.Linq;

namespace Talent.Graphs
{
    public class GraphData : IClonable<GraphData>
    {
        public string Name { get; set; }
        public string ReferenceGraphID { get; set; }
        public Metadata DocumentMeta { get; set; }
        public IReadOnlyCollection<Note> Notes => _notes;
        private List<Note> _notes = new();

        public void AddNote(Note note)
        {
            _notes.Add(note);
        }

        /// <summary>
        /// Creates a copy of the graph data
        /// </summary>
        public GraphData GetCopy()
        {
            GraphData resultData = new GraphData
            {
                Name = Name,
                ReferenceGraphID = ReferenceGraphID,
                _notes = _notes.Select(note => note.GetCopy()).ToList(),
                DocumentMeta = DocumentMeta.GetCopy()
            };

            return resultData;
        }
    }
}