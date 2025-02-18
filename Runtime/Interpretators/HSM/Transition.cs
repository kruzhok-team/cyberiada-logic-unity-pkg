using System;
using System.Collections.Generic;
using Talent.Logic.Bus;

namespace Talent.Logic.HSM
{
    /// <summary>
    /// Класс, представляющий переходы в иерархической машине состояний
    /// </summary>
    public class Transition
    {
        private readonly IBus _bus;
        private readonly ConditionChecker _conditionChecker;

        private bool _isActive;

        /// <summary>
        /// Идентификатор следующего состояния
        /// </summary>
        public string NextStateId { get; }

        /// <summary>
        /// Имя события перехода
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Команды перехода
        /// </summary>
        public IEnumerable<Command> Commands { get; }

        /// <summary>
        /// Прараметры перехода
        /// </summary>
        public string Parameters => _conditionChecker.Parameters;

        /// <summary>
        /// Вызывается при срабатывании события перехода
        /// </summary>
        public event Action<string> Triggered;

        /// <summary>
        /// Конструктор перехода машины состояний
        /// </summary>
        /// <param name="nextStateId">Идентификатор следующего состояния</param>
        /// <param name="eventName">Имя события перехода</param>
        /// <param name="bus">Шина</param>
        /// <param name="parameters">Опциональные параметры для перехода</param>
        /// <param name="commands">Команды перехода</param>
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
        /// Активирует функцию и добавляет в шину слушатель событий
        /// </summary>
        public void Active()
        {
            _isActive = true;
            _bus.AddEventListener(EventName, Receive);
        }

        /// <summary>
        /// Деактивирует функцию и удаляет слушатель событий из шины   
        /// </summary>
        public void Deactivate()
        {
            _isActive = false;
            _bus.RemoveEventListener(EventName, Receive);
        }

        private bool Receive(List<Tuple<string, string>> parameters = null)
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