using System.Collections.Generic;
using System.Linq;

namespace Talent.Graphs
{
    public class GraphData : IClonable<GraphData>
    {
        public string Name { get; set; }
        public string ReferenceGraphID { get; set; }
        public Dictionary<string, string> MetaData { get; set; }

        private List<Note> Notes { get; set; } = new();

        public void AddNote(Note note)
        {
            Notes.Add(note);
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
                Notes = Notes.Select(note => note.GetCopy()).ToList(),
                MetaData = new Dictionary<string, string>(MetaData)
            };

            return resultData;
        }
    }
}