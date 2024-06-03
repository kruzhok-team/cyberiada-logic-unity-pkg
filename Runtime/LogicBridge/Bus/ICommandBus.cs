namespace Talent.Logic.Bus
{
    public interface ICommandBus
    {
        void AddCommandListener(Listener listener);

        void AddCommandListener(string commandName, Listener listener);

        void RemoveCommandListener(string commandName, Listener listener);

        void InvokeEvent(string eventName, string value = "");
    }
}
