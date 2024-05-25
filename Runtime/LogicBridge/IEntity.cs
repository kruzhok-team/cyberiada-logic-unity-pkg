using Talent.Logic.Bus;

namespace Talent.Logic
{
    public interface IEntity
    {
        public IBus Bus { get; }
    }
}
