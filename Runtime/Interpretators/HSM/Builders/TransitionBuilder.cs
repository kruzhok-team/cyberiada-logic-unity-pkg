using System;
using System.Collections.Generic;
using Talent.Logic.Bus;

namespace Talent.Logic.HSM.Builders
{
    /// <summary>
    ///     Represents a builder class for creating instances of the Transition class.
    /// </summary>
    public class TransitionBuilder
    {
        private readonly CommandsData _commandsData = new CommandsData();

        private IBus _bus;
        private string _nextStateId = "";
        private string _eventId = "";
        private string _parameters = "";
        private string _id = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionBuilder"/> class.
        /// </summary>
        /// <param name="bus">The bus to associate with the transition.</param>
        public TransitionBuilder AddBus(IBus bus)
        {
            _bus = bus;

            return this;
        }

        /// <summary>
        ///     Sets the next state ID and event ID for the transition.
        /// </summary>
        /// <param name="nextStateId">The ID of the next state.</param>
        /// <param name="eventId">The ID of the event.</param>
        /// <returns>The TransitionBuilder instance for method chaining.</returns>
        public TransitionBuilder ToNextStateOnEvent(string nextStateId, string eventId)
        {
            _nextStateId = nextStateId;
            _eventId = eventId;

            return this;
        }

        /// <summary>
        ///     Sets the condition parameters for the transition.
        /// </summary>
        /// <param name="parameters">The condition parameters to set.</param>
        /// <returns>The TransitionBuilder instance for method chaining.</returns>
        public TransitionBuilder WithCondition(string parameters)
        {
            _parameters = parameters;

            return this;
        }

        /// <summary>
        ///     Adds a command to the TransitionBuilder with the specified command name and optional parameters.
        /// </summary>
        /// <param name="commandName">The name of the command to add.</param>
        /// <param name="parameters">Optional parameters for the command. Defaults to an empty string.</param>
        /// <returns>The TransitionBuilder instance for method chaining.</returns>
        public TransitionBuilder AddCommand(string commandName, List<Tuple<string, string>> parameters = null)
        {
            _commandsData.AddCommandStorage(commandName, parameters);

            return this;
        }

        /// <summary>
        ///     Adds a debug ID to the TransitionBuilder.
        /// </summary>
        /// <param name="id">The debug ID to add.</param>
        /// <returns>The TransitionBuilder instance for method chaining.</returns>
        public TransitionBuilder AddDebugId(string id)
        {
            _id = id;

            return this;
        }

        /// <summary>
        ///     Builds and returns a new Transition object based on the current state of the TransitionBuilder.
        /// </summary>
        /// <returns>The newly built Transition object.</returns>
        public Transition Build()
        {
            if (string.IsNullOrEmpty(_nextStateId))
                throw new Exception($"Next state is not set {_id}");

            if (string.IsNullOrEmpty(_eventId))
                throw new Exception($"Event id is not set {_id}");

            if (_bus == null)
                throw new Exception($"Bus id is not set {_id}");

            return new Transition(_nextStateId, _eventId, _bus, _parameters, _commandsData.CreateCommands(_bus));
        }
    }
}
