namespace Talent.Graph.Cyberiada
{
    public class GraphData : IClonable<GraphData>
    {
        public string Name { get; set; }

        /// <summary>
        /// Creates a copy of the graph data
        /// </summary>
        public GraphData GetCopy()
        {
            GraphData resultData = new GraphData { Name = Name };

            return resultData;
        }
    }
}
