using System;
using System.Collections.Generic;
using Talent.Logic.Bus;

namespace Talent.Logic.HSM
{
    /// <summary>
    ///     Represents an event in the HSM.
    /// </summary>
    public class Event
    {
        private readonly IEventBus _bus;
        private readonly IEnumerable<Command> _commands;
        private readonly string _eventName;

        /// <summary>
        ///     Initializes a new instance of the Event class.
        /// </summary>
        /// <param name="bus">The event bus for handling event-related operations.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="commands">The commands associated with the event.</param>
        public Event(IEventBus bus, string eventName, IEnumerable<Command> commands)
        {
            _eventName = eventName;
            _bus = bus;
            _commands = commands;
        }

        /// <summary>
        ///     Activates the event by adding an event listener to the event bus.
        /// </summary>
        public void Activate()
        {
            _bus.AddEventListener(_eventName, Receive);
        }

        /// <summary>
        ///     Deactivates the event by removing the event listener from the event bus.
        /// </summary>
        public void Deactivate()
        {
            _bus.RemoveEventListener(_eventName, Receive);
        }

        private bool Receive(List<Tuple<string, string>> parameters = null)
        {
            foreach (Command commandStorage in _commands)
            {
                commandStorage.Make();
            }

            return false;
        }
    }
}
