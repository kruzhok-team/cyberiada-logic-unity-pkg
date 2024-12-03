using System;
using System.Collections.Generic;
using Talent.Logic.Bus;

namespace Talent.Logic.HSM.Builders
{
    /// <summary>
    /// Данные, используемые событиями в иерархической машине состояний (ИМС)
    /// </summary>
    public class EventData
    {
        private readonly CommandsData _commandsData = new CommandsData();

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public string EventId { get; private set; }

        /// <summary>
        /// Конструктор данных перехода
        /// </summary>
        /// <param name="eventId">Идентификатор события</param>
        public EventData(string eventId)
        {
            EventId = eventId;
        }

        /// <summary>
        /// Добавляет данные для команды в хранилище
        /// </summary>
        /// <param name="commandName">Имя команды</param>
        /// <param name="parameters">Список параметров команды</param>
        public void AddCommandStorage(string commandName, List<Tuple<string, string>> parameters)
        {
            _commandsData.AddCommandStorage(commandName, parameters);
        }

        /// <summary>
        /// Создает команды, используя данные, добавленные в хранилище
        /// </summary>
        /// <param name="bus">Шина команд, ассоциированный с создаваемыми командами</param>
        /// <returns>Перечисление команд</returns>
        public IEnumerable<Command> CreateCommands(ICommandBus bus)
        {
            return _commandsData.CreateCommands(bus);
        }
    }
}
