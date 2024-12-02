using System;
#if UNITY && (UNITY_EDITOR || DEBUG)
using UnityEngine;
#endif

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

#if UNITY && DEBUG
            Debug.LogWarning($"Could not cast variable getter of type {typeof(T)} to {typeof(K)}");
#endif
            variable = default;
            return false;
        }

        public string GetStringVariable()
        {
            T value = _getter.Invoke();

            if (value is bool variable)
            {
                return variable ? "1" : "0";
            }

            return $"{value}";
        }
    }
}
