using Talent.Logic.Bus;

namespace Talent.Logic.HSM
{
    /// <summary>
    ///     Represents the behavior of a Hierarchical State Machine (HSM).
    /// </summary>
    public class HSMBehavior : IBehavior
    {
        private readonly State _state;
        private readonly IBus _bus;
        private readonly string _initialStateId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HSMBehavior"/> class.
        /// </summary>
        /// <param name="state">The root state of the HSM.</param>
        /// <param name="bus">The event bus for handling events.</param>
        /// <param name="initialStateId">The initial state id of the HSM.</param>
        public HSMBehavior(State state, IBus bus, string initialStateId = "")
        {
            _state = state;
            _bus = bus;
            _initialStateId = initialStateId;
        }

        /// <summary>
        ///     Starts the HSM.
        /// </summary>
        public void Start()
        {
            _state.EnterSubState(_initialStateId);
            _bus.InvokeEvent(_initialStateId);
        }

        /// <summary>
        ///     Update the HSM.
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        ///     Stop the HSM.
        /// </summary>
        public void Stop()
        {
        }
    }
}
