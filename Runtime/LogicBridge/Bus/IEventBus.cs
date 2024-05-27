namespace Talent.Logic.Bus
{
    public interface IEventBus
    {
        void AddEventListener(Listener listener);

        void AddEventListener(string eventName, Listener listener);

        void RemoveEventListener(string eventName, Listener listener);

        void InvokeCommand(string commandName, string value = "");
    }
}
