using System.Collections.Generic;
using Talent.Logic.Bus;

namespace Talent.Logic.HSM.Builders
{
    /// <summary>
    ///     Utility class for creating HSM
    /// </summary>
    public class StateBuilder
    {
        private readonly string _id;
        private readonly List<Command> _enter = new List<Command>();
        private readonly List<Command> _exit = new List<Command>();
        private readonly List<EventData> _eventToCommandData = new List<EventData>();
        private readonly List<Transition> _transitions = new List<Transition>();
        private readonly State _current = new State();
        private readonly IBus _bus;

        private string _label = "";
        private HierarchicalStateMachine _owner;
        private State _parent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StateBuilder"/> class.
        /// </summary>
        /// <param name="bus">The bus used for event handling.</param>
        /// <param name="id">The optional identifier for the state builder.</param>
        public StateBuilder(IBus bus, string id = "")
        {
            _bus = bus;
            _id = id;
        }

        /// <summary>
        ///     Sets the label of the state builder.
        /// </summary>
        /// <param name="label">The label to set.</param>
        /// <returns>The updated state builder.</returns>
        public StateBuilder AddLabel(string label)
        {
            _label = label;

            return this;
        }

        /// <summary>
        ///     Adds a child state to the current state builder.
        /// </summary>
        /// <param name="builder">The state builder for the child state.</param>
        /// <returns>The updated state builder.</returns>
        public StateBuilder AddChildState(StateBuilder builder)
        {
            _owner ??= new HierarchicalStateMachine();

            State childState = builder
                .WithParentState(_current)
                .Build();

            _owner.AddState(childState);

            return this;
        }

        /// <summary>
        ///     Adds an enter command to the state builder.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <returns>The updated state builder.</returns>
        public StateBuilder AddEnter(string commandName, string parameters)
        {
            _enter.Add(new Command(_bus, commandName, parameters));

            return this;
        }

        /// <summary>
        ///     Adds an exit command to the state builder.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <returns>The updated state builder.</returns>
        public StateBuilder AddExit(string commandName, string parameters)
        {
            _exit.Add(new Command(_bus, commandName, parameters));

            return this;
        }

        /// <summary>
        ///     Adds a transition to the state builder.
        /// </summary>
        /// <param name="nextStateId">The ID of the next state.</param>
        /// <param name="eventId">The event ID triggering the transition.</param>
        /// <param name="parameters">Optional parameters for the transition.</param>
        /// <returns>The updated state builder.</returns>
        public StateBuilder AddTransition(string nextStateId, string eventId, string parameters = "")
        {
            _transitions.Add(new Transition(nextStateId, eventId, _bus, parameters));

            return this;
        }

        /// <summary>
        ///     Adds a transition to the state builder.
        /// </summary>
        /// <param name="transitionBuilder">The transition builder.</param>
        /// <returns>The updated state builder.</returns>
        public StateBuilder AddTransition(TransitionBuilder transitionBuilder)
        {
            _transitions.Add(transitionBuilder.AddBus(_bus).Build());

            return this;
        }

        /// <summary>
        ///     Adds a command to be executed when a specific event occurs.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        /// <param name="commandName">The name of the command to be executed.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <returns>The updated state builder.</returns>
        public StateBuilder AddCommandOnEvent(string eventId, string commandName, string parameters)
        {
            int index = _eventToCommandData.FindIndex(data => data.EventId == eventId);

            if (index == -1)
            {
                _eventToCommandData.Add(new EventData(eventId));
                index = _eventToCommandData.Count - 1;
            }

            _eventToCommandData[index].AddCommandStorage(commandName, parameters);

            return this;
        }

        /// <summary>
        ///     Builds and returns a new State object based on the current state of the StateBuilder.
        /// </summary>
        /// <returns>The newly built State object.</returns>
        public State Build()
        {
            Event[] eventToCommand = null;

            if (_eventToCommandData.Count > 0)
            {
                eventToCommand = new Event[_eventToCommandData.Count];

                for (int i = 0; i < _eventToCommandData.Count; i++)
                {
                    eventToCommand[i] = new Event(
                        _bus,
                        _eventToCommandData[i].EventId,
                        _eventToCommandData[i].CreateCommands(_bus));
                }
            }

            _current.Init(
                _id,
                _label,
                _parent,
                _enter.ToArray(),
                _exit.ToArray(),
                eventToCommand,
                _transitions.ToArray(),
                _owner);

            return _current;
        }

        private StateBuilder WithParentState(State parent)
        {
            _parent = parent;

            return this;
        }
    }
}
