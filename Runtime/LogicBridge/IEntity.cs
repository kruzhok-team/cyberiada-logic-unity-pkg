using Talent.Graph.Cyberiada;
using Talent.Logic.Bus;

namespace Talent.Logic
{
    public interface IEntity : IExecutionContextSource
    {
        public IBus Bus { get; }
    }
}
