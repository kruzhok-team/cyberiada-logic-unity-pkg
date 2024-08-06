using Talent.Logic.Bus;
﻿using Talent.Graph.Cyberiada;

namespace Talent.Logic
{
    public interface IEntity : IExecutionContextSource
    {
        public IBus Bus { get; }
    }
}
