using System.Collections.Generic;
using Talent.Logic.HSM.Utilities;

namespace Talent.Logic.HSM
{
    /// <summary>
    ///     Represents a hierarchical state machine in the Talent Logic system.
    /// </summary>
    public class HierarchicalStateMachine
    {
        private readonly List<State> _states = new List<State>();

        private State _currentState;

        public IEnumerable<State> States => _states;

        /// <summary>
        ///     Adds a new states to the hierarchical state machine.
        /// </summary>
        /// <param name="states">The state to add to the state machine.</param>
        public void AddStates(IEnumerable<State> states)
        {
            _states.AddRange(states);
        }

        /// <summary>
        ///     Adds a new state to the hierarchical state machine.
        /// </summary>
        /// <param name="state">The state to add to the state machine.</param>
        public void AddState(State state)
        {
            _states.Add(state);
        }

        /// <summary>
        ///     Enters a state with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the state to enter.</param>
        public void EnterState(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            if (_currentState != null && _currentState.ID == id)
            {
                _currentState.Exit();
                _currentState.Enter();

                return;
            }

            int stateIndex = _states.FindIndex(state => state.ID == id || Utility.IsParentOfNextState(state, id));

            if (stateIndex == -1)
            {
                return;
            }

            State newState = _states[stateIndex];

            if (_currentState == newState)
            {
                _currentState.EnterSubState(id);
            }
            else
            {
                _currentState?.Exit();
                _currentState = newState;
                _currentState.Enter();
                _currentState.EnterSubState(id);
            }
        }

        /// <summary>
        ///     Exits the current state and sets it to null.
        /// </summary>
        public void ExitCurrent()
        {
            _currentState?.Exit();
            _currentState = null;
        }
    }
}
