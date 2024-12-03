using System;

namespace Talent.Logic.Bus
{
    /// <summary>
    /// Интерфейс, представляющий шину переменных
    /// </summary>
    public interface IVariableBus
    {
        /// <summary>
        /// Добавляет функцию получения переменной в шину переменных
        /// </summary>
        /// <typeparam name="T">Тип переменной</typeparam>
        /// <param name="variableName">Имя переменной</param>
        /// <param name="getter">Функция, возвращающая значение переменной</param>
        void AddVariableGetter<T>(string variableName, Func<T> getter);

        /// <summary>
        /// Удаляет функцию получения переменной из шины переменных
        /// </summary>
        /// <param name="variableName">Имя переменной</param>
        void RemoveVariableGetter(string variableName);

        /// <summary>
        /// Пытается получить значение переменной по имени и типу
        /// </summary>
        /// <typeparam name="T">Тип переменной</typeparam>
        /// <param name="variableName">Имя переменной</param>
        /// <param name="value">Если значение найдено, то возвращается значение, иначе null</param>
        /// <returns>true, если переменная существует и ее значение было успешно получено, иначе false</returns>
        bool TryGetVariableValue<T>(string variableName, out T value);

        /// <summary>
        /// Пытается получить значение переменной по имени
        /// </summary>
        /// <param name="variableName">Имя переменной</param>
        /// <param name="value">Если значение найдено, то возвращается значение, иначе null</param>
        /// <returns>true, если переменная существует и ее значение было успешно получено, иначе false</returns>
        bool TryGetVariableValue(string variableName, out string value);
    }
}
