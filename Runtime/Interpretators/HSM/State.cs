using System.Collections.Generic;

namespace Talent.Logic.HSM
{
    /// <summary>
    ///     Represents a state in the Hierarchical State Machine system.
    /// </summary>
    public class State
    {
        private State _parent;
        private HierarchicalStateMachine _owner;

        public string ID { get; private set; }
        public string Label { get; private set; } = "";

        public IEnumerable<State> ChildStates => _owner?.States;
        public IEnumerable<Command> EnterCommands { get; private set; }
        public IEnumerable<Command> ExitCommands { get; private set; }
        public IEnumerable<Event> Events { get; private set; }
        public IEnumerable<Transition> Transition { get; private set; }

        /// <summary>
        ///     Initializes the state with the given parameters.
        /// </summary>
        /// <param name="id">The ID of the state.</param>
        /// <param name="label">The label of the state.</param>
        /// <param name="parent">The parent state.</param>
        /// <param name="enter">The commands to execute when entering the state.</param>
        /// <param name="exit">The commands to execute when exiting the state.</param>
        /// <param name="events">The events associated with the state.</param>
        /// <param name="transitions">The transitions from the state.</param>
        /// <param name="owner">The owner of the state.</param>
        public void Init(
            string id,
            string label,
            State parent,
            IEnumerable<Command> enter,
            IEnumerable<Command> exit,
            IEnumerable<Event> events,
            IEnumerable<Transition> transitions,
            HierarchicalStateMachine owner)
        {
            ID = id;
            Label = label;
            EnterCommands = enter;
            ExitCommands = exit;
            Transition = transitions;
            _owner = owner;
            _parent = parent;
            Events = events;
        }

        /// <summary>
        ///     Enter the state
        /// </summary>
        public void Enter()
        {
            if (Transition != null)
            {
                foreach (Transition transition in Transition)
                {
                    transition.Triggered += OnTrigger;
                    transition.Active();
                }
            }

            if (EnterCommands != null)
            {
                foreach (Command commandStorage in EnterCommands)
                {
                    commandStorage.Make();
                }
            }

            if (Events != null)
            {
                foreach (Event eventToCommand in Events)
                {
                    eventToCommand.Activate();
                }
            }
        }

        /// <summary>
        ///     Enters a substate with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the substate to enter.</param>
        public void EnterSubState(string id)
        {
            _owner?.EnterState(id);
        }

        /// <summary>
        ///     Exit the state
        /// </summary>
        public void Exit()
        {
            _owner?.ExitCurrent();

            if (Transition != null)
            {
                foreach (Transition transition in Transition)
                {
                    transition.Triggered -= OnTrigger;
                    transition.Deactivate();
                }
            }

            if (Events != null)
            {
                foreach (Event eventToCommand in Events)
                {
                    eventToCommand.Deactivate();
                }
            }

            if (ExitCommands != null)
            {
                foreach (Command commandStorage in ExitCommands)
                {
                    commandStorage.Make();
                }
            }
        }

        private void OnTrigger(string nextStateID)
        {
            if (_parent != null)
            {
                _parent.OnTrigger(nextStateID);
            }
            else
            {
                _owner.EnterState(nextStateID);
            }
        }
    }
}
