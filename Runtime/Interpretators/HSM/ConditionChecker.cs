using System;
using System.Globalization;
using Talent.Logic.Bus;
#if UNITY && DEBUG
using UnityEngine;
#endif

namespace Talent.Logic.HSM
{
    /// <summary>
    ///     Utility class for checking conditions based on variables and parameters.
    /// </summary>
    public class ConditionChecker
    {
        private const char SeparatorChar = ' ';

        private readonly IVariableBus _bus;
        private readonly string[] _parameters;

        public string Parameters => _parameters != null ? string.Join(SeparatorChar, _parameters) : "";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionChecker"/> class.
        /// </summary>
        /// <param name="bus">The variable bus to use for evaluating conditions.</param>
        /// <param name="parameters">The parameters to use in the condition.</param>
        public ConditionChecker(IVariableBus bus, string parameters)
        {
            _bus = bus;

            if (string.IsNullOrEmpty(parameters) == false)
            {
                _parameters = parameters.Split(SeparatorChar);

                if (_parameters.Length != 3)
                {
                    throw new Exception("parameters in transition's arguments is not valid");
                }
            }
        }

        /// <summary>
        ///     Evaluates the condition based on the provided parameters and variables.
        /// </summary>
        /// <returns>True if the condition is satisfied, false otherwise.</returns>
        public bool Check()
        {
            if (_parameters == null)
            {
                return true;
            }

            if (TryGetVariableByName(_parameters[0], out float leftValue) == false)
            {
#if UNITY && DEBUG
                Debug.Log($"cant find variable:{_parameters[0]}");
#endif
                return false;
            }

            if (TryGetVariableByName(_parameters[2], out float rightValue) == false)
            {
#if UNITY && DEBUG
                Debug.Log($"cant find variable{_parameters[2]}");
#endif
                return false;
            }

            return CompareVariables(leftValue, rightValue);
        }

        private bool CompareVariables(float leftValue, float rightValue)
        {
            switch (_parameters[1])
            {
                case "==":
                    return Math.Abs(leftValue - rightValue) < float.Epsilon;
                case "<":
                    return leftValue < rightValue;
                case "<=":
                    return leftValue <= rightValue;
                case ">":
                    return leftValue > rightValue;
                case ">=":
                    return leftValue >= rightValue;
                case "!=":
                    return Math.Abs(leftValue - rightValue) >= float.Epsilon;
                default:
#if UNITY && DEBUG
                        Debug.LogError($"Cant resolve parameters {_parameters[1]} and to go next state");
#endif
                    break;
            }

            return false;
        }

        private bool TryGetVariableByName(string variableName, out float variable)
        {
            variable = default;
            bool isTryGetVariableByName = _bus.TryGetVariableValue(variableName, out string variableString);

            bool isSuccessParse = float.TryParse(
                variableString,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out variable);

#if UNITY && DEBUG
            if (isTryGetVariableByName && isSuccessParse == false)
            {
                Debug.LogError($"Parse is failed: {variableName} {variableString}");
            }
#endif

            return isTryGetVariableByName && isSuccessParse;
        }
    }
}