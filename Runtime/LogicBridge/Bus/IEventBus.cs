namespace Talent.Logic.Bus
{
    public interface IEventBus
    {
        public void AddEventListener(Listener listener);

        public void AddEventListener(string eventName, Listener listener);

        public void RemoveEventListener(string eventName, Listener listener);

        public void InvokeCommand(string commandName, string value = "");
    }
}
