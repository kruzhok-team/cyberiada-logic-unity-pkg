#if UNITY_EDITOR || DEBUG
using UnityEngine;
#endif
using System;

namespace Talent.Logic.Bus
{
    public readonly struct VariableGetter<T> : IVariableGetter
    {
        private readonly Func<T> _getter;

        public VariableGetter(Func<T> getter)
        {
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
        }

        public bool TryGetTypedVariable<K>(out K variable)
        {
            if (_getter is Func<K> typedGetter)
            {
                variable = typedGetter.Invoke();
                return true;
            }

#if UNITY_EDITOR || DEBUG
            Debug.LogWarning($"Could not cast variable getter of type {typeof(T)} to {typeof(K)}");
#endif
            variable = default;
            return false;
        }

        public string GetStringVariable()
        {
            if (typeof(T) == typeof(bool))
            {
                string result = _getter.Invoke().ToString();
                return result == "True" ? "1" : "0";
            }

            return _getter.Invoke().ToString();
        }
    }
}
