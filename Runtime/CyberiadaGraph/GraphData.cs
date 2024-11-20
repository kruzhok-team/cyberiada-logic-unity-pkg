namespace Talent.Graphs
{
    public class GraphData : IClonable<GraphData>
    { 
        public GraphData GetCopy()
        {
            GraphData resultData = new GraphData();
            return resultData;
        }
    }
}