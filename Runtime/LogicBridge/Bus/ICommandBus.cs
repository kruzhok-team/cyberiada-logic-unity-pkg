namespace Talent.Logic.Bus
{
    public interface ICommandBus
    {
        public void AddCommandListener(Listener listener);

        public void AddCommandListener(string commandName, Listener listener);

        public void RemoveCommandListener(string commandName, Listener listener);

        public void InvokeEvent(string eventName, string value = "");
    }
}
