using System;
using System.Collections.Generic;

namespace Talent.Logic.Bus
{
    /// <summary>
    ///     Represents a event bus of commands, events and variables.
    /// </summary>
    public class LocalBus : IBus
    {
        private const string AnyName = "Any";

        private readonly IDictionary<string, List<Listener>> _commands = new Dictionary<string, List<Listener>>();
        private readonly IDictionary<string, List<Listener>> _events = new Dictionary<string, List<Listener>>();
        private readonly IDictionary<string, IVariableGetter> _variables = new Dictionary<string, IVariableGetter>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalBus"/> class.
        /// </summary>
        public LocalBus()
        {
            _events.Add(AnyName, new List<Listener>());
            _commands.Add(AnyName, new List<Listener>());
        }

        /// <summary>
        ///     Adds a command listener of any command to the LocalBus.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddCommandListener(Listener listener)
        {
            AddCommandListener(AnyName, listener);
        }

        /// <summary>
        ///     Adds a command listener to the LocalBus.
        /// </summary>
        /// <param name="commandName">The name of the command to listen for.</param>
        /// <param name="listener">The listener to add.</param>
        public void AddCommandListener(string commandName, Listener listener)
        {
            AddListener(_commands, commandName, listener);
        }

        /// <summary>
        ///     Adds a event listener of any event to the LocalBus.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddEventListener(Listener listener)
        {
            AddEventListener(AnyName, listener);
        }

        /// <summary>
        ///     Adds a event listener to the LocalBus.
        /// </summary>
        /// <param name="eventName">The name of the event to listen for.</param>
        /// <param name="listener">The listener to add.</param>
        public void AddEventListener(string eventName, Listener listener)
        {
            AddListener(_events, eventName, listener);
        }

        /// <summary>
        ///     Removes a command listener from the LocalBus.
        /// </summary>
        /// <param name="commandName">The name of the command to remove the listener from.</param>
        /// <param name="listener">The listener to remove.</param>
        public void RemoveCommandListener(string commandName, Listener listener)
        {
            RemoveListener(_commands, commandName, listener);
        }

        /// <summary>
        ///     Removes a event listener from the LocalBus.
        /// </summary>
        /// <param name="eventName">The name of the event to remove the listener from.</param>
        /// <param name="listener">The listener to remove.</param>
        public void RemoveEventListener(string eventName, Listener listener)
        {
            RemoveListener(_events, eventName, listener);
        }

        /// <summary>
        ///     Invokes an event with the specified event name and optional value.
        /// </summary>
        /// <param name="eventName">The name of the event to invoke.</param>
        /// <param name="value">The optional value to pass to the event listeners.</param>
        public void InvokeEvent(string eventName, List<Tuple<string, string>> value = null)
        {
            Invoke(_events, eventName, value);
        }

        /// <summary>
        ///     Invokes a command with the specified command name and optional value.
        /// </summary>
        /// <param name="commandName">The name of the command to invoke.</param>
        /// <param name="value">The optional value to pass to the command listeners.</param>
        public void InvokeCommand(string commandName, List<Tuple<string, string>> value = null)
        {
            Invoke(_commands, commandName, value);
        }

        /// <summary>
        ///     Adds a variable getter function to the LocalBus.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="getter">The function that returns the value of the variable.</param>
        public void AddVariableGetter<T>(string variableName, Func<T> getter)
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException("Variable name cannot be null or whitespace.", nameof(variableName));
            }

            if (_variables.ContainsKey(variableName))
            {
                throw new ArgumentException("Variable getter with same variable name already exists", nameof(variableName));
            }

            _variables[variableName] = new VariableGetter<T>(getter);
        }

        /// <summary>
        ///     Tries to get the value of a variable with the given name and type.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The output value of the variable.</param>
        /// <param name="asTyped">Marker for special handling of typed variables.</param>
        /// <returns>True if the variable exists and its value was successfully retrieved, false otherwise.</returns>
        public bool TryGetVariableValue<T>(string variableName, out T value)
        {
            if (_variables.TryGetValue(variableName, out IVariableGetter getter))
            {
                if (getter.TryGetTypedVariable(out T variable))
                {
                    value = variable;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Removes a variable getter function from the LocalBus.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        public void RemoveVariableGetter(string variableName)
        {
            _variables.Remove(variableName);
        }

        /// <summary>
        ///     Tries to get the value of a variable with the given name and type.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The output value of the variable.</param>
        /// <returns>True if the variable exists and its value was successfully retrieved, false otherwise.</returns>
        public bool TryGetVariableValue(string variableName, out string value)
        {
            if (_variables.TryGetValue(variableName, out IVariableGetter getter))
            {
                value = getter.GetStringVariable();
                return true;
            }

            value = default;
            return false;
        }

        private void AddListener<T>(IDictionary<string, List<T>> container, string name, T listener)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Listener name cannot be null or whitespace.", nameof(name));
            }

            if (container.ContainsKey(name) == false)
            {
                container.Add(name, new List<T>(1));
            }

            
            container[name].Insert(0, listener);
        }

        private void RemoveListener<T>(IDictionary<string, List<T>> container, string name, T listener)
        {
            if (container.ContainsKey(name) == false)
            {
                return;
            }

            container[name].Remove(listener);
        }

        private void Invoke(IDictionary<string, List<Listener>> container, string name, List<Tuple<string, string>> value)
        {
            if (container.ContainsKey(name) == false)
            {
                return;
            }

            foreach (Listener listener in container[name])
            {
                if (listener.Invoke(value))
                {
                    break;
                }
            }
        }
    }
}
