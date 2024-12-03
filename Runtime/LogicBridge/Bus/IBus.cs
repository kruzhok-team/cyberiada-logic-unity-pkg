namespace Talent.Logic.Bus
{
    /// <summary>
    /// Интерфейс шины событий, переменных, команд
    /// </summary>
    public interface IBus : IEventBus, IVariableBus, ICommandBus
    {
    }
}
