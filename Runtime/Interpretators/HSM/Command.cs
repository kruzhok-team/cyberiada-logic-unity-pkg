using Talent.Logic.Bus;

namespace Talent.Logic.HSM
{
    /// <summary>
    ///     Represents a command that can be invoked through the event bus.
    /// </summary>
    public class Command
    {
        private readonly IEventBus _bus;
        private readonly string _commandName;
        private readonly string _parameters;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="bus">The event bus to use for invoking the command.</param>
        /// <param name="commandName">The name of the command to invoke.</param>
        /// <param name="parameters">The parameters to pass to the command.</param>
        public Command(IEventBus bus, string commandName, string parameters = "")
        {
            _bus = bus;
            _commandName = commandName;
            _parameters = parameters;
        }

        /// <summary>
        ///     Invokes the command with the specified parameters.
        /// </summary>
        public void Make()
        {
            _bus.InvokeCommand(_commandName, _parameters);
        }
    }
}
