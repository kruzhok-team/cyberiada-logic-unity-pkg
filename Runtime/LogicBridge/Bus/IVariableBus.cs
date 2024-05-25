using System;

namespace Talent.Logic.Bus
{
    public interface IVariableBus
    {
        public void AddVariableGetter(string variableName, Func<string> getter);

        public void RemoveVariableGetter(string variableName, Func<string> getter);

        public bool TryGetVariableValue(string variableName, out string value);
    }
}
