namespace Talent.Logic.Bus
{
    public interface IVariableGetter
    {
        bool TryGetTypedVariable<T>(out T result);

        string GetStringVariable();
    }
}
