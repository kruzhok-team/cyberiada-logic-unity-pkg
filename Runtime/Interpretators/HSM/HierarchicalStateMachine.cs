using System.Collections.Generic;
using Talent.Logic.HSM.Utilities;

namespace Talent.Logic.HSM
{
    /// <summary>
    /// Класс представляющий иерархические машину состояний в логической системе Талант
    /// </summary>
    public class HierarchicalStateMachine
    {
        private readonly List<State> _states = new List<State>();

        private State _currentState;

        /// <summary>
        /// Состояния, хранимые в машине состояний
        /// </summary>
        public IEnumerable<State> States => _states;

        /// <summary>
        /// Добавляет новые состояния в машину состояний
        /// </summary>
        /// <param name="states">Добавляемые состояния</param>
        public void AddStates(IEnumerable<State> states)
        {
            _states.AddRange(states);
        }

        /// <summary>
        /// Добавляет новое состояние в машину состояний
        /// </summary>
        /// <param name="state">Добавляемое состояние в машину состояний</param>
        public void AddState(State state)
        {
            _states.Add(state);
        }

        /// <summary>
        /// Входит в состояние, соответствующее уникальному идентификатору состояния
        /// </summary>
        /// <param name="id">Уникальный идентификатор состояния</param>
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
        /// Выходит из текущего состояния
        /// </summary>
        public void ExitCurrent()
        {
            _currentState?.Exit();
            _currentState = null;
        }
    }
}