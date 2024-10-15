namespace Talent.Graphs
{
    public interface IGraphComparator
    {
        bool IsGraphEqual(CyberiadaGraph graph, CyberiadaGraph otherGraph);
    }
}
