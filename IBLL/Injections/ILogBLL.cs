namespace IBLL.Injections
{
    public interface ILogBLL
    {
        void Add(int id, string name, int logLevel, string message);
    }
}
