using UnityEngine;

namespace Talent.Graphs
{
    public class Note : IClonable<Note>
    {
        public string ID { get; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Pivot { get; set; }
        public string Chunk { get; set; }


        public Note(string id)
        {
            ID = id;
            Type = "informal";
        }

        public virtual Note GetCopy()
        {
            return (Note)MemberwiseClone();
        }
    }
}