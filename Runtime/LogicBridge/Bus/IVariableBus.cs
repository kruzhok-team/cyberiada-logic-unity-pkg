using System;

namespace Talent.Logic.Bus
{
    public interface IVariableBus
    {
        void AddVariableGetter<T>(string variableName, Func<T> getter);

        void RemoveVariableGetter(string variableName);

        bool TryGetVariableValue<T>(string variableName, out T value);

        bool TryGetVariableValue(string variableName, out string value);
    }
}
