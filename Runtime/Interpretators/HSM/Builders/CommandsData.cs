using System.Collections.Generic;
using Talent.Logic.Bus;

namespace Talent.Logic.HSM.Builders
{
    /// <summary>
    ///     Class responsible for storing and creating commands based on provided data.
    /// </summary>
    public class CommandsData
    {
        private readonly List<(string commandName, string parameters)> _commandsData =
            new List<(string commandName, string parameters)>();

        /// <summary>
        ///     Adds a command to the storage.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="parameters">The parameters of the command.</param>
        public void AddCommandStorage(string commandName, string parameters)
        {
            _commandsData.Add((commandName, parameters));
        }

        /// <summary>
        ///     Creates Command objects based on the stored command data.
        /// </summary>
        /// <param name="bus">The bus to associate with the commands.</param>
        /// <returns>An IEnumerable of Command objects.</returns>
        public IEnumerable<Command> CreateCommands(IBus bus)
        {
            if (_commandsData == null || _commandsData.Count == 0)
            {
                return null;
            }

            Command[] commandStorage = new Command[_commandsData.Count];

            for (int i = 0; i < _commandsData.Count; i++)
            {
                commandStorage[i] = new Command(bus, _commandsData[i].commandName, _commandsData[i].parameters);
            }

            return commandStorage;
        }
    }
}
