using System.Collections.Generic;
using Talent.Logic.Bus;

namespace Talent.Logic.HSM.Builders
{
    /// <summary>
    ///     Represents data related to an event in the hierarchical state machine.
    /// </summary>
    public class EventData
    {
        private readonly CommandsData _commandsData = new CommandsData();

        public string EventId { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventData"/> class.
        /// </summary>
        /// <param name="eventId">The identifier of the event.</param>
        public EventData(string eventId)
        {
            EventId = eventId;
        }

        /// <summary>
        ///     Adds a command with parameter to the Commands storage.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="parameters">The parameters of the command.</param>
        public void AddCommandStorage(string commandName, string parameters)
        {
            _commandsData.AddCommandStorage(commandName, parameters);
        }

        /// <summary>
        ///     Creates an enumerable collection of Command objects based on the stored command data.
        /// </summary>
        /// <param name="bus">The bus to associate with the commands.</param>
        /// <returns>An enumerable collection of Command objects.</returns>
        public IEnumerable<Command> CreateCommands(IBus bus)
        {
            return _commandsData.CreateCommands(bus);
        }
    }
}
