namespace Talent.Graphs
{
    public class Note : IClonable<Note>
    {
        public string ID { get; }
        public NodeVisualData VisualData { get; set; } = new();
        public string Type { get; set; }
        public string Data { get; set; }
        public string Pivot { get; set; }
        public string Chunk { get; set; }


        public Note(string id)
        {
            ID = id;
        }

        public Note GetCopy()
        {
            var resultData = new Note(ID)
            {
                VisualData =
                {
                    Position = VisualData.Position,
                    Name = VisualData.Name
                },
                Type = Type,
                Data = Data,
                Pivot = Pivot,
                Chunk = Chunk
            };
            return resultData;
        }
    }
}