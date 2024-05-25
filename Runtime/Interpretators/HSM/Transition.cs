using System;
using System.Collections.Generic;
using Talent.Logic.Bus;

namespace Talent.Logic.HSM
{
    /// <summary>
    ///     Class that represents transition between HSM states
    /// </summary>
    public class Transition
    {
        private readonly IBus _bus;
        private readonly ConditionChecker _conditionChecker;

        private bool _isActive;

        public string NextStateId { get; }
        public string EventName { get; }
        public IEnumerable<Command> Commands { get; }
        public string Parameters => _conditionChecker.Parameters;

        public event Action<string> Triggered;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Transition"/> class.
        /// </summary>
        /// <param name="nextStateId">The ID of the next state.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="bus">The bus used for communication.</param>
        /// <param name="parameters">Optional parameters for the transition.</param>
        /// <param name="commands">Optional collection of commands to be executed.</param>
        public Transition(
            string nextStateId,
            string eventName,
            IBus bus,
            string parameters = "",
            IEnumerable<Command> commands = null)
        {
            NextStateId = nextStateId;
            EventName = eventName;
            Commands = commands;
            _bus = bus;
            _conditionChecker = new ConditionChecker(_bus, parameters);
        }

        /// <summary>
        ///     Activates the function and adds an event listener to the bus for the specified event name.
        /// </summary>
        public void Active()
        {
            _isActive = true;
            _bus.AddEventListener(EventName, Receive);
        }

        /// <summary>
        ///     Deactivates the function and removes the event listener from the bus for the specified event name.
        /// </summary>
        public void Deactivate()
        {
            _isActive = false;
            _bus.RemoveEventListener(EventName, Receive);
        }

        private bool Receive(string parameters = "")
        {
            if (!_isActive || !_conditionChecker.Check())
            {
                return false;
            }

            ExecuteCommand();
            Triggered?.Invoke(NextStateId);

            return true;
        }

        private void ExecuteCommand()
        {
            if (Commands == null)
            {
                return;
            }

            foreach (Command commandStorage in Commands)
            {
                commandStorage.Make();
            }
        }
    }
}
