namespace Talent.Logic.HSM.Utilities
{
    /// <summary>
    ///     Utility class containing various helper methods for working with Hierarchical State Machines (HSM).
    /// </summary>
    public static class Utility
    {
        /// <summary>
        ///     Determines whether the given parent state is a direct or indirect parent of the next state.
        /// </summary>
        /// <param name="parent">The parent state to check.</param>
        /// <param name="next">The ID of the next state.</param>
        /// <returns>True if the parent state is a parent of the next state, false otherwise.</returns>
        public static bool IsParentOfNextState(State parent, string next)
        {
            if (parent.ChildStates == null)
            {
                return false;
            }

            foreach (State state in parent.ChildStates)
            {
                if (state.ID == next || IsParentOfNextState(state, next))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
