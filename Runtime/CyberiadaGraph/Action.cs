using System;
using System.Collections.Generic;

namespace Talent.Graphs
{
    /// <summary>
    /// Класс, представляющий абстрактное поведение
    /// </summary>
    public class Action
    {
        /// <summary>
        /// Идентификатор поведения
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Список параметров поведения
        /// </summary>
        public List<Tuple<string, string>> Parameters { get; private set; }

        /// <summary>
        /// Конструктор поведения
        /// </summary>
        /// <param name="id">Уникальный идентификатор поведения</param>
        /// <param name="parameters">Список параметров поведения</param>
        public Action(string id, List<Tuple<string, string>> parameters = null)
        {
            ID = id;
            Parameters = parameters;
        }

        /// <summary>
        /// Создает копию поведения
        /// </summary>
        /// <returns>Копия поведения</returns>
        public Action GetCopy()
        {
            Action resultAction = new Action(ID, Parameters);
            return resultAction;
        }

        /// <summary>
        /// Устанавливает новый список параметров поведения
        /// </summary>
        /// <param name="parameters">Новый список параметров</param>
        public void SetParameters(List<Tuple<string, string>> parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Переводит поведение в строковое представление
        /// </summary>
        /// <returns>Результирующая строка</returns>
        public override string ToString()
        {
            return $"{nameof(Action)}, {nameof(ID)}={ID}, {nameof(Parameters)}={Parameters}";
        }
    }
}