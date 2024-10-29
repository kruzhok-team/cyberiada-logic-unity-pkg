using System;
using System.Collections.Generic;

namespace Talent.Logic.Bus
{
    public interface ICommandBus
    {
        void AddCommandListener(Listener listener);

        void AddCommandListener(string commandName, Listener listener);

        void RemoveCommandListener(string commandName, Listener listener);

        void InvokeEvent(string eventName, List<Tuple<string, string>> value = null);
    }
}
